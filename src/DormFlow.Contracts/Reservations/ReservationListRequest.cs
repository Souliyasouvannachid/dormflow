using DormFlow.Contracts.Common;

namespace DormFlow.Contracts.Reservations;

/// <summary>ຕົວກໍານົດການການຊອກຫາການຈອງ</summary>
public class ReservationListRequest : PagedQueryRequest
{
    /// <summary>ກັ່ນຕອງພຽງແຕ່ການຈອງຂອງຫໍພັກທີ່ລະບຸໄວ້</summary>
    public Guid? DormitoryId { get; set; }

    /// <summary>ກັ່ນຕອງສະເພາະການຈອງຫ້ອງທີ່ລະບຸໄວ້</summary>
    public Guid? RoomId { get; set; }

    /// <summary>ກັ່ນຕອງສະເພາະການຈອງຂອງຜູ້ເຊົ່າທີ່ລະບຸໄວ້ເທົ່ານັ້ນ</summary>
    public Guid? TenantId { get; set; }

    /// <summary>ກັ່ນຕອງສະເພາະການຈອງທີ່ສະຖານະກົງກັບທີ່ລະບຸໄວ້</summary>
    public string? Status { get; set; }
}
