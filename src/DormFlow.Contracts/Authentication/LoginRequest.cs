namespace DormFlow.Contracts.Authentication;

/// <summary>ข้อมูลสำหรับเข้าสู่ระบบ</summary>
public class LoginRequest
{
    /// <summary>อีเมลที่ใช้เข้าสู่ระบบ</summary>
    /// <example>admin@dormflow.local</example>
    public string Email { get; set; } = string.Empty;

    /// <summary>รหัสผ่าน</summary>
    public string Password { get; set; } = string.Empty;
}
