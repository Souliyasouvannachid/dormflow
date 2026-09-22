using DormFlow.Contracts.Common;

namespace DormFlow.Contracts.Deposits;

/// <summary>ຕົວກໍານົດການຊອກຫາບັນຊີເງິນຝາກ</summary>
public class DepositListRequest : PagedQueryRequest
{
    /// <summary>ການກັ່ນຕອງພຽງແຕ່ບັນຊີລາຍຊື່ຂອງຫໍພັກທີ່ລະບຸໄວ້</summary>
    public Guid? DormitoryId { get; set; }

    /// <summary>ການກັ່ນຕອງພຽງແຕ່ບັນຊີລາຍຊື່ຂອງການເຊົ່າທີ່ລະບຸໄວ້</summary>
    public Guid? ContractId { get; set; }

    /// <summary>ກັ່ນຕອງສະເພາະລາຍການທີ່ມີສະຖານະກົງກັບອັນທີ່ລະບຸໄວ້</summary>
    public string? Status { get; set; }
}
