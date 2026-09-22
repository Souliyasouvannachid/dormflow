using DormFlow.Domain.Common;
using DormFlow.Domain.Enums;

namespace DormFlow.Domain.Entities;

/// <summary>
/// ການລົງທະບຽນບ່ອນຈອດລົດຂອງຜູ້ເຊົ່າຄັ້ງດຽວ
/// </summary>
/// <remarks>
/// ແຕກຕ່າງຈາກ <see cref="Parcel"/>/<see cref="Visitor"/> ວ່າຜູ້ເຊົ່າມີທາງເລືອກ — ທີ່ນີ້ຜູ້ເຊົ່າແມ່ນຂໍ້ມູນບັງຄັບ
/// ເພາະລົດຕ້ອງມີເຈົ້າຂອງທີ່ສາມາດລະບຸຕົວຕົນໄດ້ສະເໝີ. ສ່ວນຫ້ອງແມ່ນຍັງເກັບຮັກສາໄວ້ສໍາລັບຂອບເຂດຂອງອົງກອນ/ຫໍພັກຕາມປົກກະຕິ
/// ບໍ່ມີ Entity ແມ່ຂອງ "ບ່ອນຈອດລົດ" ແຍກ (<see cref="ParkingSpotLabel"/> ເປັນຂໍ້ຄວາມຟຣີ) ເນື່ອງຈາກວ່າບໍ່ມີ
/// ຄວາມຕ້ອງການການຄຸ້ມຄອງບ່ອນຈອດລົດທີ່ເປັນລະບົບຈາກເອກະສານຕົ້ນສະບັບ
/// </remarks>
public class ParkingRegistration : TenantEntity
{
    /// <summary>ຫ້ອງເຈົ້າຂອງລົດ</summary>
    public Guid RoomId { get; set; }

    /// <summary>ເຈົ້າຂອງລົດເຊົ່າ</summary>
    public Guid TenantId { get; set; }

    /// <summary>ທະບຽນລົດ</summary>
    public required string LicensePlate { get; set; }

    /// <summary>ປະເພດຍານພາຫະນະ</summary>
    public VehicleType VehicleType { get; set; }

    /// <summary>ຍີ່ຫໍ້/ຕົວແບບລົດ</summary>
    public string? Brand { get; set; }

    /// <summary>ສີລົດ</summary>
    public string? Color { get; set; }

    /// <summary>ປ້າຍບ່ອນຈອດລົດ (ຖ້າມັນຖືກລະບຸ) ເກັບຮັກສາເປັນຂໍ້ຄວາມຟຣີ</summary>
    public string? ParkingSpotLabel { get; set; }

    /// <summary>ສະຖານະການລົງທະບຽນ</summary>
    public ParkingRegistrationStatus Status { get; set; } = ParkingRegistrationStatus.Active;

    /// <summary>ເວລາລົງທະບຽນ</summary>
    public DateTimeOffset RegisteredAt { get; set; }

    /// <summary>ເວລາການລົງທະບຽນສິ້ນສຸດລົງ null ໃນເວລາທີ່ມັນຍັງລົງທະບຽນ</summary>
    public DateTimeOffset? EndedAt { get; set; }

    /// <summary>ບັນທຶກເພີ່ມເຕີມ</summary>
    public string? Notes { get; set; }
}
