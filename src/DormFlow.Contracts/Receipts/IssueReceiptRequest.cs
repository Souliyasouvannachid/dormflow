namespace DormFlow.Contracts.Receipts;

/// <summary>คำขอออกใบเสร็จรับเงิน</summary>
public class IssueReceiptRequest
{
    /// <summary>รายการชำระเงินที่ต้องการออกใบเสร็จ ต้องเป็นรายการที่ยืนยันแล้ว (Confirmed) เท่านั้น</summary>
    public Guid PaymentId { get; set; }

    /// <summary>หมายเหตุเพิ่มเติม</summary>
    public string? Notes { get; set; }
}
