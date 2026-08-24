using DormFlow.Application.Common.Models;
using DormFlow.Application.Common.Pagination;
using DormFlow.Application.Features.Roles;
using DormFlow.Contracts.Common;
using DormFlow.Domain.Entities;
using DormFlow.Infrastructure.Persistence;
using DormFlow.Infrastructure.Persistence.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace DormFlow.Infrastructure.Identity;

/// <summary>เข้าถึงและจัดการบทบาทบน ASP.NET Core Identity</summary>
public sealed class RoleRepository(
    RoleManager<ApplicationRole> roleManager,
    DormFlowDbContext dbContext) : IRoleRepository
{
    private readonly RoleManager<ApplicationRole> _roleManager = roleManager;
    private readonly DormFlowDbContext _dbContext = dbContext;

    /// <inheritdoc />
    public async Task<bool> ExistsByNameAsync(
        string name,
        Guid? excludeRoleId,
        CancellationToken cancellationToken = default)
    {
        string normalized = _roleManager.NormalizeKey(name);

        return await _dbContext.Roles.AnyAsync(
            role => role.NormalizedName == normalized && role.Id != excludeRoleId,
            cancellationToken);
    }

    /// <inheritdoc />
    public async Task<RoleInfo?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        ApplicationRole? role = await _dbContext.Roles
            .AsNoTracking()
            .FirstOrDefaultAsync(candidate => candidate.Id == id, cancellationToken);

        return role is null ? null : await BuildRoleInfoAsync(role, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<PagedResult<RoleInfo>> GetPagedAsync(
        PagedQuery query,
        CancellationToken cancellationToken = default)
    {
        IQueryable<ApplicationRole> roles = _dbContext.Roles.AsNoTracking();

        if (query.HasSearch)
        {
            roles = roles.Where(role => role.Name != null && EF.Functions.ILike(role.Name, $"%{query.Search}%"));
        }

        roles = roles.OrderByDirection(role => role.Name, query.SortDirection);

        long totalItems = await roles.LongCountAsync(cancellationToken);

        if (totalItems == 0)
        {
            return PagedResult.Empty<RoleInfo>(query.Page, query.PageSize);
        }

        List<ApplicationRole> pageOfRoles = await roles.ApplyPaging(query).ToListAsync(cancellationToken);

        List<RoleInfo> roleInfos = [];
        foreach (ApplicationRole role in pageOfRoles)
        {
            roleInfos.Add(await BuildRoleInfoAsync(role, cancellationToken));
        }

        return PagedResult.Create((IReadOnlyList<RoleInfo>)roleInfos, query.Page, query.PageSize, totalItems);
    }

    /// <inheritdoc />
    public async Task<RoleOperationResult> CreateAsync(
        string name,
        string? description,
        CancellationToken cancellationToken = default)
    {
        ApplicationRole role = new(name)
        {
            Description = description,
            IsSystemRole = false,
        };

        IdentityResult result = await _roleManager.CreateAsync(role);

        return result.Succeeded
            ? RoleOperationResult.Success(role.Id)
            : RoleOperationResult.Failed([.. result.Errors.Select(error => error.Description)]);
    }

    /// <inheritdoc />
    public async Task<bool> UpdateAsync(
        Guid id,
        string name,
        string? description,
        CancellationToken cancellationToken = default)
    {
        ApplicationRole? role = await _roleManager.FindByIdAsync(id.ToString());
        if (role is null)
        {
            return false;
        }

        role.Description = description;

        if (!string.Equals(role.Name, name, StringComparison.Ordinal))
        {
            IdentityResult renameResult = await _roleManager.SetRoleNameAsync(role, name);
            if (!renameResult.Succeeded)
            {
                return false;
            }
        }

        IdentityResult updateResult = await _roleManager.UpdateAsync(role);
        return updateResult.Succeeded;
    }

    /// <inheritdoc />
    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        ApplicationRole? role = await _roleManager.FindByIdAsync(id.ToString());
        if (role is null)
        {
            return false;
        }

        IdentityResult result = await _roleManager.DeleteAsync(role);
        return result.Succeeded;
    }

    /// <inheritdoc />
    public async Task<bool> SetPermissionsAsync(
        Guid roleId,
        IReadOnlyList<string> permissionCodes,
        Guid? grantedBy,
        DateTimeOffset grantedAt,
        CancellationToken cancellationToken = default)
    {
        bool roleExists = await _dbContext.Roles.AnyAsync(role => role.Id == roleId, cancellationToken);
        if (!roleExists)
        {
            return false;
        }

        List<Guid> permissionIds = await _dbContext.Permissions
            .Where(permission => permissionCodes.Contains(permission.Code))
            .Select(permission => permission.Id)
            .ToListAsync(cancellationToken);

        if (permissionIds.Count != permissionCodes.Distinct(StringComparer.Ordinal).Count())
        {
            // มีรหัสสิทธิ์ที่ระบุมาไม่ตรงกับที่มีอยู่จริงในฐานข้อมูล
            return false;
        }

        // ลบสิทธิ์เดิมและเพิ่มสิทธิ์ใหม่ต้องสำเร็จหรือล้มเหลวไปด้วยกัน ไม่เช่นนั้นบทบาทจะเหลือสิทธิ์ว่างชั่วขณะ
        // ต้องครอบด้วย IExecutionStrategy ไม่ใช่ BeginTransactionAsync ตรง ๆ
        // เพราะ EnableRetryOnFailure ทำให้ Npgsql ใช้ Retrying Execution Strategy ซึ่งไม่รองรับธุรกรรมที่ผู้เรียกเปิดเอง
        IExecutionStrategy executionStrategy = _dbContext.Database.CreateExecutionStrategy();

        await executionStrategy.ExecuteAsync(async () =>
        {
            await using IDbContextTransaction transaction =
                await _dbContext.Database.BeginTransactionAsync(cancellationToken);

            await _dbContext.RolePermissions
                .Where(rolePermission => rolePermission.RoleId == roleId)
                .ExecuteDeleteAsync(cancellationToken);

            if (permissionIds.Count > 0)
            {
                _dbContext.RolePermissions.AddRange(permissionIds.Select(permissionId => new RolePermission
                {
                    RoleId = roleId,
                    PermissionId = permissionId,
                    GrantedAt = grantedAt,
                    GrantedBy = grantedBy,
                }));

                await _dbContext.SaveChangesAsync(cancellationToken);
            }

            await transaction.CommitAsync(cancellationToken);
        });

        return true;
    }

    private async Task<RoleInfo> BuildRoleInfoAsync(ApplicationRole role, CancellationToken cancellationToken)
    {
        string[] permissions = await _dbContext.RolePermissions
            .Where(rolePermission => rolePermission.RoleId == role.Id)
            .Select(rolePermission => rolePermission.Permission!.Code)
            .OrderBy(code => code)
            .ToArrayAsync(cancellationToken);

        return new RoleInfo(role.Id, role.Name ?? string.Empty, role.Description, role.IsSystemRole, permissions);
    }
}
