using DormFlow.Contracts.Common;

namespace DormFlow.Contracts.Expenses;

/// <summary>ຕົວກໍານົດການຊອກຫາລາຍການຄ່າໃຊ້ຈ່າຍ</summary>
public class ExpenseListRequest : PagedQueryRequest
{
    /// <summary>ການກັ່ນຕອງພຽງແຕ່ບັນຊີລາຍຊື່ຂອງຫໍພັກທີ່ລະບຸໄວ້</summary>
    public Guid? DormitoryId { get; set; }

    /// <summary>ກັ່ນຕອງສະເພາະປະເພດທີ່ລະບຸ</summary>
    public string? Category { get; set; }

    /// <summary>ກັ່ນຕອງສະເພາະລາຍການຈາກວັນທີທີ່ລະບຸ</summary>
    public DateOnly? ExpenseDateFrom { get; set; }

    /// <summary>ກັ່ນຕອງສະເພາະລາຍການຈົນຮອດວັນທີທີ່ກຳນົດ</summary>
    public DateOnly? ExpenseDateTo { get; set; }
}
