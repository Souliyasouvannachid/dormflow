using DormFlow.Application.Features.Permissions;
using DormFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DormFlow.Infrastructure.Persistence.Repositories;

/// <summary>ເຂົ້າເຖິງລາຍຊື່ການອະນຸຍາດທີ່ເປີດ PostgreSQL ຜ່ານ EF Core</summary>
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
