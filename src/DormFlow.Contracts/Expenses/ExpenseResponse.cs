namespace DormFlow.Contracts.Expenses;

/// <summary>ข้อมูลค่าใช้จ่ายหนึ่งรายการ</summary>
public class ExpenseResponse
{
    /// <summary>รหัสอ้างอิงของรายการ</summary>
    public Guid Id { get; init; }

    /// <summary>องค์กรที่เกี่ยวข้อง</summary>
    public Guid OrganizationId { get; init; }

    /// <summary>หอพักที่เกี่ยวข้อง</summary>
    public Guid DormitoryId { get; init; }

    /// <summary>หมวดหมู่ของค่าใช้จ่าย</summary>
    public string Category { get; init; } = string.Empty;

    /// <summary>จำนวนเงิน</summary>
    public decimal Amount { get; init; }

    /// <summary>วันที่จ่ายค่าใช้จ่าย</summary>
    public DateOnly ExpenseDate { get; init; }

    /// <summary>คำอธิบายรายการ</summary>
    public string Description { get; init; } = string.Empty;

    /// <summary>ผู้รับเงิน/ร้านค้า/ผู้ให้บริการ</summary>
    public string? PaidTo { get; init; }

    /// <summary>หมายเหตุเพิ่มเติม</summary>
    public string? Notes { get; init; }

    /// <summary>เวลาที่สร้างข้อมูล</summary>
    public DateTimeOffset CreatedAt { get; init; }
}
