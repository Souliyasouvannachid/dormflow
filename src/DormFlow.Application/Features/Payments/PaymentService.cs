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
/// Use Case ໂມດູນການຈ່າຍເງິນທັງຝົດ
/// </summary>
/// <remarks>
/// ການຈ່າຍເງິນເລີ່ມຕົ້ນແມ່ນ <see cref="PaymentStatus.Pending"/> ສະເໝີ ແລະບໍ່ມີຜົນຕໍ່ຈຳນວນໃບແຈ້ງໜີ້
/// ຈົນກ່ວາມັນໄດ້ຮັບການຢືນຢັນໂດຍຜ່ານ <see cref="ConfirmAsync"/> ເບິ່ງເຫດຜົນເພີ່ມເຕີມໄດ້ທີ່ <see cref="Domain.Entities.Payment"/>
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
    public Task<int> GetCountAsync(PaymentStatus status, CancellationToken cancellationToken = default)
    {
        PaymentListFilter filter = new(
            TenantScope.GetOrganizationRestrictionForListing(_currentUser),
            DormitoryId: null,
            InvoiceId: null,
            status,
            TenantScope.GetDormitoryRestrictionForListing(_currentUser));

        return _repository.CountAsync(filter, cancellationToken);
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
            ?? throw new NotFoundException("ບໍ່ພົບໃບແຈ້ງໜີ້ທີ່ລະບຸ");

        TenantScope.EnsureDormitoryAccess(
            _currentUser,
            invoice.OrganizationId,
            invoice.DormitoryId.GetValueOrDefault(),
            "ບໍ່ພົບໃບແຈ້ງໜີ້ທີ່ລະບຸ");

        if (invoice.Status is InvoiceStatus.Draft or InvoiceStatus.Cancelled or InvoiceStatus.Paid)
        {
            throw new ConflictException("ໃບແຈ້ງໜີ້ນີ້ຍັງບໍ່ໄດ້ອອກໃຫ້ຜູ້ເຊົ່າ ຫຼືຖືກຍົກເລີກ/ຊຳລະຄົບແລ້ວ ບໍ່ສາມາດຮັບຊຳລະເພີ່ມໄດ້");
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
            throw new ConflictException("ຢືນຢັນໄດ້ສະເພາະລາຍການທີ່ຍັງລໍຖ້າຢືນຢັນເທົ່ານັ້ນ");
        }

        Invoice invoice = await _invoiceRepository.GetByIdAsync(payment.InvoiceId, cancellationToken)
            ?? throw new NotFoundException("ບໍ່ພົບໃບແຈ້ງໜີ້ຂອງລາຍການຊຳລະເງິນນີ້");

        decimal remainingBalance = invoice.TotalAmount - invoice.PaidAmount;
        if (payment.Amount > remainingBalance)
        {
            throw new ConflictException(
                $"ຈຳນວນເງິນທີ່ຢືນຢັນ ({payment.Amount}) ເກີນຍອດຄົງເຫຼືອຂອງໃບແຈ້ງໜີ້ ({remainingBalance})");
        }

        DateTimeOffset now = _dateTimeProvider.UtcNow;
        payment.Status = PaymentStatus.Confirmed;
        payment.ConfirmedAt = now;

        DateOnly today = DateOnly.FromDateTime(now.UtcDateTime);
        ApplyPaymentToInvoice(invoice, payment.Amount, today);

        // ໂທຄັ້ງດຽວເພື່ອບັນທຶກທັງການຈ່າຍເງິນ ແລະຍອດທັງໝົດ/ສະຖານະໃບແຈ້ງໜີ້ໃນທຸລະກຳດຽວກັນ
        // ເນື່ອງຈາກວ່າ Payment ແລະ Invoice ຖືກຕິດຕາມໃນ DbContext ດຽວກັນຢູ່ໃນຄໍາຮ້ອງຂໍນີ້
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
            throw new ConflictException("ຄືນເງິນໄດ້ສະເພາະລາຍການທີ່ຢືນຢັນແລ້ວເທົ່ານັ້ນ");
        }

        Invoice invoice = await _invoiceRepository.GetByIdAsync(payment.InvoiceId, cancellationToken)
            ?? throw new NotFoundException("ບໍ່ພົບໃບແຈ້ງໜີ້ຂອງລາຍການຊຳລະເງິນນີ້");

        DateTimeOffset now = _dateTimeProvider.UtcNow;
        payment.Status = PaymentStatus.Refunded;
        payment.RefundedAt = now;
        payment.RefundReason = request.Reason;

        DateOnly today = DateOnly.FromDateTime(now.UtcDateTime);
        ReverseInvoicePayment(invoice, payment.Amount, today);

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

    private static void ApplyPaymentToInvoice(Invoice invoice, decimal amount, DateOnly asOfDate)
    {
        invoice.PaidAmount += amount;
        RecomputeInvoiceStatus(invoice, asOfDate);
    }

    private static void ReverseInvoicePayment(Invoice invoice, decimal amount, DateOnly asOfDate)
    {
        invoice.PaidAmount = Math.Max(0, invoice.PaidAmount - amount);
        RecomputeInvoiceStatus(invoice, asOfDate);
    }

    private static void RecomputeInvoiceStatus(Invoice invoice, DateOnly asOfDate)
    {
        invoice.Status = invoice.PaidAmount switch
        {
            <= 0 when invoice.DueDate < asOfDate => InvoiceStatus.Overdue,
            <= 0 => InvoiceStatus.Issued,
            _ when invoice.PaidAmount < invoice.TotalAmount && invoice.DueDate < asOfDate => InvoiceStatus.Overdue,
            _ when invoice.PaidAmount < invoice.TotalAmount => InvoiceStatus.PartiallyPaid,
            _ => InvoiceStatus.Paid,
        };
    }

    private async Task<Payment> FindAccessibleAsync(Guid id, CancellationToken cancellationToken)
    {
        Payment payment = await _repository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException("ບໍ່ພົບຂໍ້ມູນການຊຳລະເງິນທີ່ຕ້ອງການ");

        TenantScope.EnsureDormitoryAccess(
            _currentUser,
            payment.OrganizationId,
            payment.DormitoryId.GetValueOrDefault(),
            "ບໍ່ພົບຂໍ້ມູນການຊຳລະເງິນທີ່ຕ້ອງການ");

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
