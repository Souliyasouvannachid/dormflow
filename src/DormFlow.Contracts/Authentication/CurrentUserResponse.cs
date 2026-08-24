namespace DormFlow.Contracts.Authentication;

/// <summary>ข้อมูลผู้ใช้ที่เข้าสู่ระบบอยู่</summary>
public class CurrentUserResponse
{
    /// <summary>รหัสผู้ใช้</summary>
    public Guid Id { get; init; }

    /// <summary>อีเมล</summary>
    public string Email { get; init; } = string.Empty;

    /// <summary>ชื่อ</summary>
    public string FirstName { get; init; } = string.Empty;

    /// <summary>นามสกุล</summary>
    public string LastName { get; init; } = string.Empty;

    /// <summary>ชื่อเต็มสำหรับแสดงผล</summary>
    public string FullName => $"{FirstName} {LastName}".Trim();

    /// <summary>เบอร์โทรศัพท์</summary>
    public string? Phone { get; init; }

    /// <summary>ที่อยู่ของรูปโปรไฟล์</summary>
    public string? ProfileImage { get; init; }

    /// <summary>องค์กรที่สังกัด เป็น null สำหรับผู้ดูแลระบบที่ไม่ผูกกับองค์กรใด</summary>
    public Guid? OrganizationId { get; init; }

    /// <summary>สถานะของบัญชี</summary>
    public string Status { get; init; } = string.Empty;

    /// <summary>เวลาที่เข้าสู่ระบบครั้งล่าสุด</summary>
    public DateTimeOffset? LastLoginAt { get; init; }

    /// <summary>บทบาทของผู้ใช้</summary>
    public IReadOnlyList<string> Roles { get; init; } = [];

    /// <summary>รหัสสิทธิ์ที่ผู้ใช้มี</summary>
    public IReadOnlyList<string> Permissions { get; init; } = [];

    /// <summary>หอพักที่ผู้ใช้ได้รับมอบหมาย</summary>
    public IReadOnlyList<Guid> DormitoryIds { get; init; } = [];
}
