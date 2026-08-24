namespace DormFlow.Api.Options;

/// <summary>ค่าตั้ง CORS อ่านจาก configuration section ชื่อ Cors</summary>
public sealed class CorsOptions
{
    /// <summary>ชื่อ section ใน appsettings</summary>
    public const string SectionName = "Cors";

    /// <summary>ชื่อ policy ที่ใช้ในไปป์ไลน์</summary>
    public const string PolicyName = "DormFlowCors";

    /// <summary>
    /// รายการ Origin ที่อนุญาต ต้องระบุอย่างน้อยหนึ่งค่าเมื่อรันใน Production
    /// </summary>
    public IList<string> AllowedOrigins { get; } = [];
}
