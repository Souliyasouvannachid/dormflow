namespace DormFlow.Contracts.Payments;

/// <summary>คำขอบันทึกรายการชำระเงิน</summary>
public class CreatePaymentRequest
{
    /// <summary>ใบแจ้งหนี้ที่ชำระ</summary>
    public Guid InvoiceId { get; set; }

    /// <summary>จำนวนเงินที่ชำระ</summary>
    public decimal Amount { get; set; }

    /// <summary>วันเวลาที่ชำระเงิน ถ้าไม่ระบุใช้เวลาปัจจุบัน</summary>
    public DateTimeOffset? PaymentDate { get; set; }

    /// <summary>ช่องทางการชำระเงิน ("Cash", "BankTransfer", "QrCode", "Other")</summary>
    public string Method { get; set; } = string.Empty;

    /// <summary>เลขอ้างอิงการโอน/สลิป</summary>
    public string? ReferenceNumber { get; set; }

    /// <summary>หมายเหตุเพิ่มเติม</summary>
    public string? Notes { get; set; }
}
