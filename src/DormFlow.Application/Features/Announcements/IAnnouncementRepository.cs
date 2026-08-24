using DormFlow.Application.Common.Pagination;
using DormFlow.Contracts.Common;
using DormFlow.Domain.Entities;
using DormFlow.Domain.Enums;

namespace DormFlow.Application.Features.Announcements;

/// <summary>ตัวกรองสำหรับดึงรายการประกาศแบบแบ่งหน้า</summary>
public sealed record AnnouncementListFilter(
    Guid? OrganizationId,
    Guid? DormitoryId,
    AnnouncementStatus? Status,
    IReadOnlyCollection<Guid>? RestrictToDormitoryIds);

/// <summary>เข้าถึงข้อมูลประกาศในฐานข้อมูล</summary>
public interface IAnnouncementRepository
{
    /// <summary>อ่านข้อมูลประกาศจากรหัสอ้างอิง</summary>
    Task<Announcement?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>ดึงรายการประกาศแบบแบ่งหน้าตามตัวกรองที่ระบุ</summary>
    Task<PagedResult<Announcement>> GetPagedAsync(
        AnnouncementListFilter filter,
        PagedQuery query,
        CancellationToken cancellationToken = default);

    /// <summary>เพิ่มประกาศใหม่</summary>
    Task AddAsync(Announcement announcement, CancellationToken cancellationToken = default);

    /// <summary>บันทึกการเปลี่ยนแปลงทั้งหมดลงฐานข้อมูล</summary>
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
