namespace DormFlow.Contracts.Rooms;

/// <summary>ข้อมูลห้องพักหนึ่งห้อง</summary>
public class RoomResponse
{
    /// <summary>รหัสอ้างอิงของห้องพัก</summary>
    public Guid Id { get; init; }

    /// <summary>องค์กรเจ้าของห้องพัก</summary>
    public Guid OrganizationId { get; init; }

    /// <summary>หอพักที่ห้องนี้สังกัด</summary>
    public Guid DormitoryId { get; init; }

    /// <summary>อาคารที่ห้องนี้สังกัด</summary>
    public Guid BuildingId { get; init; }

    /// <summary>ชั้นที่ห้องนี้สังกัด</summary>
    public Guid FloorId { get; init; }

    /// <summary>ประเภทของห้อง</summary>
    public Guid RoomTypeId { get; init; }

    /// <summary>เลขห้อง</summary>
    public string RoomNumber { get; init; } = string.Empty;

    /// <summary>ค่าเช่ารายเดือนของห้องนี้</summary>
    public decimal MonthlyRent { get; init; }

    /// <summary>ค่ามัดจำของห้องนี้</summary>
    public decimal DepositAmount { get; init; }

    /// <summary>จำนวนผู้เข้าพักสูงสุดของห้องนี้</summary>
    public int MaximumOccupants { get; init; }

    /// <summary>สถานะของห้อง</summary>
    public string Status { get; init; } = string.Empty;

    /// <summary>คำอธิบายเพิ่มเติม</summary>
    public string? Description { get; init; }

    /// <summary>เวลาที่สร้างข้อมูล</summary>
    public DateTimeOffset CreatedAt { get; init; }

    /// <summary>เวลาที่แก้ไขข้อมูลล่าสุด</summary>
    public DateTimeOffset? UpdatedAt { get; init; }
}
