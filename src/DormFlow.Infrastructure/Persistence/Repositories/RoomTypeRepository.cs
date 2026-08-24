using DormFlow.Application.Common.Pagination;
using DormFlow.Application.Features.RoomTypes;
using DormFlow.Contracts.Common;
using DormFlow.Domain.Entities;
using DormFlow.Infrastructure.Persistence.Extensions;
using Microsoft.EntityFrameworkCore;

namespace DormFlow.Infrastructure.Persistence.Repositories;

/// <summary>เข้าถึงข้อมูลประเภทห้องบน PostgreSQL ผ่าน EF Core</summary>
public sealed class RoomTypeRepository(DormFlowDbContext dbContext) : IRoomTypeRepository
{
    private readonly DormFlowDbContext _dbContext = dbContext;

    /// <inheritdoc />
    public Task<RoomType?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => _dbContext.RoomTypes.FirstOrDefaultAsync(roomType => roomType.Id == id, cancellationToken);

    /// <inheritdoc />
    public Task<bool> ExistsByCodeAsync(
        Guid dormitoryId,
        string code,
        Guid? excludeRoomTypeId,
        CancellationToken cancellationToken = default)
        => _dbContext.RoomTypes.AnyAsync(
            roomType => roomType.DormitoryId == dormitoryId
                && roomType.Code == code
                && roomType.Id != excludeRoomTypeId,
            cancellationToken);

    /// <inheritdoc />
    public Task<PagedResult<RoomType>> GetPagedAsync(
        RoomTypeListFilter filter,
        PagedQuery query,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(filter);

        IQueryable<RoomType> roomTypes = _dbContext.RoomTypes.AsNoTracking();

        if (filter.DormitoryId.HasValue)
        {
            roomTypes = roomTypes.Where(roomType => roomType.DormitoryId == filter.DormitoryId.Value);
        }

        if (filter.Status.HasValue)
        {
            roomTypes = roomTypes.Where(roomType => roomType.Status == filter.Status.Value);
        }

        if (filter.RestrictToDormitoryIds is not null)
        {
            roomTypes = roomTypes.Where(roomType =>
                roomType.DormitoryId != null && filter.RestrictToDormitoryIds.Contains(roomType.DormitoryId.Value));
        }

        if (query.HasSearch)
        {
            roomTypes = roomTypes.Where(roomType =>
                EF.Functions.ILike(roomType.Name, $"%{query.Search}%")
                || EF.Functions.ILike(roomType.Code, $"%{query.Search}%"));
        }

        roomTypes = ApplySort(roomTypes, query);

        return roomTypes.ToPagedResultAsync(query, cancellationToken);
    }

    /// <inheritdoc />
    public Task AddAsync(RoomType roomType, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(roomType);

        _dbContext.RoomTypes.Add(roomType);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public void Remove(RoomType roomType)
    {
        ArgumentNullException.ThrowIfNull(roomType);

        _dbContext.RoomTypes.Remove(roomType);
    }

    /// <inheritdoc />
    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => await _dbContext.SaveChangesAsync(cancellationToken);

    private static IQueryable<RoomType> ApplySort(IQueryable<RoomType> roomTypes, PagedQuery query)
        => query.SortBy?.ToLowerInvariant() switch
        {
            "code" => roomTypes.OrderByDirection(roomType => roomType.Code, query.SortDirection),
            "monthlyrent" => roomTypes.OrderByDirection(roomType => roomType.MonthlyRent, query.SortDirection),
            "status" => roomTypes.OrderByDirection(roomType => roomType.Status, query.SortDirection),
            "createdat" => roomTypes.OrderByDirection(roomType => roomType.CreatedAt, query.SortDirection),
            _ => roomTypes.OrderByDirection(roomType => roomType.Name, query.SortDirection),
        };
}
