using DormFlow.Contracts.Common;

namespace DormFlow.Contracts.Rooms;

/// <summary>ຕົວກໍານົດການຊອກຫາລາຍການຫ້ອງ</summary>
public class RoomListRequest : PagedQueryRequest
{
    /// <summary>ກັ່ນຕອງສະເພາະຫ້ອງຂອງຫໍພັກທີ່ກຳນົດໄວ້</summary>
    public Guid? DormitoryId { get; set; }

    /// <summary>ການກັ່ນຕອງພຽງແຕ່ຫ້ອງຂອງອາຄານທີ່ລະບຸໄວ້</summary>
    public Guid? BuildingId { get; set; }

    /// <summary>ກັ່ນຕອງສະເພາະຫ້ອງຂອງຊັ້ນທີ່ລະບຸ</summary>
    public Guid? FloorId { get; set; }

    /// <summary>ກັ່ນຕອງສະເພາະຫ້ອງຂອງປະເພດຫ້ອງທີ່ລະບຸ</summary>
    public Guid? RoomTypeId { get; set; }

    /// <summary>ກັ່ນຕອງສະເພາະຫ້ອງທີ່ມີສະຖານະດຽວກັນຕາມທີ່ລະບຸໄວ້</summary>
    public string? Status { get; set; }
}
