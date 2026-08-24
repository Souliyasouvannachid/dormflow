using DormFlow.Contracts.Common;
using DormFlow.Contracts.Rooms;

namespace DormFlow.Application.Features.Rooms;

/// <summary>Use Case ทั้งหมดของโมดูลห้องพัก</summary>
public interface IRoomService
{
    /// <summary>ดึงรายการห้องพักแบบแบ่งหน้า</summary>
    Task<PagedResult<RoomResponse>> GetPagedAsync(
        RoomListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>ดึงรายการห้องว่างที่พร้อมให้เช่าแบบแบ่งหน้า</summary>
    Task<PagedResult<RoomResponse>> GetAvailableAsync(
        RoomListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>สรุปจำนวนห้องแยกตามสถานะ</summary>
    Task<RoomSummaryResponse> GetSummaryAsync(
        RoomSummaryRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>อ่านข้อมูลห้องพักจากรหัสอ้างอิง</summary>
    Task<RoomResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>สร้างห้องพักใหม่</summary>
    Task<RoomResponse> CreateAsync(CreateRoomRequest request, CancellationToken cancellationToken = default);

    /// <summary>แก้ไขข้อมูลห้องพัก</summary>
    Task<RoomResponse> UpdateAsync(
        Guid id,
        UpdateRoomRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>เปลี่ยนสถานะห้องพัก</summary>
    Task<RoomResponse> ChangeStatusAsync(Guid id, string status, CancellationToken cancellationToken = default);

    /// <summary>ลบห้องพัก</summary>
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
