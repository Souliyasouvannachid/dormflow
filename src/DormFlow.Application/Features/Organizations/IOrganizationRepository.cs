using DormFlow.Application.Common.Pagination;
using DormFlow.Contracts.Common;
using DormFlow.Domain.Entities;
using DormFlow.Domain.Enums;

namespace DormFlow.Application.Features.Organizations;

/// <summary>ตัวกรองสำหรับดึงรายการองค์กรแบบแบ่งหน้า</summary>
/// <param name="OrganizationId">จำกัดผลลัพธ์ให้เห็นเฉพาะองค์กรนี้แห่งเดียว เป็น null เมื่อ SuperAdmin เห็นได้ทุกองค์กร</param>
/// <param name="Status">กรองเฉพาะองค์กรที่มีสถานะตรงกัน เป็น null เมื่อไม่จำกัด</param>
public sealed record OrganizationListFilter(Guid? OrganizationId, OrganizationStatus? Status);

/// <summary>เข้าถึงข้อมูลองค์กรในฐานข้อมูล</summary>
public interface IOrganizationRepository
{
    /// <summary>อ่านองค์กรจากรหัสอ้างอิง</summary>
    Task<Organization?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>ตรวจว่ามีองค์กรที่ใช้รหัสนี้อยู่แล้วหรือไม่</summary>
    Task<bool> ExistsByCodeAsync(
        string code,
        Guid? excludeOrganizationId,
        CancellationToken cancellationToken = default);

    /// <summary>ดึงรายการองค์กรแบบแบ่งหน้าตามตัวกรองที่ระบุ</summary>
    Task<PagedResult<Organization>> GetPagedAsync(
        OrganizationListFilter filter,
        PagedQuery query,
        CancellationToken cancellationToken = default);

    /// <summary>เพิ่มองค์กรใหม่</summary>
    Task AddAsync(Organization organization, CancellationToken cancellationToken = default);

    /// <summary>ทำเครื่องหมายลบองค์กร ระบบจะแปลงเป็น Soft Delete ให้อัตโนมัติตอนบันทึก</summary>
    void Remove(Organization organization);

    /// <summary>บันทึกการเปลี่ยนแปลงทั้งหมดลงฐานข้อมูล</summary>
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
