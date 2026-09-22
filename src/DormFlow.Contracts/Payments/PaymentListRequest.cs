using DormFlow.Contracts.Common;

namespace DormFlow.Contracts.Payments;

/// <summary>ຕົວກໍານົດການຊອກຫາລາຍການການຈ່າຍເງິນ</summary>
public class PaymentListRequest : PagedQueryRequest
{
    /// <summary>ການກັ່ນຕອງພຽງແຕ່ບັນຊີລາຍຊື່ຂອງຫໍພັກທີ່ລະບຸໄວ້</summary>
    public Guid? DormitoryId { get; set; }

    /// <summary>ກັ່ນຕອງສະເພາະລາຍການໃບແຈ້ງໜີ້ທີ່ລະບຸໄວ້ເທົ່ານັ້ນ</summary>
    public Guid? InvoiceId { get; set; }

    /// <summary>ກັ່ນຕອງສະເພາະລາຍການທີ່ມີສະຖານະກົງກັບອັນທີ່ລະບຸໄວ້</summary>
    public string? Status { get; set; }
}
