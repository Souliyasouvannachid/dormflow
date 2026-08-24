using DormFlow.Application.Common.Abstractions;
using DormFlow.Application.Common.Authorization;
using DormFlow.Domain.Entities;
using DormFlow.Domain.Enums;
using DormFlow.Infrastructure.Identity;
using DormFlow.Infrastructure.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DormFlow.Infrastructure.Persistence;

/// <summary>
/// สร้างข้อมูลตั้งต้นที่ระบบต้องมีจึงจะทำงานได้ ได้แก่ สิทธิ์ บทบาท และบัญชีผู้ดูแลระบบ
/// </summary>
/// <remarks>
/// ทำงานได้หลายรอบโดยไม่สร้างข้อมูลซ้ำ จึงเรียกทุกครั้งที่แอปพลิเคชันเริ่มทำงานได้อย่างปลอดภัย
/// สิทธิ์ที่ถูกถอนออกจากบทบาทโดยผู้ดูแลจะไม่ถูกเพิ่มกลับ เพราะระบบเพิ่มสิทธิ์เริ่มต้น
/// เฉพาะตอนที่บทบาทนั้นเพิ่งถูกสร้างขึ้นใหม่เท่านั้น
/// </remarks>
public sealed class DatabaseSeeder(
    DormFlowDbContext dbContext,
    RoleManager<ApplicationRole> roleManager,
    UserManager<ApplicationUser> userManager,
    IDateTimeProvider dateTimeProvider,
    IOptions<SeedOptions> seedOptions,
    ILogger<DatabaseSeeder> logger)
{
    private static readonly Action<ILogger, int, Exception?> PermissionsSeededCallback =
        LoggerMessage.Define<int>(
            LogLevel.Information,
            new EventId(4000, "PermissionsSeeded"),
            "เพิ่มสิทธิ์ใหม่ {Count} รายการลงฐานข้อมูล");

    private static readonly Action<ILogger, string, Exception?> RoleCreatedCallback =
        LoggerMessage.Define<string>(
            LogLevel.Information,
            new EventId(4001, "RoleCreated"),
            "สร้างบทบาท {RoleName} พร้อมสิทธิ์เริ่มต้นแล้ว");

    private static readonly Action<ILogger, Exception?> AdminSkippedCallback =
        LoggerMessage.Define(
            LogLevel.Warning,
            new EventId(4002, "AdminSeedSkipped"),
            $"ข้ามการสร้างบัญชีผู้ดูแลระบบเพราะยังไม่ได้กำหนด {SeedOptions.AdminEmailVariable} " +
            $"และ {SeedOptions.AdminPasswordVariable}");

    private static readonly Action<ILogger, Exception?> AdminCreatedCallback =
        LoggerMessage.Define(
            LogLevel.Information,
            new EventId(4003, "AdminCreated"),
            "สร้างบัญชีผู้ดูแลระบบเริ่มต้นเรียบร้อยแล้ว");

    private static readonly Action<ILogger, string, Exception?> AdminCreationFailedCallback =
        LoggerMessage.Define<string>(
            LogLevel.Error,
            new EventId(4004, "AdminCreationFailed"),
            "สร้างบัญชีผู้ดูแลระบบไม่สำเร็จ {Errors}");

    private readonly DormFlowDbContext _dbContext = dbContext;
    private readonly RoleManager<ApplicationRole> _roleManager = roleManager;
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly IDateTimeProvider _dateTimeProvider = dateTimeProvider;
    private readonly SeedOptions _seedOptions = seedOptions.Value;
    private readonly ILogger<DatabaseSeeder> _logger = logger;

    /// <summary>สร้างข้อมูลตั้งต้นทั้งหมด</summary>
    /// <param name="cancellationToken">โทเคนสำหรับยกเลิกคำสั่ง</param>
    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        await SeedPermissionsAsync(cancellationToken);
        await SeedRolesAsync(cancellationToken);
        await SeedSuperAdminAsync(cancellationToken);
    }

    /// <summary>เพิ่มสิทธิ์ที่ยังไม่มีในฐานข้อมูล</summary>
    private async Task SeedPermissionsAsync(CancellationToken cancellationToken)
    {
        HashSet<string> existingCodes = await _dbContext.Permissions
            .Select(permission => permission.Code)
            .ToHashSetAsync(StringComparer.Ordinal, cancellationToken);

        List<Permission> newPermissions =
        [
            .. PermissionCodes.All
                .Where(definition => !existingCodes.Contains(definition.Code))
                .Select(definition => new Permission
                {
                    Code = definition.Code,
                    Name = definition.Name,
                    Description = definition.Description,
                    Module = definition.Module,
                }),
        ];

        if (newPermissions.Count == 0)
        {
            return;
        }

        _dbContext.Permissions.AddRange(newPermissions);
        await _dbContext.SaveChangesAsync(cancellationToken);

        PermissionsSeededCallback(_logger, newPermissions.Count, null);
    }

    /// <summary>สร้างบทบาทมาตรฐานพร้อมสิทธิ์เริ่มต้น</summary>
    private async Task SeedRolesAsync(CancellationToken cancellationToken)
    {
        Dictionary<string, Guid> permissionIdsByCode = await _dbContext.Permissions
            .ToDictionaryAsync(
                permission => permission.Code,
                permission => permission.Id,
                StringComparer.Ordinal,
                cancellationToken);

        foreach (string roleName in DormFlowRoles.All)
        {
            if (await _roleManager.RoleExistsAsync(roleName))
            {
                continue;
            }

            ApplicationRole role = new(roleName)
            {
                Description = $"บทบาทมาตรฐานของระบบ: {roleName}",
                IsSystemRole = true,
            };

            IdentityResult result = await _roleManager.CreateAsync(role);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException(
                    $"สร้างบทบาท {roleName} ไม่สำเร็จ: {string.Join(", ", result.Errors.Select(error => error.Description))}");
            }

            await GrantDefaultPermissionsAsync(role, permissionIdsByCode, cancellationToken);

            RoleCreatedCallback(_logger, roleName, null);
        }
    }

    private async Task GrantDefaultPermissionsAsync(
        ApplicationRole role,
        Dictionary<string, Guid> permissionIdsByCode,
        CancellationToken cancellationToken)
    {
        DateTimeOffset now = _dateTimeProvider.UtcNow;

        List<RolePermission> rolePermissions =
        [
            .. RolePermissionDefaults.ForRole(role.Name!)
                .Where(permissionIdsByCode.ContainsKey)
                .Select(code => new RolePermission
                {
                    RoleId = role.Id,
                    PermissionId = permissionIdsByCode[code],
                    GrantedAt = now,
                }),
        ];

        if (rolePermissions.Count == 0)
        {
            return;
        }

        _dbContext.RolePermissions.AddRange(rolePermissions);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// สร้างบัญชีผู้ดูแลระบบเริ่มต้นจากค่าใน environment variable
    /// </summary>
    /// <remarks>
    /// ถ้าไม่ได้กำหนดค่าไว้ ระบบจะข้ามขั้นตอนนี้และบันทึกคำเตือน
    /// จะไม่มีการสร้างบัญชีที่ฝังรหัสผ่านไว้ในโค้ดเด็ดขาด
    /// </remarks>
    private async Task SeedSuperAdminAsync(CancellationToken cancellationToken)
    {
        if (!_seedOptions.HasAdminCredentials)
        {
            AdminSkippedCallback(_logger, null);
            return;
        }

        string email = _seedOptions.AdminEmail!.Trim();

        if (await _userManager.FindByEmailAsync(email) is not null)
        {
            return;
        }

        ApplicationUser admin = new()
        {
            UserName = email,
            Email = email,
            EmailConfirmed = true,
            FirstName = string.IsNullOrWhiteSpace(_seedOptions.AdminFirstName) ? "System" : _seedOptions.AdminFirstName.Trim(),
            LastName = string.IsNullOrWhiteSpace(_seedOptions.AdminLastName) ? "Administrator" : _seedOptions.AdminLastName.Trim(),
            Status = UserStatus.Active,

            // ผู้ดูแลระบบสูงสุดไม่ผูกกับองค์กรใด จึงเข้าถึงข้อมูลข้ามองค์กรได้
            OrganizationId = null,
        };

        IdentityResult createResult = await _userManager.CreateAsync(admin, _seedOptions.AdminPassword!);

        if (!createResult.Succeeded)
        {
            AdminCreationFailedCallback(
                _logger,
                string.Join(", ", createResult.Errors.Select(error => error.Description)),
                null);

            return;
        }

        IdentityResult roleResult = await _userManager.AddToRoleAsync(admin, DormFlowRoles.SuperAdmin);

        if (!roleResult.Succeeded)
        {
            throw new InvalidOperationException(
                $"เพิ่มบทบาท SuperAdmin ให้บัญชีผู้ดูแลไม่สำเร็จ: {string.Join(", ", roleResult.Errors.Select(error => error.Description))}");
        }

        AdminCreatedCallback(_logger, null);

        _ = cancellationToken;
    }
}
