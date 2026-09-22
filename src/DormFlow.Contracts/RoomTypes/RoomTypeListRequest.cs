using DormFlow.Contracts.Common;

namespace DormFlow.Contracts.RoomTypes;

/// <summary>ຕົວກໍານົດການຊອກຫາລາຍການປະເພດຫ້ອງ</summary>
public class RoomTypeListRequest : PagedQueryRequest
{
    /// <summary>ການກັ່ນຕອງສະເພາະສໍາລັບປະເພດຫ້ອງຂອງຫໍພັກທີ່ລະບຸໄວ້</summary>
    public Guid? DormitoryId { get; set; }

    /// <summary>ກັ່ນຕອງສະເພາະຫ້ອງທີ່ມີສະຖານະກົງກັບທີ່ລະບຸໄວ້</summary>
    public string? Status { get; set; }
}
