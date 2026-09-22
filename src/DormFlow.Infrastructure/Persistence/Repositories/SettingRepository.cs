using DormFlow.Application.Common.Pagination;
using DormFlow.Application.Features.Settings;
using DormFlow.Contracts.Common;
using DormFlow.Domain.Entities;
using DormFlow.Infrastructure.Persistence.Extensions;
using Microsoft.EntityFrameworkCore;

namespace DormFlow.Infrastructure.Persistence.Repositories;

/// <summary>ເຂົ້າເຖິງຂໍ້ມູນການຕັ້ງຄ່າ PostgreSQL ຜ່ານ EF Core</summary>
public sealed class SettingRepository(DormFlowDbContext dbContext) : ISettingRepository
{
    private readonly DormFlowDbContext _dbContext = dbContext;

    /// <inheritdoc />
    public Task<Setting?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => _dbContext.Settings.FirstOrDefaultAsync(setting => setting.Id == id, cancellationToken);

    /// <inheritdoc />
    public Task<Setting?> GetByKeyAsync(Guid organizationId, string key, CancellationToken cancellationToken = default)
        => _dbContext.Settings.FirstOrDefaultAsync(
            setting => setting.OrganizationId == organizationId && setting.Key == key,
            cancellationToken);

    /// <inheritdoc />
    public Task<PagedResult<Setting>> GetPagedAsync(
        SettingListFilter filter,
        PagedQuery query,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(filter);

        IQueryable<Setting> settings = _dbContext.Settings
            .AsNoTracking()
            .Where(setting => setting.OrganizationId == filter.OrganizationId);

        if (query.HasSearch)
        {
            settings = settings.Where(setting => EF.Functions.ILike(setting.Key, $"%{query.Search}%"));
        }

        settings = ApplySort(settings, query);

        return settings.ToPagedResultAsync(query, cancellationToken);
    }

    /// <inheritdoc />
    public Task AddAsync(Setting setting, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(setting);

        _dbContext.Settings.Add(setting);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public void Remove(Setting setting)
    {
        ArgumentNullException.ThrowIfNull(setting);

        _dbContext.Settings.Remove(setting);
    }

    /// <inheritdoc />
    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => await _dbContext.SaveChangesAsync(cancellationToken);

    private static IQueryable<Setting> ApplySort(IQueryable<Setting> settings, PagedQuery query)
        => query.SortBy?.ToLowerInvariant() switch
        {
            "key" => settings.OrderByDirection(setting => setting.Key, query.SortDirection),
            "updatedat" => settings.OrderByDirection(setting => setting.UpdatedAt, query.SortDirection),
            _ => settings.OrderByDirection(setting => setting.CreatedAt, query.SortDirection),
        };
}
