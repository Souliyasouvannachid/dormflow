using DormFlow.Domain.Common;
using DormFlow.Domain.Enums;

namespace DormFlow.Domain.Entities;

/// <summary>
/// ບັນທຶກການເຂົ້າ-ນັກທ່ອງທ່ຽວອອກຄັ້ງດຽວ
/// </summary>
/// <remarks>
/// ສະເຝີ tied ກັບຫ້ອງ (ສໍາລັບຂອບເຂດການຈັດຕັ້ງ/ຫໍພັກ ແລະລະບຸຫ້ອງໃດທີ່ເຈົ້າຈະໄປຢ້ຽມຢາມ) ສໍາລັບຜູ້ເຊົ່າ, ມັນເປັນທາງເລືອກ
/// ເຊັ່ນດຽວກັນກັບ <see cref="Parcel"/> ເພາະຫ້ອງອາດມີຜູ້ເຊົ່າຮ່ວມຫຼາຍຄົນ
/// </remarks>
public class Visitor : TenantEntity
{
    /// <summary>ຫ້ອງທີ່ນັກທ່ອງທ່ຽວມາຢ້ຽມຢາມ</summary>
    public Guid RoomId { get; set; }

    /// <summary>ຜູ້ເຊົ່າທີ່ເຂົ້າມາຢ້ຽມຢາມແມ່ນ null ເມື່ອບໍ່ໄດ້ລະບຸ</summary>
    public Guid? TenantId { get; set; }

    /// <summary>ຊື່ນັກທ່ອງທ່ຽວ</summary>
    public required string VisitorName { get; set; }

    /// <summary>ເບີໂທລະສັບຂອງຜູ້ເຂົ້າຊົມ</summary>
    public string? VisitorPhone { get; set; }

    /// <summary>ບັດປະຈຳຕົວ ຫຼືໝາຍເລກໜັງສືຜ່ານແດນຂອງຜູ້ເຂົ້າຊົມ ເພື່ອບັນທຶກຄວາມປອດໄພ</summary>
    public string? IdNumber { get; set; }

    /// <summary>ຈຸດປະສົງຂອງການຢ້ຽມຢາມ</summary>
    public string? Purpose { get; set; }

    /// <summary>ເວລາເຂົ້າຫໍພັກ</summary>
    public DateTimeOffset CheckInAt { get; set; }

    /// <summary>ສະຖານະນັກທ່ອງທ່ຽວ</summary>
    public VisitorStatus Status { get; set; } = VisitorStatus.CheckedIn;

    /// <summary>ເວລາອອກຈາກຫໍພັກ null ໃນເວລາທີ່ມັນຍັງບໍ່ທັນອອກ</summary>
    public DateTimeOffset? CheckOutAt { get; set; }

    /// <summary>ບັນທຶກເພີ່ມເຕີມ</summary>
    public string? Notes { get; set; }
}
