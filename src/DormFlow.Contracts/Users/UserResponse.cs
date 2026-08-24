namespace DormFlow.Contracts.Users;

/// <summary>ข้อมูลผู้ใช้งานหนึ่งบัญชี</summary>
public class UserResponse
{
    /// <summary>รหัสผู้ใช้</summary>
    public Guid Id { get; init; }

    /// <summary>อีเมล</summary>
    public string Email { get; init; } = string.Empty;

    /// <summary>ชื่อ</summary>
    public string FirstName { get; init; } = string.Empty;

    /// <summary>นามสกุล</summary>
    public string LastName { get; init; } = string.Empty;

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

    /// <summary>หอพักที่ผู้ใช้ได้รับมอบหมาย</summary>
    public IReadOnlyList<UserDormitoryAssignmentResponse> Dormitories { get; init; } = [];

    /// <summary>เวลาที่สร้างบัญชี</summary>
    public DateTimeOffset CreatedAt { get; init; }
}

/// <summary>การมอบหมายหอพักหนึ่งแห่งให้ผู้ใช้</summary>
public class UserDormitoryAssignmentResponse
{
    /// <summary>หอพักที่ได้รับมอบหมาย</summary>
    public Guid DormitoryId { get; init; }

    /// <summary>เป็นหอพักหลักของผู้ใช้รายนี้หรือไม่</summary>
    public bool IsPrimary { get; init; }

    /// <summary>เวลาที่มอบหมาย</summary>
    public DateTimeOffset AssignedAt { get; init; }
}
