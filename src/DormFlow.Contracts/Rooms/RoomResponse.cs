namespace DormFlow.Contracts.Rooms;

/// <summary>ຂໍ້ມູນຫ້ອງຜຶ່ງ</summary>
public class RoomResponse
{
    /// <summary>ລະຫັດອ້າງອີງຫ້ອງ</summary>
    public Guid Id { get; init; }

    /// <summary>ອົງກອນເຈົ້າຂອງຫ້ອງ</summary>
    public Guid OrganizationId { get; init; }

    /// <summary>ຫໍພັກທີ່ຫ້ອງນີ້ເປັນຂອງ</summary>
    public Guid DormitoryId { get; init; }

    /// <summary>ຊື່ຂອງຫໍພັກຫ້ອງນີ້ເປັນຂອງ</summary>
    public string DormitoryName { get; init; } = string.Empty;

    /// <summary>ອາຄານທີ່ຫ້ອງນີ້ຂຶ້ນກັບ</summary>
    public Guid BuildingId { get; init; }

    /// <summary>ຊື່ຂອງອາຄານທີ່ຫ້ອງນີ້ຂຶ້ນກັບ</summary>
    public string BuildingName { get; init; } = string.Empty;

    /// <summary>ຊັ້ນທີ່ຫ້ອງນີ້ເປັນຂອງ</summary>
    public Guid FloorId { get; init; }

    /// <summary>ຊື່ຂອງຊັ້ນທີ່ຫ້ອງນີ້ຂຶ້ນກັບ</summary>
    public string FloorName { get; init; } = string.Empty;

    /// <summary>ປະເພດຫ້ອງ</summary>
    public Guid RoomTypeId { get; init; }

    /// <summary>ຊື່ປະເພດຫ້ອງ</summary>
    public string RoomTypeName { get; init; } = string.Empty;

    /// <summary>ໝາຍເລກຫ້ອງ</summary>
    public string RoomNumber { get; init; } = string.Empty;

    /// <summary>ຄ່າເຊົ່າລາຍເດືອນຂອງຫ້ອງນີ້</summary>
    public decimal MonthlyRent { get; init; }

    /// <summary>ຝາກຫ້ອງນີ້</summary>
    public decimal DepositAmount { get; init; }

    /// <summary>ຈໍານວນຄົນສູງສຸດຂອງຫ້ອງນີ້</summary>
    public int MaximumOccupants { get; init; }

    /// <summary>ສະຖານະຫ້ອງ</summary>
    public string Status { get; init; } = string.Empty;

    /// <summary>ຄໍາອະທິບາຍເພີ່ມເຕີມ</summary>
    public string? Description { get; init; }

    /// <summary>ຕໍາແໜ່ງ X ຂອງຈຸດຄລິກກ່ຽວກັບແຜນການກໍ່ສ້າງ (% ຂອງຄວາມກວ້າງຂອງແຜນວາດ) ບໍ່ມີມູນຄ່າຖ້າຕໍາແຜ່ງບໍ່ໄດ້ຮັບການສ້າງຕັ້ງຂຶ້ນ</summary>
    public decimal? FloorPlanX { get; init; }

    /// <summary>ຕໍາແໜ່ງ Y ຂອງຈຸດຄລິກກ່ຽວກັບແຜນການກໍ່ສ້າງ (% ຄວາມສູງຂອງແຜນວາດ)</summary>
    public decimal? FloorPlanY { get; init; }

    /// <summary>ຄວາມກວ້າງຂອງຈຸດຄລິກໃນແຜນການກໍ່ສ້າງ (% ຂອງຄວາມກວ້າງຂອງແຜນວາດ)</summary>
    public decimal? FloorPlanWidth { get; init; }

    /// <summary>ຄວາມສູງຂອງຈຸດຄລິກກ່ຽວກັບແຜນການກໍ່ສ້າງ (% ຄວາມສູງຂອງແຜນວາດ)</summary>
    public decimal? FloorPlanHeight { get; init; }

    /// <summary>ເວລາສ້າງຂໍ້ມູນ</summary>
    public DateTimeOffset CreatedAt { get; init; }

    /// <summary>ເວລາຂອງການດັດແກ້ຫຼ້າສຸດ</summary>
    public DateTimeOffset? UpdatedAt { get; init; }
}
