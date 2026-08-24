using DormFlow.Domain.Enums;

namespace DormFlow.Application.Common.Models;

/// <summary>
/// ข้อมูลของผู้ใช้เท่าที่จำเป็นต่อการออกโทเคนและตอบกลับข้อมูลผู้ใช้ปัจจุบัน
/// </summary>
/// <remarks>
/// แยกจาก Entity ของ Identity เพื่อไม่ให้ชั้น Application ต้องรู้จัก ASP.NET Core Identity
/// และเพื่อไม่ให้ค่าอย่างแฮชรหัสผ่านหลุดออกมานอกชั้น Infrastructure
/// </remarks>
public sealed record UserSecurityProfile
{
    /// <summary>รหัสผู้ใช้</summary>
    public required Guid UserId { get; init; }

    /// <summary>อีเมล</summary>
    public required string Email { get; init; }

    /// <summary>ชื่อ</summary>
    public required string FirstName { get; init; }

    /// <summary>นามสกุล</summary>
    public required string LastName { get; init; }

    /// <summary>เบอร์โทรศัพท์</summary>
    public string? Phone { get; init; }

    /// <summary>ที่อยู่ของรูปโปรไฟล์</summary>
    public string? ProfileImage { get; init; }

    /// <summary>องค์กรที่สังกัด เป็น null สำหรับผู้ดูแลระบบที่ไม่ผูกกับองค์กรใด</summary>
    public Guid? OrganizationId { get; init; }

    /// <summary>สถานะของบัญชี</summary>
    public required UserStatus Status { get; init; }

    /// <summary>เวลาที่เข้าสู่ระบบครั้งล่าสุด</summary>
    public DateTimeOffset? LastLoginAt { get; init; }

    /// <summary>บทบาทของผู้ใช้</summary>
    public IReadOnlyList<string> Roles { get; init; } = [];

    /// <summary>รหัสสิทธิ์ทั้งหมดที่ผู้ใช้ได้รับจากทุกบทบาทรวมกัน</summary>
    public IReadOnlyList<string> Permissions { get; init; } = [];

    /// <summary>หอพักที่ผู้ใช้ได้รับมอบหมาย</summary>
    public IReadOnlyList<Guid> DormitoryIds { get; init; } = [];

    /// <summary>รายละเอียดการมอบหมายหอพักของผู้ใช้ ใช้แสดงผลในหน้าจอดูแลผู้ใช้</summary>
    public IReadOnlyList<UserDormitoryAssignmentInfo> DormitoryAssignments { get; init; } = [];

    /// <summary>เวลาที่สร้างบัญชี</summary>
    public DateTimeOffset CreatedAt { get; init; }
}

/// <summary>รายละเอียดการมอบหมายหอพักหนึ่งแห่งให้ผู้ใช้</summary>
/// <param name="DormitoryId">หอพักที่ได้รับมอบหมาย</param>
/// <param name="IsPrimary">เป็นหอพักหลักของผู้ใช้รายนี้หรือไม่</param>
/// <param name="AssignedAt">เวลาที่มอบหมาย</param>
public sealed record UserDormitoryAssignmentInfo(Guid DormitoryId, bool IsPrimary, DateTimeOffset AssignedAt);
