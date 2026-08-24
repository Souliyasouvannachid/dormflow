using DormFlow.Application.Common.Pagination;
using DormFlow.Application.Features.Rooms;
using DormFlow.Contracts.Common;
using DormFlow.Domain.Entities;
using DormFlow.Domain.Enums;
using DormFlow.Infrastructure.Persistence.Extensions;
using Microsoft.EntityFrameworkCore;

namespace DormFlow.Infrastructure.Persistence.Repositories;

/// <summary>เข้าถึงข้อมูลห้องพักบน PostgreSQL ผ่าน EF Core</summary>
public sealed class RoomRepository(DormFlowDbContext dbContext) : IRoomRepository
{
    private readonly DormFlowDbContext _dbContext = dbContext;

    /// <inheritdoc />
    public Task<Room?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => _dbContext.Rooms.FirstOrDefaultAsync(room => room.Id == id, cancellationToken);

    /// <inheritdoc />
    public Task<bool> ExistsByRoomNumberAsync(
        Guid buildingId,
        string roomNumber,
        Guid? excludeRoomId,
        CancellationToken cancellationToken = default)
        => _dbContext.Rooms.AnyAsync(
            room => room.BuildingId == buildingId
                && room.RoomNumber == roomNumber
                && room.Id != excludeRoomId,
            cancellationToken);

    /// <inheritdoc />
    public Task<PagedResult<Room>> GetPagedAsync(
        RoomListFilter filter,
        PagedQuery query,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(filter);

        IQueryable<Room> rooms = ApplyFilter(_dbContext.Rooms.AsNoTracking(), filter);

        if (query.HasSearch)
        {
            rooms = rooms.Where(room => EF.Functions.ILike(room.RoomNumber, $"%{query.Search}%"));
        }

        rooms = ApplySort(rooms, query);

        return rooms.ToPagedResultAsync(query, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyDictionary<RoomStatus, int>> GetStatusCountsAsync(
        RoomSummaryFilter filter,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(filter);

        IQueryable<Room> rooms = _dbContext.Rooms.AsNoTracking();

        if (filter.DormitoryId.HasValue)
        {
            rooms = rooms.Where(room => room.DormitoryId == filter.DormitoryId.Value);
        }

        if (filter.BuildingId.HasValue)
        {
            rooms = rooms.Where(room => room.BuildingId == filter.BuildingId.Value);
        }

        if (filter.RestrictToDormitoryIds is not null)
        {
            rooms = rooms.Where(room =>
                room.DormitoryId != null && filter.RestrictToDormitoryIds.Contains(room.DormitoryId.Value));
        }

        List<(RoomStatus Status, int Count)> grouped = await rooms
            .GroupBy(room => room.Status)
            .Select(group => new ValueTuple<RoomStatus, int>(group.Key, group.Count()))
            .ToListAsync(cancellationToken);

        return grouped.ToDictionary(entry => entry.Status, entry => entry.Count);
    }

    /// <inheritdoc />
    public Task AddAsync(Room room, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(room);

        _dbContext.Rooms.Add(room);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public void Remove(Room room)
    {
        ArgumentNullException.ThrowIfNull(room);

        _dbContext.Rooms.Remove(room);
    }

    /// <inheritdoc />
    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => await _dbContext.SaveChangesAsync(cancellationToken);

    private static IQueryable<Room> ApplyFilter(IQueryable<Room> rooms, RoomListFilter filter)
    {
        if (filter.DormitoryId.HasValue)
        {
            rooms = rooms.Where(room => room.DormitoryId == filter.DormitoryId.Value);
        }

        if (filter.BuildingId.HasValue)
        {
            rooms = rooms.Where(room => room.BuildingId == filter.BuildingId.Value);
        }

        if (filter.FloorId.HasValue)
        {
            rooms = rooms.Where(room => room.FloorId == filter.FloorId.Value);
        }

        if (filter.RoomTypeId.HasValue)
        {
            rooms = rooms.Where(room => room.RoomTypeId == filter.RoomTypeId.Value);
        }

        if (filter.Status.HasValue)
        {
            rooms = rooms.Where(room => room.Status == filter.Status.Value);
        }

        if (filter.RestrictToDormitoryIds is not null)
        {
            rooms = rooms.Where(room =>
                room.DormitoryId != null && filter.RestrictToDormitoryIds.Contains(room.DormitoryId.Value));
        }

        return rooms;
    }

    private static IQueryable<Room> ApplySort(IQueryable<Room> rooms, PagedQuery query)
        => query.SortBy?.ToLowerInvariant() switch
        {
            "monthlyrent" => rooms.OrderByDirection(room => room.MonthlyRent, query.SortDirection),
            "status" => rooms.OrderByDirection(room => room.Status, query.SortDirection),
            "createdat" => rooms.OrderByDirection(room => room.CreatedAt, query.SortDirection),
            _ => rooms.OrderByDirection(room => room.RoomNumber, query.SortDirection),
        };
}
