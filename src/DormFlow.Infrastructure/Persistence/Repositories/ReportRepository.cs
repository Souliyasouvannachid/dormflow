using DormFlow.Application.Common.Pagination;
using DormFlow.Application.Features.Reports;
using DormFlow.Contracts.Common;
using DormFlow.Domain.Entities;
using DormFlow.Domain.Enums;
using DormFlow.Infrastructure.Persistence.Extensions;
using Microsoft.EntityFrameworkCore;

namespace DormFlow.Infrastructure.Persistence.Repositories;

/// <summary>ເຂົ້າເຖິງຂໍ້ມູນເພື່ອສ້າງບົດລາຍງານກ່ຽວກັບ PostgreSQL ຜ່ານ EF Core</summary>
public sealed class ReportRepository(DormFlowDbContext dbContext) : IReportRepository
{
    /// <summary>ຈໍານວນແຖວສູງສຸດທີ່ອະນຸຍາດໃຫ້ສົ່ງອອກໃນຄໍາຮ້ອງຂໍດຽວ. ປ້ອງກັນການດຶງຂໍ້ມູນຫຼາຍເກີນໄປໃນຄໍາຮ້ອງຂໍດຽວ</summary>
    private const int MaxExportRows = 5000;

    private static readonly InvoiceStatus[] OpenInvoiceStatuses = [InvoiceStatus.Issued, InvoiceStatus.PartiallyPaid];

    private readonly DormFlowDbContext _dbContext = dbContext;

    /// <inheritdoc />
    public async Task<RevenueSummary> GetRevenueSummaryAsync(
        RevenueReportFilter filter,
        DateOnly asOfDate,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(filter);

        IQueryable<Invoice> invoices = ApplyRevenueFilter(_dbContext.Invoices.AsNoTracking(), filter);

        var aggregate = await invoices
            .GroupBy(invoice => 1)
            .Select(group => new
            {
                Count = group.Count(),
                PaidCount = group.Count(invoice => invoice.Status == InvoiceStatus.Paid),
                TotalInvoiced = group.Sum(invoice => invoice.TotalAmount),
                TotalCollected = group.Sum(invoice => invoice.PaidAmount),
            })
            .FirstOrDefaultAsync(cancellationToken);

        int overdueCount = await invoices.CountAsync(
            invoice => OpenInvoiceStatuses.Contains(invoice.Status) && invoice.DueDate < asOfDate,
            cancellationToken);

        return aggregate is null
            ? new RevenueSummary(0, 0, 0, 0, 0)
            : new RevenueSummary(aggregate.Count, aggregate.PaidCount, overdueCount, aggregate.TotalInvoiced, aggregate.TotalCollected);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<RevenueTrendSummary>> GetRevenueTrendAsync(
        RevenueReportFilter filter,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(filter);

        return await ApplyRevenueFilter(_dbContext.Invoices.AsNoTracking(), filter)
            .GroupBy(invoice => invoice.PeriodMonth)
            .Select(group => new RevenueTrendSummary(
                group.Key,
                group.Sum(invoice => invoice.PaidAmount)))
            .OrderBy(summary => summary.PeriodMonth)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public Task<PagedResult<OutstandingInvoiceRow>> GetOutstandingInvoicesAsync(
        OutstandingInvoicesFilter filter,
        PagedQuery query,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(filter);

        IQueryable<Invoice> invoices = ApplyOutstandingFilter(_dbContext.Invoices.AsNoTracking(), filter);

        invoices = query.SortBy?.ToLowerInvariant() == "remainingbalance"
            ? invoices.OrderByDirection(invoice => invoice.TotalAmount - invoice.PaidAmount, query.SortDirection)
            : invoices.OrderByDirection(invoice => invoice.DueDate, query.SortDirection);

        return invoices.ToPagedResultAsync(ToRowExpression(), query, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<OutstandingInvoiceRow>> GetOutstandingInvoicesForExportAsync(
        OutstandingInvoicesFilter filter,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(filter);

        IQueryable<Invoice> invoices = ApplyOutstandingFilter(_dbContext.Invoices.AsNoTracking(), filter);

        return await invoices
            .OrderBy(invoice => invoice.DueDate)
            .Take(MaxExportRows)
            .Select(ToRowExpression())
            .ToListAsync(cancellationToken);
    }

    private static IQueryable<Invoice> ApplyRevenueFilter(IQueryable<Invoice> invoices, RevenueReportFilter filter)
    {
        invoices = invoices.Where(invoice =>
            invoice.PeriodMonth >= filter.PeriodFrom
            && invoice.PeriodMonth <= filter.PeriodTo
            && invoice.Status != InvoiceStatus.Cancelled);

        if (filter.OrganizationId.HasValue)
        {
            invoices = invoices.Where(invoice => invoice.OrganizationId == filter.OrganizationId.Value);
        }

        if (filter.DormitoryId.HasValue)
        {
            invoices = invoices.Where(invoice => invoice.DormitoryId == filter.DormitoryId.Value);
        }

        if (filter.RestrictToDormitoryIds is not null)
        {
            invoices = invoices.Where(invoice =>
                invoice.DormitoryId != null && filter.RestrictToDormitoryIds.Contains(invoice.DormitoryId.Value));
        }

        return invoices;
    }

    private static IQueryable<Invoice> ApplyOutstandingFilter(IQueryable<Invoice> invoices, OutstandingInvoicesFilter filter)
    {
        invoices = invoices.Where(invoice => OpenInvoiceStatuses.Contains(invoice.Status));

        if (filter.OrganizationId.HasValue)
        {
            invoices = invoices.Where(invoice => invoice.OrganizationId == filter.OrganizationId.Value);
        }

        if (filter.DormitoryId.HasValue)
        {
            invoices = invoices.Where(invoice => invoice.DormitoryId == filter.DormitoryId.Value);
        }

        if (filter.RestrictToDormitoryIds is not null)
        {
            invoices = invoices.Where(invoice =>
                invoice.DormitoryId != null && filter.RestrictToDormitoryIds.Contains(invoice.DormitoryId.Value));
        }

        return invoices;
    }

    private static System.Linq.Expressions.Expression<Func<Invoice, OutstandingInvoiceRow>> ToRowExpression()
        => invoice => new OutstandingInvoiceRow(
            invoice.Id,
            invoice.InvoiceNumber,
            invoice.ContractId,
            invoice.DormitoryId,
            invoice.DueDate,
            invoice.TotalAmount,
            invoice.PaidAmount);
}
