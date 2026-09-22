namespace DormFlow.Contracts.Deposits;

/// <summary>ຂໍ້ມູນເງິນຝາກຜຶ່ງ</summary>
public class DepositResponse
{
    /// <summary>ລະຫັດອ້າງອີງເງິນຝາກ</summary>
    public Guid Id { get; init; }

    /// <summary>ອົງກອນທີ່ກ່ຽວຂ້ອງ</summary>
    public Guid OrganizationId { get; init; }

    /// <summary>ຫໍພັກທີ່ກ່ຽວຂ້ອງ</summary>
    public Guid DormitoryId { get; init; }

    /// <summary>ສັນຍາເຊົ່າທີ່ເງິນຝາກນີ້ຖືກຜູກມັດ</summary>
    public Guid ContractId { get; init; }

    /// <summary>ຈໍານວນເງິນຝາກທີ່ໄດ້ຮັບ</summary>
    public decimal Amount { get; init; }

    /// <summary>ສະຖານະຂອງເງິນຝາກ</summary>
    public string Status { get; init; } = string.Empty;

    /// <summary>ເວລາທີ່ຈະໄດ້ຮັບເງິນຝາກ</summary>
    public DateTimeOffset CollectedAt { get; init; }

    /// <summary>ຈໍານວນທີ່ສົ່ງຄືນ</summary>
    public decimal RefundedAmount { get; init; }

    /// <summary>ເວລາຄືນເງິນ</summary>
    public DateTimeOffset? RefundedAt { get; init; }

    /// <summary>ເຫດຜົນສໍາລັບສ່ວນທີ່ຖືກຫັກບໍ່ຖືກສົ່ງຄືນ</summary>
    public string? DeductionReason { get; init; }

    /// <summary>ບັນທຶກເພີ່ມເຕີມ</summary>
    public string? Notes { get; init; }

    /// <summary>ເວລາສ້າງຂໍ້ມູນ</summary>
    public DateTimeOffset CreatedAt { get; init; }
}
