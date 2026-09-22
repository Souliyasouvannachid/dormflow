using DormFlow.Contracts.Common;

namespace DormFlow.Contracts.Parking;

/// <summary>ຕົວກໍານົດການການຊອກຫາການລົງທະບຽນບ່ອນຈອດລົດ</summary>
public class ParkingRegistrationListRequest : PagedQueryRequest
{
    /// <summary>ການກັ່ນຕອງພຽງແຕ່ບັນຊີລາຍຊື່ຂອງຫໍພັກທີ່ລະບຸໄວ້</summary>
    public Guid? DormitoryId { get; set; }

    /// <summary>ການກັ່ນຕອງພຽງແຕ່ລາຍການສໍາລັບຜູ້ເຊົ່າທີ່ລະບຸ</summary>
    public Guid? TenantId { get; set; }

    /// <summary>ກັ່ນຕອງສະເພາະລາຍການທີ່ມີສະຖານະກົງກັບອັນທີ່ລະບຸໄວ້</summary>
    public string? Status { get; set; }
}
