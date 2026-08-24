using DormFlow.Domain.Common;
using DormFlow.Domain.Enums;

namespace DormFlow.Domain.Entities;

/// <summary>
/// งานแจ้งซ่อมบำรุงหนึ่งรายการของห้องหนึ่งห้อง
/// </summary>
/// <remarks>
/// ไม่มีฟิลด์ผู้แจ้งแยกต่างหาก เพราะ <see cref="Common.IAuditableEntity.CreatedBy"/> ที่ตั้งค่าอัตโนมัติ
/// ทุกครั้งที่บันทึกก็บอกได้อยู่แล้วว่าใครเป็นผู้แจ้ง
/// </remarks>
public class MaintenanceRequest : TenantEntity
{
    /// <summary>ห้องที่แจ้งซ่อม</summary>
    public Guid RoomId { get; set; }

    /// <summary>หัวข้อของงานซ่อม</summary>
    public required string Title { get; set; }

    /// <summary>รายละเอียดเพิ่มเติมของปัญหา</summary>
    public string? Description { get; set; }

    /// <summary>ระดับความสำคัญ</summary>
    public MaintenancePriority Priority { get; set; } = MaintenancePriority.Medium;

    /// <summary>สถานะของงานซ่อม</summary>
    public MaintenanceStatus Status { get; set; } = MaintenanceStatus.Reported;

    /// <summary>ช่างซ่อมบำรุงที่ได้รับมอบหมาย เป็น null เมื่อยังไม่ได้มอบหมาย</summary>
    public Guid? AssignedTechnicianId { get; set; }

    /// <summary>เวลาที่มอบหมายช่างล่าสุด</summary>
    public DateTimeOffset? AssignedAt { get; set; }

    /// <summary>เวลาที่ซ่อมเสร็จ เป็น null เมื่อยังไม่เสร็จ</summary>
    public DateTimeOffset? CompletedAt { get; set; }

    /// <summary>บันทึกผลการซ่อม</summary>
    public string? ResolutionNotes { get; set; }
}
