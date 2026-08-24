using DormFlow.Contracts.Common;
using DormFlow.Contracts.RoomTypes;

namespace DormFlow.Application.Features.RoomTypes;

/// <summary>Use Case ทั้งหมดของโมดูลประเภทห้อง</summary>
public interface IRoomTypeService
{
    /// <summary>ดึงรายการประเภทห้องแบบแบ่งหน้า</summary>
    Task<PagedResult<RoomTypeResponse>> GetPagedAsync(
        RoomTypeListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>อ่านข้อมูลประเภทห้องจากรหัสอ้างอิง</summary>
    Task<RoomTypeResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>สร้างประเภทห้องใหม่</summary>
    Task<RoomTypeResponse> CreateAsync(
        CreateRoomTypeRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>แก้ไขข้อมูลประเภทห้อง</summary>
    Task<RoomTypeResponse> UpdateAsync(
        Guid id,
        UpdateRoomTypeRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>เปลี่ยนสถานะประเภทห้อง</summary>
    Task<RoomTypeResponse> ChangeStatusAsync(
        Guid id,
        string status,
        CancellationToken cancellationToken = default);

    /// <summary>ลบประเภทห้อง</summary>
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
