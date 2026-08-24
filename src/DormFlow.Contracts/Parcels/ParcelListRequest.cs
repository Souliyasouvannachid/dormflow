using DormFlow.Contracts.Common;

namespace DormFlow.Contracts.Parcels;

/// <summary>พารามิเตอร์การค้นหารายการพัสดุ</summary>
public class ParcelListRequest : PagedQueryRequest
{
    /// <summary>กรองเฉพาะรายการของหอพักที่ระบุ</summary>
    public Guid? DormitoryId { get; set; }

    /// <summary>กรองเฉพาะรายการของห้องที่ระบุ</summary>
    public Guid? RoomId { get; set; }

    /// <summary>กรองเฉพาะรายการที่มีสถานะตรงกับที่ระบุ</summary>
    public string? Status { get; set; }
}
