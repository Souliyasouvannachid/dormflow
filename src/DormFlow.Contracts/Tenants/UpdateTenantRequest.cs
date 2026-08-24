namespace DormFlow.Contracts.Tenants;

/// <summary>คำขอแก้ไขข้อมูลผู้เช่า</summary>
public class UpdateTenantRequest
{
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
