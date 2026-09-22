using DormFlow.Application.Common.Pagination;
using DormFlow.Contracts.Common;
using DormFlow.Domain.Entities;
using DormFlow.Domain.Enums;

namespace DormFlow.Application.Features.Deposits;

/// <summary>ການກັ່ນຕອງສໍາລັບການດຶງບັນຊີເງິນຝາກ paginated</summary>
public sealed record DepositListFilter(
    Guid? OrganizationId,
    Guid? DormitoryId,
    Guid? ContractId,
    DepositStatus? Status,
    IReadOnlyCollection<Guid>? RestrictToDormitoryIds);

/// <summary>ເຂົ້າເຖິງຂໍ້ມູນເງິນຝາກໃນຖານຂໍ້ມູນ</summary>
public interface IDepositRepository
{
    /// <summary>ອ່ານເງິນຝາກຈາກລະຫັດການສົ່ງຕໍ່</summary>
    Task<Deposit?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>ກວດເບິ່ງວ່າຂໍ້ຕົກລົງການເຊົ່ານີ້ໄດ້ຮັບເງິນຝາກຫຼືບໍ່</summary>
    Task<bool> ExistsByContractIdAsync(Guid contractId, CancellationToken cancellationToken = default);

    /// <summary>ດຶງເອົາບັນຊີເງິນຝາກທີ່ມີ paginated ໂດຍອີງໃສ່ຕົວກອງທີ່ກໍານົດໄວ້</summary>
    Task<PagedResult<Deposit>> GetPagedAsync(
        DepositListFilter filter,
        PagedQuery query,
        CancellationToken cancellationToken = default);

    /// <summary>ເພີ່ມເງິນຝາກໃໝ່</summary>
    Task AddAsync(Deposit deposit, CancellationToken cancellationToken = default);

    /// <summary>ບັນທຶກການປ່ຽນແປງທັງໝົດໃສ່ຖານຂໍ້ມູນ</summary>
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
