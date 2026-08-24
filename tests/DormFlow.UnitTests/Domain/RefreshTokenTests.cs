using DormFlow.Domain.Entities;
using DormFlow.Domain.Enums;
using DormFlow.Domain.Exceptions;
using FluentAssertions;
using Xunit;

namespace DormFlow.UnitTests.Domain;

/// <summary>ตรวจตรรกะวงจรชีวิตของ Refresh Token</summary>
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
    public void IsExpired_เมื่อยังไม่ถึงเวลาหมดอายุ_ต้องเป็นเท็จ()
    {
        DateTimeOffset now = DateTimeOffset.UtcNow;
        RefreshToken token = CreateToken(now.AddDays(1));

        token.IsExpired(now).Should().BeFalse();
    }

    [Fact]
    public void IsExpired_เมื่อถึงเวลาหมดอายุพอดี_ต้องเป็นจริง()
    {
        DateTimeOffset now = DateTimeOffset.UtcNow;
        RefreshToken token = CreateToken(now);

        token.IsExpired(now).Should().BeTrue();
    }

    [Fact]
    public void IsActive_เมื่อยังไม่หมดอายุและไม่ถูกเพิกถอน_ต้องเป็นจริง()
    {
        DateTimeOffset now = DateTimeOffset.UtcNow;
        RefreshToken token = CreateToken(now.AddDays(1));

        token.IsActive(now).Should().BeTrue();
    }

    [Fact]
    public void IsActive_หลังถูกเพิกถอนแล้ว_ต้องเป็นเท็จ()
    {
        DateTimeOffset now = DateTimeOffset.UtcNow;
        RefreshToken token = CreateToken(now.AddDays(1));

        token.Revoke(now, RefreshTokenRevocationReason.Logout);

        token.IsActive(now).Should().BeFalse();
        token.IsRevoked.Should().BeTrue();
    }

    [Fact]
    public void Revoke_ต้องบันทึกสาเหตุและเวลาที่เพิกถอน()
    {
        DateTimeOffset now = DateTimeOffset.UtcNow;
        RefreshToken token = CreateToken(now.AddDays(1));

        token.Revoke(now, RefreshTokenRevocationReason.ReuseDetected, "203.0.113.10");

        token.RevokedAt.Should().Be(now);
        token.RevokedByIp.Should().Be("203.0.113.10");
        token.ReasonRevoked.Should().Be(RefreshTokenRevocationReason.ReuseDetected);
    }

    [Fact]
    public void Revoke_เมื่อระบุโทเคนที่มาแทน_ต้องบันทึก_ReplacedByTokenId()
    {
        DateTimeOffset now = DateTimeOffset.UtcNow;
        RefreshToken token = CreateToken(now.AddDays(1));
        Guid replacementId = Guid.CreateVersion7();

        token.Revoke(now, RefreshTokenRevocationReason.ReplacedByNewToken, replacedByTokenId: replacementId);

        token.ReplacedByTokenId.Should().Be(replacementId);
    }

    [Fact]
    public void Revoke_เมื่อถูกเพิกถอนไปแล้ว_ต้องโยน_DomainException()
    {
        DateTimeOffset now = DateTimeOffset.UtcNow;
        RefreshToken token = CreateToken(now.AddDays(1));
        token.Revoke(now, RefreshTokenRevocationReason.Logout);

        Action act = () => token.Revoke(now, RefreshTokenRevocationReason.RevokedManually);

        act.Should().Throw<DomainException>();
    }
}
