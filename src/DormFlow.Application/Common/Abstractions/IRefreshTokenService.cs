using DormFlow.Application.Common.Models;
using DormFlow.Domain.Enums;

namespace DormFlow.Application.Common.Abstractions;

/// <summary>
/// จัดการวงจรชีวิตของ Refresh Token ทั้งการออก การตรวจสอบ การหมุน และการเพิกถอน
/// </summary>
public interface IRefreshTokenService
{
    /// <summary>ออก Refresh Token ใบใหม่ให้ผู้ใช้</summary>
    /// <param name="userId">เจ้าของโทเคน</param>
    /// <param name="jwtId">ค่า jti ของ Access Token ที่ออกคู่กัน</param>
    /// <param name="ipAddress">หมายเลข IP ต้นทาง</param>
    /// <param name="userAgent">User Agent ของคำขอ</param>
    /// <param name="cancellationToken">โทเคนสำหรับยกเลิกคำสั่ง</param>
    Task<IssuedRefreshToken> IssueAsync(
        Guid userId,
        string jwtId,
        string? ipAddress,
        string? userAgent,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// ตรวจสอบว่าโทเคนที่ผู้เรียกส่งมายังใช้ต่ออายุได้หรือไม่
    /// </summary>
    /// <remarks>
    /// เมื่อพบว่าโทเคนถูกหมุนไปแล้วแต่ถูกนำกลับมาใช้ซ้ำ
    /// เมธอดนี้จะเพิกถอนโทเคนที่ยังใช้งานได้ทั้งหมดของผู้ใช้รายนั้นทันทีก่อนคืนผลลัพธ์
    /// เพราะแปลว่าโทเคนหลุดไปอยู่ในมือผู้อื่นแล้ว
    /// </remarks>
    /// <param name="rawToken">ค่าโทเคนที่ผู้เรียกส่งมา</param>
    /// <param name="ipAddress">หมายเลข IP ต้นทาง</param>
    /// <param name="cancellationToken">โทเคนสำหรับยกเลิกคำสั่ง</param>
    Task<RefreshTokenValidationResult> ValidateAsync(
        string rawToken,
        string? ipAddress,
        CancellationToken cancellationToken = default);

    /// <summary>เพิกถอนโทเคนใบเดิมแล้วออกใบใหม่แทนในธุรกรรมเดียวกัน</summary>
    /// <param name="currentTokenId">รหัสของโทเคนใบเดิม</param>
    /// <param name="jwtId">ค่า jti ของ Access Token ใบใหม่</param>
    /// <param name="ipAddress">หมายเลข IP ต้นทาง</param>
    /// <param name="userAgent">User Agent ของคำขอ</param>
    /// <param name="cancellationToken">โทเคนสำหรับยกเลิกคำสั่ง</param>
    Task<IssuedRefreshToken> RotateAsync(
        Guid currentTokenId,
        string jwtId,
        string? ipAddress,
        string? userAgent,
        CancellationToken cancellationToken = default);

    /// <summary>เพิกถอนโทเคนหนึ่งใบ</summary>
    /// <param name="rawToken">ค่าโทเคนที่ต้องการเพิกถอน</param>
    /// <param name="reason">สาเหตุที่เพิกถอน</param>
    /// <param name="ipAddress">หมายเลข IP ต้นทาง</param>
    /// <param name="cancellationToken">โทเคนสำหรับยกเลิกคำสั่ง</param>
    /// <returns>true เมื่อพบและเพิกถอนสำเร็จ</returns>
    Task<bool> RevokeAsync(
        string rawToken,
        RefreshTokenRevocationReason reason,
        string? ipAddress,
        CancellationToken cancellationToken = default);

    /// <summary>เพิกถอนโทเคนที่ยังใช้งานได้ทั้งหมดของผู้ใช้</summary>
    /// <param name="userId">เจ้าของโทเคน</param>
    /// <param name="reason">สาเหตุที่เพิกถอน</param>
    /// <param name="ipAddress">หมายเลข IP ต้นทาง</param>
    /// <param name="cancellationToken">โทเคนสำหรับยกเลิกคำสั่ง</param>
    /// <returns>จำนวนโทเคนที่ถูกเพิกถอน</returns>
    Task<int> RevokeAllForUserAsync(
        Guid userId,
        RefreshTokenRevocationReason reason,
        string? ipAddress,
        CancellationToken cancellationToken = default);
}
