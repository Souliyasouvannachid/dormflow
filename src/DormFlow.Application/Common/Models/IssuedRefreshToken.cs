namespace DormFlow.Application.Common.Models;

/// <summary>
/// Refresh Token ที่เพิ่งออกให้ผู้ใช้
/// </summary>
/// <remarks>
/// <paramref name="Value"/> คือค่าจริงที่ส่งให้ผู้เรียกและปรากฏเพียงครั้งเดียวเท่านั้น
/// ฐานข้อมูลเก็บไว้เฉพาะค่าแฮช จึงไม่มีทางอ่านค่านี้กลับมาได้อีก
/// </remarks>
/// <param name="Value">ค่าโทเคนจริง</param>
/// <param name="TokenId">รหัสของแถวในตาราง refresh_tokens</param>
/// <param name="ExpiresAt">เวลาหมดอายุ</param>
public sealed record IssuedRefreshToken(
    string Value,
    Guid TokenId,
    DateTimeOffset ExpiresAt);
