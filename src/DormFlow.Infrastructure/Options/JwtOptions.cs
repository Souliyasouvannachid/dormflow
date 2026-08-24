using System.ComponentModel.DataAnnotations;

namespace DormFlow.Infrastructure.Options;

/// <summary>
/// ค่าตั้งของ JWT อ่านจาก configuration section ชื่อ Jwt
/// </summary>
/// <remarks>
/// SigningKey เป็นความลับ ต้องกำหนดผ่าน environment variable ชื่อ Jwt__SigningKey
/// หรือ Secret Manager เท่านั้น ห้ามใส่ค่าจริงลงใน appsettings.json
/// </remarks>
public sealed class JwtOptions
{
    /// <summary>ชื่อ section ใน appsettings</summary>
    public const string SectionName = "Jwt";

    /// <summary>ความยาวขั้นต่ำของกุญแจ HMAC-SHA256 หน่วยเป็นตัวอักษร เทียบเท่า 256 บิต</summary>
    public const int MinimumSigningKeyLength = 32;

    /// <summary>กุญแจที่ใช้เซ็นและตรวจลายเซ็นของโทเคน</summary>
    [Required(ErrorMessage = "ต้องกำหนด Jwt:SigningKey ผ่าน environment variable หรือ Secret Manager")]
    [MinLength(MinimumSigningKeyLength, ErrorMessage = "Jwt:SigningKey ต้องยาวอย่างน้อย 32 ตัวอักษร")]
    public string SigningKey { get; set; } = string.Empty;

    /// <summary>ผู้ออกโทเคน</summary>
    [Required]
    public string Issuer { get; set; } = "DormFlow";

    /// <summary>ผู้รับโทเคน</summary>
    [Required]
    public string Audience { get; set; } = "DormFlow.Client";

    /// <summary>อายุของ Access Token หน่วยเป็นนาที</summary>
    [Range(1, 120)]
    public int AccessTokenMinutes { get; set; } = 15;

    /// <summary>อายุของ Refresh Token หน่วยเป็นวัน</summary>
    [Range(1, 365)]
    public int RefreshTokenDays { get; set; } = 7;

    /// <summary>
    /// ค่าความคลาดเคลื่อนของเวลาที่ยอมรับได้ หน่วยเป็นวินาที
    /// ตั้งเป็น 0 เพื่อให้โทเคนหมดอายุตรงเวลาจริง ไม่ใช่ยืดออกไปอีก 5 นาทีตามค่าเริ่มต้นของไลบรารี
    /// </summary>
    [Range(0, 300)]
    public int ClockSkewSeconds { get; set; }
}
