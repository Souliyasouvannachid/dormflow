using DormFlow.Domain.Common;
using DormFlow.Domain.Enums;

namespace DormFlow.Domain.Entities;

/// <summary>
/// ສັນຍາເຊົ່າຜຶ່ງຫ້ອງ
/// </summary>
/// <remarks>
/// ລະບົບຍັງບໍ່ທັນມີ Entity ແຍກຕ່າງຫາກສໍາລັບ CheckIn/CheckOut ໃນຂັ້ນຕອນນີ້
/// ເນື່ອງຈາກວ່າເອກະສານຕົ້ນສະບັບບໍ່ໄດ້ລະບຸລາຍລະອຽດຂອງສອງໂມດູນນີ້ (ແຕກຕ່າງຈາກ Phase 1 ລະບຸໄວ້ຢ່າງຄົບຖ້ວນ)
/// ດັ່ງນັ້ນ, ມັນໄດ້ຖືກລວມເຂົ້າເປັນການກະທໍາໂດຍອີງໃສ່ສັນຍາແທນ: ເປີດໃຊ້ສັນຍາ (<see cref="ContractStatus.Active"/>) ແມ່ນຈຸດເຊັກອິນ
/// ແລະສິ້ນສຸດສັນຍາ (<see cref="ContractStatus.Terminated"/>) ແມ່ນຈຸດເຊັກເອົາ
/// ຖ້າຫາກວ່າທ່ານຕ້ອງການທີ່ຈະບັນທຶກລາຍລະອຽດຂອງການກວດກາຫ້ອງຫຼືຮູບພາບຕາມການເຂົ້າ-ອອກ
/// ຄ່ອຍໆແຍກອອກເປັນ Entity ຂອງຕົນເອງຕໍ່ມາໂດຍບໍ່ມີຜົນກະທົບຕໍ່ໂຄງສ້າງນີ້
/// </remarks>
public class Contract : TenantEntity
{
    /// <summary>ຫ້ອງສັນຍາ</summary>
    public Guid RoomId { get; set; }

    /// <summary>ຜູ້ເຊົ່າຕາມສັນຍາ</summary>
    public Guid TenantId { get; set; }

    /// <summary>ການຈອງທີ່ເຂົ້າມາໃນສັນຍານີ້ແມ່ນ null ໃນເວລາທີ່ເຮັດສັນຍາໂດຍບໍ່ມີການເຮັດການຈອງລ່ວງຜ້າ</summary>
    public Guid? ReservationId { get; set; }

    /// <summary>ໝາຍເລກສັນຍາແມ່ນເປັນເອກະລັກພາຍໃນອົງກອນດຽວກັນ</summary>
    public required string ContractNumber { get; set; }

    /// <summary>ວັນທີເລີ່ມຕົ້ນຂອງສັນຍາ</summary>
    public DateOnly StartDate { get; set; }

    /// <summary>ກຳນົດວັນທີສິ້ນສຸດສັນຍາ</summary>
    public DateOnly EndDate { get; set; }

    /// <summary>ການເຊົ່າລາຍເດືອນຕາມສັນຍາສະບັບນີ້ ສຳເນົາມາຈາກຫ້ອງເມື່ອເຮັດສັນຍາ. ມັນບໍ່ປ່ຽນແປງຕາມຄ່າເຊົ່າຫ້ອງຕໍ່ມາ</summary>
    public decimal MonthlyRent { get; set; }

    /// <summary>ຝາກເງິນຕາມສັນຍານີ້</summary>
    public decimal DepositAmount { get; set; }

    /// <summary>ສະຖານະສັນຍາ</summary>
    public ContractStatus Status { get; set; } = ContractStatus.Draft;

    /// <summary>ເວລາເປີດໃຊ້ສັນຍາ (ເຊັກອິນ) ແມ່ນ null ໃນເວລາທີ່ມັນຍັງເປັນຮ່າງກາຍ</summary>
    public DateTimeOffset? SignedAt { get; set; }

    /// <summary>ເວລາສິ້ນສຸດສັນຍາ (ກວດເບິ່ງ) ແມ່ນ null ໃນເວລາທີ່ມັນຍັງບໍ່ຝົດ</summary>
    public DateTimeOffset? TerminatedAt { get; set; }

    /// <summary>ເຫດຜົນຂອງການຍົກເລີກສັນຍາກ່ອນໄວອັນຄວນ ຫຼືເມື່ອຄົບກຳນົດ</summary>
    public string? TerminationReason { get; set; }

    /// <summary>ບັນທຶກເພີ່ມເຕີມ</summary>
    public string? Notes { get; set; }
}
