using DormFlow.Contracts.Common;

namespace DormFlow.Contracts.Visitors;

/// <summary>พารามิเตอร์การค้นหารายการผู้มาเยือน</summary>
public class VisitorListRequest : PagedQueryRequest
{
    /// <summary>กรองเฉพาะรายการของหอพักที่ระบุ</summary>
    public Guid? DormitoryId { get; set; }

    /// <summary>กรองเฉพาะรายการของห้องที่ระบุ</summary>
    public Guid? RoomId { get; set; }

    /// <summary>กรองเฉพาะรายการที่มีสถานะตรงกับที่ระบุ</summary>
    public string? Status { get; set; }
}
