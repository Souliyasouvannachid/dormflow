using DormFlow.Application.Common.Abstractions;
using DormFlow.Application.Common.Models;
using DormFlow.Domain.Entities;
using DormFlow.Domain.Enums;
using DormFlow.Infrastructure.Options;
using DormFlow.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace DormFlow.Infrastructure.Authentication;

/// <summary>
/// ການຄຸ້ມຄອງວົງຈອນຊີວິດຂອງ Refresh Token ສຸດ PostgreSQL
/// </summary>
/// <remarks>
/// ກົນໄກການນໍາໃຊ້ Token Rotation ທຸກໆຄັ້ງທີ່ທ່ານຕໍ່ອາຍຸ ໂຕເຄັນຕົ້ນສະບັບໄດ້ຖືກຖອນຄືນ ແລະມີການອອກອັນໃໝ່ແທນ
/// ຖ້າໃບທີ່ຖືກຝຸນແລ້ວຖືກນໍາມາໃຊ້ໃຝ່, ນີ້ຝາຍຄວາມວ່າ tokens ໄດ້ຕົກຢູ່ໃນມືທີ່ບໍ່ຖືກຕ້ອງ
/// tokens ທີ່ເຮັດວຽກທັງຝົດສໍາລັບຜູ້ໃຊ້ນັ້ນຈະຖືກຖອນຄືນທັນທີ
/// </remarks>
public sealed class RefreshTokenService(
    DormFlowDbContext dbContext,
    IDateTimeProvider dateTimeProvider,
    IOptions<JwtOptions> jwtOptions) : IRefreshTokenService
{
    private readonly DormFlowDbContext _dbContext = dbContext;
    private readonly IDateTimeProvider _dateTimeProvider = dateTimeProvider;
    private readonly JwtOptions _jwtOptions = jwtOptions.Value;

    /// <inheritdoc />
    public async Task<IssuedRefreshToken> IssueAsync(
        Guid userId,
        string jwtId,
        string? ipAddress,
        string? userAgent,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(jwtId);

        (RefreshToken entity, string rawToken) = CreateToken(userId, jwtId, ipAddress, userAgent);

        _dbContext.RefreshTokens.Add(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new IssuedRefreshToken(rawToken, entity.Id, entity.ExpiresAt);
    }

    /// <inheritdoc />
    public async Task<RefreshTokenValidationResult> ValidateAsync(
        string rawToken,
        string? ipAddress,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(rawToken))
        {
            return RefreshTokenValidationResult.Invalid(RefreshTokenValidationStatus.NotFound);
        }

        string tokenHash = RefreshTokenGenerator.ComputeHash(rawToken);

        RefreshToken? token = await _dbContext.RefreshTokens
            .FirstOrDefaultAsync(candidate => candidate.TokenHash == tokenHash, cancellationToken);

        if (token is null)
        {
            return RefreshTokenValidationResult.Invalid(RefreshTokenValidationStatus.NotFound);
        }

        DateTimeOffset now = _dateTimeProvider.UtcNow;

        if (token.IsRevoked)
        {
            // ມັນໄດ້ຖືກຖອນຄືນເພາະວ່າໂທເຄັນໄດ້ຖືກຝຸນໄປກ່ອນແລ້ວ. ແຕ່ຍັງມີຄົນທີ່ໃຊ້ເຄື່ອງເກົ່າ. ມັນຝາຍຄວາມວ່າ token ຖືກຮົ່ວ
            if (token.ReasonRevoked == RefreshTokenRevocationReason.ReplacedByNewToken)
            {
                await RevokeAllForUserAsync(
                    token.UserId,
                    RefreshTokenRevocationReason.ReuseDetected,
                    ipAddress,
                    cancellationToken);

                return RefreshTokenValidationResult.Invalid(
                    RefreshTokenValidationStatus.ReuseDetected,
                    token.UserId);
            }

            return RefreshTokenValidationResult.Invalid(RefreshTokenValidationStatus.Revoked, token.UserId);
        }

        if (token.IsExpired(now))
        {
            return RefreshTokenValidationResult.Invalid(RefreshTokenValidationStatus.Expired, token.UserId);
        }

        return RefreshTokenValidationResult.Valid(token.Id, token.UserId);
    }

    /// <inheritdoc />
    public async Task<IssuedRefreshToken> RotateAsync(
        Guid currentTokenId,
        string jwtId,
        string? ipAddress,
        string? userAgent,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(jwtId);

        RefreshToken current = await _dbContext.RefreshTokens
                .FirstOrDefaultAsync(token => token.Id == currentTokenId, cancellationToken)
            ?? throw new InvalidOperationException($"ບໍ່ພົບ Refresh Token ລະຫັດ {currentTokenId} ທີ່ຕ້ອງການໝູນ");

        (RefreshToken replacement, string rawToken) = CreateToken(
            current.UserId,
            jwtId,
            ipAddress,
            userAgent);

        current.Revoke(
            _dateTimeProvider.UtcNow,
            RefreshTokenRevocationReason.ReplacedByNewToken,
            ipAddress,
            replacement.Id);

        _dbContext.RefreshTokens.Add(replacement);

        // ການຖອນໃບຢັ້ງຢືນເກົ່າແລະການອອກໃບຢັ້ງຢືນໃໝ່ຕ້ອງສຳເລັດຫຼືບໍ່ພ້ອມກັນ
        // SaveChangesAsync ການໃຊ້ເວລາຜຶ່ງແລ້ວກວມເອົາທັງສອງໃນຜຶ່ງທຸລະກໍາ
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new IssuedRefreshToken(rawToken, replacement.Id, replacement.ExpiresAt);
    }

    /// <inheritdoc />
    public async Task<bool> RevokeAsync(
        string rawToken,
        RefreshTokenRevocationReason reason,
        string? ipAddress,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(rawToken))
        {
            return false;
        }

        string tokenHash = RefreshTokenGenerator.ComputeHash(rawToken);
        DateTimeOffset now = _dateTimeProvider.UtcNow;

        RefreshToken? token = await _dbContext.RefreshTokens
            .FirstOrDefaultAsync(candidate => candidate.TokenHash == tokenHash, cancellationToken);

        if (token is null || !token.IsActive(now))
        {
            return false;
        }

        token.Revoke(now, reason, ipAddress);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return true;
    }

    /// <inheritdoc />
    public async Task<int> RevokeAllForUserAsync(
        Guid userId,
        RefreshTokenRevocationReason reason,
        string? ipAddress,
        CancellationToken cancellationToken = default)
    {
        DateTimeOffset now = _dateTimeProvider.UtcNow;

        List<RefreshToken> activeTokens = await _dbContext.RefreshTokens
            .Where(token => token.UserId == userId && token.RevokedAt == null && token.ExpiresAt > now)
            .ToListAsync(cancellationToken);

        if (activeTokens.Count == 0)
        {
            return 0;
        }

        foreach (RefreshToken token in activeTokens)
        {
            token.Revoke(now, reason, ipAddress);
        }

        await _dbContext.SaveChangesAsync(cancellationToken);

        return activeTokens.Count;
    }

    private (RefreshToken Entity, string RawToken) CreateToken(
        Guid userId,
        string jwtId,
        string? ipAddress,
        string? userAgent)
    {
        string rawToken = RefreshTokenGenerator.CreateToken();

        RefreshToken entity = new()
        {
            UserId = userId,
            TokenHash = RefreshTokenGenerator.ComputeHash(rawToken),
            JwtId = jwtId,
            ExpiresAt = _dateTimeProvider.UtcNow.AddDays(_jwtOptions.RefreshTokenDays),
            CreatedByIp = ipAddress,
            UserAgent = Truncate(userAgent, 512),
        };

        return (entity, rawToken);
    }

    private static string? Truncate(string? value, int maxLength)
        => value is null || value.Length <= maxLength ? value : value[..maxLength];
}
