using DormFlow.Contracts.Common;

namespace DormFlow.Contracts.Invoices;

/// <summary>ຕົວກໍານົດການຊອກຫາເສັ້ນໃບເກັບເງິນ</summary>
public class InvoiceListRequest : PagedQueryRequest
{
    /// <summary>ກັ່ນຕອງສະເພາະໃບແຈ້ງໜີ້ສຳລັບຫໍພັກທີ່ລະບຸ</summary>
    public Guid? DormitoryId { get; set; }

    /// <summary>ກັ່ນຕອງພຽງແຕ່ໃນໃບເກັບເງິນສໍາລັບການເຊົ່າທີ່ລະບຸໄວ້</summary>
    public Guid? ContractId { get; set; }

    /// <summary>ກັ່ນຕອງພຽງແຕ່ໃບເກັບເງິນທີ່ມີສະຖານະກົງກັນກັບທີ່ລະບຸໄວ້</summary>
    public string? Status { get; set; }

    /// <summary>ກັ່ນຕອງສະເພາະໃບແຈ້ງໜີ້ສຳລັບເດືອນສະເພາະ</summary>
    public DateOnly? PeriodMonth { get; set; }
}
