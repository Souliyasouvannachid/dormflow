using DormFlow.Contracts.Common;
using DormFlow.Contracts.Invoices;

namespace DormFlow.Application.Features.Invoices;

/// <summary>Use Case ทั้งหมดของโมดูลใบแจ้งหนี้</summary>
public interface IInvoiceService
{
    /// <summary>ดึงรายการใบแจ้งหนี้แบบแบ่งหน้า</summary>
    Task<PagedResult<InvoiceResponse>> GetPagedAsync(
        InvoiceListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>อ่านข้อมูลใบแจ้งหนี้จากรหัสอ้างอิง</summary>
    Task<InvoiceResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// สร้างใบแจ้งหนี้ใหม่ในสถานะร่าง คำนวณค่าน้ำ-ไฟจากค่ามิเตอร์ของเดือนที่ระบุให้อัตโนมัติ
    /// </summary>
    Task<InvoiceResponse> CreateAsync(
        CreateInvoiceRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>ออกใบแจ้งหนี้ให้ผู้เช่า (Draft → Issued)</summary>
    Task<InvoiceResponse> IssueAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>ยกเลิกใบแจ้งหนี้ ทำได้เฉพาะเมื่อยังไม่มีการชำระเงินเข้ามา</summary>
    Task<InvoiceResponse> CancelAsync(Guid id, CancellationToken cancellationToken = default);
}
