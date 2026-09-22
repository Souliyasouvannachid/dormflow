using DormFlow.Contracts.Common;

namespace DormFlow.Contracts.Maintenance;

/// <summary>ການແຈ້ງເຕືອນການບຳລຸງຮັກສາຕົວກໍານົດການຊອກຫາລາຍການວຽກ</summary>
public class MaintenanceRequestListRequest : PagedQueryRequest
{
    /// <summary>ການກັ່ນຕອງພຽງແຕ່ບັນຊີລາຍຊື່ຂອງຫໍພັກທີ່ລະບຸໄວ້</summary>
    public Guid? DormitoryId { get; set; }

    /// <summary>ການກັ່ນຕອງພຽງແຕ່ບັນຊີລາຍຊື່ຂອງຫ້ອງທີ່ລະບຸໄວ້</summary>
    public Guid? RoomId { get; set; }

    /// <summary>ກັ່ນຕອງສະເພາະລາຍການທີ່ມີສະຖານະກົງກັບອັນທີ່ລະບຸໄວ້</summary>
    public string? Status { get; set; }

    /// <summary>ກັ່ນຕອງສະເພາະລາຍການທີ່ມີລະດັບຄວາມສຳຄັນກົງກັບທີ່ລະບຸໄວ້</summary>
    public string? Priority { get; set; }

    /// <summary>ກັ່ນຕອງສະເພາະລາຍການທີ່ມອບໃຫ້ນັກວິຊາການສະເພາະ</summary>
    public Guid? AssignedTechnicianId { get; set; }
}
