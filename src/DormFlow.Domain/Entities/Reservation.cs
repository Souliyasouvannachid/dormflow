using DormFlow.Domain.Common;
using DormFlow.Domain.Enums;

namespace DormFlow.Domain.Entities;

/// <summary>
/// ການຈອງຫ້ອງຜຶ່ງຄັ້ງກ່ອນທີ່ຈະເຮັດສັນຍາເຊົ່າຕົວຈິງ
/// </summary>
/// <remarks>
/// ການຈອງເຮັດໃຫ້ຫ້ອງປ່ຽນສະຖານະເປັນ <see cref="RoomStatus.Reserved"/> ທັນທີ
/// ເພື່ອປ້ອງກັນການຖືກຈອງອີກໃນຂະນະທີ່ຜູ້ເຊົ່າກໍາລັງຕັດສິນໃຈຫຼືກະກຽມເອກະສານສັນຍາ
/// </remarks>
public class Reservation : TenantEntity
{
    public DateTimeOffset? ConfirmedAt { get; set; }
    /// <summary>ຫ້ອງສະຫງວນ</summary>
    public Guid RoomId { get; set; }

    /// <summary>ຜູ້ເຊົ່າທີ່ຈອງແລ້ວ</summary>
    public Guid TenantId { get; set; }

    /// <summary>ວັນທີຈອງ</summary>
    public DateTimeOffset ReservationDate { get; set; }

    /// <summary>ວັນທີທີ່ຄາດວ່າຈະຢູ່</summary>
    public DateOnly MoveInDate { get; set; }

    /// <summary>ໃນເວລາທີ່ຕັນຫ້ອງຈະຝົດອາຍຸຖ້າຫາກວ່າສັນຍາຍັງບໍ່ໄດ້ເຊັນກ່ອນເວລານີ້, ຫ້ອງຈະໄດ້ຮັບການປ່ອຍອອກມາເມື່ອຄືນ</summary>
    public DateTimeOffset ExpiresAt { get; set; }

    /// <summary>ຄ່າຈອງຫ້ອງ</summary>
    public decimal ReservationFee { get; set; }

    /// <summary>ສະຖານະການຈອງ</summary>
    public ReservationStatus Status { get; set; } = ReservationStatus.Pending;

    /// <summary>ບັນທຶກເພີ່ມເຕີມ</summary>
    public string? Notes { get; set; }
}
