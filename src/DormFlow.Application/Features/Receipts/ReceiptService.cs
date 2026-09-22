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

/// <summary>Use Case ໂມດູນໃບຮັບເງິນທັງໝົດ</summary>
public sealed class ReceiptService(
    IReceiptRepository repository,
    IPaymentRepository paymentRepository,
    ICurrentUser currentUser,
    IDateTimeProvider dateTimeProvider,
    IAuditLogWriter auditLogWriter,
    IValidator<IssueReceiptRequest> issueValidator) : IReceiptService
{
    private const string AuditEntityName = "Receipt";

    /// <summary>ຈຳນວນຄັ້ງສູງສຸດເພື່ອລອງໃຊ້ເລກຮັບໃໝ່ແບບສຸ່ມເມື່ອມີການຕຳກັນກັບອັນທີ່ມີຢູ່ແລ້ວ</summary>
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
            ?? throw new NotFoundException("ບໍ່ພົບລາຍການຊຳລະເງິນທີ່ລະບຸ");

        TenantScope.EnsureDormitoryAccess(
            _currentUser,
            payment.OrganizationId,
            payment.DormitoryId.GetValueOrDefault(),
            "ບໍ່ພົບລາຍການຊຳລະເງິນທີ່ລະບຸ");

        if (payment.Status != PaymentStatus.Confirmed)
        {
            throw new ConflictException("ສາມາດອອກໃບຮັບເງິນໄດ້ສະເພາະລາຍການຊຳລະເງິນທີ່ຢືນຢັນແລ້ວເທົ່ານັ້ນ");
        }

        if (await _repository.ExistsByPaymentIdAsync(payment.Id, cancellationToken))
        {
            throw new ConflictException("ລາຍການຊຳລະເງິນນີ້ຖືກອອກໃບຮັບເງິນໄປແລ້ວ");
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

        throw new InvalidOperationException("ບໍ່ສາມາດສຸ່ມເລກທີໃບຮັບເງິນທີ່ບໍ່ຊ້ຳກັນໄດ້ ກະລຸນາລອງໃໝ່ອີກຄັ້ງ");
    }

    private async Task<Receipt> FindAccessibleAsync(Guid id, CancellationToken cancellationToken)
    {
        Receipt receipt = await _repository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException("ບໍ່ພົບຂໍ້ມູນໃບຮັບເງິນທີ່ຕ້ອງການ");

        TenantScope.EnsureDormitoryAccess(
            _currentUser,
            receipt.OrganizationId,
            receipt.DormitoryId.GetValueOrDefault(),
            "ບໍ່ພົບຂໍ້ມູນໃບຮັບເງິນທີ່ຕ້ອງການ");

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
