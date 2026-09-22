using DormFlow.Contracts.Common;

namespace DormFlow.Contracts.MeterReadings;

/// <summary>ຕົວກໍານົດການຊອກຫາລາຍການມູນຄ່າແມັດ</summary>
public class MeterReadingListRequest : PagedQueryRequest
{
    /// <summary>ການກັ່ນຕອງພຽງແຕ່ບັນຊີລາຍຊື່ຂອງຫໍພັກທີ່ລະບຸໄວ້</summary>
    public Guid? DormitoryId { get; set; }

    /// <summary>ການກັ່ນຕອງພຽງແຕ່ບັນຊີລາຍຊື່ຂອງຫ້ອງທີ່ລະບຸໄວ້</summary>
    public Guid? RoomId { get; set; }

    /// <summary>ກັ່ນຕອງສະເພາະປະເພດແມັດທີ່ລະບຸ</summary>
    public string? MeterType { get; set; }

    /// <summary>ກັ່ນຕອງສະເພາະເດືອນໃບບິນທີ່ລະບຸໄວ້ເທົ່ານັ້ນ</summary>
    public DateOnly? PeriodMonth { get; set; }
}
