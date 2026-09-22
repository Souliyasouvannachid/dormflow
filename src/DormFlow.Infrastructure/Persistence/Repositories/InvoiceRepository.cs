using DormFlow.Application.Common.Pagination;
using DormFlow.Application.Features.Invoices;
using DormFlow.Contracts.Common;
using DormFlow.Domain.Entities;
using DormFlow.Domain.Enums;
using DormFlow.Infrastructure.Persistence.Extensions;
using Microsoft.EntityFrameworkCore;

namespace DormFlow.Infrastructure.Persistence.Repositories;

/// <summary>ເຂົ້າເຖິງຂໍ້ມູນໃບແຈ້ງໜີ້ຢູ່ PostgreSQL ຜ່ານ EF Core</summary>
public sealed class InvoiceRepository(DormFlowDbContext dbContext) : IInvoiceRepository
{
    private readonly DormFlowDbContext _dbContext = dbContext;

    /// <inheritdoc />
    public Task<Invoice?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => _dbContext.Invoices.FirstOrDefaultAsync(invoice => invoice.Id == id, cancellationToken);

    /// <inheritdoc />
    public Task<bool> ExistsByInvoiceNumberAsync(
        Guid organizationId,
        string invoiceNumber,
        CancellationToken cancellationToken = default)
        => _dbContext.Invoices.AnyAsync(
            invoice => invoice.OrganizationId == organizationId && invoice.InvoiceNumber == invoiceNumber,
            cancellationToken);

    /// <inheritdoc />
    public Task<bool> ExistsForContractPeriodAsync(
        Guid contractId,
        DateOnly periodMonth,
        CancellationToken cancellationToken = default)
        => _dbContext.Invoices.AnyAsync(
            invoice => invoice.ContractId == contractId && invoice.PeriodMonth == periodMonth,
            cancellationToken);

    /// <inheritdoc />
    public Task<PagedResult<Invoice>> GetPagedAsync(
        InvoiceListFilter filter,
        PagedQuery query,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(filter);

        IQueryable<Invoice> invoices = _dbContext.Invoices.AsNoTracking();

        if (filter.OrganizationId.HasValue)
        {
            invoices = invoices.Where(invoice => invoice.OrganizationId == filter.OrganizationId.Value);
        }

        if (filter.DormitoryId.HasValue)
        {
            invoices = invoices.Where(invoice => invoice.DormitoryId == filter.DormitoryId.Value);
        }

        if (filter.ContractId.HasValue)
        {
            invoices = invoices.Where(invoice => invoice.ContractId == filter.ContractId.Value);
        }

        if (filter.Status.HasValue)
        {
            invoices = invoices.Where(invoice => invoice.Status == filter.Status.Value);
        }

        if (filter.PeriodMonth.HasValue)
        {
            invoices = invoices.Where(invoice => invoice.PeriodMonth == filter.PeriodMonth.Value);
        }

        if (filter.RestrictToDormitoryIds is not null)
        {
            invoices = invoices.Where(invoice =>
                invoice.DormitoryId != null && filter.RestrictToDormitoryIds.Contains(invoice.DormitoryId.Value));
        }

        if (query.HasSearch)
        {
            invoices = invoices.Where(invoice => EF.Functions.ILike(invoice.InvoiceNumber, $"%{query.Search}%"));
        }

        invoices = ApplySort(invoices, query);

        return invoices.ToPagedResultAsync(query, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Invoice>> GetOverdueCandidatesAsync(
        DateOnly asOfDate,
        int batchSize,
        CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(batchSize);

        return await _dbContext.Invoices
            .Where(invoice =>
                (invoice.Status == InvoiceStatus.Issued || invoice.Status == InvoiceStatus.PartiallyPaid)
                && invoice.DueDate < asOfDate)
            .OrderBy(invoice => invoice.DueDate)
            .Take(batchSize)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public Task AddAsync(Invoice invoice, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(invoice);

        _dbContext.Invoices.Add(invoice);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => await _dbContext.SaveChangesAsync(cancellationToken);

    private static IQueryable<Invoice> ApplySort(IQueryable<Invoice> invoices, PagedQuery query)
        => query.SortBy?.ToLowerInvariant() switch
        {
            "invoicenumber" => invoices.OrderByDirection(invoice => invoice.InvoiceNumber, query.SortDirection),
            "periodmonth" => invoices.OrderByDirection(invoice => invoice.PeriodMonth, query.SortDirection),
            "duedate" => invoices.OrderByDirection(invoice => invoice.DueDate, query.SortDirection),
            "status" => invoices.OrderByDirection(invoice => invoice.Status, query.SortDirection),
            _ => invoices.OrderByDirection(invoice => invoice.CreatedAt, query.SortDirection),
        };
}
