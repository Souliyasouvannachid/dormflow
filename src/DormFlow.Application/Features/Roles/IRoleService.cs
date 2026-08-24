using DormFlow.Contracts.Common;
using DormFlow.Contracts.Roles;

namespace DormFlow.Application.Features.Roles;

/// <summary>Use Case ทั้งหมดของโมดูลบทบาท</summary>
public interface IRoleService
{
    /// <summary>ดึงรายการบทบาทแบบแบ่งหน้า</summary>
    Task<PagedResult<RoleResponse>> GetPagedAsync(
        RoleListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>อ่านข้อมูลบทบาทจากรหัสอ้างอิง</summary>
    Task<RoleResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>สร้างบทบาทใหม่</summary>
    Task<RoleResponse> CreateAsync(CreateRoleRequest request, CancellationToken cancellationToken = default);

    /// <summary>แก้ไขข้อมูลบทบาท</summary>
    Task<RoleResponse> UpdateAsync(
        Guid id,
        UpdateRoleRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>ลบบทบาท</summary>
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>กำหนดสิทธิ์ให้บทบาท แทนที่สิทธิ์เดิมทั้งหมด</summary>
    Task<RoleResponse> AssignPermissionsAsync(
        Guid id,
        AssignRolePermissionsRequest request,
        CancellationToken cancellationToken = default);
}
