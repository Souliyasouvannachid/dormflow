using DormFlow.Domain.Common;
using DormFlow.Domain.Enums;
using DormFlow.Domain.Exceptions;

namespace DormFlow.Domain.Entities;

/// <summary>
/// Refresh Token หนึ่งใบ เก็บเฉพาะค่าแฮชของโทเคน ไม่เก็บค่าจริง
/// </summary>
/// <remarks>
/// ถ้าฐานข้อมูลรั่ว ผู้ที่ได้ข้อมูลไปจะนำค่าแฮชไปขอ Access Token ต่อไม่ได้
/// เพราะระบบตรวจโดยแฮชค่าที่ผู้เรียกส่งมาแล้วเทียบ ไม่ได้เทียบค่าที่เก็บไว้โดยตรง
/// </remarks>
public class RefreshToken : BaseEntity
{
    /// <summary>ผู้ใช้ที่เป็นเจ้าของโทเคน</summary>
    public Guid UserId { get; set; }

    /// <summary>ค่าแฮชของโทเคน</summary>
    public required string TokenHash { get; set; }

    /// <summary>รหัสอ้างอิงของ Access Token ที่ออกคู่กัน ใช้ตรวจสอบย้อนหลัง</summary>
    public required string JwtId { get; set; }

    /// <summary>เวลาหมดอายุ</summary>
    public DateTimeOffset ExpiresAt { get; set; }

    /// <summary>หมายเลข IP ที่ขอโทเคนนี้</summary>
    public string? CreatedByIp { get; set; }

    /// <summary>User Agent ที่ขอโทเคนนี้</summary>
    public string? UserAgent { get; set; }

    /// <summary>เวลาที่ถูกเพิกถอน</summary>
    public DateTimeOffset? RevokedAt { get; set; }

    /// <summary>หมายเลข IP ที่สั่งเพิกถอน</summary>
    public string? RevokedByIp { get; set; }

    /// <summary>โทเคนใบใหม่ที่มาแทนใบนี้ตามกลไก Token Rotation</summary>
    public Guid? ReplacedByTokenId { get; set; }

    /// <summary>สาเหตุที่ถูกเพิกถอน</summary>
    public RefreshTokenRevocationReason? ReasonRevoked { get; set; }

    /// <summary>ถูกเพิกถอนไปแล้วหรือยัง</summary>
    public bool IsRevoked => RevokedAt.HasValue;

    /// <summary>ตรวจว่าโทเคนหมดอายุแล้วหรือยัง</summary>
    /// <param name="now">เวลาปัจจุบัน</param>
    public bool IsExpired(DateTimeOffset now) => now >= ExpiresAt;

    /// <summary>ตรวจว่าโทเคนยังใช้ต่ออายุได้อยู่หรือไม่</summary>
    /// <param name="now">เวลาปัจจุบัน</param>
    public bool IsActive(DateTimeOffset now) => !IsRevoked && !IsExpired(now);

    /// <summary>เพิกถอนโทเคนใบนี้</summary>
    /// <param name="now">เวลาที่เพิกถอน</param>
    /// <param name="reason">สาเหตุที่เพิกถอน</param>
    /// <param name="revokedByIp">หมายเลข IP ที่สั่งเพิกถอน</param>
    /// <param name="replacedByTokenId">โทเคนใบใหม่ที่มาแทน กรณีเพิกถอนเพราะหมุนโทเคน</param>
    /// <exception cref="DomainException">เมื่อโทเคนถูกเพิกถอนไปแล้ว</exception>
    public void Revoke(
        DateTimeOffset now,
        RefreshTokenRevocationReason reason,
        string? revokedByIp = null,
        Guid? replacedByTokenId = null)
    {
        if (IsRevoked)
        {
            throw new DomainException("Refresh Token ใบนี้ถูกเพิกถอนไปแล้ว");
        }

        RevokedAt = now;
        RevokedByIp = revokedByIp;
        ReasonRevoked = reason;
        ReplacedByTokenId = replacedByTokenId;
    }
}
