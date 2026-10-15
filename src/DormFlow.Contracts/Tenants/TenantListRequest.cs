using DormFlow.Contracts.Common;

namespace DormFlow.Contracts.Tenants;

/// <summary>ຕົວກໍານົດການຊອກຫາລາຍຊື່ຜູ້ເຊົ່າ</summary>
public class TenantListRequest : PagedQueryRequest
{
    public bool CurrentOnly { get; set; }
    public Guid? DormitoryId { get; set; }
    /// <summary>ການກັ່ນຕອງພຽງແຕ່ສໍາລັບຜູ້ເຊົ່າຂອງອົງກອນທີ່ລະບຸ. ມີປະສິດພາບພຽງແຕ່ຖ້າຜູ້ໂທແມ່ນ SuperAdmin</summary>
    public Guid? OrganizationId { get; set; }

    /// <summary>ກັ່ນຕອງສະເພາະຜູ້ເຊົ່າທີ່ສະຖານະກົງກັບທີ່ລະບຸໄວ້</summary>
    public string? Status { get; set; }
}
