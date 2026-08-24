using DormFlow.Domain.Common;

namespace DormFlow.Domain.Entities;

/// <summary>
/// สิทธิ์การใช้งานหนึ่งรายการ เช่น rooms.create
/// </summary>
/// <remarks>
/// เป็นข้อมูลอ้างอิงที่ระบบสร้างให้เองตอน Seed ไม่ได้ให้ผู้ใช้เพิ่มเอง
/// เพราะทุกรหัสสิทธิ์ต้องมีจุดที่ตรวจสอบอยู่ในโค้ดจริง
/// </remarks>
public class Permission : BaseEntity
{
    /// <summary>รหัสสิทธิ์ที่ใช้อ้างอิงในโค้ดและใน Claim ของ Access Token</summary>
    public required string Code { get; set; }

    /// <summary>ชื่อสิทธิ์สำหรับแสดงผล</summary>
    public required string Name { get; set; }

    /// <summary>คำอธิบายเพิ่มเติม</summary>
    public string? Description { get; set; }

    /// <summary>โมดูลที่สิทธิ์นี้สังกัด ใช้จัดกลุ่มในหน้าจอกำหนดสิทธิ์</summary>
    public required string Module { get; set; }

    /// <summary>บทบาทที่ได้รับสิทธิ์นี้</summary>
    public ICollection<RolePermission> RolePermissions { get; } = [];
}
