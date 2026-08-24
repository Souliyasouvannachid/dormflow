namespace DormFlow.Contracts.Buildings;

/// <summary>ข้อมูลอาคารหนึ่งหลัง</summary>
public class BuildingResponse
{
    /// <summary>รหัสอ้างอิงของอาคาร</summary>
    public Guid Id { get; init; }

    /// <summary>องค์กรเจ้าของอาคาร</summary>
    public Guid OrganizationId { get; init; }

    /// <summary>หอพักที่อาคารนี้สังกัด</summary>
    public Guid DormitoryId { get; init; }

    /// <summary>รหัสอาคาร</summary>
    public string Code { get; init; } = string.Empty;

    /// <summary>ชื่ออาคาร</summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>คำอธิบายเพิ่มเติม</summary>
    public string? Description { get; init; }

    /// <summary>จำนวนชั้นทั้งหมดของอาคาร</summary>
    public int TotalFloors { get; init; }

    /// <summary>สถานะของอาคาร</summary>
    public string Status { get; init; } = string.Empty;

    /// <summary>เวลาที่สร้างข้อมูล</summary>
    public DateTimeOffset CreatedAt { get; init; }

    /// <summary>เวลาที่แก้ไขข้อมูลล่าสุด</summary>
    public DateTimeOffset? UpdatedAt { get; init; }
}
