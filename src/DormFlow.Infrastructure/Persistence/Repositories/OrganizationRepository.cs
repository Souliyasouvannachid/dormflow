using DormFlow.Application.Common.Pagination;
using DormFlow.Application.Features.Organizations;
using DormFlow.Contracts.Common;
using DormFlow.Domain.Entities;
using DormFlow.Infrastructure.Persistence.Extensions;
using Microsoft.EntityFrameworkCore;

namespace DormFlow.Infrastructure.Persistence.Repositories;

/// <summary>ເຂົ້າເຖິງຂໍ້ມູນກ່ຽວກັບບໍລິສັດ PostgreSQL ຜ່ານ EF Core</summary>
public sealed class OrganizationRepository(DormFlowDbContext dbContext) : IOrganizationRepository
{
    private readonly DormFlowDbContext _dbContext = dbContext;

    /// <inheritdoc />
    public Task<Organization?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => _dbContext.Organizations.FirstOrDefaultAsync(organization => organization.Id == id, cancellationToken);

    /// <inheritdoc />
    public Task<bool> ExistsByCodeAsync(
        string code,
        Guid? excludeOrganizationId,
        CancellationToken cancellationToken = default)
        => _dbContext.Organizations.AnyAsync(
            organization => organization.Code == code && organization.Id != excludeOrganizationId,
            cancellationToken);

    /// <inheritdoc />
    public Task<PagedResult<Organization>> GetPagedAsync(
        OrganizationListFilter filter,
        PagedQuery query,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(filter);

        IQueryable<Organization> organizations = _dbContext.Organizations.AsNoTracking();

        if (filter.OrganizationId.HasValue)
        {
            organizations = organizations.Where(organization => organization.Id == filter.OrganizationId.Value);
        }

        if (filter.Status.HasValue)
        {
            organizations = organizations.Where(organization => organization.Status == filter.Status.Value);
        }

        if (query.HasSearch)
        {
            organizations = organizations.Where(organization =>
                EF.Functions.ILike(organization.Name, $"%{query.Search}%")
                || EF.Functions.ILike(organization.Code, $"%{query.Search}%"));
        }

        organizations = ApplySort(organizations, query);

        return organizations.ToPagedResultAsync(query, cancellationToken);
    }

    /// <inheritdoc />
    public Task AddAsync(Organization organization, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(organization);

        _dbContext.Organizations.Add(organization);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public void Remove(Organization organization)
    {
        ArgumentNullException.ThrowIfNull(organization);

        _dbContext.Organizations.Remove(organization);
    }

    /// <inheritdoc />
    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => await _dbContext.SaveChangesAsync(cancellationToken);

    private static IQueryable<Organization> ApplySort(IQueryable<Organization> organizations, PagedQuery query)
        => query.SortBy?.ToLowerInvariant() switch
        {
            "code" => organizations.OrderByDirection(organization => organization.Code, query.SortDirection),
            "status" => organizations.OrderByDirection(organization => organization.Status, query.SortDirection),
            "createdat" => organizations.OrderByDirection(organization => organization.CreatedAt, query.SortDirection),
            _ => organizations.OrderByDirection(organization => organization.Name, query.SortDirection),
        };
}
