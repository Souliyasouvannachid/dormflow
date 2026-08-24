namespace DormFlow.Contracts.Rooms;

/// <summary>คำขอสร้างห้องพักใหม่</summary>
public class CreateRoomRequest
{
    /// <summary>อาคารที่ห้องนี้จะสังกัด</summary>
    public Guid BuildingId { get; set; }

    /// <summary>ชั้นที่ห้องนี้จะสังกัด ต้องอยู่ในอาคารเดียวกับ BuildingId</summary>
    public Guid FloorId { get; set; }

    /// <summary>ประเภทของห้อง ต้องอยู่ในหอพักเดียวกับอาคารที่เลือก</summary>
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
