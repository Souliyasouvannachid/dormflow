using DormFlow.Contracts.Common;

namespace DormFlow.Contracts.Settings;

/// <summary>ຕັ້ງຄ່າຕົວກໍານົດການຊອກຫາລາຍຊື່</summary>
public class SettingListRequest : PagedQueryRequest
{
    /// <summary>ອົງກອນເປົ້າຝາຍຕ້ອງການພຽງແຕ່ຖ້າຜູ້ໂທແມ່ນ SuperAdmin</summary>
    public Guid? OrganizationId { get; set; }
}
