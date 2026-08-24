namespace DormFlow.Contracts.Authentication;

/// <summary>ข้อมูลสำหรับขอลิงก์ตั้งรหัสผ่านใหม่</summary>
public class ForgotPasswordRequest
{
    /// <summary>อีเมลของบัญชีที่ลืมรหัสผ่าน</summary>
    public string Email { get; set; } = string.Empty;
}
