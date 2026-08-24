using DormFlow.Application.Common.Pagination;
using DormFlow.Contracts.Common;
using DormFlow.Domain.Entities;

namespace DormFlow.Application.Features.Settings;

/// <summary>ตัวกรองสำหรับดึงรายการค่าตั้งแบบแบ่งหน้า</summary>
public sealed record SettingListFilter(Guid OrganizationId);

/// <summary>เข้าถึงข้อมูลค่าตั้งในฐานข้อมูล</summary>
public interface ISettingRepository
{
    /// <summary>อ่านค่าตั้งจากรหัสอ้างอิง</summary>
    Task<Setting?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>อ่านค่าตั้งจากองค์กรและคีย์</summary>
    Task<Setting?> GetByKeyAsync(Guid organizationId, string key, CancellationToken cancellationToken = default);

    /// <summary>ดึงรายการค่าตั้งแบบแบ่งหน้าตามตัวกรองที่ระบุ</summary>
    Task<PagedResult<Setting>> GetPagedAsync(
        SettingListFilter filter,
        PagedQuery query,
        CancellationToken cancellationToken = default);

    /// <summary>เพิ่มค่าตั้งใหม่</summary>
    Task AddAsync(Setting setting, CancellationToken cancellationToken = default);

    /// <summary>ทำเครื่องหมายลบค่าตั้ง ระบบจะแปลงเป็น Soft Delete ให้อัตโนมัติตอนบันทึก</summary>
    void Remove(Setting setting);

    /// <summary>บันทึกการเปลี่ยนแปลงทั้งหมดลงฐานข้อมูล</summary>
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
