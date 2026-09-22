namespace DormFlow.Contracts.Rooms;

/// <summary>ຮ້ອງຂໍການສ້າງຫ້ອງໃຝ່</summary>
public class CreateRoomRequest
{
    /// <summary>ອາຄານທີ່ຫ້ອງນີ້ຈະຂຶ້ນກັບ</summary>
    public Guid BuildingId { get; set; }

    /// <summary>ຊັ້ນທີ່ຫ້ອງນີ້ຈະເປັນຂອງຈະຕ້ອງຢູ່ໃນອາຄານດຽວກັນ BuildingId</summary>
    public Guid FloorId { get; set; }

    /// <summary>ປະເພດຫ້ອງຕ້ອງຢູ່ໃນຫໍພັກດຽວກັນກັບອາຄານທີ່ເລືອກ</summary>
    public Guid RoomTypeId { get; set; }

    /// <summary>ຕົວເລກຫ້ອງແມ່ນເປັນເອກະລັກພາຍໃນອາຄານດຽວກັນ</summary>
    public string RoomNumber { get; set; } = string.Empty;

    /// <summary>ຄ່າເຊົ່າລາຍເດືອນຂອງຫ້ອງນີ້</summary>
    public decimal MonthlyRent { get; set; }

    /// <summary>ຝາກຫ້ອງນີ້</summary>
    public decimal DepositAmount { get; set; }

    /// <summary>ຈໍານວນຄົນສູງສຸດຂອງຫ້ອງນີ້</summary>
    public int MaximumOccupants { get; set; } = 1;

    /// <summary>ຄໍາອະທິບາຍເພີ່ມເຕີມ</summary>
    public string? Description { get; set; }

    /// <summary>ຕໍາແໜ່ງ X ຂອງຈຸດຄລິກກ່ຽວກັບແຜນການກໍ່ສ້າງ (% ຂອງຄວາມກວ້າງຂອງແຜນວາດ)</summary>
    public decimal? FloorPlanX { get; set; }

    /// <summary>ຕໍາແໜ່ງ Y ຂອງຈຸດຄລິກກ່ຽວກັບແຜນການກໍ່ສ້າງ (% ຄວາມສູງຂອງແຜນວາດ)</summary>
    public decimal? FloorPlanY { get; set; }

    /// <summary>ຄວາມກວ້າງຂອງຈຸດຄລິກໃນແຜນການກໍ່ສ້າງ (% ຂອງຄວາມກວ້າງຂອງແຜນວາດ)</summary>
    public decimal? FloorPlanWidth { get; set; }

    /// <summary>ຄວາມສູງຂອງຈຸດຄລິກກ່ຽວກັບແຜນການກໍ່ສ້າງ (% ຄວາມສູງຂອງແຜນວາດ)</summary>
    public decimal? FloorPlanHeight { get; set; }
}
