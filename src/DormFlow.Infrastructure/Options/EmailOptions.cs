using System.ComponentModel.DataAnnotations;

namespace DormFlow.Infrastructure.Options;

/// <summary>ค่าตั้งของการส่งอีเมล อ่านจาก configuration section ชื่อ Email</summary>
public sealed class EmailOptions
{
    /// <summary>ชื่อ section ใน appsettings</summary>
    public const string SectionName = "Email";

    /// <summary>อีเมลผู้ส่ง</summary>
    [Required]
    [EmailAddress]
    public string FromAddress { get; set; } = "no-reply@dormflow.local";

    /// <summary>ชื่อผู้ส่งที่แสดงในอีเมล</summary>
    public string FromName { get; set; } = "DormFlow";

    /// <summary>
    /// โฟลเดอร์ที่ใช้เก็บอีเมลขาออกแทนการส่งจริง
    /// เส้นทางสัมพัทธ์จะถูกคิดจากโฟลเดอร์ที่แอปพลิเคชันทำงานอยู่
    /// </summary>
    [Required]
    public string PickupDirectory { get; set; } = "logs/emails";
}
