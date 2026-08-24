using DormFlow.Application.Common.Pagination;
using DormFlow.Contracts.Common;
using DormFlow.Domain.Entities;
using DormFlow.Domain.Enums;

namespace DormFlow.Application.Features.Payments;

/// <summary>ตัวกรองสำหรับดึงรายการชำระเงินแบบแบ่งหน้า</summary>
public sealed record PaymentListFilter(
    Guid? DormitoryId,
    Guid? InvoiceId,
    PaymentStatus? Status,
    IReadOnlyCollection<Guid>? RestrictToDormitoryIds);

/// <summary>เข้าถึงข้อมูลการชำระเงินในฐานข้อมูล</summary>
public interface IPaymentRepository
{
    /// <summary>อ่านรายการชำระเงินจากรหัสอ้างอิง</summary>
    Task<Payment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>ดึงรายการชำระเงินแบบแบ่งหน้าตามตัวกรองที่ระบุ</summary>
    Task<PagedResult<Payment>> GetPagedAsync(
        PaymentListFilter filter,
        PagedQuery query,
        CancellationToken cancellationToken = default);

    /// <summary>เพิ่มรายการชำระเงินใหม่</summary>
    Task AddAsync(Payment payment, CancellationToken cancellationToken = default);

    /// <summary>บันทึกการเปลี่ยนแปลงทั้งหมดลงฐานข้อมูล รวมถึงยอดใบแจ้งหนี้ที่ถูกแก้ไขในธุรกรรมเดียวกัน</summary>
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
