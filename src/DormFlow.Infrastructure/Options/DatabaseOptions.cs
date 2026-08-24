using System.ComponentModel.DataAnnotations;

namespace DormFlow.Infrastructure.Options;

/// <summary>
/// ค่าตั้งของการเชื่อมต่อฐานข้อมูล อ่านจาก configuration section ชื่อ Database
/// </summary>
public sealed class DatabaseOptions
{
    /// <summary>ชื่อ section ใน appsettings</summary>
    public const string SectionName = "Database";

    /// <summary>จำนวนครั้งสูงสุดที่ลองเชื่อมต่อใหม่เมื่อเจอข้อผิดพลาดชั่วคราว</summary>
    [Range(0, 10)]
    public int MaxRetryCount { get; set; } = 3;

    /// <summary>ระยะเวลาสูงสุดระหว่างการลองใหม่แต่ละครั้ง (วินาที)</summary>
    [Range(1, 120)]
    public int MaxRetryDelaySeconds { get; set; } = 10;

    /// <summary>เวลาสูงสุดที่ยอมให้คำสั่ง SQL หนึ่งคำสั่งทำงาน (วินาที)</summary>
    [Range(1, 600)]
    public int CommandTimeoutSeconds { get; set; } = 30;

    /// <summary>
    /// แสดงค่าพารามิเตอร์ของคำสั่ง SQL ใน Log หรือไม่
    /// ต้องเป็น false เสมอใน Production เพราะทำให้ข้อมูลส่วนบุคคลหลุดลง Log
    /// </summary>
    public bool EnableSensitiveDataLogging { get; set; }

    /// <summary>แสดงรายละเอียดข้อผิดพลาดของ EF Core เพิ่มเติมหรือไม่ ใช้เฉพาะตอนพัฒนา</summary>
    public bool EnableDetailedErrors { get; set; }

    /// <summary>
    /// รัน Migration ที่ค้างอยู่ให้อัตโนมัติตอนแอปพลิเคชันเริ่มทำงานหรือไม่
    /// ควรเปิดเฉพาะบนเครื่องนักพัฒนา ส่วน Production ควรรัน Migration เป็นขั้นตอนแยกก่อน deploy
    /// </summary>
    public bool AutoMigrate { get; set; }

    /// <summary>สร้างข้อมูลตั้งต้น เช่น สิทธิ์และบทบาท ตอนแอปพลิเคชันเริ่มทำงานหรือไม่</summary>
    public bool SeedOnStartup { get; set; } = true;
}
