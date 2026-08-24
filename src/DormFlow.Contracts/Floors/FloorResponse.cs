namespace DormFlow.Contracts.Floors;

/// <summary>ข้อมูลชั้นหนึ่งของอาคาร</summary>
public class FloorResponse
{
    /// <summary>รหัสอ้างอิงของชั้น</summary>
    public Guid Id { get; init; }

    /// <summary>องค์กรเจ้าของชั้นนี้</summary>
    public Guid OrganizationId { get; init; }

    /// <summary>หอพักที่ชั้นนี้สังกัด</summary>
    public Guid DormitoryId { get; init; }

    /// <summary>อาคารที่ชั้นนี้สังกัด</summary>
    public Guid BuildingId { get; init; }

    /// <summary>ลำดับชั้น</summary>
    public int FloorNumber { get; init; }

    /// <summary>ชื่อชั้น สำหรับแสดงผล</summary>
    public string? Name { get; init; }

    /// <summary>คำอธิบายเพิ่มเติม</summary>
    public string? Description { get; init; }

    /// <summary>สถานะของชั้น</summary>
    public string Status { get; init; } = string.Empty;

    /// <summary>เวลาที่สร้างข้อมูล</summary>
    public DateTimeOffset CreatedAt { get; init; }

    /// <summary>เวลาที่แก้ไขข้อมูลล่าสุด</summary>
    public DateTimeOffset? UpdatedAt { get; init; }
}
