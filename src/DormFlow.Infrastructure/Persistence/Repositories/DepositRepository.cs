using DormFlow.Application.Common.Pagination;
using DormFlow.Application.Features.Deposits;
using DormFlow.Contracts.Common;
using DormFlow.Domain.Entities;
using DormFlow.Infrastructure.Persistence.Extensions;
using Microsoft.EntityFrameworkCore;

namespace DormFlow.Infrastructure.Persistence.Repositories;

/// <summary>เข้าถึงข้อมูลเงินมัดจำบน PostgreSQL ผ่าน EF Core</summary>
public sealed class DepositRepository(DormFlowDbContext dbContext) : IDepositRepository
{
    private readonly DormFlowDbContext _dbContext = dbContext;

    /// <inheritdoc />
    public Task<Deposit?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => _dbContext.Deposits.FirstOrDefaultAsync(deposit => deposit.Id == id, cancellationToken);

    /// <inheritdoc />
    public Task<bool> ExistsByContractIdAsync(Guid contractId, CancellationToken cancellationToken = default)
        => _dbContext.Deposits.AnyAsync(deposit => deposit.ContractId == contractId, cancellationToken);

    /// <inheritdoc />
    public Task<PagedResult<Deposit>> GetPagedAsync(
        DepositListFilter filter,
        PagedQuery query,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(filter);

        IQueryable<Deposit> deposits = _dbContext.Deposits.AsNoTracking();

        if (filter.OrganizationId.HasValue)
        {
            deposits = deposits.Where(deposit => deposit.OrganizationId == filter.OrganizationId.Value);
        }

        if (filter.DormitoryId.HasValue)
        {
            deposits = deposits.Where(deposit => deposit.DormitoryId == filter.DormitoryId.Value);
        }

        if (filter.ContractId.HasValue)
        {
            deposits = deposits.Where(deposit => deposit.ContractId == filter.ContractId.Value);
        }

        if (filter.Status.HasValue)
        {
            deposits = deposits.Where(deposit => deposit.Status == filter.Status.Value);
        }

        if (filter.RestrictToDormitoryIds is not null)
        {
            deposits = deposits.Where(deposit =>
                deposit.DormitoryId != null && filter.RestrictToDormitoryIds.Contains(deposit.DormitoryId.Value));
        }

        deposits = ApplySort(deposits, query);

        return deposits.ToPagedResultAsync(query, cancellationToken);
    }

    /// <inheritdoc />
    public Task AddAsync(Deposit deposit, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(deposit);

        _dbContext.Deposits.Add(deposit);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => await _dbContext.SaveChangesAsync(cancellationToken);

    private static IQueryable<Deposit> ApplySort(IQueryable<Deposit> deposits, PagedQuery query)
        => query.SortBy?.ToLowerInvariant() switch
        {
            "amount" => deposits.OrderByDirection(deposit => deposit.Amount, query.SortDirection),
            "status" => deposits.OrderByDirection(deposit => deposit.Status, query.SortDirection),
            "collectedat" => deposits.OrderByDirection(deposit => deposit.CollectedAt, query.SortDirection),
            _ => deposits.OrderByDirection(deposit => deposit.CreatedAt, query.SortDirection),
        };
}
