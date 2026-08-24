using DormFlow.Contracts.Common;
using DormFlow.Contracts.Payments;

namespace DormFlow.Application.Features.Payments;

/// <summary>Use Case ทั้งหมดของโมดูลการชำระเงิน</summary>
public interface IPaymentService
{
    /// <summary>ดึงรายการชำระเงินแบบแบ่งหน้า</summary>
    Task<PagedResult<PaymentResponse>> GetPagedAsync(
        PaymentListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>อ่านข้อมูลรายการชำระเงินจากรหัสอ้างอิง</summary>
    Task<PaymentResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// บันทึกรายการชำระเงินใหม่ในสถานะรอยืนยัน ยังไม่มีผลต่อยอดใบแจ้งหนี้จนกว่าจะถูกยืนยัน
    /// </summary>
    Task<PaymentResponse> CreateAsync(
        CreatePaymentRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>ยืนยันรายการชำระเงิน นำยอดไปหักใบแจ้งหนี้และปรับสถานะใบแจ้งหนี้ตามยอดคงเหลือ</summary>
    Task<PaymentResponse> ConfirmAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>คืนเงิน ทำได้เฉพาะรายการที่ยืนยันแล้ว ยอดจะถูกหักออกจากใบแจ้งหนี้คืน</summary>
    Task<PaymentResponse> RefundAsync(
        Guid id,
        RefundPaymentRequest request,
        CancellationToken cancellationToken = default);
}
