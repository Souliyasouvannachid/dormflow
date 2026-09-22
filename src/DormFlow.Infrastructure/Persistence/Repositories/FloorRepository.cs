using System.Globalization;
using DormFlow.Application.Common.Pagination;
using DormFlow.Application.Features.Floors;
using DormFlow.Contracts.Common;
using DormFlow.Domain.Entities;
using DormFlow.Infrastructure.Persistence.Extensions;
using Microsoft.EntityFrameworkCore;

namespace DormFlow.Infrastructure.Persistence.Repositories;

/// <summary>ເຂົ້າເຖິງຂໍ້ມູນຊັ້ນອາຄານ PostgreSQL ຜ່ານ EF Core</summary>
public sealed class FloorRepository(DormFlowDbContext dbContext) : IFloorRepository
{
    private readonly DormFlowDbContext _dbContext = dbContext;

    /// <inheritdoc />
    public Task<Floor?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => _dbContext.Floors.FirstOrDefaultAsync(floor => floor.Id == id, cancellationToken);

    /// <inheritdoc />
    public async Task<IReadOnlyDictionary<Guid, string>> GetNamesByIdsAsync(
        IReadOnlyCollection<Guid> ids, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(ids);
        if (ids.Count == 0)
        {
            return new Dictionary<Guid, string>();
        }

        return await _dbContext.Floors.AsNoTracking()
            .Where(floor => ids.Contains(floor.Id))
            .ToDictionaryAsync(
                floor => floor.Id,
                floor => floor.Name ?? floor.FloorNumber.ToString(CultureInfo.InvariantCulture),
                cancellationToken);
    }

    /// <inheritdoc />
    public Task<bool> ExistsByFloorNumberAsync(
        Guid buildingId,
        int floorNumber,
        Guid? excludeFloorId,
        CancellationToken cancellationToken = default)
        => _dbContext.Floors.AnyAsync(
            floor => floor.BuildingId == buildingId
                && floor.FloorNumber == floorNumber
                && floor.Id != excludeFloorId,
            cancellationToken);

    /// <inheritdoc />
    public Task<PagedResult<Floor>> GetPagedAsync(
        FloorListFilter filter,
        PagedQuery query,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(filter);

        IQueryable<Floor> floors = _dbContext.Floors.AsNoTracking();

        if (filter.OrganizationId.HasValue)
        {
            floors = floors.Where(floor => floor.OrganizationId == filter.OrganizationId.Value);
        }

        if (filter.BuildingId.HasValue)
        {
            floors = floors.Where(floor => floor.BuildingId == filter.BuildingId.Value);
        }

        if (filter.Status.HasValue)
        {
            floors = floors.Where(floor => floor.Status == filter.Status.Value);
        }

        if (filter.RestrictToDormitoryIds is not null)
        {
            floors = floors.Where(floor =>
                floor.DormitoryId != null && filter.RestrictToDormitoryIds.Contains(floor.DormitoryId.Value));
        }

        if (query.HasSearch)
        {
            floors = floors.Where(floor => floor.Name != null && EF.Functions.ILike(floor.Name, $"%{query.Search}%"));
        }

        floors = ApplySort(floors, query);

        return floors.ToPagedResultAsync(query, cancellationToken);
    }

    /// <inheritdoc />
    public Task AddAsync(Floor floor, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(floor);

        _dbContext.Floors.Add(floor);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public void Remove(Floor floor)
    {
        ArgumentNullException.ThrowIfNull(floor);

        _dbContext.Floors.Remove(floor);
    }

    /// <inheritdoc />
    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => await _dbContext.SaveChangesAsync(cancellationToken);

    private static IQueryable<Floor> ApplySort(IQueryable<Floor> floors, PagedQuery query)
        => query.SortBy?.ToLowerInvariant() switch
        {
            "status" => floors.OrderByDirection(floor => floor.Status, query.SortDirection),
            "createdat" => floors.OrderByDirection(floor => floor.CreatedAt, query.SortDirection),
            _ => floors.OrderByDirection(floor => floor.FloorNumber, query.SortDirection),
        };
}
