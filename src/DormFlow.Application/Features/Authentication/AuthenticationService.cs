using DormFlow.Application.Common.Abstractions;
using DormFlow.Application.Common.Exceptions;
using DormFlow.Application.Common.Models;
using DormFlow.Contracts.Authentication;
using DormFlow.Domain.Enums;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace DormFlow.Application.Features.Authentication;

/// <summary>
/// Use Case ທັງໝົດກ່ຽວກັບການຢັ້ງຢືນຕົວຕົນ
/// </summary>
/// <remarks>
/// ຫ້ອງຮຽນນີ້ເຮັດການຕັດສິນໃຈທາງທຸລະກິດເທົ່ານັ້ນ. ບໍ່ຮູ້ຈັກ HTTP, EF Core ຫຼື Identity
/// ລາຍລະອຽດເຫຼົ່ານັ້ນແມ່ນຢູ່ເບື້ອງຫຼັງ interface ຢູ່ເທິງພື້ນ Infrastructure ເປັນຜູ້ໃຫ້ບໍລິການ
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
    /// <summary>ຊື່ Entity ໃຊ້ເພື່ອຄວບຄຸມເຫດການທີ່ກ່ຽວຂ້ອງກັບການຢັ້ງຢືນຕົວຕົນໃນ Audit Log</summary>
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
            // ການຍົກເລີກລະບົບຕ່ອງໂສ້ໂທເຄັນທັງໝົດໄດ້ເກີດຂຶ້ນພາຍໃນແລ້ວ ValidateAsync
            // ທີ່ນີ້, ທັງຝົດທີ່ເຫຼືອແມ່ນການບັນທຶກເຫດການເພື່ອໃຫ້ທີມງານຄວາມປອດໄພສາມາດທົບທວນຄືນໄດ້
            AuthenticationLog.RefreshTokenReuseDetected(_logger, validation.UserId, _currentUser.IpAddress);

            await _auditLogWriter.WriteAsync(
                AuditAction.RefreshTokenReuseDetected,
                AuditEntityName,
                validation.UserId?.ToString(),
                validation.UserId,
                cancellationToken: cancellationToken);

            throw new UnauthorizedAccessException("Refresh Token ບໍ່ຖືກຕ້ອງ ກະລຸນາເຂົ້າສູ່ລະບົບໃໝ່");
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

            throw new UnauthorizedAccessException("ບັນຊີນີ້ຖືກປິດການນຳໃຊ້ ກະລຸນາຕິດຕໍ່ຜູ້ດູແລລະບົບ");
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

        // ມັນບໍ່ໄດ້ບອກອັນທີ່ເອີ້ນວ່າ tokens ວ່າພວກມັນມີຢູ່ແທ້ຫຼືບໍ່. ເພື່ອບໍ່ໃຫ້ໃຊ້ endpoint ນີ້ກວດສອບການມີຢູ່ຂອງໂທເຄັນ
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

        // ຕອບກັບຂໍ້ຄວາມດຽວກັນສະເໝີວ່າພົບບັນຊີນັ້ນຫຼືບໍ່
        // ໃນຄໍາສັ່ງທີ່ຈະບໍ່ endpoint ນີ້ແມ່ນໃຊ້ເພື່ອກວດເບິ່ງວ່າອີເມລ໌ໃດມີບັນຊີຢູ່ໃນລະບົບ
        if (token is null)
        {
            AuthenticationLog.ForgotPasswordForUnknownAccount(_logger);
            return;
        }

        await _emailSender.SendAsync(
            new EmailMessage(
                email,
                "ຕັ້ງລະຫັດຜ່ານໃໝ່ສຳລັບ DormFlow",
                $"""
                 ທ່ານໄດ້ຂໍຕັ້ງລະຫັດຜ່ານໃໝ່ສຳລັບບັນຊີ {email}

                 ໂທເຄນສຳລັບຕັ້ງລະຫັດຜ່ານໃໝ່:
                 {token}

                 ນຳໂທເຄນນີ້ໄປໃຊ້ກັບ POST /api/v1/auth/reset-password ພາຍໃນເວລາທີ່ກຳນົດ
                 ຫາກທ່ານບໍ່ໄດ້ເປັນຜູ້ຂໍ ບໍ່ຕ້ອງດຳເນີນການໃດໆ ລະຫັດຜ່ານເດີມຍັງໃຊ້ງານໄດ້ຕາມປົກກະຕິ
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
                    : "ຕັ້ງລະຫັດຜ່ານໃໝ່ບໍ່ສຳເລັດ ໂທເຄນອາດໝົດອາຍຸຫຼືຖືກໃຊ້ໄປແລ້ວ");
        }

        UserSecurityProfile? profile = await _userAccountService.GetSecurityProfileByEmailAsync(email, cancellationToken);
        if (profile is not null)
        {
            // ການປ່ຽນລະຫັດຜ່ານ ເຊດຊັນຕົ້ນສະບັບທັງໝົດຈະຕ້ອງຖືກຕັດສິດໃນທັນທີ
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
                    : "ປ່ຽນລະຫັດຜ່ານບໍ່ສຳເລັດ");
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
            throw new NotFoundException("ບໍ່ພົບຂໍ້ມູນຜູ້ໃຊ້ຂອງໂທເຄນນີ້");
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
        // ຢ່າບັນທຶກອີເມວ ຫຼືລະຫັດຜ່ານຂອງທ່ານໄວ້ໃນ Audit Log ເນື່ອງຈາກວ່າມັນເປັນຂໍ້ມູນສ່ວນບຸກຄົນ
        // ບັນທຶກພຽງແຕ່ສາເຫດ, ເປັນສໍາລັບຈໍານວນ IP ແລະ User Agent ຖືກຕື່ມໃສ່ໂດຍອັດຕະໂນມັດ
        AuthenticationLog.LoginFailed(_logger, _currentUser.IpAddress, result.Status);

        await _auditLogWriter.WriteAsync(
            AuditAction.LoginFailed,
            AuditEntityName,
            newValues: $$"""{"reason":"{{result.Status}}"}""",
            cancellationToken: cancellationToken);
    }

    private Guid RequireAuthenticatedUserId()
        => _currentUser.UserId
            ?? throw new UnauthorizedAccessException("ກະລຸນາເຂົ້າສູ່ລະບົບກ່ອນນຳໃຊ້ຄຳສັ່ງນີ້");

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
    /// ແປງເຫດຜົນການເຂົ້າສູ່ລະບົບລົ້ມເຫຼວ Exception
    /// </summary>
    /// <remarks>
    /// ໃນກໍລະນີທີ່ຢູ່ອີເມລ໌ທີ່ບໍ່ມີຕົວຈິງແລະໃນກໍລະນີຂອງລະຫັດຜ່ານຜິດພາດສະເຝີຕອບກັບຂໍ້ຄວາມດຽວກັນ
    /// ນີ້ແມ່ນເພື່ອປ້ອງກັນບໍ່ໃຫ້ຜູ້ໂຈມຕີໃຊ້ຜ້າເຂົ້າສູ່ລະບົບເພື່ອກວດເບິ່ງວ່າມີບັນຊີອີເມວໃດ
    /// </remarks>
    private static Exception ToLoginException(UserAuthenticationResult result) => result.Status switch
    {
        UserAuthenticationStatus.LockedOut => new ForbiddenAccessException(
            result.LockoutEndAt is null
                ? "ບັນຊີນີ້ຖືກລັອກຊົ່ວຄາວເພາະປ້ອນລະຫັດຜ່ານຜິດຫຼາຍຄັ້ງ"
                : $"ບັນຊີນີ້ຖືກລັອກຊົ່ວຄາວ ກະລຸນາລອງໃໝ່ຫຼັງ {result.LockoutEndAt.Value.ToLocalTime():HH:mm}"),

        UserAuthenticationStatus.AccountDisabled => new ForbiddenAccessException(
            "ບັນຊີນີ້ຖືກປິດການນຳໃຊ້ ກະລຸນາຕິດຕໍ່ຜູ້ດູແລລະບົບ"),

        UserAuthenticationStatus.OrganizationDisabled => new ForbiddenAccessException(
            "ອົງກອນຂອງທ່ານຖືກລະງັບການນຳໃຊ້ ກະລຸນາຕິດຕໍ່ຜູ້ດູແລລະບົບ"),

        UserAuthenticationStatus.RequiresTwoFactor => new ForbiddenAccessException(
            "ບັນຊີນີ້ເປີດການຢືນຢັນຕົວຕົນສອງຂັ້ນຕອນໄວ້ ກະລຸນາຢືນຢັນຕົວຕົນຂັ້ນທີສອງ"),

        _ => new UnauthorizedAccessException("ອີເມວຫຼືລະຫັດຜ່ານບໍ່ຖືກຕ້ອງ"),
    };

    private static Exception ToRefreshException(RefreshTokenValidationStatus status) => status switch
    {
        RefreshTokenValidationStatus.Expired =>
            new UnauthorizedAccessException("Refresh Token ໝົດອາຍຸແລ້ວ ກະລຸນາເຂົ້າສູ່ລະບົບໃໝ່"),

        RefreshTokenValidationStatus.Revoked =>
            new UnauthorizedAccessException("Refresh Token ຖືກເພີກຖອນໄປແລ້ວ ກະລຸນາເຂົ້າສູ່ລະບົບໃໝ່"),

        RefreshTokenValidationStatus.AccountDisabled =>
            new ForbiddenAccessException("ບັນຊີນີ້ຖືກປິດການນຳໃຊ້ ກະລຸນາຕິດຕໍ່ຜູ້ດູແລລະບົບ"),

        _ => new UnauthorizedAccessException("Refresh Token ບໍ່ຖືກຕ້ອງ ກະລຸນາເຂົ້າສູ່ລະບົບໃໝ່"),
    };
}
