using DormFlow.Contracts.Common;

namespace DormFlow.Contracts.Receipts;

/// <summary>พารามิเตอร์การค้นหารายการใบเสร็จรับเงิน</summary>
public class ReceiptListRequest : PagedQueryRequest
{
    /// <summary>กรองเฉพาะรายการของหอพักที่ระบุ</summary>
    public Guid? DormitoryId { get; set; }

    /// <summary>กรองเฉพาะใบเสร็จของรายการชำระเงินที่ระบุ</summary>
    public Guid? PaymentId { get; set; }
}
