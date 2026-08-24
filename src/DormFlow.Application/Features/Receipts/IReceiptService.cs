using DormFlow.Contracts.Common;
using DormFlow.Contracts.Receipts;

namespace DormFlow.Application.Features.Receipts;

/// <summary>Use Case ทั้งหมดของโมดูลใบเสร็จรับเงิน</summary>
public interface IReceiptService
{
    /// <summary>ดึงรายการใบเสร็จรับเงินแบบแบ่งหน้า</summary>
    Task<PagedResult<ReceiptResponse>> GetPagedAsync(
        ReceiptListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>อ่านข้อมูลใบเสร็จรับเงินจากรหัสอ้างอิง</summary>
    Task<ReceiptResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// ออกใบเสร็จรับเงินให้กับรายการชำระเงินที่ยืนยันแล้ว รายการชำระเงินหนึ่งรายการออกได้เพียงครั้งเดียว
    /// </summary>
    Task<ReceiptResponse> IssueAsync(
        IssueReceiptRequest request,
        CancellationToken cancellationToken = default);
}
