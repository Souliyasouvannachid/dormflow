using DormFlow.Application.Common.Pagination;
using DormFlow.Contracts.Common;
using DormFlow.Domain.Entities;
using DormFlow.Domain.Enums;

namespace DormFlow.Application.Features.Floors;

/// <summary>ตัวกรองสำหรับดึงรายการชั้นแบบแบ่งหน้า</summary>
/// <param name="OrganizationId">จำกัดผลลัพธ์ให้เห็นเฉพาะชั้นขององค์กรนี้ เป็น null เฉพาะ SuperAdmin เท่านั้น</param>
/// <param name="BuildingId">กรองเฉพาะชั้นของอาคารที่ระบุ เป็น null เมื่อไม่จำกัด</param>
/// <param name="Status">กรองเฉพาะชั้นที่มีสถานะตรงกัน เป็น null เมื่อไม่จำกัด</param>
/// <param name="RestrictToDormitoryIds">จำกัดผลลัพธ์ให้เห็นเฉพาะชั้นของหอพักในรายการนี้ เป็น null เมื่อไม่จำกัด</param>
public sealed record FloorListFilter(
    Guid? OrganizationId,
    Guid? BuildingId,
    FloorStatus? Status,
    IReadOnlyCollection<Guid>? RestrictToDormitoryIds);

/// <summary>เข้าถึงข้อมูลชั้นของอาคารในฐานข้อมูล</summary>
public interface IFloorRepository
{
    /// <summary>อ่านชั้นจากรหัสอ้างอิง</summary>
    Task<Floor?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>ตรวจว่ามีชั้นที่ใช้ลำดับนี้อยู่แล้วในอาคารเดียวกันหรือไม่</summary>
    Task<bool> ExistsByFloorNumberAsync(
        Guid buildingId,
        int floorNumber,
        Guid? excludeFloorId,
        CancellationToken cancellationToken = default);

    /// <summary>ดึงรายการชั้นแบบแบ่งหน้าตามตัวกรองที่ระบุ</summary>
    Task<PagedResult<Floor>> GetPagedAsync(
        FloorListFilter filter,
        PagedQuery query,
        CancellationToken cancellationToken = default);

    /// <summary>เพิ่มชั้นใหม่</summary>
    Task AddAsync(Floor floor, CancellationToken cancellationToken = default);

    /// <summary>ทำเครื่องหมายลบชั้น ระบบจะแปลงเป็น Soft Delete ให้อัตโนมัติตอนบันทึก</summary>
    void Remove(Floor floor);

    /// <summary>บันทึกการเปลี่ยนแปลงทั้งหมดลงฐานข้อมูล</summary>
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
