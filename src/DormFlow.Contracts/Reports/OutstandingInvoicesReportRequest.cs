using DormFlow.Contracts.Common;

namespace DormFlow.Contracts.Reports;

/// <summary>พารามิเตอร์ของรายงานใบแจ้งหนี้ค้างชำระ</summary>
public class OutstandingInvoicesReportRequest : PagedQueryRequest
{
    /// <summary>กรองเฉพาะหอพักที่ระบุ เป็น null เมื่อต้องการทุกหอพักที่ผู้เรียกเข้าถึงได้</summary>
    public Guid? DormitoryId { get; set; }
}
