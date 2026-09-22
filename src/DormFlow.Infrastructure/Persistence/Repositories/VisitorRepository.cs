using DormFlow.Application.Common.Pagination;
using DormFlow.Application.Features.Visitors;
using DormFlow.Contracts.Common;
using DormFlow.Domain.Entities;
using DormFlow.Infrastructure.Persistence.Extensions;
using Microsoft.EntityFrameworkCore;

namespace DormFlow.Infrastructure.Persistence.Repositories;

/// <summary>ເຂົ້າເຖິງຂໍ້ມູນຜູ້ເຂົ້າຊົມຢູ່ PostgreSQL ຜ່ານ EF Core</summary>
public sealed class VisitorRepository(DormFlowDbContext dbContext) : IVisitorRepository
{
    private readonly DormFlowDbContext _dbContext = dbContext;

    /// <inheritdoc />
    public Task<Visitor?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => _dbContext.Visitors.FirstOrDefaultAsync(visitor => visitor.Id == id, cancellationToken);

    /// <inheritdoc />
    public Task<PagedResult<Visitor>> GetPagedAsync(
        VisitorListFilter filter,
        PagedQuery query,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(filter);

        IQueryable<Visitor> visitors = _dbContext.Visitors.AsNoTracking();

        if (filter.OrganizationId.HasValue)
        {
            visitors = visitors.Where(visitor => visitor.OrganizationId == filter.OrganizationId.Value);
        }

        if (filter.DormitoryId.HasValue)
        {
            visitors = visitors.Where(visitor => visitor.DormitoryId == filter.DormitoryId.Value);
        }

        if (filter.RoomId.HasValue)
        {
            visitors = visitors.Where(visitor => visitor.RoomId == filter.RoomId.Value);
        }

        if (filter.Status.HasValue)
        {
            visitors = visitors.Where(visitor => visitor.Status == filter.Status.Value);
        }

        if (filter.RestrictToDormitoryIds is not null)
        {
            visitors = visitors.Where(visitor =>
                visitor.DormitoryId != null && filter.RestrictToDormitoryIds.Contains(visitor.DormitoryId.Value));
        }

        if (query.HasSearch)
        {
            visitors = visitors.Where(visitor => EF.Functions.ILike(visitor.VisitorName, $"%{query.Search}%"));
        }

        visitors = ApplySort(visitors, query);

        return visitors.ToPagedResultAsync(query, cancellationToken);
    }

    /// <inheritdoc />
    public Task<int> CountAsync(VisitorListFilter filter, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(filter);

        IQueryable<Visitor> visitors = _dbContext.Visitors.AsNoTracking();

        if (filter.OrganizationId.HasValue)
        {
            visitors = visitors.Where(visitor => visitor.OrganizationId == filter.OrganizationId.Value);
        }

        if (filter.DormitoryId.HasValue)
        {
            visitors = visitors.Where(visitor => visitor.DormitoryId == filter.DormitoryId.Value);
        }

        if (filter.RoomId.HasValue)
        {
            visitors = visitors.Where(visitor => visitor.RoomId == filter.RoomId.Value);
        }

        if (filter.Status.HasValue)
        {
            visitors = visitors.Where(visitor => visitor.Status == filter.Status.Value);
        }

        if (filter.RestrictToDormitoryIds is not null)
        {
            visitors = visitors.Where(visitor =>
                visitor.DormitoryId != null && filter.RestrictToDormitoryIds.Contains(visitor.DormitoryId.Value));
        }

        return visitors.CountAsync(cancellationToken);
    }

    /// <inheritdoc />
    public Task AddAsync(Visitor visitor, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(visitor);

        _dbContext.Visitors.Add(visitor);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => await _dbContext.SaveChangesAsync(cancellationToken);

    private static IQueryable<Visitor> ApplySort(IQueryable<Visitor> visitors, PagedQuery query)
        => query.SortBy?.ToLowerInvariant() switch
        {
            "status" => visitors.OrderByDirection(visitor => visitor.Status, query.SortDirection),
            "checkinat" => visitors.OrderByDirection(visitor => visitor.CheckInAt, query.SortDirection),
            _ => visitors.OrderByDirection(visitor => visitor.CreatedAt, query.SortDirection),
        };
}
