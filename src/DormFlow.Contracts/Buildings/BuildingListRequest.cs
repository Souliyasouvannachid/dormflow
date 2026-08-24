using DormFlow.Contracts.Common;

namespace DormFlow.Contracts.Buildings;

/// <summary>พารามิเตอร์การค้นหารายการอาคาร</summary>
public class BuildingListRequest : PagedQueryRequest
{
    /// <summary>กรองเฉพาะอาคารของหอพักที่ระบุ</summary>
    public Guid? DormitoryId { get; set; }

    /// <summary>กรองเฉพาะอาคารที่มีสถานะตรงกับที่ระบุ</summary>
    public string? Status { get; set; }
}
