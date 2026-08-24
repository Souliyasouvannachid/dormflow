using DormFlow.Application.Common.Abstractions;
using DormFlow.Application.Common.Models;
using DormFlow.Domain.Entities;
using DormFlow.Domain.Enums;
using DormFlow.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DormFlow.Infrastructure.Identity;

/// <summary>
/// การดำเนินการกับบัญชีผู้ใช้ที่ทำงานบน ASP.NET Core Identity
/// </summary>
/// <remarks>
/// คลาสนี้เป็นจุดเดียวในระบบที่แตะ <see cref="UserManager{TUser}"/> โดยตรง
/// ทำให้ชั้น Application ไม่มีทางเข้าถึงแฮชรหัสผ่านหรือเขียนตรรกะการตรวจรหัสผ่านเอง
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
            // นับความพยายามที่ผิดพลาดเพื่อให้กลไกล็อกบัญชีของ Identity ทำงาน
            await _userManager.AccessFailedAsync(user);

            return await _userManager.IsLockedOutAsync(user)
                ? UserAuthenticationResult.Failed(UserAuthenticationStatus.LockedOut, user.LockoutEnd)
                : UserAuthenticationResult.Failed(UserAuthenticationStatus.InvalidCredentials);
        }

        // ตรวจสถานะของบัญชีและองค์กรหลังจากรหัสผ่านถูกต้องแล้วเท่านั้น
        // เพื่อไม่ให้ผู้ที่ไม่ทราบรหัสผ่านใช้ข้อความตอบกลับแยกแยะว่าอีเมลนี้มีอยู่จริง
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
        // อัปเดตเฉพาะคอลัมน์เดียวโดยไม่ต้องโหลด entity ทั้งตัวเข้ามาก่อน
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

        // ไม่สร้างโทเคนให้บัญชีที่ถูกปิดใช้งาน แต่ผู้เรียกจะได้รับข้อความเดียวกันเสมอ
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
            // ตอบข้อความกลาง ๆ เพื่อไม่ให้แยกแยะได้ว่าอีเมลนี้มีบัญชีอยู่หรือไม่
            return PasswordOperationResult.Failed("ตั้งรหัสผ่านใหม่ไม่สำเร็จ โทเคนอาจหมดอายุหรือถูกใช้ไปแล้ว");
        }

        IdentityResult result = await _userManager.ResetPasswordAsync(user, token, newPassword);

        if (!result.Succeeded)
        {
            return PasswordOperationResult.Failed([.. result.Errors.Select(TranslateIdentityError)]);
        }

        // ปลดล็อกบัญชีที่ถูกล็อกจากการกรอกรหัสผ่านผิด เพราะเจ้าของยืนยันตัวตนผ่านอีเมลแล้ว
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
            return PasswordOperationResult.Failed("ไม่พบบัญชีผู้ใช้ หรือบัญชีถูกปิดใช้งาน");
        }

        IdentityResult result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);

        return result.Succeeded
            ? PasswordOperationResult.Success
            : PasswordOperationResult.Failed([.. result.Errors.Select(TranslateIdentityError)]);
    }

    private async Task<bool> IsOrganizationActiveAsync(
        Guid? organizationId,
        CancellationToken cancellationToken)
    {
        // ผู้ดูแลระบบที่ไม่ผูกกับองค์กรใดไม่ต้องผ่านการตรวจนี้
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

        // รวมสิทธิ์จากทุกบทบาทของผู้ใช้ในคำสั่งเดียว แล้วตัดค่าซ้ำที่ฝั่งฐานข้อมูล
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

    /// <summary>
    /// แปลงรหัสข้อผิดพลาดของ Identity เป็นข้อความภาษาไทย
    /// </summary>
    /// <remarks>
    /// ข้อความเริ่มต้นของ Identity เป็นภาษาอังกฤษทั้งหมด
    /// แปลเฉพาะกรณีที่ผู้ใช้พบบ่อย ส่วนที่เหลือคืนข้อความเดิมเพื่อไม่ให้ข้อมูลหาย
    /// </remarks>
    private static string TranslateIdentityError(IdentityError error) => error.Code switch
    {
        "PasswordMismatch" => "รหัสผ่านปัจจุบันไม่ถูกต้อง",
        "InvalidToken" => "โทเคนไม่ถูกต้องหรือหมดอายุแล้ว กรุณาขอลิงก์ตั้งรหัสผ่านใหม่อีกครั้ง",
        "PasswordTooShort" => "รหัสผ่านสั้นเกินไป",
        "PasswordRequiresDigit" => "รหัสผ่านต้องมีตัวเลขอย่างน้อยหนึ่งตัว",
        "PasswordRequiresLower" => "รหัสผ่านต้องมีตัวอักษรพิมพ์เล็กอย่างน้อยหนึ่งตัว",
        "PasswordRequiresUpper" => "รหัสผ่านต้องมีตัวอักษรพิมพ์ใหญ่อย่างน้อยหนึ่งตัว",
        "PasswordRequiresUniqueChars" => "รหัสผ่านต้องมีอักขระที่ไม่ซ้ำกันมากกว่านี้",
        "UserLockedOut" => "บัญชีนี้ถูกล็อกชั่วคราว กรุณาลองใหม่ภายหลัง",
        _ => error.Description,
    };
}
