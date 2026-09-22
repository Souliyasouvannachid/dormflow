using DormFlow.Contracts.Common;

namespace DormFlow.Contracts.Floors;

/// <summary>ຕົວກໍານົດການຊອກຫາລາຍຊື່ຊັ້ນ</summary>
public class FloorListRequest : PagedQueryRequest
{
    /// <summary>ການກັ່ນຕອງພຽງແຕ່ຊັ້ນຂອງອາຄານທີ່ກໍານົດໄວ້</summary>
    public Guid? BuildingId { get; set; }

    /// <summary>ກັ່ນຕອງພຽງແຕ່ຊັ້ນຂໍ້ມູນທີ່ມີສະຖານະກົງກັບອັນທີ່ລະບຸໄວ້</summary>
    public string? Status { get; set; }
}
