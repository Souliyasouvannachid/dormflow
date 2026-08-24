using DormFlow.Application.Common.Abstractions;
using DormFlow.Application.Common.Exceptions;
using DormFlow.Application.Common.Models;
using DormFlow.Contracts.Authentication;
using DormFlow.Domain.Enums;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace DormFlow.Application.Features.Authentication;

/// <summary>
/// Use Case ทั้งหมดที่เกี่ยวกับการยืนยันตัวตน
/// </summary>
/// <remarks>
/// คลาสนี้ตัดสินใจเชิงธุรกิจอย่างเดียว ไม่รู้จัก HTTP, EF Core หรือ Identity
/// รายละเอียดเหล่านั้นอยู่หลัง interface ที่ชั้น Infrastructure เป็นผู้ให้บริการ
/// </remarks>
public sealed class AuthenticationService(
    IUserAccountService userAccountService,
    IAccessTokenGenerator accessTokenGenerator,
    IRefreshTokenService refreshTokenService,
    IAuditLogWriter auditLogWriter,
    IEmailSender emailSender,
    ICurrentUser currentUser,
    IDateTimeProvider dateTimeProvider,
    IValidator<LoginRequest> loginValidator,
    IValidator<RefreshTokenRequest> refreshTokenValidator,
    IValidator<LogoutRequest> logoutValidator,
    IValidator<ForgotPasswordRequest> forgotPasswordValidator,
    IValidator<ResetPasswordRequest> resetPasswordValidator,
    IValidator<ChangePasswordRequest> changePasswordValidator,
    ILogger<AuthenticationService> logger) : IAuthenticationService
{
    /// <summary>ชื่อ Entity ที่ใช้กำกับเหตุการณ์เกี่ยวกับการยืนยันตัวตนใน Audit Log</summary>
    private const string AuditEntityName = "Authentication";

    private readonly IUserAccountService _userAccountService = userAccountService;
    private readonly IAccessTokenGenerator _accessTokenGenerator = accessTokenGenerator;
    private readonly IRefreshTokenService _refreshTokenService = refreshTokenService;
    private readonly IAuditLogWriter _auditLogWriter = auditLogWriter;
    private readonly IEmailSender _emailSender = emailSender;
    private readonly ICurrentUser _currentUser = currentUser;
    private readonly IDateTimeProvider _dateTimeProvider = dateTimeProvider;
    private readonly IValidator<LoginRequest> _loginValidator = loginValidator;
    private readonly IValidator<RefreshTokenRequest> _refreshTokenValidator = refreshTokenValidator;
    private readonly IValidator<LogoutRequest> _logoutValidator = logoutValidator;
    private readonly IValidator<ForgotPasswordRequest> _forgotPasswordValidator = forgotPasswordValidator;
    private readonly IValidator<ResetPasswordRequest> _resetPasswordValidator = resetPasswordValidator;
    private readonly IValidator<ChangePasswordRequest> _changePasswordValidator = changePasswordValidator;
    private readonly ILogger<AuthenticationService> _logger = logger;

    /// <inheritdoc />
    public async Task<AuthTokenResponse> LoginAsync(
        LoginRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        await _loginValidator.ValidateAndThrowAsync(request, cancellationToken);

        string email = request.Email.Trim();
        UserAuthenticationResult result = await _userAccountService.AuthenticateAsync(
            email,
            request.Password,
            cancellationToken);

        if (result.Status != UserAuthenticationStatus.Success || result.Profile is null)
        {
            await RecordFailedLoginAsync(email, result, cancellationToken);
            throw ToLoginException(result);
        }

        UserSecurityProfile profile = result.Profile;
        DateTimeOffset now = _dateTimeProvider.UtcNow;

        await _userAccountService.RecordSuccessfulLoginAsync(profile.UserId, now, cancellationToken);

        AuthTokenResponse response = await IssueTokensAsync(profile, cancellationToken);

        await _auditLogWriter.WriteAsync(
            AuditAction.Login,
            AuditEntityName,
            profile.UserId.ToString(),
            profile.UserId,
            profile.OrganizationId,
            cancellationToken: cancellationToken);

        return response;
    }

    /// <inheritdoc />
    public async Task<AuthTokenResponse> RefreshTokenAsync(
        RefreshTokenRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        await _refreshTokenValidator.ValidateAndThrowAsync(request, cancellationToken);

        RefreshTokenValidationResult validation = await _refreshTokenService.ValidateAsync(
            request.RefreshToken,
            _currentUser.IpAddress,
            cancellationToken);

        if (validation.Status == RefreshTokenValidationStatus.ReuseDetected)
        {
            // การเพิกถอนทั้งสายโทเคนเกิดขึ้นแล้วภายใน ValidateAsync
            // ที่นี่เหลือแค่บันทึกเหตุการณ์เพื่อให้ทีมความปลอดภัยตรวจสอบย้อนหลังได้
            AuthenticationLog.RefreshTokenReuseDetected(_logger, validation.UserId, _currentUser.IpAddress);

            await _auditLogWriter.WriteAsync(
                AuditAction.RefreshTokenReuseDetected,
                AuditEntityName,
                validation.UserId?.ToString(),
                validation.UserId,
                cancellationToken: cancellationToken);

            throw new UnauthorizedAccessException("Refresh Token ไม่ถูกต้อง กรุณาเข้าสู่ระบบใหม่");
        }

        if (validation.Status != RefreshTokenValidationStatus.Valid
            || validation.TokenId is null
            || validation.UserId is null)
        {
            throw ToRefreshException(validation.Status);
        }

        UserSecurityProfile? profile = await _userAccountService.GetSecurityProfileAsync(
            validation.UserId.Value,
            cancellationToken);

        if (profile is null || profile.Status != UserStatus.Active)
        {
            await _refreshTokenService.RevokeAllForUserAsync(
                validation.UserId.Value,
                RefreshTokenRevocationReason.AccountDisabled,
                _currentUser.IpAddress,
                cancellationToken);

            throw new UnauthorizedAccessException("บัญชีนี้ถูกปิดใช้งาน กรุณาติดต่อผู้ดูแลระบบ");
        }

        AccessToken accessToken = _accessTokenGenerator.Generate(profile);
        IssuedRefreshToken refreshToken = await _refreshTokenService.RotateAsync(
            validation.TokenId.Value,
            accessToken.JwtId,
            _currentUser.IpAddress,
            _currentUser.UserAgent,
            cancellationToken);

        await _auditLogWriter.WriteAsync(
            AuditAction.RefreshToken,
            AuditEntityName,
            profile.UserId.ToString(),
            profile.UserId,
            profile.OrganizationId,
            cancellationToken: cancellationToken);

        return BuildResponse(accessToken, refreshToken, profile);
    }

    /// <inheritdoc />
    public async Task LogoutAsync(LogoutRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        await _logoutValidator.ValidateAndThrowAsync(request, cancellationToken);

        bool revoked = await _refreshTokenService.RevokeAsync(
            request.RefreshToken,
            RefreshTokenRevocationReason.Logout,
            _currentUser.IpAddress,
            cancellationToken);

        // ไม่บอกผู้เรียกว่าโทเคนมีอยู่จริงหรือไม่ เพื่อไม่ให้ใช้ endpoint นี้ตรวจสอบความมีอยู่ของโทเคน
        if (!revoked)
        {
            AuthenticationLog.LogoutWithInactiveToken(_logger);
            return;
        }

        await _auditLogWriter.WriteAsync(
            AuditAction.Logout,
            AuditEntityName,
            _currentUser.UserId?.ToString(),
            cancellationToken: cancellationToken);
    }

    /// <inheritdoc />
    public async Task ForgotPasswordAsync(
        ForgotPasswordRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        await _forgotPasswordValidator.ValidateAndThrowAsync(request, cancellationToken);

        string email = request.Email.Trim();
        string? token = await _userAccountService.GeneratePasswordResetTokenAsync(email, cancellationToken);

        // ตอบข้อความเดียวกันเสมอไม่ว่าจะพบบัญชีหรือไม่
        // เพื่อไม่ให้ endpoint นี้ถูกใช้ไล่ตรวจว่าอีเมลใดมีบัญชีอยู่ในระบบบ้าง
        if (token is null)
        {
            AuthenticationLog.ForgotPasswordForUnknownAccount(_logger);
            return;
        }

        await _emailSender.SendAsync(
            new EmailMessage(
                email,
                "ตั้งรหัสผ่านใหม่สำหรับ DormFlow",
                $"""
                 คุณได้ขอตั้งรหัสผ่านใหม่สำหรับบัญชี {email}

                 โทเคนสำหรับตั้งรหัสผ่านใหม่:
                 {token}

                 นำโทเคนนี้ไปใช้กับ POST /api/v1/auth/reset-password ภายในเวลาที่กำหนด
                 หากคุณไม่ได้เป็นผู้ขอ ไม่ต้องดำเนินการใด ๆ รหัสผ่านเดิมยังใช้งานได้ตามปกติ
                 """),
            cancellationToken);

        await _auditLogWriter.WriteAsync(
            AuditAction.ForgotPassword,
            AuditEntityName,
            cancellationToken: cancellationToken);
    }

    /// <inheritdoc />
    public async Task ResetPasswordAsync(
        ResetPasswordRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        await _resetPasswordValidator.ValidateAndThrowAsync(request, cancellationToken);

        string email = request.Email.Trim();
        PasswordOperationResult result = await _userAccountService.ResetPasswordAsync(
            email,
            request.Token,
            request.NewPassword,
            cancellationToken);

        if (!result.Succeeded)
        {
            throw new ValidationFailedException(
                result.Errors.Count > 0
                    ? string.Join(" ", result.Errors)
                    : "ตั้งรหัสผ่านใหม่ไม่สำเร็จ โทเคนอาจหมดอายุหรือถูกใช้ไปแล้ว");
        }

        UserSecurityProfile? profile = await _userAccountService.GetSecurityProfileByEmailAsync(email, cancellationToken);
        if (profile is not null)
        {
            // รหัสผ่านเปลี่ยนแล้ว เซสชันเดิมทั้งหมดต้องหมดสิทธิ์ทันที
            await _refreshTokenService.RevokeAllForUserAsync(
                profile.UserId,
                RefreshTokenRevocationReason.PasswordChanged,
                _currentUser.IpAddress,
                cancellationToken);
        }

        await _auditLogWriter.WriteAsync(
            AuditAction.ResetPassword,
            AuditEntityName,
            profile?.UserId.ToString(),
            profile?.UserId,
            profile?.OrganizationId,
            cancellationToken: cancellationToken);
    }

    /// <inheritdoc />
    public async Task ChangePasswordAsync(
        ChangePasswordRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        await _changePasswordValidator.ValidateAndThrowAsync(request, cancellationToken);

        Guid userId = RequireAuthenticatedUserId();

        PasswordOperationResult result = await _userAccountService.ChangePasswordAsync(
            userId,
            request.CurrentPassword,
            request.NewPassword,
            cancellationToken);

        if (!result.Succeeded)
        {
            throw new ValidationFailedException(
                result.Errors.Count > 0
                    ? string.Join(" ", result.Errors)
                    : "เปลี่ยนรหัสผ่านไม่สำเร็จ");
        }

        await _refreshTokenService.RevokeAllForUserAsync(
            userId,
            RefreshTokenRevocationReason.PasswordChanged,
            _currentUser.IpAddress,
            cancellationToken);

        await _auditLogWriter.WriteAsync(
            AuditAction.ChangePassword,
            AuditEntityName,
            userId.ToString(),
            userId,
            cancellationToken: cancellationToken);
    }

    /// <inheritdoc />
    public async Task<CurrentUserResponse> GetCurrentUserAsync(CancellationToken cancellationToken = default)
    {
        Guid userId = RequireAuthenticatedUserId();

        UserSecurityProfile? profile = await _userAccountService.GetSecurityProfileAsync(userId, cancellationToken);

        if (profile is null)
        {
            throw new NotFoundException("ไม่พบข้อมูลผู้ใช้ของโทเคนนี้");
        }

        return ToCurrentUserResponse(profile);
    }

    private async Task<AuthTokenResponse> IssueTokensAsync(
        UserSecurityProfile profile,
        CancellationToken cancellationToken)
    {
        AccessToken accessToken = _accessTokenGenerator.Generate(profile);

        IssuedRefreshToken refreshToken = await _refreshTokenService.IssueAsync(
            profile.UserId,
            accessToken.JwtId,
            _currentUser.IpAddress,
            _currentUser.UserAgent,
            cancellationToken);

        return BuildResponse(accessToken, refreshToken, profile);
    }

    private async Task RecordFailedLoginAsync(
        string email,
        UserAuthenticationResult result,
        CancellationToken cancellationToken)
    {
        // ไม่บันทึกอีเมลหรือรหัสผ่านลงใน Audit Log เพราะเป็นข้อมูลที่ระบุตัวบุคคลได้
        // บันทึกเฉพาะสาเหตุ ส่วนหมายเลข IP และ User Agent ถูกเติมให้อัตโนมัติ
        AuthenticationLog.LoginFailed(_logger, _currentUser.IpAddress, result.Status);

        await _auditLogWriter.WriteAsync(
            AuditAction.LoginFailed,
            AuditEntityName,
            newValues: $$"""{"reason":"{{result.Status}}"}""",
            cancellationToken: cancellationToken);
    }

    private Guid RequireAuthenticatedUserId()
        => _currentUser.UserId
            ?? throw new UnauthorizedAccessException("กรุณาเข้าสู่ระบบก่อนใช้งานคำสั่งนี้");

    private static AuthTokenResponse BuildResponse(
        AccessToken accessToken,
        IssuedRefreshToken refreshToken,
        UserSecurityProfile profile)
        => new()
        {
            AccessToken = accessToken.Value,
            TokenType = "Bearer",
            ExpiresIn = accessToken.ExpiresInSeconds,
            ExpiresAt = accessToken.ExpiresAt,
            RefreshToken = refreshToken.Value,
            RefreshTokenExpiresAt = refreshToken.ExpiresAt,
            User = ToCurrentUserResponse(profile),
        };

    private static CurrentUserResponse ToCurrentUserResponse(UserSecurityProfile profile)
        => new()
        {
            Id = profile.UserId,
            Email = profile.Email,
            FirstName = profile.FirstName,
            LastName = profile.LastName,
            Phone = profile.Phone,
            ProfileImage = profile.ProfileImage,
            OrganizationId = profile.OrganizationId,
            Status = profile.Status.ToString(),
            LastLoginAt = profile.LastLoginAt,
            Roles = profile.Roles,
            Permissions = profile.Permissions,
            DormitoryIds = profile.DormitoryIds,
        };

    /// <summary>
    /// แปลงสาเหตุที่เข้าสู่ระบบไม่สำเร็จเป็น Exception
    /// </summary>
    /// <remarks>
    /// กรณีอีเมลไม่มีอยู่จริงกับกรณีรหัสผ่านผิด ตอบข้อความเดียวกันเสมอ
    /// เพื่อไม่ให้ผู้โจมตีใช้หน้าเข้าสู่ระบบไล่ตรวจว่าอีเมลใดมีบัญชีอยู่
    /// </remarks>
    private static Exception ToLoginException(UserAuthenticationResult result) => result.Status switch
    {
        UserAuthenticationStatus.LockedOut => new ForbiddenAccessException(
            result.LockoutEndAt is null
                ? "บัญชีนี้ถูกล็อกชั่วคราวเพราะกรอกรหัสผ่านผิดหลายครั้ง"
                : $"บัญชีนี้ถูกล็อกชั่วคราว กรุณาลองใหม่หลัง {result.LockoutEndAt.Value.ToLocalTime():HH:mm}"),

        UserAuthenticationStatus.AccountDisabled => new ForbiddenAccessException(
            "บัญชีนี้ถูกปิดใช้งาน กรุณาติดต่อผู้ดูแลระบบ"),

        UserAuthenticationStatus.OrganizationDisabled => new ForbiddenAccessException(
            "องค์กรของคุณถูกระงับการใช้งาน กรุณาติดต่อผู้ดูแลระบบ"),

        UserAuthenticationStatus.RequiresTwoFactor => new ForbiddenAccessException(
            "บัญชีนี้เปิดการยืนยันตัวตนสองขั้นตอนไว้ กรุณายืนยันตัวตนขั้นที่สอง"),

        _ => new UnauthorizedAccessException("อีเมลหรือรหัสผ่านไม่ถูกต้อง"),
    };

    private static Exception ToRefreshException(RefreshTokenValidationStatus status) => status switch
    {
        RefreshTokenValidationStatus.Expired =>
            new UnauthorizedAccessException("Refresh Token หมดอายุแล้ว กรุณาเข้าสู่ระบบใหม่"),

        RefreshTokenValidationStatus.Revoked =>
            new UnauthorizedAccessException("Refresh Token ถูกเพิกถอนไปแล้ว กรุณาเข้าสู่ระบบใหม่"),

        RefreshTokenValidationStatus.AccountDisabled =>
            new ForbiddenAccessException("บัญชีนี้ถูกปิดใช้งาน กรุณาติดต่อผู้ดูแลระบบ"),

        _ => new UnauthorizedAccessException("Refresh Token ไม่ถูกต้อง กรุณาเข้าสู่ระบบใหม่"),
    };
}
