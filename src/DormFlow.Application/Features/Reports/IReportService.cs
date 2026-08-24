using DormFlow.Contracts.Common;
using DormFlow.Contracts.Reports;

namespace DormFlow.Application.Features.Reports;

/// <summary>Use Case ทั้งหมดของโมดูลรายงาน</summary>
public interface IReportService
{
    /// <summary>สรุปยอดรายได้ในช่วงเวลาที่ระบุ</summary>
    Task<RevenueReportResponse> GetRevenueSummaryAsync(
        RevenueReportRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>ส่งออกรายงานสรุปรายได้เป็นไฟล์ CSV</summary>
    Task<byte[]> ExportRevenueSummaryAsync(
        RevenueReportRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>ดึงรายการใบแจ้งหนี้ค้างชำระแบบแบ่งหน้า</summary>
    Task<PagedResult<OutstandingInvoiceItemResponse>> GetOutstandingInvoicesAsync(
        OutstandingInvoicesReportRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>ส่งออกรายงานใบแจ้งหนี้ค้างชำระเป็นไฟล์ CSV</summary>
    Task<byte[]> ExportOutstandingInvoicesAsync(
        OutstandingInvoicesReportRequest request,
        CancellationToken cancellationToken = default);
}
