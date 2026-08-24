namespace DormFlow.Domain.Enums;

/// <summary>สถานะของพัสดุ</summary>
public enum ParcelStatus
{
    /// <summary>รับพัสดุไว้ที่หอพักแล้ว รอผู้เช่ามารับ</summary>
    Received = 1,

    /// <summary>ผู้เช่ารับพัสดุไปแล้ว</summary>
    PickedUp = 2,
}
