namespace DormFlow.Domain.Enums;

/// <summary>สถานะขององค์กรที่ใช้งานระบบ</summary>
public enum OrganizationStatus
{
    /// <summary>ใช้งานได้ตามปกติ</summary>
    Active = 1,

    /// <summary>ปิดใช้งานชั่วคราวโดยองค์กรเอง</summary>
    Inactive = 2,

    /// <summary>ถูกระงับโดยผู้ดูแลระบบ ผู้ใช้ในองค์กรจะเข้าสู่ระบบไม่ได้</summary>
    Suspended = 3,
}
