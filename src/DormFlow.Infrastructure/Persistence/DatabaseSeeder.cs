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
/// ສ້າງຂໍ້ມູນເລີ່ມຕົ້ນທີ່ລະບົບຈະຕ້ອງເຮັດວຽກ, ລວມທັງການອະນຸຍາດ, ບົດບາດ, ແລະບັນຊີຜູ້ເບິ່ງແຍງລະບົບ
/// </summary>
/// <remarks>
/// ສາມາດເຮັດວຽກໄດ້ຫຼາຍຄັ້ງໂດຍບໍ່ຕ້ອງຊໍ້າກັນຂໍ້ມູນ. ມັນຖືກເອີ້ນວ່າດັ່ງນັ້ນທຸກຄັ້ງທີ່ຄໍາຮ້ອງສະຝັກໄດ້ຖືກເລີ່ມຕົ້ນຢ່າງປອດໄພ
/// ການອະນຸຍາດທີ່ຖືກເອົາອອກຈາກບົດບາດໂດຍຜູ້ເບິ່ງແຍງລະບົບຈະບໍ່ຖືກເພີ່ມຄືນ. ເນື່ອງຈາກວ່າລະບົບເພີ່ມການອະນຸຍາດເລີ່ມຕົ້ນ
/// ພຽງແຕ່ໃນເວລາທີ່ບົດບາດໄດ້ຖືກສ້າງຕັ້ງຂື້ນໃຝ່
/// </remarks>
public sealed class DatabaseSeeder(
    DormFlowDbContext dbContext,
    RoleManager<ApplicationRole> roleManager,
    UserManager<ApplicationUser> userManager,
    IDateTimeProvider dateTimeProvider,
    IOptions<SeedOptions> seedOptions,
    ILogger<DatabaseSeeder> logger)
{
    private const string SeedAdvisoryLockName = "dormflow:database-seeder";

    private static readonly Action<ILogger, int, Exception?> PermissionsSeededCallback =
        LoggerMessage.Define<int>(
            LogLevel.Information,
            new EventId(4000, "PermissionsSeeded"),
            "ເພີ່ມສິດໃໝ່ {Count} ລາຍການລົງຖານຂໍ້ມູນ");

    private static readonly Action<ILogger, string, Exception?> RoleCreatedCallback =
        LoggerMessage.Define<string>(
            LogLevel.Information,
            new EventId(4001, "RoleCreated"),
            "ສ້າງບົດບາດ {RoleName} ພ້ອມສິດເລີ່ມຕົ້ນແລ້ວ");

    private static readonly Action<ILogger, Exception?> AdminSkippedCallback =
        LoggerMessage.Define(
            LogLevel.Warning,
            new EventId(4002, "AdminSeedSkipped"),
            $"ຂ້າມການສ້າງບັນຊີຜູ້ດູແລລະບົບ ເນື່ອງຈາກຍັງບໍ່ໄດ້ກຳນົດ {SeedOptions.AdminEmailVariable} " +
            $"ແລະ {SeedOptions.AdminPasswordVariable}");

    private static readonly Action<ILogger, Exception?> AdminCreatedCallback =
        LoggerMessage.Define(
            LogLevel.Information,
            new EventId(4003, "AdminCreated"),
            "ສ້າງບັນຊີຜູ້ດູແລລະບົບເລີ່ມຕົ້ນຮຽບຮ້ອຍແລ້ວ");

    private static readonly Action<ILogger, string, Exception?> AdminCreationFailedCallback =
        LoggerMessage.Define<string>(
            LogLevel.Error,
            new EventId(4004, "AdminCreationFailed"),
            "ສ້າງບັນຊີຜູ້ດູແລລະບົບບໍ່ສຳເລັດ {Errors}");

    private readonly DormFlowDbContext _dbContext = dbContext;
    private readonly RoleManager<ApplicationRole> _roleManager = roleManager;
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly IDateTimeProvider _dateTimeProvider = dateTimeProvider;
    private readonly SeedOptions _seedOptions = seedOptions.Value;
    private readonly ILogger<DatabaseSeeder> _logger = logger;

    /// <summary>ສ້າງຂໍ້ມູນເບື້ອງຕົ້ນທັງໝົດ</summary>
    /// <param name="cancellationToken">Token ສໍາລັບການຍົກເລີກຄໍາສັ່ງ</param>
    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        await _dbContext.Database.OpenConnectionAsync(cancellationToken);

        try
        {
            await _dbContext.Database.ExecuteSqlRawAsync(
                "SELECT pg_advisory_lock(hashtext({0}))",
                new object[] { SeedAdvisoryLockName },
                cancellationToken);

            await SeedPermissionsAsync(cancellationToken);
            await SeedRolesAsync(cancellationToken);
            await SeedSuperAdminAsync(cancellationToken);
        }
        finally
        {
            try
            {
                await _dbContext.Database.ExecuteSqlRawAsync(
                    "SELECT pg_advisory_unlock(hashtext({0}))",
                    new object[] { SeedAdvisoryLockName },
                    CancellationToken.None);
            }
            finally
            {
                await _dbContext.Database.CloseConnectionAsync();
            }
        }
    }

    /// <summary>ເພີ່ມການອະນຸຍາດທີ່ບໍ່ມີຢູ່ໃນຖານຂໍ້ມູນ</summary>
    private async Task SeedPermissionsAsync(CancellationToken cancellationToken)
    {
        Dictionary<string, PermissionDefinition> definitionsByCode = PermissionCodes.All
            .ToDictionary(definition => definition.Code, StringComparer.Ordinal);

        List<Permission> existingPermissions = await _dbContext.Permissions
            .Where(permission => definitionsByCode.Keys.Contains(permission.Code))
            .ToListAsync(cancellationToken);

        HashSet<string> existingCodes = existingPermissions
            .Select(permission => permission.Code)
            .ToHashSet(StringComparer.Ordinal);

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

        bool existingPermissionsChanged = false;
        foreach (Permission permission in existingPermissions)
        {
            PermissionDefinition definition = definitionsByCode[permission.Code];
            if (permission.Name == definition.Name
                && permission.Description == definition.Description
                && permission.Module == definition.Module)
            {
                continue;
            }

            permission.Name = definition.Name;
            permission.Description = definition.Description;
            permission.Module = definition.Module;
            existingPermissionsChanged = true;
        }

        if (newPermissions.Count > 0)
        {
            _dbContext.Permissions.AddRange(newPermissions);
        }

        if (newPermissions.Count == 0 && !existingPermissionsChanged)
        {
            return;
        }

        await _dbContext.SaveChangesAsync(cancellationToken);

        if (newPermissions.Count > 0)
        {
            PermissionsSeededCallback(_logger, newPermissions.Count, null);
        }
    }

    /// <summary>ສ້າງບົດບາດມາດຕະຖານດ້ວຍການອະນຸຍາດເລີ່ມຕົ້ນ</summary>
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
            string roleDescription = GetRoleDescription(roleName);
            ApplicationRole? existingRole = await _roleManager.FindByNameAsync(roleName);
            if (existingRole is not null)
            {
                if (existingRole.Description != roleDescription)
                {
                    existingRole.Description = roleDescription;
                    IdentityResult updateResult = await _roleManager.UpdateAsync(existingRole);
                    if (!updateResult.Succeeded)
                    {
                        throw new InvalidOperationException(
                            $"ອັບເດດຄຳອະທິບາຍບົດບາດ {roleName} ບໍ່ສຳເລັດ: {string.Join(", ", updateResult.Errors.Select(IdentityErrorTranslator.Translate))}");
                    }
                }

                continue;
            }

            ApplicationRole role = new(roleName)
            {
                Description = roleDescription,
                IsSystemRole = true,
            };

            IdentityResult result = await _roleManager.CreateAsync(role);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException(
                    $"ສ້າງບົດບາດ {roleName} ບໍ່ສຳເລັດ: {string.Join(", ", result.Errors.Select(IdentityErrorTranslator.Translate))}");
            }

            await GrantDefaultPermissionsAsync(role, permissionIdsByCode, cancellationToken);

            RoleCreatedCallback(_logger, roleName, null);
        }
    }

    private static string GetRoleDescription(string roleName) => roleName switch
    {
        DormFlowRoles.SuperAdmin => "ຜູ້ດູແລລະບົບຂັ້ນສູງ",
        DormFlowRoles.Owner => "ເຈົ້າຂອງອົງກອນ",
        DormFlowRoles.DormManager => "ຜູ້ຈັດການຫໍພັກ",
        DormFlowRoles.Accountant => "ພະນັກງານບັນຊີ",
        DormFlowRoles.Staff => "ພະນັກງານ",
        DormFlowRoles.Technician => "ຊ່າງເຕັກນິກ",
        DormFlowRoles.Tenant => "ຜູ້ເຊົ່າ",
        _ => "ບົດບາດມາດຕະຖານຂອງລະບົບ",
    };

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
    /// ສ້າງບັນຊີຜູ້ເບິ່ງແຍງລະບົບເລີ່ມຕົ້ນຈາກຄ່າໃນ environment variable
    /// </summary>
    /// <remarks>
    /// ຖ້າບໍ່ໄດ້ຕັ້ງຄ່າ ລະບົບຈະຂ້າມຂັ້ນຕອນນີ້ ແລະບັນທຶກການເຕືອນ
    /// ຢ່າສ້າງບັນຊີດ້ວຍລະຫັດຜ່ານທີ່ຝັງຢູ່ໃນລະຫັດ
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
            FirstName = string.IsNullOrWhiteSpace(_seedOptions.AdminFirstName) ? "ລະບົບ" : _seedOptions.AdminFirstName.Trim(),
            LastName = string.IsNullOrWhiteSpace(_seedOptions.AdminLastName) ? "ຜູ້ດູແລ" : _seedOptions.AdminLastName.Trim(),
            Status = UserStatus.Active,

            // ຜູ້ບໍລິຫານສູງສຸດບໍ່ໄດ້ຕິດພັນກັບອົງກອນໃດໆ. ດັ່ງນັ້ນ, ຂໍ້ມູນສາມາດເຂົ້າເຖິງທົ່ວອົງກອນ
            OrganizationId = null,
        };

        IdentityResult createResult = await _userManager.CreateAsync(admin, _seedOptions.AdminPassword!);

        if (!createResult.Succeeded)
        {
            AdminCreationFailedCallback(
                _logger,
                string.Join(", ", createResult.Errors.Select(IdentityErrorTranslator.Translate)),
                null);

            return;
        }

        IdentityResult roleResult = await _userManager.AddToRoleAsync(admin, DormFlowRoles.SuperAdmin);

        if (!roleResult.Succeeded)
        {
            throw new InvalidOperationException(
                $"ເພີ່ມບົດບາດ SuperAdmin ໃຫ້ບັນຊີຜູ້ດູແລບໍ່ສຳເລັດ: {string.Join(", ", roleResult.Errors.Select(IdentityErrorTranslator.Translate))}");
        }

        AdminCreatedCallback(_logger, null);

        _ = cancellationToken;
    }
}
