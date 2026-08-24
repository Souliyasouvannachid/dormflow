namespace DormFlow.Contracts.Rooms;

/// <summary>คำขอแก้ไขข้อมูลห้องพัก</summary>
/// <remarks>ไม่มีฟิลด์ BuildingId เพราะการย้ายห้องข้ามอาคารไม่ใช่การแก้ไขข้อมูลปกติ</remarks>
public class UpdateRoomRequest
{
    /// <summary>ชั้นที่ห้องนี้จะสังกัด ต้องอยู่ในอาคารเดียวกับห้องนี้</summary>
    public Guid FloorId { get; set; }

    /// <summary>ประเภทของห้อง ต้องอยู่ในหอพักเดียวกับห้องนี้</summary>
    public Guid RoomTypeId { get; set; }

    /// <summary>เลขห้อง ไม่ซ้ำกันภายในอาคารเดียวกัน</summary>
    public string RoomNumber { get; set; } = string.Empty;

    /// <summary>ค่าเช่ารายเดือนของห้องนี้</summary>
    public decimal MonthlyRent { get; set; }

    /// <summary>ค่ามัดจำของห้องนี้</summary>
    public decimal DepositAmount { get; set; }

    /// <summary>จำนวนผู้เข้าพักสูงสุดของห้องนี้</summary>
    public int MaximumOccupants { get; set; } = 1;

    /// <summary>คำอธิบายเพิ่มเติม</summary>
    public string? Description { get; set; }
}
