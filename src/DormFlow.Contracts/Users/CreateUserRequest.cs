namespace DormFlow.Contracts.Users;

/// <summary>คำขอสร้างบัญชีผู้ใช้ใหม่</summary>
public class CreateUserRequest
{
    /// <summary>
    /// องค์กรที่ผู้ใช้จะสังกัด มีผลเฉพาะเมื่อผู้เรียกเป็น SuperAdmin เท่านั้น
    /// ผู้ใช้อื่นถูกบังคับให้สร้างบัญชีในองค์กรของตนเองเสมอไม่ว่าจะระบุค่านี้หรือไม่
    /// </summary>
    public Guid? OrganizationId { get; set; }

    /// <summary>อีเมลที่ใช้เข้าสู่ระบบ ไม่ซ้ำกันทั้งระบบ</summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>รหัสผ่านเริ่มต้น</summary>
    public string Password { get; set; } = string.Empty;

    /// <summary>ชื่อ</summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>นามสกุล</summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>เบอร์โทรศัพท์</summary>
    public string? Phone { get; set; }
}
