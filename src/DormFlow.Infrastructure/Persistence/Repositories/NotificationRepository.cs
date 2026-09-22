using DormFlow.Application.Common.Pagination;
using DormFlow.Application.Features.Notifications;
using DormFlow.Contracts.Common;
using DormFlow.Domain.Entities;
using DormFlow.Infrastructure.Persistence.Extensions;
using Microsoft.EntityFrameworkCore;

namespace DormFlow.Infrastructure.Persistence.Repositories;

/// <summary>ເຂົ້າເຖິງຂໍ້ມູນການແຈ້ງເຕືອນເປີດ PostgreSQL ຜ່ານ EF Core</summary>
public sealed class NotificationRepository(DormFlowDbContext dbContext) : INotificationRepository
{
    private readonly DormFlowDbContext _dbContext = dbContext;

    /// <inheritdoc />
    public Task<Notification?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => _dbContext.Notifications.FirstOrDefaultAsync(notification => notification.Id == id, cancellationToken);

    /// <inheritdoc />
    public Task<PagedResult<Notification>> GetPagedAsync(
        NotificationListFilter filter,
        PagedQuery query,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(filter);

        IQueryable<Notification> notifications = _dbContext.Notifications
            .AsNoTracking()
            .Where(notification => notification.RecipientUserId == filter.RecipientUserId);

        if (filter.IsRead.HasValue)
        {
            notifications = notifications.Where(notification => notification.IsRead == filter.IsRead.Value);
        }

        if (query.HasSearch)
        {
            notifications = notifications.Where(notification => EF.Functions.ILike(notification.Title, $"%{query.Search}%"));
        }

        notifications = ApplySort(notifications, query);

        return notifications.ToPagedResultAsync(query, cancellationToken);
    }

    /// <inheritdoc />
    public Task AddAsync(Notification notification, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(notification);

        _dbContext.Notifications.Add(notification);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => await _dbContext.SaveChangesAsync(cancellationToken);

    private static IQueryable<Notification> ApplySort(IQueryable<Notification> notifications, PagedQuery query)
        => query.SortBy?.ToLowerInvariant() switch
        {
            "isread" => notifications.OrderByDirection(notification => notification.IsRead, query.SortDirection),
            _ => notifications.OrderByDirection(notification => notification.CreatedAt, query.SortDirection),
        };
}
