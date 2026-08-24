namespace DormFlow.Contracts.Rooms;

/// <summary>ผลสรุปจำนวนห้องแยกตามสถานะ</summary>
public class RoomSummaryResponse
{
    /// <summary>จำนวนห้องทั้งหมดที่ตรงเงื่อนไข</summary>
    public int TotalRooms { get; init; }

    /// <summary>จำนวนห้องที่ว่าง พร้อมให้เช่า</summary>
    public int Available { get; init; }

    /// <summary>จำนวนห้องที่ถูกจองไว้แล้ว</summary>
    public int Reserved { get; init; }

    /// <summary>จำนวนห้องที่มีผู้เช่าอยู่</summary>
    public int Occupied { get; init; }

    /// <summary>จำนวนห้องที่อยู่ระหว่างทำความสะอาด</summary>
    public int Cleaning { get; init; }

    /// <summary>จำนวนห้องที่อยู่ระหว่างซ่อมบำรุง</summary>
    public int Maintenance { get; init; }

    /// <summary>จำนวนห้องที่ปิดใช้งาน</summary>
    public int Inactive { get; init; }
}
