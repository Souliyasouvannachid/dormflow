namespace DormFlow.Contracts.Reports;

/// <summary>ผลสรุปรายได้ในช่วงเวลาที่ระบุ</summary>
public class RevenueReportResponse
{
    /// <summary>เดือนเริ่มต้นของช่วงที่สรุป</summary>
    public DateOnly PeriodFrom { get; init; }

    /// <summary>เดือนสิ้นสุดของช่วงที่สรุป</summary>
    public DateOnly PeriodTo { get; init; }

    /// <summary>จำนวนใบแจ้งหนี้ทั้งหมดในช่วง (ไม่นับที่ถูกยกเลิก)</summary>
    public int InvoiceCount { get; init; }

    /// <summary>จำนวนใบแจ้งหนี้ที่ชำระครบแล้ว</summary>
    public int PaidInvoiceCount { get; init; }

    /// <summary>
    /// จำนวนใบแจ้งหนี้ที่เกินกำหนดชำระ ณ เวลาที่ขอรายงาน (คำนวณสด จากวันครบกำหนดเทียบกับวันนี้
    /// เพราะระบบยังไม่มี Background Job ที่ตั้งสถานะ Overdue ให้อัตโนมัติ)
    /// </summary>
    public int OverdueInvoiceCount { get; init; }

    /// <summary>ยอดที่ออกใบแจ้งหนี้รวมทั้งหมดในช่วง</summary>
    public decimal TotalInvoiced { get; init; }

    /// <summary>ยอดที่เก็บเงินได้จริงรวมทั้งหมดในช่วง</summary>
    public decimal TotalCollected { get; init; }

    /// <summary>ยอดค้างชำระรวม (TotalInvoiced - TotalCollected)</summary>
    public decimal TotalOutstanding { get; init; }
}
