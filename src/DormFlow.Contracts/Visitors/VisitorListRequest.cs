using DormFlow.Contracts.Common;

namespace DormFlow.Contracts.Visitors;

/// <summary>ຕົວກໍານົດການຊອກຫາລາຍຊື່ແຂກ</summary>
public class VisitorListRequest : PagedQueryRequest
{
    /// <summary>ການກັ່ນຕອງພຽງແຕ່ບັນຊີລາຍຊື່ຂອງຫໍພັກທີ່ລະບຸໄວ້</summary>
    public Guid? DormitoryId { get; set; }

    /// <summary>ການກັ່ນຕອງພຽງແຕ່ບັນຊີລາຍຊື່ຂອງຫ້ອງທີ່ລະບຸໄວ້</summary>
    public Guid? RoomId { get; set; }

    /// <summary>ກັ່ນຕອງສະເພາະລາຍການທີ່ມີສະຖານະກົງກັບອັນທີ່ລະບຸໄວ້</summary>
    public string? Status { get; set; }
}
