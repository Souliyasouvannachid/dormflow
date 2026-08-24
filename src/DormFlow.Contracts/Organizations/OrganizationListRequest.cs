using DormFlow.Contracts.Common;

namespace DormFlow.Contracts.Organizations;

/// <summary>พารามิเตอร์การค้นหารายการองค์กร</summary>
public class OrganizationListRequest : PagedQueryRequest
{
    /// <summary>กรองเฉพาะองค์กรที่มีสถานะตรงกับที่ระบุ</summary>
    public string? Status { get; set; }
}
