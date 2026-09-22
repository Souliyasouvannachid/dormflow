using DormFlow.Contracts.Common;

namespace DormFlow.Contracts.Notifications;

/// <summary>ຕົວກໍານົດການຊອກຫາການແຈ້ງເຕືອນຂອງຕົນເອງ</summary>
public class NotificationListRequest : PagedQueryRequest
{
    /// <summary>ກັ່ນຕອງສະເພາະລາຍການທີ່ໄດ້ອ່ານແລ້ວ/ຍັງບໍ່ໄດ້ອ່ານດັ່ງທີ່ໄດ້ກ່າວ</summary>
    public bool? IsRead { get; set; }
}
