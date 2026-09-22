using DormFlow.Domain.Common;
using DormFlow.Domain.Enums;

namespace DormFlow.Domain.Entities;

/// <summary>
/// ການຈ່າຍເງິນຜຶ່ງໃບແຈ້ງຜີ້
/// </summary>
/// <remarks>
/// ບັນທຶກເປັນ <see cref="PaymentStatus.Pending"/> ທຳອິດສະເໝີ. ມັນບໍ່ມີຜົນຕໍ່ກັບຈຳນວນໃບແຈ້ງໜີ້
/// ຈົນກ່ວາໄດ້ຮັບການຢືນຢັນ (<see cref="PaymentStatus.Confirmed"/>) ສິດຜ່ານ payments.approve
/// ເພື່ອໃຫ້ມີຈຸດກວດກາກ່ອນເງິນເຂົ້າລະບົບຕົວຈິງ. ໂດຍສະເພາະໃນກໍລະນີຂອງການໂອນເງິນ/QR ຈະຕ້ອງປຽບທຽບກັບໃບຝາກເງິນຂອງທະນາຄານ
/// </remarks>
public class Payment : TenantEntity
{
    /// <summary>ໃບແຈ້ງໜີ້ຖືກຈ່າຍ</summary>
    public Guid InvoiceId { get; set; }

    /// <summary>ຈໍານວນການຈ່າຍເງິນ</summary>
    public decimal Amount { get; set; }

    /// <summary>ວັນທີແລະເວລາຂອງການຈ່າຍເງິນ</summary>
    public DateTimeOffset PaymentDate { get; set; }

    /// <summary>ຊ່ອງທາງການຈ່າຍເງິນ</summary>
    public PaymentMethod Method { get; set; }

    /// <summary>ໝາຍເລກອ້າງອີງການໂອນ/ເລື່ອນແມ່ນ null ເມື່ອຈ່າຍດ້ວຍເງິນສົດ</summary>
    public string? ReferenceNumber { get; set; }

    /// <summary>ສະຖານະຂອງລາຍການການຈ່າຍເງິນ</summary>
    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;

    /// <summary>ເວລາທີ່ຈະຢືນຢັນລາຍການນີ້ແມ່ນ null ໃນເວລາທີ່ຍັງບໍ່ທັນໄດ້ຮັບການຢືນຢັນ</summary>
    public DateTimeOffset? ConfirmedAt { get; set; }

    /// <summary>ເວລາຄືນເງິນແມ່ນ null ໃນເວລາທີ່ເງິນຍັງບໍ່ໄດ້ຄືນ</summary>
    public DateTimeOffset? RefundedAt { get; set; }

    /// <summary>ເຫດຜົນສໍາລັບການຄືນເງິນ</summary>
    public string? RefundReason { get; set; }

    /// <summary>ບັນທຶກເພີ່ມເຕີມ</summary>
    public string? Notes { get; set; }
}
