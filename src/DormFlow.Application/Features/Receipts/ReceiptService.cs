using System.Globalization;
using DormFlow.Application.Common.Abstractions;
using DormFlow.Application.Common.Authorization;
using DormFlow.Application.Common.Exceptions;
using DormFlow.Application.Common.Pagination;
using DormFlow.Application.Features.Payments;
using DormFlow.Contracts.Common;
using DormFlow.Contracts.Receipts;
using DormFlow.Domain.Entities;
using DormFlow.Domain.Enums;
using FluentValidation;

namespace DormFlow.Application.Features.Receipts;

/// <summary>Use Case ทั้งหมดของโมดูลใบเสร็จรับเงิน</summary>
public sealed class ReceiptService(
    IReceiptRepository repository,
    IPaymentRepository paymentRepository,
    ICurrentUser currentUser,
    IDateTimeProvider dateTimeProvider,
    IAuditLogWriter auditLogWriter,
    IValidator<IssueReceiptRequest> issueValidator) : IReceiptService
{
    private const string AuditEntityName = "Receipt";

    /// <summary>จำนวนครั้งสูงสุดที่ลองสุ่มเลขที่ใบเสร็จใหม่เมื่อชนกับที่มีอยู่แล้ว</summary>
    private const int MaxReceiptNumberAttempts = 5;

    private static readonly string[] AllowedSortFields = ["receiptNumber", "issuedAt", "createdAt"];

    private readonly IReceiptRepository _repository = repository;
    private readonly IPaymentRepository _paymentRepository = paymentRepository;
    private readonly ICurrentUser _currentUser = currentUser;
    private readonly IDateTimeProvider _dateTimeProvider = dateTimeProvider;
    private readonly IAuditLogWriter _auditLogWriter = auditLogWriter;
    private readonly IValidator<IssueReceiptRequest> _issueValidator = issueValidator;

    /// <inheritdoc />
    public async Task<PagedResult<ReceiptResponse>> GetPagedAsync(
        ReceiptListRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        PagedQuery query = PagedQueryNormalizer.Normalize(request, AllowedSortFields, defaultSortBy: "issuedAt");

        ReceiptListFilter filter = new(
            TenantScope.GetOrganizationRestrictionForListing(_currentUser),
            request.DormitoryId,
            request.PaymentId,
            TenantScope.GetDormitoryRestrictionForListing(_currentUser));

        PagedResult<Receipt> result = await _repository.GetPagedAsync(filter, query, cancellationToken);

        return PagedResult.Create(
            (IReadOnlyList<ReceiptResponse>)[.. result.Items.Select(ToResponse)],
            result.Page,
            result.PageSize,
            result.TotalItems);
    }

    /// <inheritdoc />
    public async Task<ReceiptResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => ToResponse(await FindAccessibleAsync(id, cancellationToken));

    /// <inheritdoc />
    public async Task<ReceiptResponse> IssueAsync(
        IssueReceiptRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        await _issueValidator.ValidateAndThrowAsync(request, cancellationToken);

        Payment payment = await _paymentRepository.GetByIdAsync(request.PaymentId, cancellationToken)
            ?? throw new NotFoundException("ไม่พบรายการชำระเงินที่ระบุ");

        TenantScope.EnsureDormitoryAccess(
            _currentUser,
            payment.OrganizationId,
            payment.DormitoryId.GetValueOrDefault(),
            "ไม่พบรายการชำระเงินที่ระบุ");

        if (payment.Status != PaymentStatus.Confirmed)
        {
            throw new ConflictException("ออกใบเสร็จได้เฉพาะรายการชำระเงินที่ยืนยันแล้วเท่านั้น");
        }

        if (await _repository.ExistsByPaymentIdAsync(payment.Id, cancellationToken))
        {
            throw new ConflictException("รายการชำระเงินนี้ถูกออกใบเสร็จไปแล้ว");
        }

        string receiptNumber = await GenerateUniqueReceiptNumberAsync(payment.OrganizationId, cancellationToken);

        Receipt receipt = new()
        {
            OrganizationId = payment.OrganizationId,
            DormitoryId = payment.DormitoryId,
            PaymentId = payment.Id,
            ReceiptNumber = receiptNumber,
            Amount = payment.Amount,
            IssuedAt = _dateTimeProvider.UtcNow,
            Notes = request.Notes,
        };

        await _repository.AddAsync(receipt, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        await _auditLogWriter.WriteAsync(
            AuditAction.Create,
            AuditEntityName,
            receipt.Id.ToString(),
            organizationId: receipt.OrganizationId,
            cancellationToken: cancellationToken);

        return ToResponse(receipt);
    }

    private async Task<string> GenerateUniqueReceiptNumberAsync(Guid organizationId, CancellationToken cancellationToken)
    {
        string today = _dateTimeProvider.UtcNow.ToString("yyyyMMdd", CultureInfo.InvariantCulture);

        for (int attempt = 0; attempt < MaxReceiptNumberAttempts; attempt++)
        {
            string candidate = $"RCP-{today}-{Guid.NewGuid().ToString("N")[..6].ToUpperInvariant()}";

            if (!await _repository.ExistsByReceiptNumberAsync(organizationId, candidate, cancellationToken))
            {
                return candidate;
            }
        }

        throw new InvalidOperationException("ไม่สามารถสุ่มเลขที่ใบเสร็จที่ไม่ซ้ำกันได้ กรุณาลองใหม่อีกครั้ง");
    }

    private async Task<Receipt> FindAccessibleAsync(Guid id, CancellationToken cancellationToken)
    {
        Receipt receipt = await _repository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException("ไม่พบข้อมูลใบเสร็จรับเงินที่ต้องการ");

        TenantScope.EnsureDormitoryAccess(
            _currentUser,
            receipt.OrganizationId,
            receipt.DormitoryId.GetValueOrDefault(),
            "ไม่พบข้อมูลใบเสร็จรับเงินที่ต้องการ");

        return receipt;
    }

    private static ReceiptResponse ToResponse(Receipt receipt) => new()
    {
        Id = receipt.Id,
        OrganizationId = receipt.OrganizationId,
        DormitoryId = receipt.DormitoryId.GetValueOrDefault(),
        PaymentId = receipt.PaymentId,
        ReceiptNumber = receipt.ReceiptNumber,
        Amount = receipt.Amount,
        IssuedAt = receipt.IssuedAt,
        Notes = receipt.Notes,
        CreatedAt = receipt.CreatedAt,
    };
}
