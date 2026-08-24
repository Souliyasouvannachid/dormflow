using DormFlow.Contracts.Common;

namespace DormFlow.Contracts.RoomTypes;

/// <summary>พารามิเตอร์การค้นหารายการประเภทห้อง</summary>
public class RoomTypeListRequest : PagedQueryRequest
{
    /// <summary>กรองเฉพาะประเภทห้องของหอพักที่ระบุ</summary>
    public Guid? DormitoryId { get; set; }

    /// <summary>กรองเฉพาะประเภทห้องที่มีสถานะตรงกับที่ระบุ</summary>
    public string? Status { get; set; }
}
