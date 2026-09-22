using DormFlow.Contracts.Common;

namespace DormFlow.Contracts.Leases;

/// <summary>ເຊົ່າລາຍຊື່ຕົວກໍານົດການຊອກຫາ</summary>
public class ContractListRequest : PagedQueryRequest
{
    /// <summary>ກັ່ນຕອງພຽງແຕ່ສັນຍາຂອງຫໍພັກທີ່ລະບຸໄວ້</summary>
    public Guid? DormitoryId { get; set; }

    /// <summary>ການກັ່ນຕອງພຽງແຕ່ສັນຍາຂອງຫ້ອງທີ່ກໍານົດໄວ້</summary>
    public Guid? RoomId { get; set; }

    /// <summary>ກັ່ນຕອງພຽງແຕ່ສັນຍາຂອງຜູ້ເຊົ່າທີ່ລະບຸໄວ້</summary>
    public Guid? TenantId { get; set; }

    /// <summary>ກັ່ນຕອງພຽງແຕ່ການເຮັດສັນຍາທີ່ມີສະຖານະທີ່ກົງກັບທີ່ລະບຸໄວ້</summary>
    public string? Status { get; set; }
}
