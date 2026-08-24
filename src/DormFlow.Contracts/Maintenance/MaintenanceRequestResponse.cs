namespace DormFlow.Contracts.Maintenance;

/// <summary>ข้อมูลงานแจ้งซ่อมบำรุงหนึ่งรายการ</summary>
public class MaintenanceRequestResponse
{
    /// <summary>รหัสอ้างอิงของงานซ่อม</summary>
    public Guid Id { get; init; }

    /// <summary>องค์กรที่เกี่ยวข้อง</summary>
    public Guid OrganizationId { get; init; }

    /// <summary>หอพักที่เกี่ยวข้อง</summary>
    public Guid DormitoryId { get; init; }

    /// <summary>ห้องที่แจ้งซ่อม</summary>
    public Guid RoomId { get; init; }

    /// <summary>หัวข้อของงานซ่อม</summary>
    public string Title { get; init; } = string.Empty;

    /// <summary>รายละเอียดเพิ่มเติมของปัญหา</summary>
    public string? Description { get; init; }

    /// <summary>ระดับความสำคัญ</summary>
    public string Priority { get; init; } = string.Empty;

    /// <summary>สถานะของงานซ่อม</summary>
    public string Status { get; init; } = string.Empty;

    /// <summary>ช่างซ่อมบำรุงที่ได้รับมอบหมาย</summary>
    public Guid? AssignedTechnicianId { get; init; }

    /// <summary>เวลาที่มอบหมายช่างล่าสุด</summary>
    public DateTimeOffset? AssignedAt { get; init; }

    /// <summary>เวลาที่ซ่อมเสร็จ</summary>
    public DateTimeOffset? CompletedAt { get; init; }

    /// <summary>บันทึกผลการซ่อม</summary>
    public string? ResolutionNotes { get; init; }

    /// <summary>เวลาที่สร้างข้อมูล</summary>
    public DateTimeOffset CreatedAt { get; init; }
}
