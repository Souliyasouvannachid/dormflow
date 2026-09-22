using DormFlow.Application.Common.Pagination;
using DormFlow.Application.Features.Parcels;
using DormFlow.Contracts.Common;
using DormFlow.Domain.Entities;
using DormFlow.Infrastructure.Persistence.Extensions;
using Microsoft.EntityFrameworkCore;

namespace DormFlow.Infrastructure.Persistence.Repositories;

/// <summary>ເຂົ້າເຖິງຂໍ້ມູນ parcel on PostgreSQL ຜ່ານ EF Core</summary>
public sealed class ParcelRepository(DormFlowDbContext dbContext) : IParcelRepository
{
    private readonly DormFlowDbContext _dbContext = dbContext;

    /// <inheritdoc />
    public Task<Parcel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => _dbContext.Parcels.FirstOrDefaultAsync(parcel => parcel.Id == id, cancellationToken);

    /// <inheritdoc />
    public Task<PagedResult<Parcel>> GetPagedAsync(
        ParcelListFilter filter,
        PagedQuery query,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(filter);

        IQueryable<Parcel> parcels = _dbContext.Parcels.AsNoTracking();

        if (filter.OrganizationId.HasValue)
        {
            parcels = parcels.Where(parcel => parcel.OrganizationId == filter.OrganizationId.Value);
        }

        if (filter.DormitoryId.HasValue)
        {
            parcels = parcels.Where(parcel => parcel.DormitoryId == filter.DormitoryId.Value);
        }

        if (filter.RoomId.HasValue)
        {
            parcels = parcels.Where(parcel => parcel.RoomId == filter.RoomId.Value);
        }

        if (filter.Status.HasValue)
        {
            parcels = parcels.Where(parcel => parcel.Status == filter.Status.Value);
        }

        if (filter.RestrictToDormitoryIds is not null)
        {
            parcels = parcels.Where(parcel =>
                parcel.DormitoryId != null && filter.RestrictToDormitoryIds.Contains(parcel.DormitoryId.Value));
        }

        parcels = ApplySort(parcels, query);

        return parcels.ToPagedResultAsync(query, cancellationToken);
    }

    /// <inheritdoc />
    public Task<int> CountAsync(ParcelListFilter filter, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(filter);

        IQueryable<Parcel> parcels = _dbContext.Parcels.AsNoTracking();

        if (filter.OrganizationId.HasValue)
        {
            parcels = parcels.Where(parcel => parcel.OrganizationId == filter.OrganizationId.Value);
        }

        if (filter.DormitoryId.HasValue)
        {
            parcels = parcels.Where(parcel => parcel.DormitoryId == filter.DormitoryId.Value);
        }

        if (filter.RoomId.HasValue)
        {
            parcels = parcels.Where(parcel => parcel.RoomId == filter.RoomId.Value);
        }

        if (filter.Status.HasValue)
        {
            parcels = parcels.Where(parcel => parcel.Status == filter.Status.Value);
        }

        if (filter.RestrictToDormitoryIds is not null)
        {
            parcels = parcels.Where(parcel =>
                parcel.DormitoryId != null && filter.RestrictToDormitoryIds.Contains(parcel.DormitoryId.Value));
        }

        return parcels.CountAsync(cancellationToken);
    }

    /// <inheritdoc />
    public Task AddAsync(Parcel parcel, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(parcel);

        _dbContext.Parcels.Add(parcel);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => await _dbContext.SaveChangesAsync(cancellationToken);

    private static IQueryable<Parcel> ApplySort(IQueryable<Parcel> parcels, PagedQuery query)
        => query.SortBy?.ToLowerInvariant() switch
        {
            "status" => parcels.OrderByDirection(parcel => parcel.Status, query.SortDirection),
            "receivedat" => parcels.OrderByDirection(parcel => parcel.ReceivedAt, query.SortDirection),
            _ => parcels.OrderByDirection(parcel => parcel.CreatedAt, query.SortDirection),
        };
}
