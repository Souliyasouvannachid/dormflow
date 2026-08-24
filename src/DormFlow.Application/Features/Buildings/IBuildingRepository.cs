using DormFlow.Application.Common.Pagination;
using DormFlow.Contracts.Common;
using DormFlow.Domain.Entities;
using DormFlow.Domain.Enums;

namespace DormFlow.Application.Features.Buildings;

/// <summary>ตัวกรองสำหรับดึงรายการอาคารแบบแบ่งหน้า</summary>
/// <param name="OrganizationId">จำกัดผลลัพธ์ให้เห็นเฉพาะอาคารขององค์กรนี้ เป็น null เฉพาะ SuperAdmin เท่านั้น</param>
/// <param name="DormitoryId">กรองเฉพาะอาคารของหอพักที่ระบุ เป็น null เมื่อไม่จำกัด</param>
/// <param name="Status">กรองเฉพาะอาคารที่มีสถานะตรงกัน เป็น null เมื่อไม่จำกัด</param>
/// <param name="RestrictToDormitoryIds">จำกัดผลลัพธ์ให้เห็นเฉพาะอาคารของหอพักในรายการนี้ เป็น null เมื่อไม่จำกัด</param>
public sealed record BuildingListFilter(
    Guid? OrganizationId,
    Guid? DormitoryId,
    BuildingStatus? Status,
    IReadOnlyCollection<Guid>? RestrictToDormitoryIds);

/// <summary>เข้าถึงข้อมูลอาคารในฐานข้อมูล</summary>
public interface IBuildingRepository
{
    /// <summary>อ่านอาคารจากรหัสอ้างอิง</summary>
    Task<Building?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>ตรวจว่ามีอาคารที่ใช้รหัสนี้อยู่แล้วในหอพักเดียวกันหรือไม่</summary>
    Task<bool> ExistsByCodeAsync(
        Guid dormitoryId,
        string code,
        Guid? excludeBuildingId,
        CancellationToken cancellationToken = default);

    /// <summary>ดึงรายการอาคารแบบแบ่งหน้าตามตัวกรองที่ระบุ</summary>
    Task<PagedResult<Building>> GetPagedAsync(
        BuildingListFilter filter,
        PagedQuery query,
        CancellationToken cancellationToken = default);

    /// <summary>เพิ่มอาคารใหม่</summary>
    Task AddAsync(Building building, CancellationToken cancellationToken = default);

    /// <summary>ทำเครื่องหมายลบอาคาร ระบบจะแปลงเป็น Soft Delete ให้อัตโนมัติตอนบันทึก</summary>
    void Remove(Building building);

    /// <summary>บันทึกการเปลี่ยนแปลงทั้งหมดลงฐานข้อมูล</summary>
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
