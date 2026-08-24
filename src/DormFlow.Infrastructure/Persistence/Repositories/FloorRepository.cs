using DormFlow.Application.Common.Pagination;
using DormFlow.Application.Features.Floors;
using DormFlow.Contracts.Common;
using DormFlow.Domain.Entities;
using DormFlow.Infrastructure.Persistence.Extensions;
using Microsoft.EntityFrameworkCore;

namespace DormFlow.Infrastructure.Persistence.Repositories;

/// <summary>เข้าถึงข้อมูลชั้นของอาคารบน PostgreSQL ผ่าน EF Core</summary>
public sealed class FloorRepository(DormFlowDbContext dbContext) : IFloorRepository
{
    private readonly DormFlowDbContext _dbContext = dbContext;

    /// <inheritdoc />
    public Task<Floor?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => _dbContext.Floors.FirstOrDefaultAsync(floor => floor.Id == id, cancellationToken);

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
