using System.Globalization;
using DormFlow.Application.Common.Abstractions;
using DormFlow.Application.Common.Authorization;
using DormFlow.Application.Common.Exceptions;
using DormFlow.Application.Common.Pagination;
using DormFlow.Application.Features.Contracts;
using DormFlow.Application.Features.Dormitories;
using DormFlow.Application.Features.MeterReadings;
using DormFlow.Contracts.Common;
using DormFlow.Contracts.Invoices;
using DormFlow.Domain.Entities;
using DormFlow.Domain.Enums;
using FluentValidation;

namespace DormFlow.Application.Features.Invoices;

/// <summary>Use Case ทั้งหมดของโมดูลใบแจ้งหนี้</summary>
public sealed class InvoiceService(
    IInvoiceRepository repository,
    IContractRepository contractRepository,
    IDormitoryRepository dormitoryRepository,
    IMeterReadingRepository meterReadingRepository,
    ICurrentUser currentUser,
    IDateTimeProvider dateTimeProvider,
    IAuditLogWriter auditLogWriter,
    IValidator<CreateInvoiceRequest> createValidator) : IInvoiceService
{
    private const string AuditEntityName = "Invoice";

    /// <summary>จำนวนครั้งสูงสุดที่ลองสุ่มเลขที่ใบแจ้งหนี้ใหม่เมื่อชนกับที่มีอยู่แล้ว</summary>
    private const int MaxInvoiceNumberAttempts = 5;

    private static readonly string[] AllowedSortFields = ["invoiceNumber", "periodMonth", "dueDate", "status", "createdAt"];

    private readonly IInvoiceRepository _repository = repository;
    private readonly IContractRepository _contractRepository = contractRepository;
    private readonly IDormitoryRepository _dormitoryRepository = dormitoryRepository;
    private readonly IMeterReadingRepository _meterReadingRepository = meterReadingRepository;
    private readonly ICurrentUser _currentUser = currentUser;
    private readonly IDateTimeProvider _dateTimeProvider = dateTimeProvider;
    private readonly IAuditLogWriter _auditLogWriter = auditLogWriter;
    private readonly IValidator<CreateInvoiceRequest> _createValidator = createValidator;

    /// <inheritdoc />
    public async Task<PagedResult<InvoiceResponse>> GetPagedAsync(
        InvoiceListRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        PagedQuery query = PagedQueryNormalizer.Normalize(request, AllowedSortFields, defaultSortBy: "periodMonth");

        InvoiceListFilter filter = new(
            request.DormitoryId,
            request.ContractId,
            ParseStatusOrNull(request.Status),
            request.PeriodMonth.HasValue ? NormalizeToFirstOfMonth(request.PeriodMonth.Value) : null,
            TenantScope.GetDormitoryRestrictionForListing(_currentUser));

        PagedResult<Invoice> result = await _repository.GetPagedAsync(filter, query, cancellationToken);

        return PagedResult.Create(
            (IReadOnlyList<InvoiceResponse>)[.. result.Items.Select(ToResponse)],
            result.Page,
            result.PageSize,
            result.TotalItems);
    }

    /// <inheritdoc />
    public async Task<InvoiceResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => ToResponse(await FindAccessibleAsync(id, cancellationToken));

    /// <inheritdoc />
    public async Task<InvoiceResponse> CreateAsync(
        CreateInvoiceRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        await _createValidator.ValidateAndThrowAsync(request, cancellationToken);

        Contract contract = await _contractRepository.GetByIdAsync(request.ContractId, cancellationToken)
            ?? throw new NotFoundException("ไม่พบสัญญาเช่าที่ระบุ");

        TenantScope.EnsureDormitoryAccess(
            _currentUser,
            contract.OrganizationId,
            contract.DormitoryId.GetValueOrDefault(),
            "ไม่พบสัญญาเช่าที่ระบุ");

        if (contract.Status != ContractStatus.Active)
        {
            throw new ConflictException("ออกใบแจ้งหนี้ได้เฉพาะสัญญาเช่าที่เปิดใช้งานอยู่เท่านั้น");
        }

        DateOnly periodMonth = NormalizeToFirstOfMonth(request.PeriodMonth);

        if (await _repository.ExistsForContractPeriodAsync(contract.Id, periodMonth, cancellationToken))
        {
            throw new ConflictException("สัญญาเช่านี้ถูกออกใบแจ้งหนี้ของเดือนที่ระบุไปแล้ว");
        }

        Dormitory? dormitory = contract.DormitoryId.HasValue
            ? await _dormitoryRepository.GetByIdAsync(contract.DormitoryId.Value, cancellationToken)
            : null;

        decimal waterAmount = await CalculateUtilityAmountAsync(
            contract.RoomId, MeterType.Water, periodMonth, dormitory?.WaterRatePerUnit ?? 0, cancellationToken);
        decimal electricityAmount = await CalculateUtilityAmountAsync(
            contract.RoomId, MeterType.Electricity, periodMonth, dormitory?.ElectricityRatePerUnit ?? 0, cancellationToken);

        DateOnly issueDate = request.IssueDate ?? DateOnly.FromDateTime(_dateTimeProvider.UtcNow.UtcDateTime);
        DateOnly dueDate = ComputeDueDate(issueDate, dormitory?.PaymentDueDay ?? 5);

        string invoiceNumber = await GenerateUniqueInvoiceNumberAsync(contract.OrganizationId, cancellationToken);

        Invoice invoice = new()
        {
            OrganizationId = contract.OrganizationId,
            DormitoryId = contract.DormitoryId,
            ContractId = contract.Id,
            InvoiceNumber = invoiceNumber,
            PeriodMonth = periodMonth,
            IssueDate = issueDate,
            DueDate = dueDate,
            RentAmount = contract.MonthlyRent,
            WaterAmount = waterAmount,
            ElectricityAmount = electricityAmount,
            LateFeeAmount = 0,
            TotalAmount = contract.MonthlyRent + waterAmount + electricityAmount,
            PaidAmount = 0,
            Notes = request.Notes,
        };

        await _repository.AddAsync(invoice, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        await _auditLogWriter.WriteAsync(
            AuditAction.Create,
            AuditEntityName,
            invoice.Id.ToString(),
            organizationId: invoice.OrganizationId,
            cancellationToken: cancellationToken);

        return ToResponse(invoice);
    }

    /// <inheritdoc />
    public async Task<InvoiceResponse> IssueAsync(Guid id, CancellationToken cancellationToken = default)
    {
        Invoice invoice = await FindAccessibleAsync(id, cancellationToken);

        if (invoice.Status != InvoiceStatus.Draft)
        {
            throw new ConflictException("ออกให้ผู้เช่าได้เฉพาะใบแจ้งหนี้ที่ยังเป็นร่างเท่านั้น");
        }

        invoice.Status = InvoiceStatus.Issued;

        await _repository.SaveChangesAsync(cancellationToken);

        await _auditLogWriter.WriteAsync(
            AuditAction.ChangeStatus,
            AuditEntityName,
            invoice.Id.ToString(),
            organizationId: invoice.OrganizationId,
            newValues: $$"""{"status":"{{InvoiceStatus.Issued}}"}""",
            cancellationToken: cancellationToken);

        return ToResponse(invoice);
    }

    /// <inheritdoc />
    public async Task<InvoiceResponse> CancelAsync(Guid id, CancellationToken cancellationToken = default)
    {
        Invoice invoice = await FindAccessibleAsync(id, cancellationToken);

        if (invoice.Status == InvoiceStatus.Cancelled)
        {
            throw new ConflictException("ใบแจ้งหนี้นี้ถูกยกเลิกไปแล้ว");
        }

        if (invoice.PaidAmount > 0)
        {
            throw new ConflictException("ยกเลิกไม่ได้เพราะมีการชำระเงินเข้ามาแล้ว ต้องคืนเงินรายการที่ชำระไว้ก่อน");
        }

        invoice.Status = InvoiceStatus.Cancelled;

        await _repository.SaveChangesAsync(cancellationToken);

        await _auditLogWriter.WriteAsync(
            AuditAction.ChangeStatus,
            AuditEntityName,
            invoice.Id.ToString(),
            organizationId: invoice.OrganizationId,
            newValues: $$"""{"status":"{{InvoiceStatus.Cancelled}}"}""",
            cancellationToken: cancellationToken);

        return ToResponse(invoice);
    }

    private async Task<decimal> CalculateUtilityAmountAsync(
        Guid roomId,
        MeterType meterType,
        DateOnly periodMonth,
        decimal ratePerUnit,
        CancellationToken cancellationToken)
    {
        MeterReading? reading = await _meterReadingRepository.GetForPeriodAsync(
            roomId, meterType, periodMonth, cancellationToken);

        return (reading?.UnitsUsed ?? 0) * ratePerUnit;
    }

    private static DateOnly ComputeDueDate(DateOnly issueDate, int paymentDueDay)
    {
        DateOnly candidate = ClampToMonth(issueDate.Year, issueDate.Month, paymentDueDay);

        if (candidate <= issueDate)
        {
            DateOnly nextMonth = issueDate.AddMonths(1);
            candidate = ClampToMonth(nextMonth.Year, nextMonth.Month, paymentDueDay);
        }

        return candidate;
    }

    private static DateOnly ClampToMonth(int year, int month, int day)
        => new(year, month, Math.Min(day, DateTime.DaysInMonth(year, month)));

    private async Task<string> GenerateUniqueInvoiceNumberAsync(Guid organizationId, CancellationToken cancellationToken)
    {
        string today = _dateTimeProvider.UtcNow.ToString("yyyyMMdd", CultureInfo.InvariantCulture);

        for (int attempt = 0; attempt < MaxInvoiceNumberAttempts; attempt++)
        {
            string candidate = $"INV-{today}-{Guid.NewGuid().ToString("N")[..6].ToUpperInvariant()}";

            if (!await _repository.ExistsByInvoiceNumberAsync(organizationId, candidate, cancellationToken))
            {
                return candidate;
            }
        }

        throw new InvalidOperationException("ไม่สามารถสุ่มเลขที่ใบแจ้งหนี้ที่ไม่ซ้ำกันได้ กรุณาลองใหม่อีกครั้ง");
    }

    private async Task<Invoice> FindAccessibleAsync(Guid id, CancellationToken cancellationToken)
    {
        Invoice invoice = await _repository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException("ไม่พบข้อมูลใบแจ้งหนี้ที่ต้องการ");

        TenantScope.EnsureDormitoryAccess(
            _currentUser,
            invoice.OrganizationId,
            invoice.DormitoryId.GetValueOrDefault(),
            "ไม่พบข้อมูลใบแจ้งหนี้ที่ต้องการ");

        return invoice;
    }

    private static DateOnly NormalizeToFirstOfMonth(DateOnly date) => new(date.Year, date.Month, 1);

    private static InvoiceStatus? ParseStatusOrNull(string? status)
        => !string.IsNullOrWhiteSpace(status) && Enum.TryParse(status, ignoreCase: true, out InvoiceStatus parsed)
            ? parsed
            : null;

    private static InvoiceResponse ToResponse(Invoice invoice) => new()
    {
        Id = invoice.Id,
        OrganizationId = invoice.OrganizationId,
        DormitoryId = invoice.DormitoryId.GetValueOrDefault(),
        ContractId = invoice.ContractId,
        InvoiceNumber = invoice.InvoiceNumber,
        PeriodMonth = invoice.PeriodMonth,
        IssueDate = invoice.IssueDate,
        DueDate = invoice.DueDate,
        RentAmount = invoice.RentAmount,
        WaterAmount = invoice.WaterAmount,
        ElectricityAmount = invoice.ElectricityAmount,
        LateFeeAmount = invoice.LateFeeAmount,
        TotalAmount = invoice.TotalAmount,
        PaidAmount = invoice.PaidAmount,
        Status = invoice.Status.ToString(),
        Notes = invoice.Notes,
        CreatedAt = invoice.CreatedAt,
    };
}
