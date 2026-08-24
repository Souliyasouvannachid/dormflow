namespace DormFlow.Contracts.Authentication;

/// <summary>ข้อมูลสำหรับเปลี่ยนรหัสผ่านของบัญชีที่เข้าสู่ระบบอยู่</summary>
public class ChangePasswordRequest
{
    /// <summary>รหัสผ่านปัจจุบัน</summary>
    public string CurrentPassword { get; set; } = string.Empty;

    /// <summary>รหัสผ่านใหม่</summary>
    public string NewPassword { get; set; } = string.Empty;

    /// <summary>ยืนยันรหัสผ่านใหม่</summary>
    public string ConfirmPassword { get; set; } = string.Empty;
}
