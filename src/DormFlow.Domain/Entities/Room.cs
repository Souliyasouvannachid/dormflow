using DormFlow.Domain.Common;
using DormFlow.Domain.Enums;

namespace DormFlow.Domain.Entities;

/// <summary>ຫ້ອງໜຶ່ງ</summary>
/// <remarks>
/// ເກັບກໍາ <see cref="MonthlyRent"/>, <see cref="DepositAmount"/> ແລະ <see cref="MaximumOccupants"/>
/// ແຍກອອກຈາກມູນຄ່າຂອງ <see cref="RoomType"/> ດັ່ງນັ້ນລາຄາຫ້ອງສາມາດປັບຕາມພື້ນຖານຕໍ່ຫ້ອງ
/// ຄ່າເລີ່ມຕົ້ນໃນເວລາທີ່ສ້າງຫ້ອງໃຝ່ຄວນຈະຖືກຄັດລອກຈາກປະເພດຫ້ອງທີ່ເລືອກ
/// </remarks>
public class Room : TenantEntity
{
    /// <summary>ອາຄານທີ່ຫ້ອງນີ້ຂຶ້ນກັບ</summary>
    public Guid BuildingId { get; set; }

    /// <summary>ຊັ້ນທີ່ຫ້ອງນີ້ເປັນຂອງ</summary>
    public Guid FloorId { get; set; }

    /// <summary>ປະເພດຫ້ອງ</summary>
    public Guid RoomTypeId { get; set; }

    /// <summary>ຕົວເລກຫ້ອງແມ່ນເປັນເອກະລັກພາຍໃນອາຄານດຽວກັນ</summary>
    public required string RoomNumber { get; set; }

    /// <summary>ຄ່າເຊົ່າລາຍເດືອນຂອງຫ້ອງນີ້</summary>
    public decimal MonthlyRent { get; set; }

    /// <summary>ຝາກຫ້ອງນີ້</summary>
    public decimal DepositAmount { get; set; }

    /// <summary>ຈໍານວນຄົນສູງສຸດຂອງຫ້ອງນີ້</summary>
    public int MaximumOccupants { get; set; } = 1;

    /// <summary>ສະຖານະຫ້ອງ</summary>
    public RoomStatus Status { get; set; } = RoomStatus.Available;

    /// <summary>ຄໍາອະທິບາຍເພີ່ມເຕີມ</summary>
    public string? Description { get; set; }

    /// <summary>ຕໍາແໜ່ງ X ຂອງຈຸດຄລິກກ່ຽວກັບແຜນການກໍ່ສ້າງ (% ຂອງຄວາມກວ້າງຂອງແຜນວາດ) ບໍ່ມີມູນຄ່າຖ້າຕໍາແຜ່ງບໍ່ໄດ້ຮັບການສ້າງຕັ້ງຂຶ້ນ</summary>
    public decimal? FloorPlanX { get; set; }

    /// <summary>ຕໍາແໜ່ງ Y ຂອງຈຸດຄລິກກ່ຽວກັບແຜນການກໍ່ສ້າງ (% ຄວາມສູງຂອງແຜນວາດ)</summary>
    public decimal? FloorPlanY { get; set; }

    /// <summary>ຄວາມກວ້າງຂອງຈຸດຄລິກໃນແຜນການກໍ່ສ້າງ (% ຂອງຄວາມກວ້າງຂອງແຜນວາດ)</summary>
    public decimal? FloorPlanWidth { get; set; }

    /// <summary>ຄວາມສູງຂອງຈຸດຄລິກກ່ຽວກັບແຜນການກໍ່ສ້າງ (% ຄວາມສູງຂອງແຜນວາດ)</summary>
    public decimal? FloorPlanHeight { get; set; }
}
