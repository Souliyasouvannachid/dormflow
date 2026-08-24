using DormFlow.Contracts.Common;

namespace DormFlow.Contracts.Floors;

/// <summary>พารามิเตอร์การค้นหารายการชั้น</summary>
public class FloorListRequest : PagedQueryRequest
{
    /// <summary>กรองเฉพาะชั้นของอาคารที่ระบุ</summary>
    public Guid? BuildingId { get; set; }

    /// <summary>กรองเฉพาะชั้นที่มีสถานะตรงกับที่ระบุ</summary>
    public string? Status { get; set; }
}
