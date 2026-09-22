namespace DormFlow.Domain.Enums;

/// <summary>ຊ່ອງທາງການຈ່າຍເງິນ</summary>
public enum PaymentMethod
{
    /// <summary>ເງິນສົດ</summary>
    Cash = 1,

    /// <summary>ໂອນທະນາຄານ</summary>
    BankTransfer = 2,

    /// <summary>ສະແກນ QR Code</summary>
    QrCode = 3,

    /// <summary>ຊ່ອງທາງອື່ນໆ</summary>
    Other = 4,
}
