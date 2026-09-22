namespace DormFlow.Contracts.Payments;

/// <summary>ຄຳຮ້ອງຂໍບັນທຶກການຈ່າຍເງິນ</summary>
public class CreatePaymentRequest
{
    /// <summary>ໃບແຈ້ງໜີ້ຖືກຈ່າຍ</summary>
    public Guid InvoiceId { get; set; }

    /// <summary>ຈໍານວນການຈ່າຍເງິນ</summary>
    public decimal Amount { get; set; }

    /// <summary>ວັນທີ ແລະເວລາຈ່າຍເງິນ ຖ້າບໍ່ໄດ້ລະບຸ, ໃຫ້ໃຊ້ເວລາປັດຈຸບັນ</summary>
    public DateTimeOffset? PaymentDate { get; set; }

    /// <summary>ຊ່ອງທາງການຈ່າຍເງິນ ("Cash", "BankTransfer", "QrCode", "Other")</summary>
    public string Method { get; set; } = string.Empty;

    /// <summary>ໝາຍເລກອ້າງອີງການໂອນ/ເລື່ອນ</summary>
    public string? ReferenceNumber { get; set; }

    /// <summary>ບັນທຶກເພີ່ມເຕີມ</summary>
    public string? Notes { get; set; }
}
