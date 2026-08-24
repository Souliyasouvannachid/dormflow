using DormFlow.Contracts.Common;
using DormFlow.Contracts.Deposits;

namespace DormFlow.Application.Features.Deposits;

/// <summary>Use Case ทั้งหมดของโมดูลเงินมัดจำ</summary>
public interface IDepositService
{
    /// <summary>ดึงรายการเงินมัดจำแบบแบ่งหน้า</summary>
    Task<PagedResult<DepositResponse>> GetPagedAsync(
        DepositListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>อ่านข้อมูลเงินมัดจำจากรหัสอ้างอิง</summary>
    Task<DepositResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>บันทึกการรับเงินมัดจำใหม่ สัญญาเช่าหนึ่งฉบับรับได้เพียงครั้งเดียว</summary>
    Task<DepositResponse> CollectAsync(
        CollectDepositRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// คืนเงินมัดจำ ทำได้เฉพาะสัญญาเช่าที่สิ้นสุดแล้วและเงินมัดจำที่ยังไม่เคยคืน ทำได้เพียงครั้งเดียวต่อรายการ
    /// </summary>
    Task<DepositResponse> RefundAsync(
        Guid id,
        RefundDepositRequest request,
        CancellationToken cancellationToken = default);
}
