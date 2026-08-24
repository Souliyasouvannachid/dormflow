namespace DormFlow.Application.Common.Models;

/// <summary>สถานะผลการตรวจสอบชื่อผู้ใช้และรหัสผ่าน</summary>
public enum UserAuthenticationStatus
{
    /// <summary>ยืนยันตัวตนสำเร็จ</summary>
    Success = 1,

    /// <summary>อีเมลหรือรหัสผ่านไม่ถูกต้อง</summary>
    InvalidCredentials = 2,

    /// <summary>บัญชีถูกล็อกชั่วคราวเพราะกรอกรหัสผ่านผิดเกินจำนวนที่กำหนด</summary>
    LockedOut = 3,

    /// <summary>บัญชีถูกปิดใช้งานหรือถูกระงับ</summary>
    AccountDisabled = 4,

    /// <summary>องค์กรที่ผู้ใช้สังกัดถูกปิดใช้งานหรือถูกระงับ</summary>
    OrganizationDisabled = 5,

    /// <summary>ต้องยืนยันตัวตนขั้นที่สองก่อนจึงจะเข้าสู่ระบบได้</summary>
    RequiresTwoFactor = 6,
}

/// <summary>ผลการตรวจสอบชื่อผู้ใช้และรหัสผ่าน</summary>
/// <param name="Status">สถานะผลลัพธ์</param>
/// <param name="Profile">ข้อมูลผู้ใช้ มีค่าเฉพาะเมื่อยืนยันตัวตนสำเร็จ</param>
/// <param name="LockoutEndAt">เวลาที่การล็อกบัญชีจะสิ้นสุด มีค่าเฉพาะเมื่อบัญชีถูกล็อก</param>
public sealed record UserAuthenticationResult(
    UserAuthenticationStatus Status,
    UserSecurityProfile? Profile = null,
    DateTimeOffset? LockoutEndAt = null)
{
    /// <summary>สร้างผลลัพธ์ที่สำเร็จ</summary>
    /// <param name="profile">ข้อมูลผู้ใช้</param>
    public static UserAuthenticationResult Success(UserSecurityProfile profile)
        => new(UserAuthenticationStatus.Success, profile);

    /// <summary>สร้างผลลัพธ์ที่ไม่สำเร็จ</summary>
    /// <param name="status">สาเหตุที่ไม่สำเร็จ</param>
    /// <param name="lockoutEndAt">เวลาที่การล็อกบัญชีจะสิ้นสุด</param>
    public static UserAuthenticationResult Failed(
        UserAuthenticationStatus status,
        DateTimeOffset? lockoutEndAt = null)
        => new(status, null, lockoutEndAt);
}
