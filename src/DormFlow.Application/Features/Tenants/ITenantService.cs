using DormFlow.Contracts.Common;
using DormFlow.Contracts.Tenants;

namespace DormFlow.Application.Features.Tenants;

/// <summary>Use Case ทั้งหมดของโมดูลผู้เช่า</summary>
public interface ITenantService
{
    /// <summary>ดึงรายการผู้เช่าแบบแบ่งหน้า</summary>
    Task<PagedResult<TenantResponse>> GetPagedAsync(
        TenantListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>อ่านข้อมูลผู้เช่าจากรหัสอ้างอิง</summary>
    Task<TenantResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>สร้างข้อมูลผู้เช่าใหม่</summary>
    Task<TenantResponse> CreateAsync(CreateTenantRequest request, CancellationToken cancellationToken = default);

    /// <summary>แก้ไขข้อมูลผู้เช่า</summary>
    Task<TenantResponse> UpdateAsync(
        Guid id,
        UpdateTenantRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>เปลี่ยนสถานะผู้เช่า</summary>
    Task<TenantResponse> ChangeStatusAsync(Guid id, string status, CancellationToken cancellationToken = default);
}
