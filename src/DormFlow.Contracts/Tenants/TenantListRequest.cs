using DormFlow.Contracts.Common;

namespace DormFlow.Contracts.Tenants;

/// <summary>พารามิเตอร์การค้นหารายการผู้เช่า</summary>
public class TenantListRequest : PagedQueryRequest
{
    /// <summary>กรองเฉพาะผู้เช่าขององค์กรที่ระบุ มีผลเฉพาะเมื่อผู้เรียกเป็น SuperAdmin</summary>
    public Guid? OrganizationId { get; set; }

    /// <summary>กรองเฉพาะผู้เช่าที่มีสถานะตรงกับที่ระบุ</summary>
    public string? Status { get; set; }
}
