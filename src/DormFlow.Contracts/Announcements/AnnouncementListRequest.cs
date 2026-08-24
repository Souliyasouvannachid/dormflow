using DormFlow.Contracts.Common;

namespace DormFlow.Contracts.Announcements;

/// <summary>พารามิเตอร์การค้นหารายการประกาศ</summary>
public class AnnouncementListRequest : PagedQueryRequest
{
    /// <summary>กรองเฉพาะรายการของหอพักที่ระบุ</summary>
    public Guid? DormitoryId { get; set; }

    /// <summary>กรองเฉพาะรายการที่มีสถานะตรงกับที่ระบุ</summary>
    public string? Status { get; set; }
}
