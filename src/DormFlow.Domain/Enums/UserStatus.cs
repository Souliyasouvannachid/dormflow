namespace DormFlow.Domain.Enums;

/// <summary>สถานะของบัญชีผู้ใช้</summary>
public enum UserStatus
{
    /// <summary>ใช้งานได้ตามปกติ</summary>
    Active = 1,

    /// <summary>ปิดใช้งาน เข้าสู่ระบบไม่ได้</summary>
    Inactive = 2,

    /// <summary>ถูกระงับ เข้าสู่ระบบไม่ได้จนกว่าผู้ดูแลจะปลดระงับ</summary>
    Suspended = 3,
}
