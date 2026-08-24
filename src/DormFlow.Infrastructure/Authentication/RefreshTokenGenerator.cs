using System.Security.Cryptography;

namespace DormFlow.Infrastructure.Authentication;

/// <summary>
/// สร้างและแฮชค่าของ Refresh Token
/// </summary>
/// <remarks>
/// ค่าจริงของโทเคนถูกส่งให้ผู้เรียกเพียงครั้งเดียว ฐานข้อมูลเก็บเฉพาะค่าแฮช
/// การตรวจสอบทำโดยแฮชค่าที่ผู้เรียกส่งมาแล้วค้นหาด้วยค่าแฮชนั้น
/// ถ้าฐานข้อมูลรั่ว ผู้ที่ได้ข้อมูลไปจะนำค่าแฮชไปแลก Access Token ต่อไม่ได้
/// </remarks>
public static class RefreshTokenGenerator
{
    /// <summary>จำนวนไบต์สุ่มที่ใช้สร้างโทเคนหนึ่งใบ</summary>
    private const int TokenByteLength = 64;

    /// <summary>สร้างค่าโทเคนใหม่แบบสุ่มด้วยตัวสร้างเลขสุ่มเชิงเข้ารหัส</summary>
    public static string CreateToken()
        => Base64UrlEncode(RandomNumberGenerator.GetBytes(TokenByteLength));

    /// <summary>คำนวณค่าแฮชของโทเคนในรูปแบบเลขฐานสิบหกตัวพิมพ์เล็ก</summary>
    /// <param name="token">ค่าโทเคนจริง</param>
    /// <exception cref="ArgumentException">เมื่อค่าโทเคนว่าง</exception>
    public static string ComputeHash(string token)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(token);

        byte[] hash = SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(token));
        return Convert.ToHexStringLower(hash);
    }

    /// <summary>
    /// เข้ารหัสไบต์เป็นข้อความที่ใช้ได้ทั้งใน URL และใน JSON
    /// ตัดอักขระที่ต้องหลบหลีกใน URL ออกทั้งหมด
    /// </summary>
    private static string Base64UrlEncode(byte[] bytes)
        => Convert.ToBase64String(bytes)
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
}
