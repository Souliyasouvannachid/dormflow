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

/// <summary>Use Case ໂມດູນໃບແຈ້ງໜີ້ທັງໝົດ</summary>
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
    private const int OverdueBatchSize = 500;

    /// <summary>ຈຳນວນຄັ້ງສູງສຸດເພື່ອລອງໃຊ້ໝາຍເລກໃບແຈ້ງໜີ້ໃໝ່ແບບສຸ່ມເມື່ອມີການຕຳກັນກັບອັນທີ່ມີຢູ່ແລ້ວ</summary>
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
            TenantScope.GetOrganizationRestrictionForListing(_currentUser),
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
            ?? throw new NotFoundException("ບໍ່ພົບສັນຍາເຊົ່າທີ່ລະບຸ");

        TenantScope.EnsureDormitoryAccess(
            _currentUser,
            contract.OrganizationId,
            contract.DormitoryId.GetValueOrDefault(),
            "ບໍ່ພົບສັນຍາເຊົ່າທີ່ລະບຸ");

        if (contract.Status != ContractStatus.Active)
        {
            throw new ConflictException("ສາມາດອອກໃບແຈ້ງໜີ້ໄດ້ສະເພາະສັນຍາເຊົ່າທີ່ເປີດໃຊ້ງານຢູ່ເທົ່ານັ້ນ");
        }

        DateOnly periodMonth = NormalizeToFirstOfMonth(request.PeriodMonth);

        if (await _repository.ExistsForContractPeriodAsync(contract.Id, periodMonth, cancellationToken))
        {
            throw new ConflictException("ສັນຍາເຊົ່ານີ້ຖືກອອກໃບແຈ້ງໜີ້ຂອງເດືອນທີ່ລະບຸໄປແລ້ວ");
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
            throw new ConflictException("ສາມາດອອກໃຫ້ຜູ້ເຊົ່າໄດ້ສະເພາະໃບແຈ້ງໜີ້ທີ່ຍັງເປັນສະບັບຮ່າງເທົ່ານັ້ນ");
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
            throw new ConflictException("ໃບແຈ້ງໜີ້ນີ້ຖືກຍົກເລີກໄປແລ້ວ");
        }

        if (invoice.PaidAmount > 0)
        {
            throw new ConflictException("ບໍ່ສາມາດຍົກເລີກໄດ້ເນື່ອງຈາກມີການຊຳລະເງິນເຂົ້າມາແລ້ວ ຕ້ອງຄືນເງິນລາຍການທີ່ຊຳລະໄວ້ກ່ອນ");
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

    /// <inheritdoc />
    public async Task<int> MarkOverdueInvoicesAsync(CancellationToken cancellationToken = default)
    {
        DateOnly today = DateOnly.FromDateTime(_dateTimeProvider.UtcNow.UtcDateTime);
        int overdueCount = 0;

        while (true)
        {
            IReadOnlyList<Invoice> overdueCandidates = await _repository.GetOverdueCandidatesAsync(
                today,
                OverdueBatchSize,
                cancellationToken);

            if (overdueCandidates.Count == 0)
            {
                break;
            }

            IReadOnlyDictionary<Guid, Dormitory> dormitories = await _dormitoryRepository.GetByIdsAsync(
                overdueCandidates
                    .Where(invoice => invoice.DormitoryId.HasValue)
                    .Select(invoice => invoice.DormitoryId!.Value)
                    .Distinct()
                    .ToArray(),
                cancellationToken);

            foreach (Invoice invoice in overdueCandidates)
            {
                Dormitory? dormitory = invoice.DormitoryId.HasValue
                    && dormitories.TryGetValue(invoice.DormitoryId.Value, out Dormitory? loadedDormitory)
                    ? loadedDormitory
                    : null;

                decimal lateFee = CalculateLateFee(invoice, dormitory);

                invoice.LateFeeAmount += lateFee;
                invoice.TotalAmount += lateFee;
                invoice.Status = InvoiceStatus.Overdue;

                await _auditLogWriter.WriteAsync(
                    AuditAction.ChangeStatus,
                    AuditEntityName,
                    invoice.Id.ToString(),
                    organizationId: invoice.OrganizationId,
                    newValues: $$"""{"status":"{{InvoiceStatus.Overdue}}","lateFeeAmount":{{lateFee}}}""",
                    cancellationToken: cancellationToken);
            }

            await _repository.SaveChangesAsync(cancellationToken);
            overdueCount += overdueCandidates.Count;

            if (overdueCandidates.Count < OverdueBatchSize)
            {
                break;
            }
        }

        return overdueCount;
    }

    /// <summary>
    /// ຄິດໄລ່ຄ່າປັບໃໝການຈ່າຍເງິນຊ້າເມື່ອໃບແຈ້ງໜີ້ປ່ຽນເປັນ Overdue
    /// ເປີເຊັນຄິດໄລ່ຈາກຍອດຍັງເຫຼືອ (TotalAmount - PaidAmount) ໃນເວລານັ້ນ, ມັນບໍ່ແມ່ນຈໍານວນເຕັມຂອງໃບບິນຄ່າ
    /// ເພື່ອບໍ່ໃຫ້ມີການລົງໂທດອີກເທື່ອຜຶ່ງດ້ວຍຈໍານວນທີ່ຈ່າຍແລ້ວ
    /// </summary>
    private static decimal CalculateLateFee(Invoice invoice, Dormitory? dormitory)
    {
        if (dormitory is null)
        {
            return 0;
        }

        return dormitory.LateFeeType switch
        {
            LateFeeType.Fixed => dormitory.LateFeeAmount,
            LateFeeType.Percentage => Math.Round(
                (invoice.TotalAmount - invoice.PaidAmount) * dormitory.LateFeeAmount / 100m,
                2),
            _ => 0,
        };
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

        throw new InvalidOperationException("ບໍ່ສາມາດສຸ່ມເລກທີໃບແຈ້ງໜີ້ທີ່ບໍ່ຊ້ຳກັນໄດ້ ກະລຸນາລອງໃໝ່ອີກຄັ້ງ");
    }

    private async Task<Invoice> FindAccessibleAsync(Guid id, CancellationToken cancellationToken)
    {
        Invoice invoice = await _repository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException("ບໍ່ພົບຂໍ້ມູນໃບແຈ້ງໜີ້ທີ່ຕ້ອງການ");

        TenantScope.EnsureDormitoryAccess(
            _currentUser,
            invoice.OrganizationId,
            invoice.DormitoryId.GetValueOrDefault(),
            "ບໍ່ພົບຂໍ້ມູນໃບແຈ້ງໜີ້ທີ່ຕ້ອງການ");

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
