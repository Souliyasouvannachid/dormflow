namespace DormFlow.Contracts.RoomTypes;

/// <summary>คำขอสร้างประเภทห้องใหม่</summary>
public class CreateRoomTypeRequest
{
    /// <summary>หอพักที่ประเภทห้องนี้จะสังกัด</summary>
    public Guid DormitoryId { get; set; }

    /// <summary>รหัสประเภทห้อง ไม่ซ้ำกันภายในหอพักเดียวกัน</summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>ชื่อประเภทห้อง</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>คำอธิบายเพิ่มเติม</summary>
    public string? Description { get; set; }

    /// <summary>ค่าเช่ารายเดือน</summary>
    public decimal MonthlyRent { get; set; }

    /// <summary>ค่าเช่ารายวัน เว้นว่างเมื่อไม่รองรับการเช่ารายวัน</summary>
    public decimal? DailyRent { get; set; }

    /// <summary>ค่ามัดจำ</summary>
    public decimal DepositAmount { get; set; }

    /// <summary>จำนวนผู้เข้าพักสูงสุด</summary>
    public int MaximumOccupants { get; set; } = 1;
}
