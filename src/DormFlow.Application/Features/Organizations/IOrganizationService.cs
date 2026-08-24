using DormFlow.Contracts.Common;
using DormFlow.Contracts.Organizations;

namespace DormFlow.Application.Features.Organizations;

/// <summary>Use Case ทั้งหมดของโมดูลองค์กร</summary>
public interface IOrganizationService
{
    /// <summary>ดึงรายการองค์กรแบบแบ่งหน้า</summary>
    Task<PagedResult<OrganizationResponse>> GetPagedAsync(
        OrganizationListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>อ่านข้อมูลองค์กรจากรหัสอ้างอิง</summary>
    Task<OrganizationResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>สร้างองค์กรใหม่</summary>
    Task<OrganizationResponse> CreateAsync(
        CreateOrganizationRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>แก้ไขข้อมูลองค์กร</summary>
    Task<OrganizationResponse> UpdateAsync(
        Guid id,
        UpdateOrganizationRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>เปลี่ยนสถานะองค์กร</summary>
    Task<OrganizationResponse> ChangeStatusAsync(
        Guid id,
        string status,
        CancellationToken cancellationToken = default);

    /// <summary>ลบองค์กร</summary>
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
