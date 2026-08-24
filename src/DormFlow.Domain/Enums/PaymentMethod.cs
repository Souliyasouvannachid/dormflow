namespace DormFlow.Domain.Enums;

/// <summary>ช่องทางการชำระเงิน</summary>
public enum PaymentMethod
{
    /// <summary>เงินสด</summary>
    Cash = 1,

    /// <summary>โอนผ่านธนาคาร</summary>
    BankTransfer = 2,

    /// <summary>สแกน QR Code</summary>
    QrCode = 3,

    /// <summary>ช่องทางอื่น ๆ</summary>
    Other = 4,
}
