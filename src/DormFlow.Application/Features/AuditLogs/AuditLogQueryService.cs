using DormFlow.Application.Common.Abstractions;
using DormFlow.Application.Common.Authorization;
using DormFlow.Application.Common.Pagination;
using DormFlow.Contracts.AuditLogs;
using DormFlow.Contracts.Common;
using DormFlow.Domain.Entities;
using DormFlow.Domain.Enums;

namespace DormFlow.Application.Features.AuditLogs;

/// <summary>Use Case ทั้งหมดของโมดูล Audit Log</summary>
public sealed class AuditLogQueryService(
    IAuditLogRepository repository,
    ICurrentUser currentUser) : IAuditLogQueryService
{
    private static readonly string[] AllowedSortFields = ["createdAt", "action", "entityName"];

    private readonly IAuditLogRepository _repository = repository;
    private readonly ICurrentUser _currentUser = currentUser;

    /// <inheritdoc />
    public async Task<PagedResult<AuditLogResponse>> GetPagedAsync(
        AuditLogListRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        PagedQuery query = PagedQueryNormalizer.Normalize(request, AllowedSortFields, defaultSortBy: "createdAt");

        // เหตุการณ์ล่าสุดควรอยู่บนสุดเสมอเมื่อผู้เรียกไม่ได้ระบุการเรียงลำดับมาเอง
        if (string.IsNullOrWhiteSpace(request.SortBy))
        {
            query = query with { SortDirection = SortDirection.Desc };
        }

        Guid? organizationId = _currentUser.IsSuperAdmin ? request.OrganizationId : _currentUser.OrganizationId;

        AuditLogListFilter filter = new(
            organizationId,
            request.DormitoryId,
            request.UserId,
            ParseActionOrNull(request.Action),
            request.EntityName,
            request.From,
            request.To,
            TenantScope.GetDormitoryRestrictionForListing(_currentUser));

        PagedResult<AuditLog> result = await _repository.GetPagedAsync(filter, query, cancellationToken);

        return PagedResult.Create(
            (IReadOnlyList<AuditLogResponse>)[.. result.Items.Select(ToResponse)],
            result.Page,
            result.PageSize,
            result.TotalItems);
    }

    private static AuditAction? ParseActionOrNull(string? action)
        => !string.IsNullOrWhiteSpace(action) && Enum.TryParse(action, ignoreCase: true, out AuditAction parsed)
            ? parsed
            : null;

    private static AuditLogResponse ToResponse(AuditLog auditLog) => new()
    {
        Id = auditLog.Id,
        OrganizationId = auditLog.OrganizationId,
        DormitoryId = auditLog.DormitoryId,
        UserId = auditLog.UserId,
        Action = auditLog.Action.ToString(),
        EntityName = auditLog.EntityName,
        EntityId = auditLog.EntityId,
        OldValues = auditLog.OldValues,
        NewValues = auditLog.NewValues,
        IpAddress = auditLog.IpAddress,
        UserAgent = auditLog.UserAgent,
        RequestPath = auditLog.RequestPath,
        CreatedAt = auditLog.CreatedAt,
    };
}
