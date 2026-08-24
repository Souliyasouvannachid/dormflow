using System.ComponentModel.DataAnnotations;

namespace DormFlow.Api.Options;

/// <summary>ข้อมูลประกอบเอกสาร OpenAPI อ่านจาก configuration section ชื่อ Api</summary>
public sealed class ApiOptions
{
    /// <summary>ชื่อ section ใน appsettings</summary>
    public const string SectionName = "Api";

    /// <summary>ชื่อเอกสาร API</summary>
    [Required]
    public string Title { get; set; } = "DormFlow API";

    /// <summary>คำอธิบายระบบ</summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>ชื่อผู้ดูแล API</summary>
    public string ContactName { get; set; } = string.Empty;

    /// <summary>อีเมลผู้ดูแล API</summary>
    [EmailAddress]
    public string ContactEmail { get; set; } = string.Empty;
}
