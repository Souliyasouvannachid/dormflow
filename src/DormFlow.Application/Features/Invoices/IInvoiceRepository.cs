using DormFlow.Application.Common.Pagination;
using DormFlow.Contracts.Common;
using DormFlow.Domain.Entities;
using DormFlow.Domain.Enums;

namespace DormFlow.Application.Features.Invoices;

/// <summary>ตัวกรองสำหรับดึงรายการใบแจ้งหนี้แบบแบ่งหน้า</summary>
public sealed record InvoiceListFilter(
    Guid? OrganizationId,
    Guid? DormitoryId,
    Guid? ContractId,
    InvoiceStatus? Status,
    DateOnly? PeriodMonth,
    IReadOnlyCollection<Guid>? RestrictToDormitoryIds);

/// <summary>เข้าถึงข้อมูลใบแจ้งหนี้ในฐานข้อมูล</summary>
public interface IInvoiceRepository
{
    /// <summary>อ่านใบแจ้งหนี้จากรหัสอ้างอิง</summary>
    Task<Invoice?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>ตรวจว่ามีใบแจ้งหนี้ที่ใช้เลขที่นี้อยู่แล้วในองค์กรเดียวกันหรือไม่</summary>
    Task<bool> ExistsByInvoiceNumberAsync(
        Guid organizationId,
        string invoiceNumber,
        CancellationToken cancellationToken = default);

    /// <summary>ตรวจว่าสัญญาเช่านี้ถูกออกบิลของเดือนที่ระบุไปแล้วหรือยัง</summary>
    Task<bool> ExistsForContractPeriodAsync(
        Guid contractId,
        DateOnly periodMonth,
        CancellationToken cancellationToken = default);

    /// <summary>ดึงรายการใบแจ้งหนี้แบบแบ่งหน้าตามตัวกรองที่ระบุ</summary>
    Task<PagedResult<Invoice>> GetPagedAsync(
        InvoiceListFilter filter,
        PagedQuery query,
        CancellationToken cancellationToken = default);

    /// <summary>เพิ่มใบแจ้งหนี้ใหม่</summary>
    Task AddAsync(Invoice invoice, CancellationToken cancellationToken = default);

    /// <summary>บันทึกการเปลี่ยนแปลงทั้งหมดลงฐานข้อมูล รวมถึงการเปลี่ยนแปลงของ Entity อื่นที่ถูกติดตามอยู่ในธุรกรรมเดียวกัน</summary>
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
