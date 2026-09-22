using DormFlow.Application.Common.Pagination;
using DormFlow.Contracts.Common;
using DormFlow.Domain.Entities;
using DormFlow.Domain.Enums;

namespace DormFlow.Application.Features.Parcels;

/// <summary>ການກັ່ນຕອງສໍາລັບການດຶງບັນຊີລາຍຊື່ paginated ຂອງ parcels</summary>
public sealed record ParcelListFilter(
    Guid? OrganizationId,
    Guid? DormitoryId,
    Guid? RoomId,
    ParcelStatus? Status,
    IReadOnlyCollection<Guid>? RestrictToDormitoryIds);

/// <summary>ເຂົ້າເຖິງຂໍ້ມູນ parcel ໃນຖານຂໍ້ມູນ</summary>
public interface IParcelRepository
{
    /// <summary>ອ່ານ parcel ຈາກລະຫັດອ້າງອີງ</summary>
    Task<Parcel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>ດຶງເອົາບັນຊີລາຍຊື່ຂອງ parcels ທີ່ມີ paginated ໂດຍອີງໃສ່ການກັ່ນຕອງທີ່ລະບຸໄວ້</summary>
    Task<PagedResult<Parcel>> GetPagedAsync(
        ParcelListFilter filter,
        PagedQuery query,
        CancellationToken cancellationToken = default);

    /// <summary>ນັບ parcel ຕາມຕົວກອງໂດຍບໍ່ດຶງລາຍການ</summary>
    Task<int> CountAsync(ParcelListFilter filter, CancellationToken cancellationToken = default);

    /// <summary>ເພີ່ມ parcel ໃຝ່</summary>
    Task AddAsync(Parcel parcel, CancellationToken cancellationToken = default);

    /// <summary>ບັນທຶກການປ່ຽນແປງທັງໝົດໃສ່ຖານຂໍ້ມູນ</summary>
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
