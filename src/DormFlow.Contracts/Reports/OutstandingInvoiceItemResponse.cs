namespace DormFlow.Contracts.Reports;

/// <summary>ใบแจ้งหนี้ค้างชำระหนึ่งฉบับ</summary>
public class OutstandingInvoiceItemResponse
{
    /// <summary>รหัสอ้างอิงของใบแจ้งหนี้</summary>
    public Guid InvoiceId { get; init; }

    /// <summary>เลขที่ใบแจ้งหนี้</summary>
    public string InvoiceNumber { get; init; } = string.Empty;

    /// <summary>สัญญาเช่าที่เกี่ยวข้อง</summary>
    public Guid ContractId { get; init; }

    /// <summary>หอพักที่เกี่ยวข้อง</summary>
    public Guid DormitoryId { get; init; }

    /// <summary>วันที่ครบกำหนดชำระ</summary>
    public DateOnly DueDate { get; init; }

    /// <summary>ยอดรวมทั้งบิล</summary>
    public decimal TotalAmount { get; init; }

    /// <summary>ยอดที่ชำระแล้ว</summary>
    public decimal PaidAmount { get; init; }

    /// <summary>ยอดคงเหลือที่ยังต้องชำระ</summary>
    public decimal RemainingBalance { get; init; }

    /// <summary>จำนวนวันที่เกินกำหนดชำระแล้ว เป็น 0 เมื่อยังไม่ถึงกำหนด</summary>
    public int DaysOverdue { get; init; }
}
