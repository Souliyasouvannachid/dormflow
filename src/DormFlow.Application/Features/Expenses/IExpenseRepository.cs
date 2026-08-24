using DormFlow.Application.Common.Pagination;
using DormFlow.Contracts.Common;
using DormFlow.Domain.Entities;
using DormFlow.Domain.Enums;

namespace DormFlow.Application.Features.Expenses;

/// <summary>ตัวกรองสำหรับดึงรายการค่าใช้จ่ายแบบแบ่งหน้า</summary>
public sealed record ExpenseListFilter(
    Guid? OrganizationId,
    Guid? DormitoryId,
    ExpenseCategory? Category,
    DateOnly? ExpenseDateFrom,
    DateOnly? ExpenseDateTo,
    IReadOnlyCollection<Guid>? RestrictToDormitoryIds);

/// <summary>เข้าถึงข้อมูลค่าใช้จ่ายในฐานข้อมูล</summary>
public interface IExpenseRepository
{
    /// <summary>อ่านค่าใช้จ่ายจากรหัสอ้างอิง</summary>
    Task<Expense?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>ดึงรายการค่าใช้จ่ายแบบแบ่งหน้าตามตัวกรองที่ระบุ</summary>
    Task<PagedResult<Expense>> GetPagedAsync(
        ExpenseListFilter filter,
        PagedQuery query,
        CancellationToken cancellationToken = default);

    /// <summary>เพิ่มค่าใช้จ่ายใหม่</summary>
    Task AddAsync(Expense expense, CancellationToken cancellationToken = default);

    /// <summary>บันทึกการเปลี่ยนแปลงทั้งหมดลงฐานข้อมูล</summary>
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
