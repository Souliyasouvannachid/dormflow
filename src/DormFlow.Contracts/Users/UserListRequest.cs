using DormFlow.Contracts.Common;

namespace DormFlow.Contracts.Users;

/// <summary>ຕົວກໍານົດການຊອກຫາລາຍຊື່ຜູ້ໃຊ້</summary>
public class UserListRequest : PagedQueryRequest
{
    /// <summary>
    /// ການກັ່ນຕອງພຽງແຕ່ສໍາລັບຜູ້ໃຊ້ຂອງອົງກອນທີ່ລະບຸ. ມີປະສິດພາບພຽງແຕ່ຖ້າຜູ້ໂທແມ່ນ SuperAdmin
    /// ເຫັນໄດ້ສະເໝີຕໍ່ກັບຜູ້ໃຊ້ອື່ນສະເພາະຜູ້ໃຊ້ໃນອົງກອນຂອງຕົນເອງວ່າຄ່ານີ້ຖືກລະບຸຫຼືບໍ່
    /// </summary>
    public Guid? OrganizationId { get; set; }

    /// <summary>ການກັ່ນຕອງສະເພາະຜູ້ໃຊ້ທີ່ຖືກມອບຝາຍໃຫ້ຫໍພັກທີ່ລະບຸໄວ້</summary>
    public Guid? DormitoryId { get; set; }

    /// <summary>ກັ່ນຕອງສະເພາະຜູ້ໃຊ້ທີ່ສະຖານະກົງກັບທີ່ລະບຸໄວ້</summary>
    public string? Status { get; set; }
}
