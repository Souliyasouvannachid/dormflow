using DormFlow.Application.Common.Pagination;
using DormFlow.Contracts.Common;
using DormFlow.Domain.Entities;
using DormFlow.Domain.Enums;

namespace DormFlow.Application.Features.Contracts;

/// <summary>ການກັ່ນຕອງສໍາລັບການດຶງບັນຊີລາຍຊື່ paginated ຂອງສັນຍາເຊົ່າ</summary>
public sealed record ContractListFilter(
    Guid? OrganizationId,
    Guid? DormitoryId,
    Guid? RoomId,
    Guid? TenantId,
    ContractStatus? Status,
    IReadOnlyCollection<Guid>? RestrictToDormitoryIds);

/// <summary>ເຂົ້າເຖິງຂໍ້ມູນການເຊົ່າໃນຖານຂໍ້ມູນ</summary>
public interface IContractRepository
{
    /// <summary>ອ່ານສັນຍາເຊົ່າຈາກລະຫັດອ້າງອີງ</summary>
    Task<Contract?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>ກວດເບິ່ງວ່າໝາຍເລກສັນຍານີ້ມີຢູ່ໃນອົງກອນດຽວກັນຫຼືບໍ່. ໃຊ້ໃນເວລາທີ່ການສຸ່ມຕົວເລກສັນຍາເພື່ອປ້ອງກັນການປະທະກັນ</summary>
    Task<bool> ExistsByContractNumberAsync(
        Guid organizationId,
        string contractNumber,
        CancellationToken cancellationToken = default);

    /// <summary>ກວດເບິ່ງວ່າຫ້ອງທີ່ກໍານົດມີສັນຍາທີ່ມີການເຄື່ອນໄຫວ (Draft ຫຼື Active) ລໍຖ້າຢູ່ຫຼືບໍ່?</summary>
    Task<bool> HasOpenContractAsync(Guid roomId, CancellationToken cancellationToken = default);

    /// <summary>ດຶງເອົາບັນຊີລາຍຊື່ຂອງສັນຍາເຊົ່າໂດຍອີງຕາມຕົວກອງທີ່ລະບຸໄວ້</summary>
    Task<PagedResult<Contract>> GetPagedAsync(
        ContractListFilter filter,
        PagedQuery query,
        CancellationToken cancellationToken = default);

    /// <summary>ເພີ່ມສັນຍາເຊົ່າໃໝ່</summary>
    Task AddAsync(Contract contract, CancellationToken cancellationToken = default);

    /// <summary>ບັນທຶກການປ່ຽນແປງທັງໝົດໃສ່ຖານຂໍ້ມູນ. ລວມທັງການປ່ຽນແປງໃນ Entity ທຸລະກໍາທີ່ຕິດຕາມອື່ນແມ່ນຢູ່ໃນທຸລະກໍາດຽວກັນ</summary>
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
