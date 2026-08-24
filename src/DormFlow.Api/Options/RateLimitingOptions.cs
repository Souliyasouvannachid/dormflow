using System.ComponentModel.DataAnnotations;

namespace DormFlow.Api.Options;

/// <summary>ค่าตั้ง Rate Limiting อ่านจาก configuration section ชื่อ RateLimiting</summary>
public sealed class RateLimitingOptions
{
    /// <summary>ชื่อ section ใน appsettings</summary>
    public const string SectionName = "RateLimiting";

    /// <summary>ชื่อ policy ที่ใช้เป็นค่าเริ่มต้นของทุก Endpoint</summary>
    public const string GlobalPolicyName = "dormflow-global";

    /// <summary>เปิดใช้งาน Rate Limiting หรือไม่</summary>
    public bool Enabled { get; set; } = true;

    /// <summary>จำนวนคำขอสูงสุดต่อหนึ่งช่วงเวลา</summary>
    [Range(1, 100000)]
    public int PermitLimit { get; set; } = 100;

    /// <summary>ความยาวของช่วงเวลา (วินาที)</summary>
    [Range(1, 3600)]
    public int WindowSeconds { get; set; } = 60;

    /// <summary>จำนวนคำขอที่ยอมให้รอคิวเมื่อเกินโควตา</summary>
    [Range(0, 1000)]
    public int QueueLimit { get; set; }
}
