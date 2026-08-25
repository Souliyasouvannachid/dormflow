using DormFlow.Application.Common.Pagination;
using DormFlow.Application.Features.Reservations;
using DormFlow.Contracts.Common;
using DormFlow.Domain.Entities;
using DormFlow.Domain.Enums;
using DormFlow.Infrastructure.Persistence.Extensions;
using Microsoft.EntityFrameworkCore;

namespace DormFlow.Infrastructure.Persistence.Repositories;

/// <summary>เข้าถึงข้อมูลการจองบน PostgreSQL ผ่าน EF Core</summary>
public sealed class ReservationRepository(DormFlowDbContext dbContext) : IReservationRepository
{
    private readonly DormFlowDbContext _dbContext = dbContext;

    /// <inheritdoc />
    public Task<Reservation?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => _dbContext.Reservations.FirstOrDefaultAsync(reservation => reservation.Id == id, cancellationToken);

    /// <inheritdoc />
    public Task<bool> HasPendingReservationAsync(Guid roomId, CancellationToken cancellationToken = default)
        => _dbContext.Reservations.AnyAsync(
            reservation => reservation.RoomId == roomId && reservation.Status == ReservationStatus.Pending,
            cancellationToken);

    /// <inheritdoc />
    public Task<PagedResult<Reservation>> GetPagedAsync(
        ReservationListFilter filter,
        PagedQuery query,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(filter);

        IQueryable<Reservation> reservations = _dbContext.Reservations.AsNoTracking();

        if (filter.OrganizationId.HasValue)
        {
            reservations = reservations.Where(reservation => reservation.OrganizationId == filter.OrganizationId.Value);
        }

        if (filter.DormitoryId.HasValue)
        {
            reservations = reservations.Where(reservation => reservation.DormitoryId == filter.DormitoryId.Value);
        }

        if (filter.RoomId.HasValue)
        {
            reservations = reservations.Where(reservation => reservation.RoomId == filter.RoomId.Value);
        }

        if (filter.TenantId.HasValue)
        {
            reservations = reservations.Where(reservation => reservation.TenantId == filter.TenantId.Value);
        }

        if (filter.Status.HasValue)
        {
            reservations = reservations.Where(reservation => reservation.Status == filter.Status.Value);
        }

        if (filter.RestrictToDormitoryIds is not null)
        {
            reservations = reservations.Where(reservation =>
                reservation.DormitoryId != null && filter.RestrictToDormitoryIds.Contains(reservation.DormitoryId.Value));
        }

        reservations = ApplySort(reservations, query);

        return reservations.ToPagedResultAsync(query, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Reservation>> GetExpiredPendingAsync(
        DateTimeOffset asOf,
        CancellationToken cancellationToken = default)
        => await _dbContext.Reservations
            .Where(reservation => reservation.Status == ReservationStatus.Pending && reservation.ExpiresAt <= asOf)
            .ToListAsync(cancellationToken);

    /// <inheritdoc />
    public Task AddAsync(Reservation reservation, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(reservation);

        _dbContext.Reservations.Add(reservation);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => await _dbContext.SaveChangesAsync(cancellationToken);

    private static IQueryable<Reservation> ApplySort(IQueryable<Reservation> reservations, PagedQuery query)
        => query.SortBy?.ToLowerInvariant() switch
        {
            "moveindate" => reservations.OrderByDirection(reservation => reservation.MoveInDate, query.SortDirection),
            "expiresat" => reservations.OrderByDirection(reservation => reservation.ExpiresAt, query.SortDirection),
            "status" => reservations.OrderByDirection(reservation => reservation.Status, query.SortDirection),
            _ => reservations.OrderByDirection(reservation => reservation.CreatedAt, query.SortDirection),
        };
}
