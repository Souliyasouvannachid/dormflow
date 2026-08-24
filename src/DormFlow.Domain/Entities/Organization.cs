using DormFlow.Domain.Common;
using DormFlow.Domain.Enums;

namespace DormFlow.Domain.Entities;

/// <summary>
/// องค์กรที่ใช้งานระบบ เป็นรากของการแยกข้อมูลแบบหลายผู้เช่า (Tenant Root)
/// </summary>
/// <remarks>
/// สืบทอดจาก <see cref="BaseEntity"/> ไม่ใช่ <see cref="TenantEntity"/>
/// เพราะองค์กรคือตัวขอบเขตเอง จึงไม่มีองค์กรแม่ให้อ้างอิง
/// </remarks>
public class Organization : BaseEntity
{
    /// <summary>รหัสองค์กร ไม่ซ้ำกันทั้งระบบ</summary>
    public required string Code { get; set; }

    /// <summary>ชื่อองค์กร</summary>
    public required string Name { get; set; }

    /// <summary>เลขประจำตัวผู้เสียภาษี</summary>
    public string? TaxId { get; set; }

    /// <summary>เบอร์โทรศัพท์ติดต่อ</summary>
    public string? Phone { get; set; }

    /// <summary>อีเมลติดต่อ</summary>
    public string? Email { get; set; }

    /// <summary>ที่อยู่</summary>
    public string? Address { get; set; }

    /// <summary>สถานะขององค์กร</summary>
    public OrganizationStatus Status { get; set; } = OrganizationStatus.Active;

    /// <summary>องค์กรอยู่ในสถานะที่ผู้ใช้เข้าสู่ระบบได้หรือไม่</summary>
    public bool CanUsersSignIn => Status == OrganizationStatus.Active;
}
