using DormFlow.Application.Common.Pagination;
using DormFlow.Contracts.Common;
using DormFlow.Domain.Entities;

namespace DormFlow.Application.Features.Receipts;

/// <summary>ตัวกรองสำหรับดึงรายการใบเสร็จรับเงินแบบแบ่งหน้า</summary>
public sealed record ReceiptListFilter(
    Guid? OrganizationId,
    Guid? DormitoryId,
    Guid? PaymentId,
    IReadOnlyCollection<Guid>? RestrictToDormitoryIds);

/// <summary>เข้าถึงข้อมูลใบเสร็จรับเงินในฐานข้อมูล</summary>
public interface IReceiptRepository
{
    /// <summary>อ่านใบเสร็จรับเงินจากรหัสอ้างอิง</summary>
    Task<Receipt?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>ตรวจว่ารายการชำระเงินนี้ถูกออกใบเสร็จไปแล้วหรือยัง</summary>
    Task<bool> ExistsByPaymentIdAsync(Guid paymentId, CancellationToken cancellationToken = default);

    /// <summary>ตรวจว่ามีใบเสร็จที่ใช้เลขที่นี้อยู่แล้วในองค์กรเดียวกันหรือไม่</summary>
    Task<bool> ExistsByReceiptNumberAsync(
        Guid organizationId,
        string receiptNumber,
        CancellationToken cancellationToken = default);

    /// <summary>ดึงรายการใบเสร็จรับเงินแบบแบ่งหน้าตามตัวกรองที่ระบุ</summary>
    Task<PagedResult<Receipt>> GetPagedAsync(
        ReceiptListFilter filter,
        PagedQuery query,
        CancellationToken cancellationToken = default);

    /// <summary>เพิ่มใบเสร็จรับเงินใหม่</summary>
    Task AddAsync(Receipt receipt, CancellationToken cancellationToken = default);

    /// <summary>บันทึกการเปลี่ยนแปลงทั้งหมดลงฐานข้อมูล</summary>
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
