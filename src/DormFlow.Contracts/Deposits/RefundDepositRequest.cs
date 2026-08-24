namespace DormFlow.Contracts.Deposits;

/// <summary>คำขอคืนเงินมัดจำ</summary>
public class RefundDepositRequest
{
    /// <summary>จำนวนเงินที่ต้องการคืน ต้องอยู่ระหว่าง 0 ถึงจำนวนเงินมัดจำที่รับไว้ (0 หมายถึงริบทั้งหมด)</summary>
    public decimal RefundAmount { get; set; }

    /// <summary>เหตุผลของส่วนที่หักไว้ไม่คืน จำเป็นเมื่อคืนไม่เต็มจำนวน</summary>
    public string? DeductionReason { get; set; }
}
