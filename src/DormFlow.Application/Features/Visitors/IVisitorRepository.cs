using DormFlow.Application.Common.Pagination;
using DormFlow.Contracts.Common;
using DormFlow.Domain.Entities;
using DormFlow.Domain.Enums;

namespace DormFlow.Application.Features.Visitors;

/// <summary>ການກັ່ນຕອງສໍາລັບການດຶງບັນຊີລາຍຊື່ paginated ຂອງຜູ້ມາຢ້ຽມຢາມ</summary>
public sealed record VisitorListFilter(
    Guid? OrganizationId,
    Guid? DormitoryId,
    Guid? RoomId,
    VisitorStatus? Status,
    IReadOnlyCollection<Guid>? RestrictToDormitoryIds);

/// <summary>ເຂົ້າເຖິງຂໍ້ມູນຜູ້ເຂົ້າຊົມໃນຖານຂໍ້ມູນ</summary>
public interface IVisitorRepository
{
    /// <summary>ອ່ານລາຍຊື່ແຂກຈາກລະຫັດການສົ່ງຕໍ່</summary>
    Task<Visitor?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>ດຶງເອົາລາຍຊື່ຜູ້ເຂົ້າຊົມແບບ paginated ໂດຍອີງໃສ່ຕົວກອງທີ່ລະບຸ</summary>
    Task<PagedResult<Visitor>> GetPagedAsync(
        VisitorListFilter filter,
        PagedQuery query,
        CancellationToken cancellationToken = default);

    /// <summary>ນັບຜູ້ເຂົ້າຊົມຕາມຕົວກອງໂດຍບໍ່ດຶງລາຍການ</summary>
    Task<int> CountAsync(VisitorListFilter filter, CancellationToken cancellationToken = default);

    /// <summary>ເພີ່ມລາຍຊື່ແຂກໃໝ່</summary>
    Task AddAsync(Visitor visitor, CancellationToken cancellationToken = default);

    /// <summary>ບັນທຶກການປ່ຽນແປງທັງໝົດໃສ່ຖານຂໍ້ມູນ</summary>
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
