using DormFlow.Contracts.Common;
using DormFlow.Contracts.Expenses;

namespace DormFlow.Application.Features.Expenses;

/// <summary>Use Case ทั้งหมดของโมดูลค่าใช้จ่าย</summary>
public interface IExpenseService
{
    /// <summary>ดึงรายการค่าใช้จ่ายแบบแบ่งหน้า</summary>
    Task<PagedResult<ExpenseResponse>> GetPagedAsync(
        ExpenseListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>อ่านข้อมูลค่าใช้จ่ายจากรหัสอ้างอิง</summary>
    Task<ExpenseResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>บันทึกค่าใช้จ่ายใหม่</summary>
    Task<ExpenseResponse> CreateAsync(
        CreateExpenseRequest request,
        CancellationToken cancellationToken = default);
}
