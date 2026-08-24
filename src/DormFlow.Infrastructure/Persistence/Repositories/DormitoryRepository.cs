using DormFlow.Application.Common.Pagination;
using DormFlow.Application.Features.Dormitories;
using DormFlow.Contracts.Common;
using DormFlow.Domain.Entities;
using DormFlow.Infrastructure.Persistence.Extensions;
using Microsoft.EntityFrameworkCore;

namespace DormFlow.Infrastructure.Persistence.Repositories;

/// <summary>เข้าถึงข้อมูลหอพักบน PostgreSQL ผ่าน EF Core</summary>
public sealed class DormitoryRepository(DormFlowDbContext dbContext) : IDormitoryRepository
{
    private readonly DormFlowDbContext _dbContext = dbContext;

    /// <inheritdoc />
    public Task<Dormitory?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => _dbContext.Dormitories.FirstOrDefaultAsync(dormitory => dormitory.Id == id, cancellationToken);

    /// <inheritdoc />
    public Task<bool> ExistsByCodeAsync(
        Guid organizationId,
        string code,
        Guid? excludeDormitoryId,
        CancellationToken cancellationToken = default)
        => _dbContext.Dormitories.AnyAsync(
            dormitory => dormitory.OrganizationId == organizationId
                && dormitory.Code == code
                && dormitory.Id != excludeDormitoryId,
            cancellationToken);

    /// <inheritdoc />
    public Task<PagedResult<Dormitory>> GetPagedAsync(
        DormitoryListFilter filter,
        PagedQuery query,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(filter);

        IQueryable<Dormitory> dormitories = _dbContext.Dormitories.AsNoTracking();

        if (filter.OrganizationId.HasValue)
        {
            dormitories = dormitories.Where(dormitory => dormitory.OrganizationId == filter.OrganizationId.Value);
        }

        if (filter.Status.HasValue)
        {
            dormitories = dormitories.Where(dormitory => dormitory.Status == filter.Status.Value);
        }

        if (filter.RestrictToIds is not null)
        {
            dormitories = dormitories.Where(dormitory => filter.RestrictToIds.Contains(dormitory.Id));
        }

        if (query.HasSearch)
        {
            dormitories = dormitories.Where(dormitory =>
                EF.Functions.ILike(dormitory.Name, $"%{query.Search}%")
                || EF.Functions.ILike(dormitory.Code, $"%{query.Search}%"));
        }

        dormitories = ApplySort(dormitories, query);

        return dormitories.ToPagedResultAsync(query, cancellationToken);
    }

    /// <inheritdoc />
    public Task AddAsync(Dormitory dormitory, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(dormitory);

        _dbContext.Dormitories.Add(dormitory);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public void Remove(Dormitory dormitory)
    {
        ArgumentNullException.ThrowIfNull(dormitory);

        _dbContext.Dormitories.Remove(dormitory);
    }

    /// <inheritdoc />
    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => await _dbContext.SaveChangesAsync(cancellationToken);

    private static IQueryable<Dormitory> ApplySort(IQueryable<Dormitory> dormitories, PagedQuery query)
        => query.SortBy?.ToLowerInvariant() switch
        {
            "code" => dormitories.OrderByDirection(dormitory => dormitory.Code, query.SortDirection),
            "status" => dormitories.OrderByDirection(dormitory => dormitory.Status, query.SortDirection),
            "createdat" => dormitories.OrderByDirection(dormitory => dormitory.CreatedAt, query.SortDirection),
            _ => dormitories.OrderByDirection(dormitory => dormitory.Name, query.SortDirection),
        };
}
