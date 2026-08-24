namespace DormFlow.Domain.Entities;

/// <summary>
/// ความสัมพันธ์ระหว่างบทบาทกับสิทธิ์
/// </summary>
/// <remarks>
/// ใช้ Composite Key ของ RoleId และ PermissionId จึงไม่ต้องมีคอลัมน์ Id
/// และไม่ใช้ Soft Delete เพราะการถอนสิทธิ์ควรลบแถวออกจริง
/// </remarks>
public class RolePermission
{
    /// <summary>บทบาทที่ได้รับสิทธิ์</summary>
    public Guid RoleId { get; set; }

    /// <summary>สิทธิ์ที่ถูกมอบให้</summary>
    public Guid PermissionId { get; set; }

    /// <summary>เวลาที่มอบสิทธิ์</summary>
    public DateTimeOffset GrantedAt { get; set; }

    /// <summary>ผู้มอบสิทธิ์ เป็น null เมื่อมอบโดยระบบตอน Seed</summary>
    public Guid? GrantedBy { get; set; }

    /// <summary>สิทธิ์ที่อ้างถึง</summary>
    public Permission? Permission { get; set; }
}
