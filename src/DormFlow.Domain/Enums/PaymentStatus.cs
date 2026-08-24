namespace DormFlow.Domain.Enums;

/// <summary>สถานะของรายการชำระเงิน</summary>
public enum PaymentStatus
{
    /// <summary>บันทึกไว้แล้ว รอยืนยัน ยังไม่ถูกนำไปหักยอดใบแจ้งหนี้</summary>
    Pending = 1,

    /// <summary>ยืนยันแล้ว ถูกนำไปหักยอดใบแจ้งหนี้แล้ว</summary>
    Confirmed = 2,

    /// <summary>คืนเงินแล้ว ยอดถูกหักออกจากใบแจ้งหนี้คืน</summary>
    Refunded = 3,
}
