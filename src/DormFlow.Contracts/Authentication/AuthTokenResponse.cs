namespace DormFlow.Contracts.Authentication;

/// <summary>ผลลัพธ์ของการเข้าสู่ระบบหรือการต่ออายุโทเคน</summary>
public class AuthTokenResponse
{
    /// <summary>Access Token แบบ JWT ใช้แนบใน Authorization header</summary>
    public string AccessToken { get; init; } = string.Empty;

    /// <summary>ชนิดของโทเคน ใช้ค่า Bearer เสมอ</summary>
    public string TokenType { get; init; } = "Bearer";

    /// <summary>อายุที่เหลือของ Access Token หน่วยเป็นวินาที</summary>
    public int ExpiresIn { get; init; }

    /// <summary>เวลาหมดอายุของ Access Token</summary>
    public DateTimeOffset ExpiresAt { get; init; }

    /// <summary>
    /// Refresh Token ใช้ขอ Access Token ใบใหม่
    /// ทุกครั้งที่ใช้ ระบบจะออกใบใหม่ให้และเพิกถอนใบเดิมทันที
    /// </summary>
    public string RefreshToken { get; init; } = string.Empty;

    /// <summary>เวลาหมดอายุของ Refresh Token</summary>
    public DateTimeOffset RefreshTokenExpiresAt { get; init; }

    /// <summary>ข้อมูลของผู้ใช้ที่เข้าสู่ระบบ</summary>
    public CurrentUserResponse User { get; init; } = new();
}
