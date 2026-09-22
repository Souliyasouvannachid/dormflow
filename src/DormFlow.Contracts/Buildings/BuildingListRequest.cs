using DormFlow.Contracts.Common;

namespace DormFlow.Contracts.Buildings;

/// <summary>ການສ້າງຕົວກໍານົດການຊອກຫາລາຍຊື່</summary>
public class BuildingListRequest : PagedQueryRequest
{
    /// <summary>ການກັ່ນຕອງພຽງແຕ່ອາຄານຂອງຫໍພັກທີ່ລະບຸໄວ້</summary>
    public Guid? DormitoryId { get; set; }

    /// <summary>ການກັ່ນຕອງສະເພາະອາຄານທີ່ສະຖານະກົງກັບທີ່ລະບຸໄວ້</summary>
    public string? Status { get; set; }
}
