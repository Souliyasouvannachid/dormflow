using DormFlow.Application.Common.Pagination;
using DormFlow.Application.Features.Buildings;
using DormFlow.Contracts.Common;
using DormFlow.Domain.Entities;
using DormFlow.Infrastructure.Persistence.Extensions;
using Microsoft.EntityFrameworkCore;

namespace DormFlow.Infrastructure.Persistence.Repositories;

/// <summary>ເຂົ້າເຖິງຂໍ້ມູນການກໍ່ສ້າງໃນ PostgreSQL ຜ່ານ EF Core</summary>
public sealed class BuildingRepository(DormFlowDbContext dbContext) : IBuildingRepository
{
    private readonly DormFlowDbContext _dbContext = dbContext;

    /// <inheritdoc />
    public Task<Building?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => _dbContext.Buildings.FirstOrDefaultAsync(building => building.Id == id, cancellationToken);

    /// <inheritdoc />
    public async Task<IReadOnlyDictionary<Guid, string>> GetNamesByIdsAsync(
        IReadOnlyCollection<Guid> ids, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(ids);
        if (ids.Count == 0)
        {
            return new Dictionary<Guid, string>();
        }

        return await _dbContext.Buildings.AsNoTracking()
            .Where(building => ids.Contains(building.Id))
            .ToDictionaryAsync(building => building.Id, building => building.Name, cancellationToken);
    }

    /// <inheritdoc />
    public Task<bool> ExistsByCodeAsync(
        Guid dormitoryId,
        string code,
        Guid? excludeBuildingId,
        CancellationToken cancellationToken = default)
        => _dbContext.Buildings.AnyAsync(
            building => building.DormitoryId == dormitoryId
                && building.Code == code
                && building.Id != excludeBuildingId,
            cancellationToken);

    /// <inheritdoc />
    public Task<PagedResult<Building>> GetPagedAsync(
        BuildingListFilter filter,
        PagedQuery query,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(filter);

        IQueryable<Building> buildings = _dbContext.Buildings.AsNoTracking();

        if (filter.OrganizationId.HasValue)
        {
            buildings = buildings.Where(building => building.OrganizationId == filter.OrganizationId.Value);
        }

        if (filter.DormitoryId.HasValue)
        {
            buildings = buildings.Where(building => building.DormitoryId == filter.DormitoryId.Value);
        }

        if (filter.Status.HasValue)
        {
            buildings = buildings.Where(building => building.Status == filter.Status.Value);
        }

        if (filter.RestrictToDormitoryIds is not null)
        {
            buildings = buildings.Where(building =>
                building.DormitoryId != null && filter.RestrictToDormitoryIds.Contains(building.DormitoryId.Value));
        }

        if (query.HasSearch)
        {
            buildings = buildings.Where(building =>
                EF.Functions.ILike(building.Name, $"%{query.Search}%")
                || EF.Functions.ILike(building.Code, $"%{query.Search}%"));
        }

        buildings = ApplySort(buildings, query);

        return buildings.ToPagedResultAsync(query, cancellationToken);
    }

    /// <inheritdoc />
    public Task AddAsync(Building building, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(building);

        _dbContext.Buildings.Add(building);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public void Remove(Building building)
    {
        ArgumentNullException.ThrowIfNull(building);

        _dbContext.Buildings.Remove(building);
    }

    /// <inheritdoc />
    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => await _dbContext.SaveChangesAsync(cancellationToken);

    private static IQueryable<Building> ApplySort(IQueryable<Building> buildings, PagedQuery query)
        => query.SortBy?.ToLowerInvariant() switch
        {
            "code" => buildings.OrderByDirection(building => building.Code, query.SortDirection),
            "status" => buildings.OrderByDirection(building => building.Status, query.SortDirection),
            "createdat" => buildings.OrderByDirection(building => building.CreatedAt, query.SortDirection),
            _ => buildings.OrderByDirection(building => building.Name, query.SortDirection),
        };
}
