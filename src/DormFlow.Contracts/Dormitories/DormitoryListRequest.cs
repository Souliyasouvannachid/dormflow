using DormFlow.Contracts.Common;

namespace DormFlow.Contracts.Dormitories;

/// <summary>ຕົວກໍານົດການຊອກຫາລາຍຊື່ຫໍພັກ</summary>
public class DormitoryListRequest : PagedQueryRequest
{
    /// <summary>
    /// ການກັ່ນຕອງພຽງແຕ່ຫໍພັກຂອງອົງກອນທີ່ລະບຸ. ມີປະສິດພາບພຽງແຕ່ຖ້າຜູ້ໂທແມ່ນ SuperAdmin
    /// ມີພຽງແຕ່ຫໍພັກໃນອົງກອນຂອງພວກເຂົາເທົ່ານັ້ນທີ່ເຫັນໄດ້ໂດຍຜູ້ໃຊ້ອື່ນບໍ່ວ່າຈະເປັນຄ່ານີ້ລະບຸໄວ້ຫຼືບໍ່
    /// </summary>
    public Guid? OrganizationId { get; set; }

    /// <summary>ກັ່ນຕອງສະເພາະຫໍພັກທີ່ມີສະຖານະທີ່ກົງກັນ</summary>
    public string? Status { get; set; }
}
