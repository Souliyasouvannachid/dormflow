namespace DormFlow.Domain.Enums;

/// <summary>สถานะของการจดทะเบียนที่จอดรถ</summary>
public enum ParkingRegistrationStatus
{
    /// <summary>จดทะเบียนอยู่ ใช้พื้นที่จอดรถอยู่</summary>
    Active = 1,

    /// <summary>สิ้นสุดการจดทะเบียนแล้ว</summary>
    Ended = 2,
}
