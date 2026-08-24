namespace DormFlow.Contracts.RoomTypes;

/// <summary>ข้อมูลประเภทห้องหนึ่งแบบ</summary>
public class RoomTypeResponse
{
    /// <summary>รหัสอ้างอิงของประเภทห้อง</summary>
    public Guid Id { get; init; }

    /// <summary>องค์กรเจ้าของประเภทห้อง</summary>
    public Guid OrganizationId { get; init; }

    /// <summary>หอพักที่ประเภทห้องนี้สังกัด</summary>
    public Guid DormitoryId { get; init; }

    /// <summary>รหัสประเภทห้อง</summary>
    public string Code { get; init; } = string.Empty;

    /// <summary>ชื่อประเภทห้อง</summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>คำอธิบายเพิ่มเติม</summary>
    public string? Description { get; init; }

    /// <summary>ค่าเช่ารายเดือน</summary>
    public decimal MonthlyRent { get; init; }

    /// <summary>ค่าเช่ารายวัน</summary>
    public decimal? DailyRent { get; init; }

    /// <summary>ค่ามัดจำ</summary>
    public decimal DepositAmount { get; init; }

    /// <summary>จำนวนผู้เข้าพักสูงสุด</summary>
    public int MaximumOccupants { get; init; }

    /// <summary>สถานะของประเภทห้อง</summary>
    public string Status { get; init; } = string.Empty;

    /// <summary>เวลาที่สร้างข้อมูล</summary>
    public DateTimeOffset CreatedAt { get; init; }

    /// <summary>เวลาที่แก้ไขข้อมูลล่าสุด</summary>
    public DateTimeOffset? UpdatedAt { get; init; }
}
