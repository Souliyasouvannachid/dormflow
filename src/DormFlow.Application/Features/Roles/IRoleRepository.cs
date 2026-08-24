using DormFlow.Application.Common.Models;
using DormFlow.Application.Common.Pagination;
using DormFlow.Contracts.Common;

namespace DormFlow.Application.Features.Roles;

/// <summary>ผลลัพธ์ของการสร้างหรือเปลี่ยนชื่อบทบาท</summary>
/// <param name="Succeeded">สำเร็จหรือไม่</param>
/// <param name="RoleId">รหัสบทบาท มีค่าเฉพาะเมื่อสำเร็จ</param>
/// <param name="Errors">ข้อความอธิบายสาเหตุที่ไม่สำเร็จ</param>
public sealed record RoleOperationResult(bool Succeeded, Guid? RoleId, IReadOnlyList<string> Errors)
{
    /// <summary>สร้างผลลัพธ์ที่สำเร็จ</summary>
    public static RoleOperationResult Success(Guid roleId) => new(true, roleId, []);

    /// <summary>สร้างผลลัพธ์ที่ไม่สำเร็จ</summary>
    public static RoleOperationResult Failed(params string[] errors) => new(false, null, errors);
}

/// <summary>เข้าถึงและจัดการบทบาทบน ASP.NET Core Identity</summary>
public interface IRoleRepository
{
    /// <summary>ตรวจว่ามีบทบาทที่ใช้ชื่อนี้อยู่แล้วหรือไม่</summary>
    Task<bool> ExistsByNameAsync(string name, Guid? excludeRoleId, CancellationToken cancellationToken = default);

    /// <summary>อ่านข้อมูลบทบาทจากรหัสอ้างอิง</summary>
    Task<RoleInfo?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>ดึงรายการบทบาทแบบแบ่งหน้า</summary>
    Task<PagedResult<RoleInfo>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default);

    /// <summary>สร้างบทบาทใหม่</summary>
    Task<RoleOperationResult> CreateAsync(
        string name,
        string? description,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// แก้ไขชื่อและคำอธิบายของบทบาท คืนค่า false เมื่อไม่พบบทบาท
    /// ผู้เรียกต้องตรวจสอบเองก่อนแล้วว่าไม่ใช่การเปลี่ยนชื่อบทบาทมาตรฐานของระบบ
    /// </summary>
    Task<bool> UpdateAsync(
        Guid id,
        string name,
        string? description,
        CancellationToken cancellationToken = default);

    /// <summary>ลบบทบาท คืนค่า false เมื่อไม่พบบทบาท</summary>
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// กำหนดสิทธิ์ให้บทบาท แทนที่สิทธิ์เดิมทั้งหมดด้วยรายการที่ระบุ
    /// คืนค่า false เมื่อไม่พบบทบาท หรือพบรหัสสิทธิ์ที่ไม่มีอยู่จริงในระบบ
    /// </summary>
    Task<bool> SetPermissionsAsync(
        Guid roleId,
        IReadOnlyList<string> permissionCodes,
        Guid? grantedBy,
        DateTimeOffset grantedAt,
        CancellationToken cancellationToken = default);
}
