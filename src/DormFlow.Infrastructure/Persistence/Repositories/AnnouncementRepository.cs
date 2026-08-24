using DormFlow.Application.Common.Pagination;
using DormFlow.Application.Features.Announcements;
using DormFlow.Contracts.Common;
using DormFlow.Domain.Entities;
using DormFlow.Infrastructure.Persistence.Extensions;
using Microsoft.EntityFrameworkCore;

namespace DormFlow.Infrastructure.Persistence.Repositories;

/// <summary>เข้าถึงข้อมูลประกาศบน PostgreSQL ผ่าน EF Core</summary>
public sealed class AnnouncementRepository(DormFlowDbContext dbContext) : IAnnouncementRepository
{
    private readonly DormFlowDbContext _dbContext = dbContext;

    /// <inheritdoc />
    public Task<Announcement?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => _dbContext.Announcements.FirstOrDefaultAsync(announcement => announcement.Id == id, cancellationToken);

    /// <inheritdoc />
    public Task<PagedResult<Announcement>> GetPagedAsync(
        AnnouncementListFilter filter,
        PagedQuery query,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(filter);

        IQueryable<Announcement> announcements = _dbContext.Announcements.AsNoTracking();

        if (filter.OrganizationId.HasValue)
        {
            announcements = announcements.Where(announcement => announcement.OrganizationId == filter.OrganizationId.Value);
        }

        if (filter.DormitoryId.HasValue)
        {
            announcements = announcements.Where(announcement => announcement.DormitoryId == filter.DormitoryId.Value);
        }

        if (filter.Status.HasValue)
        {
            announcements = announcements.Where(announcement => announcement.Status == filter.Status.Value);
        }

        if (filter.RestrictToDormitoryIds is not null)
        {
            announcements = announcements.Where(announcement =>
                announcement.DormitoryId != null && filter.RestrictToDormitoryIds.Contains(announcement.DormitoryId.Value));
        }

        if (query.HasSearch)
        {
            announcements = announcements.Where(announcement => EF.Functions.ILike(announcement.Title, $"%{query.Search}%"));
        }

        announcements = ApplySort(announcements, query);

        return announcements.ToPagedResultAsync(query, cancellationToken);
    }

    /// <inheritdoc />
    public Task AddAsync(Announcement announcement, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(announcement);

        _dbContext.Announcements.Add(announcement);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => await _dbContext.SaveChangesAsync(cancellationToken);

    private static IQueryable<Announcement> ApplySort(IQueryable<Announcement> announcements, PagedQuery query)
        => query.SortBy?.ToLowerInvariant() switch
        {
            "title" => announcements.OrderByDirection(announcement => announcement.Title, query.SortDirection),
            "status" => announcements.OrderByDirection(announcement => announcement.Status, query.SortDirection),
            "publishedat" => announcements.OrderByDirection(announcement => announcement.PublishedAt, query.SortDirection),
            _ => announcements.OrderByDirection(announcement => announcement.CreatedAt, query.SortDirection),
        };
}
