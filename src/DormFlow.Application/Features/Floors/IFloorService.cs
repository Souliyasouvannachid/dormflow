using DormFlow.Contracts.Common;
using DormFlow.Contracts.Floors;

namespace DormFlow.Application.Features.Floors;

/// <summary>Use Case ทั้งหมดของโมดูลชั้นอาคาร</summary>
public interface IFloorService
{
    /// <summary>ดึงรายการชั้นแบบแบ่งหน้า</summary>
    Task<PagedResult<FloorResponse>> GetPagedAsync(
        FloorListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>อ่านข้อมูลชั้นจากรหัสอ้างอิง</summary>
    Task<FloorResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>สร้างชั้นใหม่</summary>
    Task<FloorResponse> CreateAsync(CreateFloorRequest request, CancellationToken cancellationToken = default);

    /// <summary>แก้ไขข้อมูลชั้น</summary>
    Task<FloorResponse> UpdateAsync(
        Guid id,
        UpdateFloorRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>เปลี่ยนสถานะชั้น</summary>
    Task<FloorResponse> ChangeStatusAsync(Guid id, string status, CancellationToken cancellationToken = default);

    /// <summary>ลบชั้น</summary>
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
