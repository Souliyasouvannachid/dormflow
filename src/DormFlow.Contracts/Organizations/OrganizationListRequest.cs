using DormFlow.Contracts.Common;

namespace DormFlow.Contracts.Organizations;

/// <summary>ຕົວກໍານົດການຊອກຫາລາຍຊື່ອົງກອນ</summary>
public class OrganizationListRequest : PagedQueryRequest
{
    /// <summary>ກັ່ນຕອງສະເພາະອົງກອນທີ່ມີສະຖານະກົງກັບທີ່ລະບຸໄວ້</summary>
    public string? Status { get; set; }
}
