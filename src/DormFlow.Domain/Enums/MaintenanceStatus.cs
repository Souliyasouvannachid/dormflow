namespace DormFlow.Domain.Enums;

/// <summary>สถานะของงานซ่อมบำรุง</summary>
public enum MaintenanceStatus
{
    /// <summary>แจ้งซ่อมแล้ว ยังไม่ได้มอบหมายช่าง</summary>
    Reported = 1,

    /// <summary>มอบหมายช่างซ่อมบำรุงแล้ว</summary>
    Assigned = 2,

    /// <summary>กำลังดำเนินการซ่อม</summary>
    InProgress = 3,

    /// <summary>ซ่อมเสร็จแล้ว</summary>
    Completed = 4,

    /// <summary>ยกเลิก</summary>
    Cancelled = 5,
}
