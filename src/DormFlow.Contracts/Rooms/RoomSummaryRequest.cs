namespace DormFlow.Contracts.Rooms;

/// <summary>ຕົວກໍານົດການສໍາລັບການສະຫຼຸບຈໍານວນຂອງຫ້ອງໂດຍສະຖານະ</summary>
public class RoomSummaryRequest
{
    /// <summary>ກັ່ນຕອງສະເພາະຫ້ອງຂອງຫໍພັກທີ່ກຳນົດໄວ້</summary>
    public Guid? DormitoryId { get; set; }

    /// <summary>ການກັ່ນຕອງພຽງແຕ່ຫ້ອງຂອງອາຄານທີ່ລະບຸໄວ້</summary>
    public Guid? BuildingId { get; set; }
}
