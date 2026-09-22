namespace DormFlow.Contracts.Common;

/// <summary>
/// ພາລາມິເຕີຂອງ List Endpoint ທີ່ສາມາດກັ່ນຕອງຂໍ້ມູນຕາມອົງກອນ ແລະຫໍພັກ
/// </summary>
/// <remarks>
/// ຄ່າທີ່ສະຜອງໃຫ້ແມ່ນພຽງແຕ່ຕົວກອງເພີ່ມເຕີມ
/// ຂອບເຂດຂອງຂໍ້ມູນຕົວຈິງທີ່ເຫັນໄດ້ໂດຍຜູ້ໃຊ້ແມ່ນຖືກຄວບຄຸມໂດຍສິດຂອງຜູ້ໃຊ້ຢູ່ຂ້າງເຊີຟເວີ
/// </remarks>
public class TenantPagedQueryRequest : PagedQueryRequest
{
    /// <summary>ການກັ່ນຕອງພຽງແຕ່ຂໍ້ມູນຂອງອົງກອນທີ່ລະບຸ</summary>
    public Guid? OrganizationId { get; set; }

    /// <summary>ກັ່ນຕອງຂໍ້ມູນສະເພາະສຳລັບຫໍພັກທີ່ລະບຸໄວ້</summary>
    public Guid? DormitoryId { get; set; }
}
