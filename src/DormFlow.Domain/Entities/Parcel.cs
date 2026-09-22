using DormFlow.Domain.Common;
using DormFlow.Domain.Enums;

namespace DormFlow.Domain.Entities;

/// <summary>
/// ພັດສະດຸ/ຈົດໝາຍສະບັບໜຶ່ງທີ່ມາຮອດຫໍພັກສຳລັບຜູ້ເຊົ່າ
/// </summary>
/// <remarks>
/// ສະເຝີ tied ກັບຫ້ອງ (ສໍາລັບຂອບເຂດການຈັດຕັ້ງ/ຫໍພັກ ແລະ ເພື່ອໃຫ້ພະນັກງານຮູ້ວ່າຫ້ອງໃດໃຫ້ແຈ້ງ)
/// ສໍາລັບຜູ້ເຊົ່າ, ມັນເປັນທາງເລືອກ. ເພາະວ່າບາງຄັ້ງພະນັກງານບັນທຶກຊຸດທີ່ມາຮອດກ່ອນເພື່ອຢືນຢັນວ່າໃຜຢູ່ໃນຫ້ອງ
/// </remarks>
public class Parcel : TenantEntity
{
    /// <summary>ຫ້ອງຊຸດນີ້ແມ່ນສົ່ງໃຫ້</summary>
    public Guid RoomId { get; set; }

    /// <summary>ຜູ້ເຊົ່າທີ່ parcel ນີ້ຖືກຈັດສົ່ງແມ່ນ null ໃນເວລາທີ່ຍັງບໍ່ໄດ້ຮັບການຢືນຢັນວ່າໃຜເປັນຂອງຫ້ອງການ</summary>
    public Guid? TenantId { get; set; }

    /// <summary>ບໍລິສັດຂົນສົ່ງ</summary>
    public string? CourierName { get; set; }

    /// <summary>ໝາຍເລກເຄື່ອງໝາຍ</summary>
    public string? TrackingNumber { get; set; }

    /// <summary>ລາຍລະອຽດການພັດທະນາເພີ່ມເຕີມ</summary>
    public string? Description { get; set; }

    /// <summary>ເວລາໄປຮັບເຄື່ອງຢູ່ຫໍພັກ</summary>
    public DateTimeOffset ReceivedAt { get; set; }

    /// <summary>ສະຖານະການຫຸ້ມຫໍ່</summary>
    public ParcelStatus Status { get; set; } = ParcelStatus.Received;

    /// <summary>ເວລາຜູ້ເຊົ່າມາຮັບຂອງແມ່ນ null ເມື່ອເຈົ້າຍັງບໍ່ໄດ້ມາຮັບມັນເທື່ອ</summary>
    public DateTimeOffset? PickedUpAt { get; set; }

    /// <summary>ຊື່ແທ້ຂອງຜູ້ທີ່ໄດ້ຮັບຊຸດ (ພວກເຂົາອາດຈະບໍ່ເປັນຜູ້ເຊົ່າໂດຍກົງ, ເຊັ່ນວ່າເພື່ອນຮ່ວມຫ້ອງ)</summary>
    public string? PickedUpByName { get; set; }

    /// <summary>ບັນທຶກເພີ່ມເຕີມ</summary>
    public string? Notes { get; set; }
}
