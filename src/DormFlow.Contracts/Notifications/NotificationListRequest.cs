using DormFlow.Contracts.Common;

namespace DormFlow.Contracts.Notifications;

/// <summary>พารามิเตอร์การค้นหาการแจ้งเตือนของตนเอง</summary>
public class NotificationListRequest : PagedQueryRequest
{
    /// <summary>กรองเฉพาะรายการที่อ่านแล้ว/ยังไม่อ่านตามที่ระบุ</summary>
    public bool? IsRead { get; set; }
}
