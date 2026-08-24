using DormFlow.Application.Common.Pagination;
using DormFlow.Contracts.Common;
using DormFlow.Domain.Entities;
using DormFlow.Domain.Enums;

namespace DormFlow.Application.Features.Dormitories;

/// <summary>ตัวกรองสำหรับดึงรายการหอพักแบบแบ่งหน้า</summary>
/// <param name="OrganizationId">กรองเฉพาะหอพักขององค์กรที่ระบุ เป็น null เมื่อไม่จำกัด</param>
/// <param name="Status">กรองเฉพาะหอพักที่มีสถานะตรงกัน เป็น null เมื่อไม่จำกัด</param>
/// <param name="RestrictToIds">
/// จำกัดผลลัพธ์ให้เห็นเฉพาะรหัสหอพักในรายการนี้ ใช้กับผู้ใช้ที่ถูกมอบหมายให้ดูแลเฉพาะบางหอพัก
/// เป็น null เมื่อผู้เรียกเห็นได้ทุกหอพักในองค์กรของตน (SuperAdmin และ Owner)
/// </param>
public sealed record DormitoryListFilter(
    Guid? OrganizationId,
    DormitoryStatus? Status,
    IReadOnlyCollection<Guid>? RestrictToIds);

/// <summary>
/// เข้าถึงข้อมูลหอพักในฐานข้อมูล
/// </summary>
/// <remarks>
/// เป็น Port ที่ชั้น Infrastructure เป็นผู้ให้บริการ ชั้น Application จึงทำงานกับ Domain Entity
/// ได้โดยไม่ต้องอ้างอิง EF Core โดยตรง
/// </remarks>
public interface IDormitoryRepository
{
    /// <summary>อ่านหอพักจากรหัสอ้างอิง</summary>
    /// <param name="id">รหัสหอพัก</param>
    /// <param name="cancellationToken">โทเคนสำหรับยกเลิกคำสั่ง</param>
    Task<Dormitory?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>ตรวจว่ามีหอพักที่ใช้รหัสนี้อยู่แล้วในองค์กรเดียวกันหรือไม่</summary>
    /// <param name="organizationId">องค์กรที่ต้องการตรวจสอบ</param>
    /// <param name="code">รหัสหอพักที่ต้องการตรวจสอบ</param>
    /// <param name="excludeDormitoryId">รหัสหอพักที่ไม่ต้องนำมานับ ใช้ตอนแก้ไขข้อมูลของหอพักเดิม</param>
    /// <param name="cancellationToken">โทเคนสำหรับยกเลิกคำสั่ง</param>
    Task<bool> ExistsByCodeAsync(
        Guid organizationId,
        string code,
        Guid? excludeDormitoryId,
        CancellationToken cancellationToken = default);

    /// <summary>ดึงรายการหอพักแบบแบ่งหน้าตามตัวกรองที่ระบุ</summary>
    /// <param name="filter">ตัวกรอง</param>
    /// <param name="query">พารามิเตอร์การแบ่งหน้าที่ปรับค่าแล้ว</param>
    /// <param name="cancellationToken">โทเคนสำหรับยกเลิกคำสั่ง</param>
    Task<PagedResult<Dormitory>> GetPagedAsync(
        DormitoryListFilter filter,
        PagedQuery query,
        CancellationToken cancellationToken = default);

    /// <summary>เพิ่มหอพักใหม่</summary>
    /// <param name="dormitory">หอพักที่ต้องการเพิ่ม</param>
    /// <param name="cancellationToken">โทเคนสำหรับยกเลิกคำสั่ง</param>
    Task AddAsync(Dormitory dormitory, CancellationToken cancellationToken = default);

    /// <summary>ทำเครื่องหมายลบหอพัก ระบบจะแปลงเป็น Soft Delete ให้อัตโนมัติตอนบันทึก</summary>
    /// <param name="dormitory">หอพักที่ต้องการลบ</param>
    void Remove(Dormitory dormitory);

    /// <summary>บันทึกการเปลี่ยนแปลงทั้งหมดลงฐานข้อมูล</summary>
    /// <param name="cancellationToken">โทเคนสำหรับยกเลิกคำสั่ง</param>
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
