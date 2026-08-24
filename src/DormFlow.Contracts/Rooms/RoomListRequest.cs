using DormFlow.Contracts.Common;

namespace DormFlow.Contracts.Rooms;

/// <summary>พารามิเตอร์การค้นหารายการห้องพัก</summary>
public class RoomListRequest : PagedQueryRequest
{
    /// <summary>กรองเฉพาะห้องของหอพักที่ระบุ</summary>
    public Guid? DormitoryId { get; set; }

    /// <summary>กรองเฉพาะห้องของอาคารที่ระบุ</summary>
    public Guid? BuildingId { get; set; }

    /// <summary>กรองเฉพาะห้องของชั้นที่ระบุ</summary>
    public Guid? FloorId { get; set; }

    /// <summary>กรองเฉพาะห้องของประเภทห้องที่ระบุ</summary>
    public Guid? RoomTypeId { get; set; }

    /// <summary>กรองเฉพาะห้องที่มีสถานะตรงกับที่ระบุ</summary>
    public string? Status { get; set; }
}
