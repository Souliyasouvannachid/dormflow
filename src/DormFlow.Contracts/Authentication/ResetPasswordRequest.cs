namespace DormFlow.Contracts.Authentication;

/// <summary>ข้อมูลสำหรับตั้งรหัสผ่านใหม่</summary>
public class ResetPasswordRequest
{
    /// <summary>อีเมลของบัญชี</summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>โทเคนที่ได้รับจากอีเมลตั้งรหัสผ่านใหม่</summary>
    public string Token { get; set; } = string.Empty;

    /// <summary>รหัสผ่านใหม่</summary>
    public string NewPassword { get; set; } = string.Empty;

    /// <summary>ยืนยันรหัสผ่านใหม่</summary>
    public string ConfirmPassword { get; set; } = string.Empty;
}
