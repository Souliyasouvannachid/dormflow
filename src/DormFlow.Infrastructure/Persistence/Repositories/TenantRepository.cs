using DormFlow.Application.Common.Pagination;
using DormFlow.Application.Features.Tenants;
using DormFlow.Contracts.Common;
using DormFlow.Domain.Entities;
using DormFlow.Infrastructure.Persistence.Extensions;
using Microsoft.EntityFrameworkCore;

namespace DormFlow.Infrastructure.Persistence.Repositories;

/// <summary>เข้าถึงข้อมูลผู้เช่าบน PostgreSQL ผ่าน EF Core</summary>
public sealed class TenantRepository(DormFlowDbContext dbContext) : ITenantRepository
{
    private readonly DormFlowDbContext _dbContext = dbContext;

    /// <inheritdoc />
    public Task<Tenant?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => _dbContext.Tenants.FirstOrDefaultAsync(tenant => tenant.Id == id, cancellationToken);

    /// <inheritdoc />
    public Task<bool> ExistsByNationalIdAsync(
        Guid organizationId,
        string nationalId,
        Guid? excludeTenantId,
        CancellationToken cancellationToken = default)
        => _dbContext.Tenants.AnyAsync(
            tenant => tenant.OrganizationId == organizationId
                && tenant.NationalId == nationalId
                && tenant.Id != excludeTenantId,
            cancellationToken);

    /// <inheritdoc />
    public Task<PagedResult<Tenant>> GetPagedAsync(
        TenantListFilter filter,
        PagedQuery query,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(filter);

        IQueryable<Tenant> tenants = _dbContext.Tenants.AsNoTracking();

        if (filter.OrganizationId.HasValue)
        {
            tenants = tenants.Where(tenant => tenant.OrganizationId == filter.OrganizationId.Value);
        }

        if (filter.Status.HasValue)
        {
            tenants = tenants.Where(tenant => tenant.Status == filter.Status.Value);
        }

        if (query.HasSearch)
        {
            tenants = tenants.Where(tenant =>
                EF.Functions.ILike(tenant.FirstName, $"%{query.Search}%")
                || EF.Functions.ILike(tenant.LastName, $"%{query.Search}%")
                || (tenant.Phone != null && EF.Functions.ILike(tenant.Phone, $"%{query.Search}%"))
                || (tenant.NationalId != null && EF.Functions.ILike(tenant.NationalId, $"%{query.Search}%")));
        }

        tenants = ApplySort(tenants, query);

        return tenants.ToPagedResultAsync(query, cancellationToken);
    }

    /// <inheritdoc />
    public Task AddAsync(Tenant tenant, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(tenant);

        _dbContext.Tenants.Add(tenant);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => await _dbContext.SaveChangesAsync(cancellationToken);

    private static IQueryable<Tenant> ApplySort(IQueryable<Tenant> tenants, PagedQuery query)
        => query.SortBy?.ToLowerInvariant() switch
        {
            "lastname" => tenants.OrderByDirection(tenant => tenant.LastName, query.SortDirection),
            "status" => tenants.OrderByDirection(tenant => tenant.Status, query.SortDirection),
            "createdat" => tenants.OrderByDirection(tenant => tenant.CreatedAt, query.SortDirection),
            _ => tenants.OrderByDirection(tenant => tenant.FirstName, query.SortDirection),
        };
}
