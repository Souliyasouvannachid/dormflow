using DormFlow.Contracts.Common;

namespace DormFlow.Contracts.Deposits;

/// <summary>พารามิเตอร์การค้นหารายการเงินมัดจำ</summary>
public class DepositListRequest : PagedQueryRequest
{
    /// <summary>กรองเฉพาะรายการของหอพักที่ระบุ</summary>
    public Guid? DormitoryId { get; set; }

    /// <summary>กรองเฉพาะรายการของสัญญาเช่าที่ระบุ</summary>
    public Guid? ContractId { get; set; }

    /// <summary>กรองเฉพาะรายการที่มีสถานะตรงกับที่ระบุ</summary>
    public string? Status { get; set; }
}
