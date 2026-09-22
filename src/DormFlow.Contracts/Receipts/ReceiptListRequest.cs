using DormFlow.Contracts.Common;

namespace DormFlow.Contracts.Receipts;

/// <summary>ຕົວກໍານົດການຊອກຫາລາຍການຮັບ</summary>
public class ReceiptListRequest : PagedQueryRequest
{
    /// <summary>ການກັ່ນຕອງພຽງແຕ່ບັນຊີລາຍຊື່ຂອງຫໍພັກທີ່ລະບຸໄວ້</summary>
    public Guid? DormitoryId { get; set; }

    /// <summary>ກັ່ນຕອງພຽງແຕ່ໃບຮັບເງິນສໍາລັບທຸລະກໍາການຈ່າຍເງິນທີ່ລະບຸ</summary>
    public Guid? PaymentId { get; set; }
}
