using DormFlow.Application.Common.Pagination;
using DormFlow.Application.Features.Parking;
using DormFlow.Contracts.Common;
using DormFlow.Domain.Entities;
using DormFlow.Domain.Enums;
using DormFlow.Infrastructure.Persistence.Extensions;
using Microsoft.EntityFrameworkCore;

namespace DormFlow.Infrastructure.Persistence.Repositories;

/// <summary>ເຂົ້າເຖິງຂໍ້ມູນການລົງທະບຽນບ່ອນຈອດລົດໃນ PostgreSQL ຜ່ານ EF Core</summary>
public sealed class ParkingRegistrationRepository(DormFlowDbContext dbContext) : IParkingRegistrationRepository
{
    private readonly DormFlowDbContext _dbContext = dbContext;

    /// <inheritdoc />
    public Task<ParkingRegistration?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => _dbContext.ParkingRegistrations.FirstOrDefaultAsync(registration => registration.Id == id, cancellationToken);

    /// <inheritdoc />
    public Task<bool> HasActiveByLicensePlateAsync(
        Guid organizationId,
        string licensePlate,
        CancellationToken cancellationToken = default)
        => _dbContext.ParkingRegistrations.AnyAsync(
            registration => registration.OrganizationId == organizationId
                && registration.LicensePlate == licensePlate
                && registration.Status == ParkingRegistrationStatus.Active,
            cancellationToken);

    /// <inheritdoc />
    public Task<PagedResult<ParkingRegistration>> GetPagedAsync(
        ParkingRegistrationListFilter filter,
        PagedQuery query,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(filter);

        IQueryable<ParkingRegistration> registrations = _dbContext.ParkingRegistrations.AsNoTracking();

        if (filter.OrganizationId.HasValue)
        {
            registrations = registrations.Where(registration => registration.OrganizationId == filter.OrganizationId.Value);
        }

        if (filter.DormitoryId.HasValue)
        {
            registrations = registrations.Where(registration => registration.DormitoryId == filter.DormitoryId.Value);
        }

        if (filter.TenantId.HasValue)
        {
            registrations = registrations.Where(registration => registration.TenantId == filter.TenantId.Value);
        }

        if (filter.Status.HasValue)
        {
            registrations = registrations.Where(registration => registration.Status == filter.Status.Value);
        }

        if (filter.RestrictToDormitoryIds is not null)
        {
            registrations = registrations.Where(registration =>
                registration.DormitoryId != null && filter.RestrictToDormitoryIds.Contains(registration.DormitoryId.Value));
        }

        if (query.HasSearch)
        {
            registrations = registrations.Where(registration =>
                EF.Functions.ILike(registration.LicensePlate, $"%{query.Search}%"));
        }

        registrations = ApplySort(registrations, query);

        return registrations.ToPagedResultAsync(query, cancellationToken);
    }

    /// <inheritdoc />
    public Task AddAsync(ParkingRegistration registration, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(registration);

        _dbContext.ParkingRegistrations.Add(registration);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => await _dbContext.SaveChangesAsync(cancellationToken);

    private static IQueryable<ParkingRegistration> ApplySort(IQueryable<ParkingRegistration> registrations, PagedQuery query)
        => query.SortBy?.ToLowerInvariant() switch
        {
            "status" => registrations.OrderByDirection(registration => registration.Status, query.SortDirection),
            "registeredat" => registrations.OrderByDirection(registration => registration.RegisteredAt, query.SortDirection),
            _ => registrations.OrderByDirection(registration => registration.CreatedAt, query.SortDirection),
        };
}
