using DormFlow.Application.Common.Pagination;
using DormFlow.Contracts.Common;
using DormFlow.Domain.Entities;
using DormFlow.Domain.Enums;

namespace DormFlow.Application.Features.Rooms;

/// <summary>ตัวกรองสำหรับดึงรายการห้องพักแบบแบ่งหน้า</summary>
public sealed record RoomListFilter(
    Guid? OrganizationId,
    Guid? DormitoryId,
    Guid? BuildingId,
    Guid? FloorId,
    Guid? RoomTypeId,
    RoomStatus? Status,
    IReadOnlyCollection<Guid>? RestrictToDormitoryIds);

/// <summary>ตัวกรองสำหรับสรุปจำนวนห้องแยกตามสถานะ</summary>
public sealed record RoomSummaryFilter(
    Guid? OrganizationId,
    Guid? DormitoryId,
    Guid? BuildingId,
    IReadOnlyCollection<Guid>? RestrictToDormitoryIds);

/// <summary>เข้าถึงข้อมูลห้องพักในฐานข้อมูล</summary>
public interface IRoomRepository
{
    /// <summary>อ่านห้องพักจากรหัสอ้างอิง</summary>
    Task<Room?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>ตรวจว่ามีห้องที่ใช้เลขนี้อยู่แล้วในอาคารเดียวกันหรือไม่</summary>
    Task<bool> ExistsByRoomNumberAsync(
        Guid buildingId,
        string roomNumber,
        Guid? excludeRoomId,
        CancellationToken cancellationToken = default);

    /// <summary>ดึงรายการห้องพักแบบแบ่งหน้าตามตัวกรองที่ระบุ</summary>
    Task<PagedResult<Room>> GetPagedAsync(
        RoomListFilter filter,
        PagedQuery query,
        CancellationToken cancellationToken = default);

    /// <summary>นับจำนวนห้องแยกตามสถานะตามตัวกรองที่ระบุ</summary>
    Task<IReadOnlyDictionary<RoomStatus, int>> GetStatusCountsAsync(
        RoomSummaryFilter filter,
        CancellationToken cancellationToken = default);

    /// <summary>เพิ่มห้องพักใหม่</summary>
    Task AddAsync(Room room, CancellationToken cancellationToken = default);

    /// <summary>ทำเครื่องหมายลบห้องพัก ระบบจะแปลงเป็น Soft Delete ให้อัตโนมัติตอนบันทึก</summary>
    void Remove(Room room);

    /// <summary>บันทึกการเปลี่ยนแปลงทั้งหมดลงฐานข้อมูล</summary>
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
