namespace DormFlow.Domain.Enums;

/// <summary>สถานะของการจอง</summary>
public enum ReservationStatus
{
    /// <summary>รอดำเนินการทำสัญญา ห้องยังถูกกันไว้อยู่</summary>
    Pending = 1,

    /// <summary>ถูกแปลงเป็นสัญญาเช่าแล้ว</summary>
    ConvertedToContract = 2,

    /// <summary>ถูกยกเลิก</summary>
    Cancelled = 3,

    /// <summary>หมดอายุการกันห้องโดยไม่มีการทำสัญญา</summary>
    Expired = 4,
}
