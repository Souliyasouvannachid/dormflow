using DormFlow.Application.Common.Abstractions;
using DormFlow.Application.Common.Authorization;
using DormFlow.Application.Common.Exceptions;
using DormFlow.Application.Common.Pagination;
using DormFlow.Application.Features.Invoices;
using DormFlow.Contracts.Common;
using DormFlow.Contracts.Payments;
using DormFlow.Domain.Entities;
using DormFlow.Domain.Enums;
using FluentValidation;

namespace DormFlow.Application.Features.Payments;

/// <summary>
/// Use Case ทั้งหมดของโมดูลการชำระเงิน
/// </summary>
/// <remarks>
/// รายการชำระเงินเริ่มต้นเป็น <see cref="PaymentStatus.Pending"/> เสมอและไม่มีผลต่อยอดใบแจ้งหนี้
/// จนกว่าจะถูกยืนยันผ่าน <see cref="ConfirmAsync"/> ดูเหตุผลเพิ่มเติมที่ <see cref="Domain.Entities.Payment"/>
/// </remarks>
public sealed class PaymentService(
    IPaymentRepository repository,
    IInvoiceRepository invoiceRepository,
    ICurrentUser currentUser,
    IDateTimeProvider dateTimeProvider,
    IAuditLogWriter auditLogWriter,
    IValidator<CreatePaymentRequest> createValidator,
    IValidator<RefundPaymentRequest> refundValidator) : IPaymentService
{
    private const string AuditEntityName = "Payment";
    private static readonly string[] AllowedSortFields = ["paymentDate", "amount", "status", "createdAt"];

    private readonly IPaymentRepository _repository = repository;
    private readonly IInvoiceRepository _invoiceRepository = invoiceRepository;
    private readonly ICurrentUser _currentUser = currentUser;
    private readonly IDateTimeProvider _dateTimeProvider = dateTimeProvider;
    private readonly IAuditLogWriter _auditLogWriter = auditLogWriter;
    private readonly IValidator<CreatePaymentRequest> _createValidator = createValidator;
    private readonly IValidator<RefundPaymentRequest> _refundValidator = refundValidator;

    /// <inheritdoc />
    public async Task<PagedResult<PaymentResponse>> GetPagedAsync(
        PaymentListRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        PagedQuery query = PagedQueryNormalizer.Normalize(request, AllowedSortFields, defaultSortBy: "createdAt");

        PaymentListFilter filter = new(
            TenantScope.GetOrganizationRestrictionForListing(_currentUser),
            request.DormitoryId,
            request.InvoiceId,
            ParseStatusOrNull(request.Status),
            TenantScope.GetDormitoryRestrictionForListing(_currentUser));

        PagedResult<Payment> result = await _repository.GetPagedAsync(filter, query, cancellationToken);

        return PagedResult.Create(
            (IReadOnlyList<PaymentResponse>)[.. result.Items.Select(ToResponse)],
            result.Page,
            result.PageSize,
            result.TotalItems);
    }

    /// <inheritdoc />
    public async Task<PaymentResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => ToResponse(await FindAccessibleAsync(id, cancellationToken));

    /// <inheritdoc />
    public async Task<PaymentResponse> CreateAsync(
        CreatePaymentRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        await _createValidator.ValidateAndThrowAsync(request, cancellationToken);

        Invoice invoice = await _invoiceRepository.GetByIdAsync(request.InvoiceId, cancellationToken)
            ?? throw new NotFoundException("ไม่พบใบแจ้งหนี้ที่ระบุ");

        TenantScope.EnsureDormitoryAccess(
            _currentUser,
            invoice.OrganizationId,
            invoice.DormitoryId.GetValueOrDefault(),
            "ไม่พบใบแจ้งหนี้ที่ระบุ");

        if (invoice.Status is InvoiceStatus.Draft or InvoiceStatus.Cancelled or InvoiceStatus.Paid)
        {
            throw new ConflictException("ใบแจ้งหนี้นี้ยังไม่ได้ออกให้ผู้เช่า หรือถูกยกเลิก/ชำระครบแล้ว ไม่สามารถรับชำระเพิ่มได้");
        }

        Payment payment = new()
        {
            OrganizationId = invoice.OrganizationId,
            DormitoryId = invoice.DormitoryId,
            InvoiceId = invoice.Id,
            Amount = request.Amount,
            PaymentDate = request.PaymentDate ?? _dateTimeProvider.UtcNow,
            Method = Enum.Parse<PaymentMethod>(request.Method, ignoreCase: true),
            ReferenceNumber = request.ReferenceNumber,
            Notes = request.Notes,
        };

        await _repository.AddAsync(payment, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        await _auditLogWriter.WriteAsync(
            AuditAction.Create,
            AuditEntityName,
            payment.Id.ToString(),
            organizationId: payment.OrganizationId,
            cancellationToken: cancellationToken);

        return ToResponse(payment);
    }

    /// <inheritdoc />
    public async Task<PaymentResponse> ConfirmAsync(Guid id, CancellationToken cancellationToken = default)
    {
        Payment payment = await FindAccessibleAsync(id, cancellationToken);

        if (payment.Status != PaymentStatus.Pending)
        {
            throw new ConflictException("ยืนยันได้เฉพาะรายการที่ยังรอยืนยันเท่านั้น");
        }

        Invoice invoice = await _invoiceRepository.GetByIdAsync(payment.InvoiceId, cancellationToken)
            ?? throw new NotFoundException("ไม่พบใบแจ้งหนี้ของรายการชำระเงินนี้");

        decimal remainingBalance = invoice.TotalAmount - invoice.PaidAmount;
        if (payment.Amount > remainingBalance)
        {
            throw new ConflictException(
                $"จำนวนเงินที่ยืนยัน ({payment.Amount}) เกินยอดคงเหลือของใบแจ้งหนี้ ({remainingBalance})");
        }

        DateTimeOffset now = _dateTimeProvider.UtcNow;
        payment.Status = PaymentStatus.Confirmed;
        payment.ConfirmedAt = now;

        ApplyPaymentToInvoice(invoice, payment.Amount);

        // เรียกครั้งเดียวเพื่อบันทึกทั้งรายการชำระเงินและยอด/สถานะใบแจ้งหนี้ในธุรกรรมเดียวกัน
        // เพราะ Payment และ Invoice ถูกติดตามอยู่ใน DbContext เดียวกันภายในคำขอนี้
        await _repository.SaveChangesAsync(cancellationToken);

        await _auditLogWriter.WriteAsync(
            AuditAction.ChangeStatus,
            AuditEntityName,
            payment.Id.ToString(),
            organizationId: payment.OrganizationId,
            newValues: $$"""{"status":"{{PaymentStatus.Confirmed}}"}""",
            cancellationToken: cancellationToken);

        return ToResponse(payment);
    }

    /// <inheritdoc />
    public async Task<PaymentResponse> RefundAsync(
        Guid id,
        RefundPaymentRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        await _refundValidator.ValidateAndThrowAsync(request, cancellationToken);

        Payment payment = await FindAccessibleAsync(id, cancellationToken);

        if (payment.Status != PaymentStatus.Confirmed)
        {
            throw new ConflictException("คืนเงินได้เฉพาะรายการที่ยืนยันแล้วเท่านั้น");
        }

        Invoice invoice = await _invoiceRepository.GetByIdAsync(payment.InvoiceId, cancellationToken)
            ?? throw new NotFoundException("ไม่พบใบแจ้งหนี้ของรายการชำระเงินนี้");

        DateTimeOffset now = _dateTimeProvider.UtcNow;
        payment.Status = PaymentStatus.Refunded;
        payment.RefundedAt = now;
        payment.RefundReason = request.Reason;

        ReverseInvoicePayment(invoice, payment.Amount);

        await _repository.SaveChangesAsync(cancellationToken);

        await _auditLogWriter.WriteAsync(
            AuditAction.ChangeStatus,
            AuditEntityName,
            payment.Id.ToString(),
            organizationId: payment.OrganizationId,
            newValues: $$"""{"status":"{{PaymentStatus.Refunded}}"}""",
            cancellationToken: cancellationToken);

        return ToResponse(payment);
    }

    private static void ApplyPaymentToInvoice(Invoice invoice, decimal amount)
    {
        invoice.PaidAmount += amount;
        RecomputeInvoiceStatus(invoice);
    }

    private static void ReverseInvoicePayment(Invoice invoice, decimal amount)
    {
        invoice.PaidAmount = Math.Max(0, invoice.PaidAmount - amount);
        RecomputeInvoiceStatus(invoice);
    }

    private static void RecomputeInvoiceStatus(Invoice invoice)
    {
        invoice.Status = invoice.PaidAmount switch
        {
            <= 0 => InvoiceStatus.Issued,
            _ when invoice.PaidAmount < invoice.TotalAmount => InvoiceStatus.PartiallyPaid,
            _ => InvoiceStatus.Paid,
        };
    }

    private async Task<Payment> FindAccessibleAsync(Guid id, CancellationToken cancellationToken)
    {
        Payment payment = await _repository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException("ไม่พบข้อมูลการชำระเงินที่ต้องการ");

        TenantScope.EnsureDormitoryAccess(
            _currentUser,
            payment.OrganizationId,
            payment.DormitoryId.GetValueOrDefault(),
            "ไม่พบข้อมูลการชำระเงินที่ต้องการ");

        return payment;
    }

    private static PaymentStatus? ParseStatusOrNull(string? status)
        => !string.IsNullOrWhiteSpace(status) && Enum.TryParse(status, ignoreCase: true, out PaymentStatus parsed)
            ? parsed
            : null;

    private static PaymentResponse ToResponse(Payment payment) => new()
    {
        Id = payment.Id,
        OrganizationId = payment.OrganizationId,
        DormitoryId = payment.DormitoryId.GetValueOrDefault(),
        InvoiceId = payment.InvoiceId,
        Amount = payment.Amount,
        PaymentDate = payment.PaymentDate,
        Method = payment.Method.ToString(),
        ReferenceNumber = payment.ReferenceNumber,
        Status = payment.Status.ToString(),
        ConfirmedAt = payment.ConfirmedAt,
        RefundedAt = payment.RefundedAt,
        RefundReason = payment.RefundReason,
        Notes = payment.Notes,
        CreatedAt = payment.CreatedAt,
    };
}
