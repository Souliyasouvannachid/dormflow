using System.Globalization;
using System.Security.Claims;
using System.Text;
using DormFlow.Application.Common.Abstractions;
using DormFlow.Application.Common.Models;
using DormFlow.Infrastructure.Options;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace DormFlow.Infrastructure.Authentication;

/// <summary>
/// ອອກ Access Token ຕົວແບບ JWT ເຊັນມັນຄືກັນ HMAC-SHA256
/// </summary>
/// <remarks>
/// ໂທເຄັນປະກອບມີທຸກບົດບາດ ແລະລະຫັດສິດພິເສດ. ເພື່ອໃຫ້ແຕ່ລະຄໍາຮ້ອງຂໍຖືກຮັບຮອງໂດຍບໍ່ຈໍາເປັນຕ້ອງໄຟໄຝ້ຖານຂໍ້ມູນ
/// ຜົນຂ້າງຄຽງແມ່ນວ່າການຖອນຄືນຈະມີຜົນເມື່ອ token ໃນປັດຈຸບັນຝົດອາຍຸ
/// ເຊິ່ງແມ່ນຍາວທີ່ສຸດ <see cref="JwtOptions.AccessTokenMinutes"/> ນາທີ
/// SuperAdmin ຢ່າໃສ່ລະຫັດການອະນຸຍາດເຂົ້າໄປໃນ token ເພາະວ່າມັນຖືກກວດສອບແລ້ວໂດຍບົດບາດ. ແລະມັນຮັກສາ token ບໍ່ໃຫ້ໃຫຍ່ເກີນໄປ
/// </remarks>
public sealed class JwtAccessTokenGenerator(
    IOptions<JwtOptions> jwtOptions,
    IDateTimeProvider dateTimeProvider) : IAccessTokenGenerator
{
    private readonly JwtOptions _jwtOptions = jwtOptions.Value;
    private readonly IDateTimeProvider _dateTimeProvider = dateTimeProvider;
    private readonly JsonWebTokenHandler _tokenHandler = new();

    /// <inheritdoc />
    public AccessToken Generate(UserSecurityProfile profile)
    {
        ArgumentNullException.ThrowIfNull(profile);

        DateTimeOffset issuedAt = _dateTimeProvider.UtcNow;
        DateTimeOffset expiresAt = issuedAt.AddMinutes(_jwtOptions.AccessTokenMinutes);
        string jwtId = Guid.CreateVersion7().ToString("N", CultureInfo.InvariantCulture);

        SecurityTokenDescriptor descriptor = new()
        {
            Issuer = _jwtOptions.Issuer,
            Audience = _jwtOptions.Audience,
            IssuedAt = issuedAt.UtcDateTime,
            NotBefore = issuedAt.UtcDateTime,
            Expires = expiresAt.UtcDateTime,
            Subject = new ClaimsIdentity(BuildClaims(profile, jwtId)),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SigningKey)),
                SecurityAlgorithms.HmacSha256),
        };

        string token = _tokenHandler.CreateToken(descriptor);

        return new AccessToken(
            token,
            jwtId,
            expiresAt,
            (int)Math.Round((expiresAt - issuedAt).TotalSeconds));
    }

    private static List<Claim> BuildClaims(UserSecurityProfile profile, string jwtId)
    {
        List<Claim> claims =
        [
            new(DormFlowClaimTypes.UserId, profile.UserId.ToString()),
            new(DormFlowClaimTypes.JwtId, jwtId),
            new(ClaimTypes.NameIdentifier, profile.UserId.ToString()),
            new(ClaimTypes.Name, profile.Email),
            new(ClaimTypes.Email, profile.Email),
            new(DormFlowClaimTypes.FirstName, profile.FirstName),
            new(DormFlowClaimTypes.LastName, profile.LastName),
        ];

        if (profile.OrganizationId.HasValue)
        {
            claims.Add(new Claim(DormFlowClaimTypes.OrganizationId, profile.OrganizationId.Value.ToString()));
        }

        foreach (string role in profile.Roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        foreach (Guid dormitoryId in profile.DormitoryIds)
        {
            claims.Add(new Claim(DormFlowClaimTypes.DormitoryId, dormitoryId.ToString()));
        }

        bool isSuperAdmin = profile.Roles.Contains(DormFlowRoles.SuperAdmin, StringComparer.Ordinal);
        if (isSuperAdmin)
        {
            return claims;
        }

        foreach (string permission in profile.Permissions)
        {
            claims.Add(new Claim(DormFlowClaimTypes.Permission, permission));
        }

        return claims;
    }
}
