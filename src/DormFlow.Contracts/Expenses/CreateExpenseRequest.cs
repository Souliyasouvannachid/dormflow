namespace DormFlow.Contracts.Expenses;

/// <summary>คำขอบันทึกค่าใช้จ่าย</summary>
public class CreateExpenseRequest
{
    /// <summary>หอพักที่เกี่ยวข้องกับค่าใช้จ่ายนี้</summary>
    public Guid DormitoryId { get; set; }

    /// <summary>หมวดหมู่ของค่าใช้จ่าย ("Utilities", "Maintenance", "Salaries", "Supplies", "Marketing", "Other")</summary>
    public string Category { get; set; } = string.Empty;

    /// <summary>จำนวนเงิน</summary>
    public decimal Amount { get; set; }

    /// <summary>วันที่จ่ายค่าใช้จ่าย</summary>
    public DateOnly ExpenseDate { get; set; }

    /// <summary>คำอธิบายรายการ</summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>ผู้รับเงิน/ร้านค้า/ผู้ให้บริการ</summary>
    public string? PaidTo { get; set; }

    /// <summary>หมายเหตุเพิ่มเติม</summary>
    public string? Notes { get; set; }
}
