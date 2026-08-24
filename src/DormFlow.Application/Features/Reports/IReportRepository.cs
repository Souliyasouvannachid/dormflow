using DormFlow.Application.Common.Pagination;
using DormFlow.Contracts.Common;

namespace DormFlow.Application.Features.Reports;

/// <summary>ตัวกรองของรายงานสรุปรายได้</summary>
public sealed record RevenueReportFilter(
    Guid? OrganizationId,
    Guid? DormitoryId,
    DateOnly PeriodFrom,
    DateOnly PeriodTo,
    IReadOnlyCollection<Guid>? RestrictToDormitoryIds);

/// <summary>ผลสรุปดิบของรายงานรายได้ ก่อนแปลงเป็น Response</summary>
public sealed record RevenueSummary(
    int InvoiceCount,
    int PaidInvoiceCount,
    int OverdueInvoiceCount,
    decimal TotalInvoiced,
    decimal TotalCollected);

/// <summary>ตัวกรองของรายงานใบแจ้งหนี้ค้างชำระ</summary>
public sealed record OutstandingInvoicesFilter(
    Guid? OrganizationId,
    Guid? DormitoryId,
    IReadOnlyCollection<Guid>? RestrictToDormitoryIds);

/// <summary>ข้อมูลดิบหนึ่งแถวของรายงานใบแจ้งหนี้ค้างชำระ ก่อนคำนวณยอดคงเหลือ/จำนวนวันเกินกำหนด</summary>
public sealed record OutstandingInvoiceRow(
    Guid InvoiceId,
    string InvoiceNumber,
    Guid ContractId,
    Guid? DormitoryId,
    DateOnly DueDate,
    decimal TotalAmount,
    decimal PaidAmount);

/// <summary>เข้าถึงข้อมูลสำหรับสร้างรายงาน โดยรวมข้อมูลจากหลาย Entity</summary>
public interface IReportRepository
{
    /// <summary>สรุปยอดรายได้ตามตัวกรองที่ระบุ</summary>
    /// <param name="filter">ตัวกรอง</param>
    /// <param name="asOfDate">วันที่ใช้อ้างอิงคำนวณจำนวนใบแจ้งหนี้ที่เกินกำหนดชำระ</param>
    /// <param name="cancellationToken">โทเคนสำหรับยกเลิกคำสั่ง</param>
    Task<RevenueSummary> GetRevenueSummaryAsync(
        RevenueReportFilter filter,
        DateOnly asOfDate,
        CancellationToken cancellationToken = default);

    /// <summary>ดึงรายการใบแจ้งหนี้ค้างชำระแบบแบ่งหน้า เรียงตามวันครบกำหนดชำระ (ใกล้ครบกำหนดที่สุดก่อน)</summary>
    Task<PagedResult<OutstandingInvoiceRow>> GetOutstandingInvoicesAsync(
        OutstandingInvoicesFilter filter,
        PagedQuery query,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// ดึงรายการใบแจ้งหนี้ค้างชำระทั้งหมดตามตัวกรอง ไม่แบ่งหน้า ใช้สำหรับส่งออกไฟล์
    /// จำกัดจำนวนแถวสูงสุดไว้ในระดับ Infrastructure เพื่อป้องกันการดึงข้อมูลจำนวนมากเกินไปในคำขอเดียว
    /// </summary>
    Task<IReadOnlyList<OutstandingInvoiceRow>> GetOutstandingInvoicesForExportAsync(
        OutstandingInvoicesFilter filter,
        CancellationToken cancellationToken = default);
}
