using System.Globalization;
using System.Text;
using DormFlow.Application.Common.Abstractions;
using DormFlow.Application.Common.Authorization;
using DormFlow.Application.Common.Pagination;
using DormFlow.Contracts.Common;
using DormFlow.Contracts.Dashboard;
using DormFlow.Contracts.Reports;
using FluentValidation;

namespace DormFlow.Application.Features.Reports;

/// <summary>Use Case ທັງຝົດຂອງໂມດູນບົດລາຍງານ</summary>
public sealed class ReportService(
    IReportRepository repository,
    ICurrentUser currentUser,
    IDateTimeProvider dateTimeProvider,
    IValidator<RevenueReportRequest> revenueValidator) : IReportService
{
    private static readonly string[] OutstandingInvoicesAllowedSortFields = ["duedate", "remainingbalance"];

    private readonly IReportRepository _repository = repository;
    private readonly ICurrentUser _currentUser = currentUser;
    private readonly IDateTimeProvider _dateTimeProvider = dateTimeProvider;
    private readonly IValidator<RevenueReportRequest> _revenueValidator = revenueValidator;

    /// <inheritdoc />
    public async Task<RevenueReportResponse> GetRevenueSummaryAsync(
        RevenueReportRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        await _revenueValidator.ValidateAndThrowAsync(request, cancellationToken);

        DateOnly today = DateOnly.FromDateTime(_dateTimeProvider.UtcNow.UtcDateTime);
        RevenueSummary summary = await _repository.GetRevenueSummaryAsync(
            BuildRevenueFilter(request), today, cancellationToken);

        return ToResponse(request, summary);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<RevenueTrendPointResponse>> GetRevenueTrendAsync(
        DateOnly periodFrom,
        DateOnly periodTo,
        CancellationToken cancellationToken = default)
    {
        DateOnly normalizedFrom = NormalizeToFirstOfMonth(periodFrom);
        DateOnly normalizedTo = NormalizeToFirstOfMonth(periodTo);

        if (normalizedFrom > normalizedTo)
        {
            throw new ArgumentException("PeriodFrom ຕ້ອງບໍ່ເປັນວັນທີຫຼັງຈາກ PeriodTo.");
        }

        RevenueReportFilter filter = BuildRevenueFilter(new RevenueReportRequest
        {
            PeriodFrom = normalizedFrom,
            PeriodTo = normalizedTo,
        });

        IReadOnlyList<RevenueTrendSummary> summaries = await _repository.GetRevenueTrendAsync(
            filter,
            cancellationToken);

        Dictionary<DateOnly, decimal> collectedByMonth = summaries
            .ToDictionary(summary => summary.PeriodMonth, summary => summary.TotalCollected);

        List<RevenueTrendPointResponse> points = [];
        for (DateOnly month = normalizedFrom; month <= normalizedTo; month = month.AddMonths(1))
        {
            points.Add(new RevenueTrendPointResponse
            {
                Label = month.ToString("MM/yyyy", CultureInfo.InvariantCulture),
                Amount = collectedByMonth.GetValueOrDefault(month),
            });
        }

        return points;
    }

    /// <inheritdoc />
    public async Task<byte[]> ExportRevenueSummaryAsync(
        RevenueReportRequest request,
        CancellationToken cancellationToken = default)
    {
        RevenueReportResponse summary = await GetRevenueSummaryAsync(request, cancellationToken);

        StringBuilder csv = new();
        csv.AppendLine("ເລີ່ມງວດ,ສິ້ນສຸດງວດ,ຈຳນວນໃບແຈ້ງໜີ້,ຈຳນວນທີ່ຊຳລະແລ້ວ,ຈຳນວນທີ່ເກີນກຳນົດ,ຍອດອອກໃບແຈ້ງໜີ້,ຍອດເກັບໄດ້,ຍອດຄ້າງຊຳລະ");
        csv.AppendLine(string.Join(',',
            summary.PeriodFrom.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
            summary.PeriodTo.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
            summary.InvoiceCount,
            summary.PaidInvoiceCount,
            summary.OverdueInvoiceCount,
            summary.TotalInvoiced,
            summary.TotalCollected,
            summary.TotalOutstanding));

        return Encoding.UTF8.GetBytes(csv.ToString());
    }

    /// <inheritdoc />
    public async Task<PagedResult<OutstandingInvoiceItemResponse>> GetOutstandingInvoicesAsync(
        OutstandingInvoicesReportRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        PagedQuery query = PagedQueryNormalizer.Normalize(
            request, OutstandingInvoicesAllowedSortFields, defaultSortBy: "duedate");

        OutstandingInvoicesFilter filter = BuildOutstandingInvoicesFilter(request.DormitoryId);

        PagedResult<OutstandingInvoiceRow> result = await _repository.GetOutstandingInvoicesAsync(
            filter, query, cancellationToken);

        DateOnly today = DateOnly.FromDateTime(_dateTimeProvider.UtcNow.UtcDateTime);

        return PagedResult.Create(
            (IReadOnlyList<OutstandingInvoiceItemResponse>)[.. result.Items.Select(row => ToResponse(row, today))],
            result.Page,
            result.PageSize,
            result.TotalItems);
    }

    /// <inheritdoc />
    public async Task<byte[]> ExportOutstandingInvoicesAsync(
        OutstandingInvoicesReportRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        OutstandingInvoicesFilter filter = BuildOutstandingInvoicesFilter(request.DormitoryId);
        IReadOnlyList<OutstandingInvoiceRow> rows = await _repository.GetOutstandingInvoicesForExportAsync(
            filter, cancellationToken);

        DateOnly today = DateOnly.FromDateTime(_dateTimeProvider.UtcNow.UtcDateTime);

        StringBuilder csv = new();
        csv.AppendLine("ເລກທີໃບແຈ້ງໜີ້,ລະຫັດສັນຍາ,ວັນຄົບກຳນົດ,ຈຳນວນທັງໝົດ,ຈຳນວນທີ່ຊຳລະ,ຍອດຄ້າງຊຳລະ,ຈຳນວນມື້ທີ່ເກີນກຳນົດ");

        foreach (OutstandingInvoiceRow row in rows)
        {
            OutstandingInvoiceItemResponse item = ToResponse(row, today);
            csv.AppendLine(string.Join(',',
                EscapeCsvField(item.InvoiceNumber),
                item.ContractId,
                item.DueDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                item.TotalAmount,
                item.PaidAmount,
                item.RemainingBalance,
                item.DaysOverdue));
        }

        return Encoding.UTF8.GetBytes(csv.ToString());
    }

    private RevenueReportFilter BuildRevenueFilter(RevenueReportRequest request) => new(
        TenantScope.GetOrganizationRestrictionForListing(_currentUser),
        request.DormitoryId,
        NormalizeToFirstOfMonth(request.PeriodFrom),
        NormalizeToFirstOfMonth(request.PeriodTo),
        TenantScope.GetDormitoryRestrictionForListing(_currentUser));

    private OutstandingInvoicesFilter BuildOutstandingInvoicesFilter(Guid? dormitoryId) => new(
        TenantScope.GetOrganizationRestrictionForListing(_currentUser),
        dormitoryId,
        TenantScope.GetDormitoryRestrictionForListing(_currentUser));

    private static DateOnly NormalizeToFirstOfMonth(DateOnly date) => new(date.Year, date.Month, 1);

    private static string EscapeCsvField(string value)
        => value.Contains(',', StringComparison.Ordinal) || value.Contains('"', StringComparison.Ordinal)
            ? $"\"{value.Replace("\"", "\"\"", StringComparison.Ordinal)}\""
            : value;

    private static RevenueReportResponse ToResponse(RevenueReportRequest request, RevenueSummary summary) => new()
    {
        PeriodFrom = request.PeriodFrom,
        PeriodTo = request.PeriodTo,
        InvoiceCount = summary.InvoiceCount,
        PaidInvoiceCount = summary.PaidInvoiceCount,
        OverdueInvoiceCount = summary.OverdueInvoiceCount,
        TotalInvoiced = summary.TotalInvoiced,
        TotalCollected = summary.TotalCollected,
        TotalOutstanding = summary.TotalInvoiced - summary.TotalCollected,
    };

    private static OutstandingInvoiceItemResponse ToResponse(OutstandingInvoiceRow row, DateOnly asOfDate)
    {
        decimal remainingBalance = row.TotalAmount - row.PaidAmount;
        int daysOverdue = Math.Max(0, asOfDate.DayNumber - row.DueDate.DayNumber);

        return new OutstandingInvoiceItemResponse
        {
            InvoiceId = row.InvoiceId,
            InvoiceNumber = row.InvoiceNumber,
            ContractId = row.ContractId,
            DormitoryId = row.DormitoryId.GetValueOrDefault(),
            DueDate = row.DueDate,
            TotalAmount = row.TotalAmount,
            PaidAmount = row.PaidAmount,
            RemainingBalance = remainingBalance,
            DaysOverdue = daysOverdue,
        };
    }
}
