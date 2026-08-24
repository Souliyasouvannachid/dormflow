using DormFlow.Application.Features.Permissions;
using DormFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DormFlow.Infrastructure.Persistence.Repositories;

/// <summary>เข้าถึงรายการสิทธิ์บน PostgreSQL ผ่าน EF Core</summary>
public sealed class PermissionRepository(DormFlowDbContext dbContext) : IPermissionRepository
{
    private readonly DormFlowDbContext _dbContext = dbContext;

    /// <inheritdoc />
    public async Task<IReadOnlyList<Permission>> GetAllAsync(CancellationToken cancellationToken = default)
        => await _dbContext.Permissions
            .AsNoTracking()
            .OrderBy(permission => permission.Module)
            .ThenBy(permission => permission.Code)
            .ToListAsync(cancellationToken);
}
