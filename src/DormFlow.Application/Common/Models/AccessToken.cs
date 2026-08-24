namespace DormFlow.Application.Common.Models;

/// <summary>Access Token ที่ออกให้ผู้ใช้พร้อมข้อมูลกำกับ</summary>
/// <param name="Value">ค่าโทเคนแบบ JWT</param>
/// <param name="JwtId">ค่า jti ของโทเคน ใช้ผูกกับ Refresh Token ที่ออกคู่กัน</param>
/// <param name="ExpiresAt">เวลาหมดอายุ</param>
/// <param name="ExpiresInSeconds">อายุที่เหลือหน่วยเป็นวินาที</param>
public sealed record AccessToken(
    string Value,
    string JwtId,
    DateTimeOffset ExpiresAt,
    int ExpiresInSeconds);
