namespace DormFlow.Infrastructure.Options;

/// <summary>
/// ค่าตั้งของ Background Job อ่านจาก configuration section ชื่อ BackgroundJobs
/// </summary>
public sealed class BackgroundJobOptions
{
    /// <summary>ชื่อ section ใน appsettings</summary>
    public const string SectionName = "BackgroundJobs";

    /// <summary>
    /// เปิดใช้งาน Background Job หรือไม่ (Dashboard และ Recurring Job ทั้งหมด)
    /// ปิดได้โดยไม่ต้อง deploy ใหม่ เผื่อกรณีต้องการหยุดชั่วคราวเพื่อบำรุงรักษา
    /// </summary>
    public bool Enabled { get; set; } = true;
}
