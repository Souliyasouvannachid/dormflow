namespace DormFlow.Contracts.Payments;

/// <summary>ຂໍ້ມູນຜຶ່ງລາຍການການຈ່າຍເງິນ</summary>
public class PaymentResponse
{
    /// <summary>ລະຫັດອ້າງອີງການເຮັດທຸລະກໍາການຈ່າຍເງິນ</summary>
    public Guid Id { get; init; }

    /// <summary>ອົງກອນທີ່ກ່ຽວຂ້ອງ</summary>
    public Guid OrganizationId { get; init; }

    /// <summary>ຫໍພັກທີ່ກ່ຽວຂ້ອງ</summary>
    public Guid DormitoryId { get; init; }

    /// <summary>ໃບແຈ້ງໜີ້ຖືກຈ່າຍ</summary>
    public Guid InvoiceId { get; init; }

    /// <summary>ຈໍານວນການຈ່າຍເງິນ</summary>
    public decimal Amount { get; init; }

    /// <summary>ວັນທີແລະເວລາຂອງການຈ່າຍເງິນ</summary>
    public DateTimeOffset PaymentDate { get; init; }

    /// <summary>ຊ່ອງທາງການຈ່າຍເງິນ</summary>
    public string Method { get; init; } = string.Empty;

    /// <summary>ໝາຍເລກອ້າງອີງການໂອນ/ເລື່ອນ</summary>
    public string? ReferenceNumber { get; init; }

    /// <summary>ສະຖານະຂອງລາຍການການຈ່າຍເງິນ</summary>
    public string Status { get; init; } = string.Empty;

    /// <summary>ເວລາທີ່ຈະຢືນຢັນລາຍການນີ້</summary>
    public DateTimeOffset? ConfirmedAt { get; init; }

    /// <summary>ເວລາຄືນເງິນ</summary>
    public DateTimeOffset? RefundedAt { get; init; }

    /// <summary>ເຫດຜົນສໍາລັບການຄືນເງິນ</summary>
    public string? RefundReason { get; init; }

    /// <summary>ບັນທຶກເພີ່ມເຕີມ</summary>
    public string? Notes { get; init; }

    /// <summary>ເວລາສ້າງຂໍ້ມູນ</summary>
    public DateTimeOffset CreatedAt { get; init; }
}
