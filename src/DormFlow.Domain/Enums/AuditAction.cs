namespace DormFlow.Domain.Enums;

/// <summary>ประเภทของเหตุการณ์ที่บันทึกลง Audit Log</summary>
public enum AuditAction
{
    /// <summary>สร้างข้อมูลใหม่</summary>
    Create = 1,

    /// <summary>แก้ไขข้อมูล</summary>
    Update = 2,

    /// <summary>ลบข้อมูล</summary>
    Delete = 3,

    /// <summary>เปลี่ยนสถานะข้อมูล</summary>
    ChangeStatus = 4,

    /// <summary>กำหนดบทบาทให้ผู้ใช้</summary>
    AssignRole = 5,

    /// <summary>มอบหมายหอพักให้ผู้ใช้</summary>
    AssignDormitory = 6,

    /// <summary>เข้าสู่ระบบสำเร็จ</summary>
    Login = 7,

    /// <summary>เข้าสู่ระบบไม่สำเร็จ</summary>
    LoginFailed = 8,

    /// <summary>ออกจากระบบ</summary>
    Logout = 9,

    /// <summary>ต่ออายุ Access Token ด้วย Refresh Token</summary>
    RefreshToken = 10,

    /// <summary>เพิกถอน Refresh Token</summary>
    RevokeToken = 11,

    /// <summary>ตรวจพบการนำ Refresh Token ที่ใช้ไปแล้วกลับมาใช้ซ้ำ</summary>
    RefreshTokenReuseDetected = 12,

    /// <summary>เปลี่ยนรหัสผ่านด้วยตนเอง</summary>
    ChangePassword = 13,

    /// <summary>ขอลิงก์ตั้งรหัสผ่านใหม่</summary>
    ForgotPassword = 14,

    /// <summary>ตั้งรหัสผ่านใหม่ด้วยโทเคนที่ได้รับ</summary>
    ResetPassword = 15,
}
