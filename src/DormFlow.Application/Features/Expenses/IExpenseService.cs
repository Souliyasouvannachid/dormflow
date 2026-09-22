using DormFlow.Contracts.Common;
using DormFlow.Contracts.Expenses;

namespace DormFlow.Application.Features.Expenses;

/// <summary>Use Case ຄ່າໃຊ້ຈ່າຍຂອງໂມດູນທັງຝົດ</summary>
public interface IExpenseService
{
    /// <summary>ດຶງເອົາບັນຊີລາຍຈ່າຍທີ່ເປັນ paginated</summary>
    Task<PagedResult<ExpenseResponse>> GetPagedAsync(
        ExpenseListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>ອ່ານຂໍ້ມູນຄ່າໃຊ້ຈ່າຍຈາກລະຫັດການສົ່ງຕໍ່</summary>
    Task<ExpenseResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>ບັນທຶກຄ່າໃຊ້ຈ່າຍໃຝ່</summary>
    Task<ExpenseResponse> CreateAsync(
        CreateExpenseRequest request,
        CancellationToken cancellationToken = default);
}
