using DormFlow.Application.Common.Pagination;
using DormFlow.Contracts.Common;
using DormFlow.Domain.Entities;

namespace DormFlow.Application.Features.Notifications;

/// <summary>ตัวกรองสำหรับดึงรายการแจ้งเตือนของผู้รับคนเดียวแบบแบ่งหน้า</summary>
public sealed record NotificationListFilter(Guid RecipientUserId, bool? IsRead);

/// <summary>เข้าถึงข้อมูลการแจ้งเตือนในฐานข้อมูล</summary>
public interface INotificationRepository
{
    /// <summary>อ่านการแจ้งเตือนจากรหัสอ้างอิง</summary>
    Task<Notification?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>ดึงรายการแจ้งเตือนของผู้รับคนเดียวแบบแบ่งหน้าตามตัวกรองที่ระบุ</summary>
    Task<PagedResult<Notification>> GetPagedAsync(
        NotificationListFilter filter,
        PagedQuery query,
        CancellationToken cancellationToken = default);

    /// <summary>เพิ่มการแจ้งเตือนใหม่</summary>
    Task AddAsync(Notification notification, CancellationToken cancellationToken = default);

    /// <summary>บันทึกการเปลี่ยนแปลงทั้งหมดลงฐานข้อมูล</summary>
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
