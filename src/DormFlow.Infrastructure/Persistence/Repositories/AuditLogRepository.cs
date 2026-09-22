using DormFlow.Application.Common.Pagination;
using DormFlow.Application.Features.AuditLogs;
using DormFlow.Contracts.Common;
using DormFlow.Domain.Entities;
using DormFlow.Infrastructure.Persistence.Extensions;
using Microsoft.EntityFrameworkCore;

namespace DormFlow.Infrastructure.Persistence.Repositories;

/// <summary>ເຂົ້າເຖິງຂໍ້ມູນ Audit Log ສຸດ PostgreSQL ຜ່ານ EF Core</summary>
public sealed class AuditLogRepository(DormFlowDbContext dbContext) : IAuditLogRepository
{
    private readonly DormFlowDbContext _dbContext = dbContext;

    /// <inheritdoc />
    public Task<PagedResult<AuditLog>> GetPagedAsync(
        AuditLogListFilter filter,
        PagedQuery query,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(filter);

        IQueryable<AuditLog> auditLogs = _dbContext.AuditLogs.AsNoTracking();

        if (filter.OrganizationId.HasValue)
        {
            auditLogs = auditLogs.Where(log => log.OrganizationId == filter.OrganizationId.Value);
        }

        if (filter.DormitoryId.HasValue)
        {
            auditLogs = auditLogs.Where(log => log.DormitoryId == filter.DormitoryId.Value);
        }

        if (filter.UserId.HasValue)
        {
            auditLogs = auditLogs.Where(log => log.UserId == filter.UserId.Value);
        }

        if (filter.Action.HasValue)
        {
            auditLogs = auditLogs.Where(log => log.Action == filter.Action.Value);
        }

        if (!string.IsNullOrWhiteSpace(filter.EntityName))
        {
            auditLogs = auditLogs.Where(log => log.EntityName == filter.EntityName);
        }

        if (filter.From.HasValue)
        {
            auditLogs = auditLogs.Where(log => log.CreatedAt >= filter.From.Value);
        }

        if (filter.To.HasValue)
        {
            auditLogs = auditLogs.Where(log => log.CreatedAt <= filter.To.Value);
        }

        if (filter.RestrictToDormitoryIds is not null)
        {
            IReadOnlyCollection<Guid> allowedDormitoryIds = filter.RestrictToDormitoryIds;
            auditLogs = auditLogs.Where(log =>
                log.DormitoryId != null && allowedDormitoryIds.Contains(log.DormitoryId.Value));
        }

        if (query.HasSearch)
        {
            auditLogs = auditLogs.Where(log =>
                EF.Functions.ILike(log.EntityName, $"%{query.Search}%")
                || (log.EntityId != null && EF.Functions.ILike(log.EntityId, $"%{query.Search}%")));
        }

        auditLogs = ApplySort(auditLogs, query);

        return auditLogs.ToPagedResultAsync(query, cancellationToken);
    }

    private static IQueryable<AuditLog> ApplySort(IQueryable<AuditLog> auditLogs, PagedQuery query)
        => query.SortBy?.ToLowerInvariant() switch
        {
            "action" => auditLogs.OrderByDirection(log => log.Action, query.SortDirection),
            "entityname" => auditLogs.OrderByDirection(log => log.EntityName, query.SortDirection),
            _ => auditLogs.OrderByDirection(log => log.CreatedAt, query.SortDirection),
        };
}
