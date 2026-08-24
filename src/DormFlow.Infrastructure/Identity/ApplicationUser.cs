using DormFlow.Domain.Common;
using DormFlow.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace DormFlow.Infrastructure.Identity;

/// <summary>
/// บัญชีผู้ใช้ของระบบ ต่อยอดจาก <see cref="IdentityUser{TKey}"/> ของ ASP.NET Core Identity
/// </summary>
/// <remarks>
/// อยู่ในชั้น Infrastructure เพราะผูกกับ ASP.NET Core Identity โดยตรง
/// ชั้น Application มองเห็นผู้ใช้ผ่าน UserSecurityProfile เท่านั้น จึงไม่มีทางแตะแฮชรหัสผ่านได้
/// การประกาศ <see cref="IAuditableEntity"/> และ <see cref="ISoftDeletable"/>
/// ทำให้ผู้ใช้ได้รับการเติมข้อมูล Audit และ Global Query Filter เช่นเดียวกับ Entity อื่นทั้งหมด
/// </remarks>
public class ApplicationUser : IdentityUser<Guid>, IAuditableEntity, ISoftDeletable
{
    /// <summary>ชื่อ</summary>
    public required string FirstName { get; set; }

    /// <summary>นามสกุล</summary>
    public required string LastName { get; set; }

    /// <summary>ที่อยู่ของรูปโปรไฟล์</summary>
    public string? ProfileImage { get; set; }

    /// <summary>องค์กรที่สังกัด เป็น null สำหรับผู้ดูแลระบบที่ดูแลข้ามองค์กร</summary>
    public Guid? OrganizationId { get; set; }

    /// <summary>สถานะของบัญชี</summary>
    public UserStatus Status { get; set; } = UserStatus.Active;

    /// <summary>เวลาที่เข้าสู่ระบบสำเร็จครั้งล่าสุด</summary>
    public DateTimeOffset? LastLoginAt { get; set; }

    /// <inheritdoc />
    public DateTimeOffset CreatedAt { get; set; }

    /// <inheritdoc />
    public Guid? CreatedBy { get; set; }

    /// <inheritdoc />
    public DateTimeOffset? UpdatedAt { get; set; }

    /// <inheritdoc />
    public Guid? UpdatedBy { get; set; }

    /// <inheritdoc />
    public bool IsDeleted { get; set; }

    /// <inheritdoc />
    public DateTimeOffset? DeletedAt { get; set; }

    /// <inheritdoc />
    public Guid? DeletedBy { get; set; }

    /// <summary>ชื่อเต็มสำหรับแสดงผล</summary>
    public string FullName => $"{FirstName} {LastName}".Trim();

    /// <summary>บัญชีอยู่ในสถานะที่เข้าสู่ระบบได้หรือไม่</summary>
    public bool CanSignIn => !IsDeleted && Status == UserStatus.Active;
}
