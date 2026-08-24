namespace DormFlow.Contracts.Tenants;

/// <summary>คำขอสร้างข้อมูลผู้เช่าใหม่</summary>
public class CreateTenantRequest
{
    /// <summary>
    /// องค์กรที่ผู้เช่าจะสังกัด มีผลเฉพาะเมื่อผู้เรียกเป็น SuperAdmin
    /// ผู้ใช้อื่นถูกบังคับให้สร้างในองค์กรของตนเองเสมอไม่ว่าจะระบุค่านี้หรือไม่
    /// </summary>
    public Guid? OrganizationId { get; set; }

    /// <summary>ชื่อ</summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>นามสกุล</summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>อีเมล</summary>
    public string? Email { get; set; }

    /// <summary>เบอร์โทรศัพท์</summary>
    public string? Phone { get; set; }

    /// <summary>เลขบัตรประจำตัวประชาชนหรือหนังสือเดินทาง</summary>
    public string? NationalId { get; set; }

    /// <summary>วันเดือนปีเกิด</summary>
    public DateOnly? DateOfBirth { get; set; }

    /// <summary>ที่อยู่ตามทะเบียนบ้าน</summary>
    public string? Address { get; set; }

    /// <summary>ชื่อผู้ติดต่อฉุกเฉิน</summary>
    public string? EmergencyContactName { get; set; }

    /// <summary>เบอร์โทรศัพท์ผู้ติดต่อฉุกเฉิน</summary>
    public string? EmergencyContactPhone { get; set; }
}
