using DormFlow.Application.Common.Pagination;
using DormFlow.Contracts.Common;
using DormFlow.Domain.Entities;
using DormFlow.Domain.Enums;

namespace DormFlow.Application.Features.RoomTypes;

/// <summary>ตัวกรองสำหรับดึงรายการประเภทห้องแบบแบ่งหน้า</summary>
public sealed record RoomTypeListFilter(
    Guid? DormitoryId,
    RoomTypeStatus? Status,
    IReadOnlyCollection<Guid>? RestrictToDormitoryIds);

/// <summary>เข้าถึงข้อมูลประเภทห้องในฐานข้อมูล</summary>
public interface IRoomTypeRepository
{
    /// <summary>อ่านประเภทห้องจากรหัสอ้างอิง</summary>
    Task<RoomType?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>ตรวจว่ามีประเภทห้องที่ใช้รหัสนี้อยู่แล้วในหอพักเดียวกันหรือไม่</summary>
    Task<bool> ExistsByCodeAsync(
        Guid dormitoryId,
        string code,
        Guid? excludeRoomTypeId,
        CancellationToken cancellationToken = default);

    /// <summary>ดึงรายการประเภทห้องแบบแบ่งหน้าตามตัวกรองที่ระบุ</summary>
    Task<PagedResult<RoomType>> GetPagedAsync(
        RoomTypeListFilter filter,
        PagedQuery query,
        CancellationToken cancellationToken = default);

    /// <summary>เพิ่มประเภทห้องใหม่</summary>
    Task AddAsync(RoomType roomType, CancellationToken cancellationToken = default);

    /// <summary>ทำเครื่องหมายลบประเภทห้อง ระบบจะแปลงเป็น Soft Delete ให้อัตโนมัติตอนบันทึก</summary>
    void Remove(RoomType roomType);

    /// <summary>บันทึกการเปลี่ยนแปลงทั้งหมดลงฐานข้อมูล</summary>
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
