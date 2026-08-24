namespace DormFlow.Contracts.Reports;

/// <summary>พารามิเตอร์ของรายงานสรุปรายได้</summary>
public class RevenueReportRequest
{
    /// <summary>กรองเฉพาะหอพักที่ระบุ เป็น null เมื่อต้องการทุกหอพักที่ผู้เรียกเข้าถึงได้</summary>
    public Guid? DormitoryId { get; set; }

    /// <summary>เดือนเริ่มต้นของช่วงที่ต้องการสรุป (นับจาก PeriodMonth ของใบแจ้งหนี้)</summary>
    public DateOnly PeriodFrom { get; set; }

    /// <summary>เดือนสิ้นสุดของช่วงที่ต้องการสรุป</summary>
    public DateOnly PeriodTo { get; set; }
}
