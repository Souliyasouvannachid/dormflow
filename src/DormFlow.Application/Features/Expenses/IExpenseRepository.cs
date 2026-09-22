using DormFlow.Application.Common.Pagination;
using DormFlow.Contracts.Common;
using DormFlow.Domain.Entities;
using DormFlow.Domain.Enums;

namespace DormFlow.Application.Features.Expenses;

/// <summary>ການກັ່ນຕອງສໍາລັບການດຶງບັນຊີລາຍຊື່ paginated ຂອງຄ່າໃຊ້ຈ່າຍ</summary>
public sealed record ExpenseListFilter(
    Guid? OrganizationId,
    Guid? DormitoryId,
    ExpenseCategory? Category,
    DateOnly? ExpenseDateFrom,
    DateOnly? ExpenseDateTo,
    IReadOnlyCollection<Guid>? RestrictToDormitoryIds);

/// <summary>ເຂົ້າເຖິງຂໍ້ມູນຄ່າໃຊ້ຈ່າຍໃນຖານຂໍ້ມູນ</summary>
public interface IExpenseRepository
{
    /// <summary>ອ່ານຄ່າໃຊ້ຈ່າຍຈາກລະຫັດການສົ່ງຕໍ່</summary>
    Task<Expense?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>ດຶງເອົາບັນຊີລາຍຈ່າຍທີ່ມີ paginated ໂດຍອີງໃສ່ຕົວກອງທີ່ກໍານົດໄວ້</summary>
    Task<PagedResult<Expense>> GetPagedAsync(
        ExpenseListFilter filter,
        PagedQuery query,
        CancellationToken cancellationToken = default);

    /// <summary>ເພີ່ມຄ່າໃຊ້ຈ່າຍໃຝ່</summary>
    Task AddAsync(Expense expense, CancellationToken cancellationToken = default);

    /// <summary>ບັນທຶກການປ່ຽນແປງທັງໝົດໃສ່ຖານຂໍ້ມູນ</summary>
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
