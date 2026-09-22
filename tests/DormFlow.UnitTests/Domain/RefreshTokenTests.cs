using DormFlow.Domain.Entities;
using DormFlow.Domain.Enums;
using DormFlow.Domain.Exceptions;
using FluentAssertions;
using Xunit;

namespace DormFlow.UnitTests.Domain;

/// <summary>ກວດເບິ່ງເຫດຜົນຂອງວົງຈອນຊີວິດຂອງ Refresh Token</summary>
public class RefreshTokenTests
{
    private static RefreshToken CreateToken(DateTimeOffset expiresAt)
        => new()
        {
            UserId = Guid.CreateVersion7(),
            TokenHash = "hash",
            JwtId = "jwt-id",
            ExpiresAt = expiresAt,
        };

    [Fact]
    public void IsExpired_ເມື່ອເວລາຍັງບໍ່ໝົດ_ຈະຕ້ອງບໍ່ຖືກຕ້ອງ()
    {
        DateTimeOffset now = DateTimeOffset.UtcNow;
        RefreshToken token = CreateToken(now.AddDays(1));

        token.IsExpired(now).Should().BeFalse();
    }

    [Fact]
    public void IsExpired_ເມື່ອເຖິງເວລາໝົດອາຍຸ_ຕ້ອງເປັນຄວາມຈິງ()
    {
        DateTimeOffset now = DateTimeOffset.UtcNow;
        RefreshToken token = CreateToken(now);

        token.IsExpired(now).Should().BeTrue();
    }

    [Fact]
    public void IsActive_ໃນເວລາທີ່ມັນຍັງບໍ່ທັນຝົດອາຍຸແລະບໍ່ໄດ້ຮັບການຖອນຄືນ_ຕ້ອງເປັນຄວາມຈິງ()
    {
        DateTimeOffset now = DateTimeOffset.UtcNow;
        RefreshToken token = CreateToken(now.AddDays(1));

        token.IsActive(now).Should().BeTrue();
    }

    [Fact]
    public void IsActive_ຫຼັງຈາກໄດ້ຖືກຍົກເລີກ_ຈະຕ້ອງບໍ່ຖືກຕ້ອງ()
    {
        DateTimeOffset now = DateTimeOffset.UtcNow;
        RefreshToken token = CreateToken(now.AddDays(1));

        token.Revoke(now, RefreshTokenRevocationReason.Logout);

        token.IsActive(now).Should().BeFalse();
        token.IsRevoked.Should().BeTrue();
    }

    [Fact]
    public void Revoke_ເຫດຜົນແລະເວລາຂອງການຖອນຕ້ອງຖືກບັນທຶກ()
    {
        DateTimeOffset now = DateTimeOffset.UtcNow;
        RefreshToken token = CreateToken(now.AddDays(1));

        token.Revoke(now, RefreshTokenRevocationReason.ReuseDetected, "203.0.113.10");

        token.RevokedAt.Should().Be(now);
        token.RevokedByIp.Should().Be("203.0.113.10");
        token.ReasonRevoked.Should().Be(RefreshTokenRevocationReason.ReuseDetected);
    }

    [Fact]
    public void Revoke_ເມື່ອກໍານົດtokenທົດແທນ_ຕ້ອງປະຫຍັດ_ReplacedByTokenId()
    {
        DateTimeOffset now = DateTimeOffset.UtcNow;
        RefreshToken token = CreateToken(now.AddDays(1));
        Guid replacementId = Guid.CreateVersion7();

        token.Revoke(now, RefreshTokenRevocationReason.ReplacedByNewToken, replacedByTokenId: replacementId);

        token.ReplacedByTokenId.Should().Be(replacementId);
    }

    [Fact]
    public void Revoke_ເມື່ອໄດ້ຖືກຖອນຄືນແລ້ວ_ຕ້ອງຖິ້ມ_DomainException()
    {
        DateTimeOffset now = DateTimeOffset.UtcNow;
        RefreshToken token = CreateToken(now.AddDays(1));
        token.Revoke(now, RefreshTokenRevocationReason.Logout);

        Action act = () => token.Revoke(now, RefreshTokenRevocationReason.RevokedManually);

        act.Should().Throw<DomainException>();
    }
}
