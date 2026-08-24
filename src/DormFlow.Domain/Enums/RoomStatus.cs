namespace DormFlow.Domain.Enums;

/// <summary>สถานะของห้องพัก</summary>
public enum RoomStatus
{
    /// <summary>ว่าง พร้อมให้เช่า</summary>
    Available = 1,

    /// <summary>ถูกจองไว้แล้ว ยังไม่เข้าพัก</summary>
    Reserved = 2,

    /// <summary>มีผู้เช่าอยู่</summary>
    Occupied = 3,

    /// <summary>อยู่ระหว่างทำความสะอาดก่อนให้เช่ารายต่อไป</summary>
    Cleaning = 4,

    /// <summary>อยู่ระหว่างซ่อมบำรุง ให้เช่าไม่ได้ชั่วคราว</summary>
    Maintenance = 5,

    /// <summary>ปิดใช้งาน ไม่นำออกให้เช่า</summary>
    Inactive = 6,
}
