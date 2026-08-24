using DormFlow.Contracts.Common;
using DormFlow.Contracts.Notifications;

namespace DormFlow.Application.Features.Notifications;

/// <summary>Use Case ทั้งหมดของโมดูลการแจ้งเตือน</summary>
public interface INotificationService
{
    /// <summary>ดึงรายการแจ้งเตือนของผู้ใช้ปัจจุบันแบบแบ่งหน้า</summary>
    Task<PagedResult<NotificationResponse>> GetMyPagedAsync(
        NotificationListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>ทำเครื่องหมายว่าอ่านแล้ว ทำได้เฉพาะการแจ้งเตือนของผู้ใช้ปัจจุบันเท่านั้น</summary>
    Task<NotificationResponse> MarkAsReadAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>ส่งการแจ้งเตือนถึงผู้ใช้หนึ่งคน</summary>
    Task<NotificationResponse> SendAsync(
        SendNotificationRequest request,
        CancellationToken cancellationToken = default);
}
