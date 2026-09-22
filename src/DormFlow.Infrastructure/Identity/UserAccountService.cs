using DormFlow.Application.Common.Abstractions;
using DormFlow.Application.Common.Models;
using DormFlow.Domain.Entities;
using DormFlow.Domain.Enums;
using DormFlow.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DormFlow.Infrastructure.Identity;

/// <summary>
/// ການປະຕິບັດກ່ຽວກັບບັນຊີຜູ້ໃຊ້ທີ່ເຮັດວຽກຢູ່ ASP.NET Core Identity
/// </summary>
/// <remarks>
/// ຫ້ອງຮຽນນີ້ແມ່ນຈຸດດຽວໃນລະບົບທີ່ຖືກສໍາຜັດ <see cref="UserManager{TUser}"/> ໂດຍກົງ
/// ເຮັດໃຫ້ພື້ນເຮືອນ Application ບໍ່ມີວິທີທີ່ຈະເຂົ້າເຖິງ hash ລະຫັດຜ່ານຫຼືຂຽນ logic ການກວດສອບລະຫັດຜ່ານຕົວມັນເອງ
/// </remarks>
public sealed class UserAccountService(
    UserManager<ApplicationUser> userManager,
    DormFlowDbContext dbContext) : IUserAccountService
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly DormFlowDbContext _dbContext = dbContext;

    /// <inheritdoc />
    public async Task<UserAuthenticationResult> AuthenticateAsync(
        string email,
        string password,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(email);

        ApplicationUser? user = await _userManager.FindByEmailAsync(email);

        if (user is null)
        {
            return UserAuthenticationResult.Failed(UserAuthenticationStatus.InvalidCredentials);
        }

        if (await _userManager.IsLockedOutAsync(user))
        {
            return UserAuthenticationResult.Failed(
                UserAuthenticationStatus.LockedOut,
                user.LockoutEnd);
        }

        if (!await _userManager.CheckPasswordAsync(user, password))
        {
            // ນັບຄວາມພະຍາຍາມທີ່ບໍ່ຖືກຕ້ອງເພື່ອເປີດໃຊ້ກົນໄກການລັອກບັນຊີ Identity ເຮັດວຽກ
            await _userManager.AccessFailedAsync(user);

            return await _userManager.IsLockedOutAsync(user)
                ? UserAuthenticationResult.Failed(UserAuthenticationStatus.LockedOut, user.LockoutEnd)
                : UserAuthenticationResult.Failed(UserAuthenticationStatus.InvalidCredentials);
        }

        // ກວດເບິ່ງສະຖານະຂອງບັນຊີ ແລະອົງກອນຂອງເຈົ້າຫຼັງຈາກລະຫັດຜ່ານຖືກຕ້ອງເທົ່ານັ້ນ
        // ນີ້ແມ່ນເພື່ອປ້ອງກັນບໍ່ໃຫ້ຄົນທີ່ບໍ່ຮູ້ລະຫັດຜ່ານຈາກການໃຊ້ຂໍ້ຄວາມຕອບກັບຈາກການຈໍາແນກວ່າອີເມວນີ້ມີຢູ່
        if (user.Status != UserStatus.Active)
        {
            return UserAuthenticationResult.Failed(UserAuthenticationStatus.AccountDisabled);
        }

        if (!await IsOrganizationActiveAsync(user.OrganizationId, cancellationToken))
        {
            return UserAuthenticationResult.Failed(UserAuthenticationStatus.OrganizationDisabled);
        }

        if (await _userManager.GetTwoFactorEnabledAsync(user))
        {
            return UserAuthenticationResult.Failed(UserAuthenticationStatus.RequiresTwoFactor);
        }

        await _userManager.ResetAccessFailedCountAsync(user);

        UserSecurityProfile profile = await BuildProfileAsync(user, cancellationToken);
        return UserAuthenticationResult.Success(profile);
    }

    /// <inheritdoc />
    public async Task<UserSecurityProfile?> GetSecurityProfileAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        ApplicationUser? user = await _dbContext.Users
            .FirstOrDefaultAsync(candidate => candidate.Id == userId, cancellationToken);

        return user is null ? null : await BuildProfileAsync(user, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<UserSecurityProfile?> GetSecurityProfileByEmailAsync(
        string email,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(email);

        string normalizedEmail = _userManager.NormalizeEmail(email);

        ApplicationUser? user = await _dbContext.Users
            .FirstOrDefaultAsync(candidate => candidate.NormalizedEmail == normalizedEmail, cancellationToken);

        return user is null ? null : await BuildProfileAsync(user, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> IsAccountUsableAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        ApplicationUser? user = await _dbContext.Users
            .FirstOrDefaultAsync(candidate => candidate.Id == userId, cancellationToken);

        return user is not null
            && user.CanSignIn
            && await IsOrganizationActiveAsync(user.OrganizationId, cancellationToken);
    }

    /// <inheritdoc />
    public async Task RecordSuccessfulLoginAsync(
        Guid userId,
        DateTimeOffset loggedInAt,
        CancellationToken cancellationToken = default)
    {
        // ອັບເດດພຽງແຕ່ຜຶ່ງຖັນໂດຍບໍ່ມີການໂຫຼດ entity ຮ່າງກາຍທັງຝົດເຂົ້າມາໃນຄັ້ງທໍາອິດ
        await _dbContext.Users
            .Where(user => user.Id == userId)
            .ExecuteUpdateAsync(
                setters => setters.SetProperty(user => user.LastLoginAt, loggedInAt),
                cancellationToken);
    }

    /// <inheritdoc />
    public async Task<string?> GeneratePasswordResetTokenAsync(
        string email,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(email);

        ApplicationUser? user = await _userManager.FindByEmailAsync(email);

        // ຢ່າສ້າງ tokens ສໍາລັບບັນຊີທີ່ພິການ. ແຕ່ຜູ້ໂທໄດ້ຮັບຂໍ້ຄວາມດຽວກັນສະເໝີ
        if (user is null || !user.CanSignIn)
        {
            return null;
        }

        return await _userManager.GeneratePasswordResetTokenAsync(user);
    }

    /// <inheritdoc />
    public async Task<PasswordOperationResult> ResetPasswordAsync(
        string email,
        string token,
        string newPassword,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(email);

        ApplicationUser? user = await _userManager.FindByEmailAsync(email);

        if (user is null || !user.CanSignIn)
        {
            // ຕອບຂໍ້ຄວາມທີ່ເປັນກາງເພື່ອບໍ່ໃຫ້ຈໍາແນກວ່າທີ່ຢູ່ອີເມວນີ້ມີບັນຊີຫຼືບໍ່
            return PasswordOperationResult.Failed("ຕັ້ງລະຫັດຜ່ານໃໝ່ບໍ່ສຳເລັດ ໂທເຄນອາດໝົດອາຍຸ ຫຼື ຖືກໃຊ້ໄປແລ້ວ");
        }

        IdentityResult result = await _userManager.ResetPasswordAsync(user, token, newPassword);

        if (!result.Succeeded)
        {
            return PasswordOperationResult.Failed([.. result.Errors.Select(IdentityErrorTranslator.Translate)]);
        }

        // ປົດລັອກບັນຊີທີ່ຖືກລັອກຈາກການໃສ່ລະຫັດຜ່ານຜິດ. ເນື່ອງຈາກວ່າເຈົ້າຂອງໄດ້ຢືນຢັນຕົວຕົນຂອງລາວຜ່ານອີເມວ
        await _userManager.ResetAccessFailedCountAsync(user);
        await _userManager.SetLockoutEndDateAsync(user, null);

        return PasswordOperationResult.Success;
    }

    /// <inheritdoc />
    public async Task<PasswordOperationResult> ChangePasswordAsync(
        Guid userId,
        string currentPassword,
        string newPassword,
        CancellationToken cancellationToken = default)
    {
        ApplicationUser? user = await _userManager.FindByIdAsync(userId.ToString());

        if (user is null || !user.CanSignIn)
        {
            return PasswordOperationResult.Failed("ບໍ່ພົບບັນຊີຜູ້ໃຊ້ ຫຼື ບັນຊີຖືກປິດການໃຊ້ງານ");
        }

        IdentityResult result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);

        return result.Succeeded
            ? PasswordOperationResult.Success
            : PasswordOperationResult.Failed([.. result.Errors.Select(IdentityErrorTranslator.Translate)]);
    }

    private async Task<bool> IsOrganizationActiveAsync(
        Guid? organizationId,
        CancellationToken cancellationToken)
    {
        // ຜູ້ບໍລິຫານທີ່ບໍ່ໄດ້ຂຶ້ນກັບອົງກອນໃດຜຶ່ງບໍ່ຈໍາເປັນຕ້ອງຜ່ານການສອບເສັງນີ້
        if (organizationId is null)
        {
            return true;
        }

        Organization? organization = await _dbContext.Organizations
            .FirstOrDefaultAsync(candidate => candidate.Id == organizationId.Value, cancellationToken);

        return organization is not null && organization.CanUsersSignIn;
    }

    private async Task<UserSecurityProfile> BuildProfileAsync(
        ApplicationUser user,
        CancellationToken cancellationToken)
    {
        IList<string> roles = await _userManager.GetRolesAsync(user);

        string[] permissions = await GetPermissionsForRolesAsync(roles, cancellationToken);

        UserDormitoryAssignmentInfo[] assignments = await _dbContext.UserDormitories
            .Where(assignment => assignment.UserId == user.Id)
            .Select(assignment => new UserDormitoryAssignmentInfo(
                assignment.DormitoryId,
                assignment.IsPrimary,
                assignment.AssignedAt))
            .ToArrayAsync(cancellationToken);

        return new UserSecurityProfile
        {
            UserId = user.Id,
            Email = user.Email ?? string.Empty,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Phone = user.PhoneNumber,
            ProfileImage = user.ProfileImage,
            OrganizationId = user.OrganizationId,
            Status = user.Status,
            LastLoginAt = user.LastLoginAt,
            CreatedAt = user.CreatedAt,
            Roles = [.. roles],
            Permissions = permissions,
            DormitoryIds = [.. assignments.Select(assignment => assignment.DormitoryId)],
            DormitoryAssignments = assignments,
        };
    }

    private async Task<string[]> GetPermissionsForRolesAsync(
        IList<string> roleNames,
        CancellationToken cancellationToken)
    {
        if (roleNames.Count == 0)
        {
            return [];
        }

        // ສົມທົບການອະນຸຍາດຈາກທຸກບົດບາດຂອງຜູ້ໃຊ້ໃນຄໍາສັ່ງດຽວ. ຫຼັງຈາກນັ້ນ, ຕັດຄ່າທີ່ຊ້ໍາກັນຢູ່ດ້ານຖານຂໍ້ມູນ
        return await _dbContext.RolePermissions
            .Where(rolePermission => _dbContext.Roles
                .Where(role => role.Name != null && roleNames.Contains(role.Name))
                .Select(role => role.Id)
                .Contains(rolePermission.RoleId))
            .Select(rolePermission => rolePermission.Permission!.Code)
            .Distinct()
            .OrderBy(code => code)
            .ToArrayAsync(cancellationToken);
    }

}
