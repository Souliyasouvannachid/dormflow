using DormFlow.Contracts.Common;

namespace DormFlow.Contracts.Payments;

/// <summary>พารามิเตอร์การค้นหารายการชำระเงิน</summary>
public class PaymentListRequest : PagedQueryRequest
{
    /// <summary>กรองเฉพาะรายการของหอพักที่ระบุ</summary>
    public Guid? DormitoryId { get; set; }

    /// <summary>กรองเฉพาะรายการของใบแจ้งหนี้ที่ระบุ</summary>
    public Guid? InvoiceId { get; set; }

    /// <summary>กรองเฉพาะรายการที่มีสถานะตรงกับที่ระบุ</summary>
    public string? Status { get; set; }
}
