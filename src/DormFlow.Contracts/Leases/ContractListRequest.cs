using DormFlow.Contracts.Common;

namespace DormFlow.Contracts.Leases;

/// <summary>พารามิเตอร์การค้นหารายการสัญญาเช่า</summary>
public class ContractListRequest : PagedQueryRequest
{
    /// <summary>กรองเฉพาะสัญญาของหอพักที่ระบุ</summary>
    public Guid? DormitoryId { get; set; }

    /// <summary>กรองเฉพาะสัญญาของห้องที่ระบุ</summary>
    public Guid? RoomId { get; set; }

    /// <summary>กรองเฉพาะสัญญาของผู้เช่าที่ระบุ</summary>
    public Guid? TenantId { get; set; }

    /// <summary>กรองเฉพาะสัญญาที่มีสถานะตรงกับที่ระบุ</summary>
    public string? Status { get; set; }
}
