using DormFlow.Contracts.Buildings;
using DormFlow.Contracts.Common;

namespace DormFlow.Application.Features.Buildings;

/// <summary>Use Case ทั้งหมดของโมดูลอาคาร</summary>
public interface IBuildingService
{
    /// <summary>ดึงรายการอาคารแบบแบ่งหน้า</summary>
    Task<PagedResult<BuildingResponse>> GetPagedAsync(
        BuildingListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>อ่านข้อมูลอาคารจากรหัสอ้างอิง</summary>
    Task<BuildingResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>สร้างอาคารใหม่</summary>
    Task<BuildingResponse> CreateAsync(CreateBuildingRequest request, CancellationToken cancellationToken = default);

    /// <summary>แก้ไขข้อมูลอาคาร</summary>
    Task<BuildingResponse> UpdateAsync(
        Guid id,
        UpdateBuildingRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>เปลี่ยนสถานะอาคาร</summary>
    Task<BuildingResponse> ChangeStatusAsync(
        Guid id,
        string status,
        CancellationToken cancellationToken = default);

    /// <summary>ลบอาคาร</summary>
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
