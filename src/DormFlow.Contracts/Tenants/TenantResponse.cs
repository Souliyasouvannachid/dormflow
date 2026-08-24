namespace DormFlow.Contracts.Tenants;

/// <summary>ข้อมูลผู้เช่าหนึ่งคน</summary>
public class TenantResponse
{
    /// <summary>รหัสอ้างอิงของผู้เช่า</summary>
    public Guid Id { get; init; }

    /// <summary>องค์กรที่ผู้เช่ารายนี้สังกัด</summary>
    public Guid OrganizationId { get; init; }

    /// <summary>ชื่อ</summary>
    public string FirstName { get; init; } = string.Empty;

    /// <summary>นามสกุล</summary>
    public string LastName { get; init; } = string.Empty;

    /// <summary>อีเมล</summary>
    public string? Email { get; init; }

    /// <summary>เบอร์โทรศัพท์</summary>
    public string? Phone { get; init; }

    /// <summary>เลขบัตรประจำตัวประชาชนหรือหนังสือเดินทาง</summary>
    public string? NationalId { get; init; }

    /// <summary>วันเดือนปีเกิด</summary>
    public DateOnly? DateOfBirth { get; init; }

    /// <summary>ที่อยู่ตามทะเบียนบ้าน</summary>
    public string? Address { get; init; }

    /// <summary>ชื่อผู้ติดต่อฉุกเฉิน</summary>
    public string? EmergencyContactName { get; init; }

    /// <summary>เบอร์โทรศัพท์ผู้ติดต่อฉุกเฉิน</summary>
    public string? EmergencyContactPhone { get; init; }

    /// <summary>สถานะของผู้เช่า</summary>
    public string Status { get; init; } = string.Empty;

    /// <summary>เวลาที่สร้างข้อมูล</summary>
    public DateTimeOffset CreatedAt { get; init; }

    /// <summary>เวลาที่แก้ไขข้อมูลล่าสุด</summary>
    public DateTimeOffset? UpdatedAt { get; init; }
}
