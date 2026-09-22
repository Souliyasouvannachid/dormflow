using DormFlow.Application.Common.Pagination;
using DormFlow.Application.Features.Maintenance;
using DormFlow.Contracts.Common;
using DormFlow.Domain.Entities;
using DormFlow.Domain.Enums;
using DormFlow.Infrastructure.Persistence.Extensions;
using Microsoft.EntityFrameworkCore;

namespace DormFlow.Infrastructure.Persistence.Repositories;

/// <summary>ເຂົ້າເຖິງຂໍ້ມູນການບຳລຸງຮັກສາຢູ່ PostgreSQL ຜ່ານ EF Core</summary>
public sealed class MaintenanceRequestRepository(DormFlowDbContext dbContext) : IMaintenanceRequestRepository
{
    private static readonly MaintenanceStatus[] OpenStatuses =
    [
        MaintenanceStatus.Reported,
        MaintenanceStatus.Assigned,
        MaintenanceStatus.InProgress,
    ];

    private readonly DormFlowDbContext _dbContext = dbContext;

    /// <inheritdoc />
    public Task<MaintenanceRequest?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => _dbContext.MaintenanceRequests.FirstOrDefaultAsync(request => request.Id == id, cancellationToken);

    /// <inheritdoc />
    public Task<bool> HasOpenRequestForRoomAsync(
        Guid roomId,
        Guid excludeRequestId,
        CancellationToken cancellationToken = default)
        => _dbContext.MaintenanceRequests.AnyAsync(
            request => request.RoomId == roomId
                && request.Id != excludeRequestId
                && OpenStatuses.Contains(request.Status),
            cancellationToken);

    /// <inheritdoc />
    public Task<PagedResult<MaintenanceRequest>> GetPagedAsync(
        MaintenanceRequestListFilter filter,
        PagedQuery query,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(filter);

        IQueryable<MaintenanceRequest> requests = _dbContext.MaintenanceRequests.AsNoTracking();

        if (filter.OrganizationId.HasValue)
        {
            requests = requests.Where(request => request.OrganizationId == filter.OrganizationId.Value);
        }

        if (filter.DormitoryId.HasValue)
        {
            requests = requests.Where(request => request.DormitoryId == filter.DormitoryId.Value);
        }

        if (filter.RoomId.HasValue)
        {
            requests = requests.Where(request => request.RoomId == filter.RoomId.Value);
        }

        if (filter.Status.HasValue)
        {
            requests = requests.Where(request => request.Status == filter.Status.Value);
        }

        if (filter.Priority.HasValue)
        {
            requests = requests.Where(request => request.Priority == filter.Priority.Value);
        }

        if (filter.AssignedTechnicianId.HasValue)
        {
            requests = requests.Where(request => request.AssignedTechnicianId == filter.AssignedTechnicianId.Value);
        }

        if (filter.RestrictToDormitoryIds is not null)
        {
            requests = requests.Where(request =>
                request.DormitoryId != null && filter.RestrictToDormitoryIds.Contains(request.DormitoryId.Value));
        }

        if (query.HasSearch)
        {
            requests = requests.Where(request => EF.Functions.ILike(request.Title, $"%{query.Search}%"));
        }

        requests = ApplySort(requests, query);

        return requests.ToPagedResultAsync(query, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyDictionary<MaintenanceStatus, int>> GetStatusCountsAsync(
        MaintenanceRequestListFilter filter,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(filter);

        IQueryable<MaintenanceRequest> requests = _dbContext.MaintenanceRequests.AsNoTracking();

        if (filter.OrganizationId.HasValue)
        {
            requests = requests.Where(request => request.OrganizationId == filter.OrganizationId.Value);
        }

        if (filter.DormitoryId.HasValue)
        {
            requests = requests.Where(request => request.DormitoryId == filter.DormitoryId.Value);
        }

        if (filter.RoomId.HasValue)
        {
            requests = requests.Where(request => request.RoomId == filter.RoomId.Value);
        }

        if (filter.RestrictToDormitoryIds is not null)
        {
            requests = requests.Where(request =>
                request.DormitoryId != null && filter.RestrictToDormitoryIds.Contains(request.DormitoryId.Value));
        }

        return await requests
            .Where(request => OpenStatuses.Contains(request.Status))
            .GroupBy(request => request.Status)
            .Select(group => new { group.Key, Count = group.Count() })
            .ToDictionaryAsync(item => item.Key, item => item.Count, cancellationToken);
    }

    /// <inheritdoc />
    public Task AddAsync(MaintenanceRequest maintenanceRequest, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(maintenanceRequest);

        _dbContext.MaintenanceRequests.Add(maintenanceRequest);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => await _dbContext.SaveChangesAsync(cancellationToken);

    private static IQueryable<MaintenanceRequest> ApplySort(IQueryable<MaintenanceRequest> requests, PagedQuery query)
        => query.SortBy?.ToLowerInvariant() switch
        {
            "priority" => requests.OrderByDirection(request => request.Priority, query.SortDirection),
            "status" => requests.OrderByDirection(request => request.Status, query.SortDirection),
            _ => requests.OrderByDirection(request => request.CreatedAt, query.SortDirection),
        };
}
