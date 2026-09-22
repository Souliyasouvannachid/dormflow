using DormFlow.Domain.Common;
using DormFlow.Domain.Enums;

namespace DormFlow.Domain.Entities;

/// <summary>
/// ຝາກໃນສັນຍາເຊົ່າຜຶ່ງ
/// </summary>
/// <remarks>
/// ແຍກອອກຈາກໂມດູນ Payments ຕັ້ງໃຈເພາະວ່າມັນເປັນກະແສເງິນທີ່ແຕກຕ່າງກັນ (ເງິນຝາກແມ່ນການຄໍ້າປະກັນທີ່ຕ້ອງໄດ້ຮັບການສົ່ງຄືນ
/// ເມື່ອສັນຍາສິ້ນສຸດລົງ ມັນບໍ່ແມ່ນລາຍຮັບຂອງຮອບບິນດຽວກັນ Invoice/Payment) ສັນຍາເຊົ່າຜຶ່ງມີເງິນຝາກພຽງແຕ່ຜຶ່ງ
/// ການຄືນເງິນແມ່ນອະນຸຍາດໃຫ້ພຽງແຕ່ຄັ້ງດຽວຕໍ່ການເຮັດທຸລະກໍາ (ເງິນຄືນເຕັມ/ບາງ/ປະຖິ້ມທັງຝົດ) ມັນບໍ່ສະຜັບສະຜູນການກັບຄືນໃນຫຼາຍການຕິດຕັ້ງເຊັ່ນ: Payment
/// </remarks>
public class Deposit : TenantEntity
{
    /// <summary>ສັນຍາເຊົ່າທີ່ເງິນຝາກນີ້ຖືກຜູກມັດ</summary>
    public Guid ContractId { get; set; }

    /// <summary>ຈໍານວນເງິນຝາກທີ່ໄດ້ຮັບ</summary>
    public decimal Amount { get; set; }

    /// <summary>ສະຖານະຂອງເງິນຝາກ</summary>
    public DepositStatus Status { get; set; } = DepositStatus.Held;

    /// <summary>ເວລາທີ່ຈະໄດ້ຮັບເງິນຝາກ</summary>
    public DateTimeOffset CollectedAt { get; set; }

    /// <summary>ຈໍານວນທີ່ໄດ້ຮັບເງິນຄືນແມ່ນ 0 ຈົນກ່ວາການຄືນເງິນແມ່ນໃຫ້</summary>
    public decimal RefundedAmount { get; set; }

    /// <summary>ເວລາຄືນເງິນແມ່ນ null ໃນເວລາທີ່ຂ້າພະເຈົ້າຍັງບໍ່ໄດ້ກັບຄືນມັນເທື່ອ</summary>
    public DateTimeOffset? RefundedAt { get; set; }

    /// <summary>ເຫດຜົນສໍາລັບສ່ວນທີ່ຖືກຫັກບໍ່ຖືກສົ່ງຄືນ (ຄວາມເສຍຫາຍ, ການປັບໃໝ, ແລະອື່ນໆ)</summary>
    public string? DeductionReason { get; set; }

    /// <summary>ບັນທຶກເພີ່ມເຕີມ</summary>
    public string? Notes { get; set; }
}
