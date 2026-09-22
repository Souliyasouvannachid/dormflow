using DormFlow.Application.Common.Pagination;
using DormFlow.Application.Features.Contracts;
using DormFlow.Contracts.Common;
using DormFlow.Domain.Entities;
using DormFlow.Domain.Enums;
using DormFlow.Infrastructure.Persistence.Extensions;
using Microsoft.EntityFrameworkCore;

namespace DormFlow.Infrastructure.Persistence.Repositories;

/// <summary>ເຂົ້າເຖິງຂໍ້ມູນການເຊົ່າໃນ PostgreSQL ຜ່ານ EF Core</summary>
public sealed class ContractRepository(DormFlowDbContext dbContext) : IContractRepository
{
    private readonly DormFlowDbContext _dbContext = dbContext;

    /// <inheritdoc />
    public Task<Contract?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => _dbContext.Contracts.FirstOrDefaultAsync(contract => contract.Id == id, cancellationToken);

    /// <inheritdoc />
    public Task<bool> ExistsByContractNumberAsync(
        Guid organizationId,
        string contractNumber,
        CancellationToken cancellationToken = default)
        => _dbContext.Contracts.AnyAsync(
            contract => contract.OrganizationId == organizationId && contract.ContractNumber == contractNumber,
            cancellationToken);

    /// <inheritdoc />
    public Task<bool> HasOpenContractAsync(Guid roomId, CancellationToken cancellationToken = default)
        => _dbContext.Contracts.AnyAsync(
            contract => contract.RoomId == roomId
                && (contract.Status == ContractStatus.Draft || contract.Status == ContractStatus.Active),
            cancellationToken);

    /// <inheritdoc />
    public Task<PagedResult<Contract>> GetPagedAsync(
        ContractListFilter filter,
        PagedQuery query,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(filter);

        IQueryable<Contract> contracts = _dbContext.Contracts.AsNoTracking();

        if (filter.OrganizationId.HasValue)
        {
            contracts = contracts.Where(contract => contract.OrganizationId == filter.OrganizationId.Value);
        }

        if (filter.DormitoryId.HasValue)
        {
            contracts = contracts.Where(contract => contract.DormitoryId == filter.DormitoryId.Value);
        }

        if (filter.RoomId.HasValue)
        {
            contracts = contracts.Where(contract => contract.RoomId == filter.RoomId.Value);
        }

        if (filter.TenantId.HasValue)
        {
            contracts = contracts.Where(contract => contract.TenantId == filter.TenantId.Value);
        }

        if (filter.Status.HasValue)
        {
            contracts = contracts.Where(contract => contract.Status == filter.Status.Value);
        }

        if (filter.RestrictToDormitoryIds is not null)
        {
            contracts = contracts.Where(contract =>
                contract.DormitoryId != null && filter.RestrictToDormitoryIds.Contains(contract.DormitoryId.Value));
        }

        if (query.HasSearch)
        {
            contracts = contracts.Where(contract => EF.Functions.ILike(contract.ContractNumber, $"%{query.Search}%"));
        }

        contracts = ApplySort(contracts, query);

        return contracts.ToPagedResultAsync(query, cancellationToken);
    }

    /// <inheritdoc />
    public Task AddAsync(Contract contract, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(contract);

        _dbContext.Contracts.Add(contract);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => await _dbContext.SaveChangesAsync(cancellationToken);

    private static IQueryable<Contract> ApplySort(IQueryable<Contract> contracts, PagedQuery query)
        => query.SortBy?.ToLowerInvariant() switch
        {
            "contractnumber" => contracts.OrderByDirection(contract => contract.ContractNumber, query.SortDirection),
            "startdate" => contracts.OrderByDirection(contract => contract.StartDate, query.SortDirection),
            "enddate" => contracts.OrderByDirection(contract => contract.EndDate, query.SortDirection),
            "status" => contracts.OrderByDirection(contract => contract.Status, query.SortDirection),
            _ => contracts.OrderByDirection(contract => contract.CreatedAt, query.SortDirection),
        };
}
