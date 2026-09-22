namespace DormFlow.Contracts.Receipts;

/// <summary>ຂໍ້ມູນໃບຮັບເງິນຜຶ່ງ</summary>
public class ReceiptResponse
{
    /// <summary>ລະຫັດອ້າງອີງໃບຮັບເງິນ</summary>
    public Guid Id { get; init; }

    /// <summary>ອົງກອນທີ່ກ່ຽວຂ້ອງ</summary>
    public Guid OrganizationId { get; init; }

    /// <summary>ຫໍພັກທີ່ກ່ຽວຂ້ອງ</summary>
    public Guid DormitoryId { get; init; }

    /// <summary>ບັນຊີລາຍຊື່ການຈ່າຍເງິນທີ່ໃບຮັບເງິນນີ້ອອກ</summary>
    public Guid PaymentId { get; init; }

    /// <summary>ໝາຍເລກໃບຮັບເງິນ</summary>
    public string ReceiptNumber { get; init; } = string.Empty;

    /// <summary>ຈຳນວນຕາມໃບຮັບ</summary>
    public decimal Amount { get; init; }

    /// <summary>ເວລາອອກໃບຮັບເງິນ</summary>
    public DateTimeOffset IssuedAt { get; init; }

    /// <summary>ບັນທຶກເພີ່ມເຕີມ</summary>
    public string? Notes { get; init; }

    /// <summary>ເວລາສ້າງຂໍ້ມູນ</summary>
    public DateTimeOffset CreatedAt { get; init; }
}
