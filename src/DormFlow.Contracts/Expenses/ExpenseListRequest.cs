using DormFlow.Contracts.Common;

namespace DormFlow.Contracts.Expenses;

/// <summary>พารามิเตอร์การค้นหารายการค่าใช้จ่าย</summary>
public class ExpenseListRequest : PagedQueryRequest
{
    /// <summary>กรองเฉพาะรายการของหอพักที่ระบุ</summary>
    public Guid? DormitoryId { get; set; }

    /// <summary>กรองเฉพาะหมวดหมู่ที่ระบุ</summary>
    public string? Category { get; set; }

    /// <summary>กรองเฉพาะรายการตั้งแต่วันที่ระบุ</summary>
    public DateOnly? ExpenseDateFrom { get; set; }

    /// <summary>กรองเฉพาะรายการจนถึงวันที่ระบุ</summary>
    public DateOnly? ExpenseDateTo { get; set; }
}
