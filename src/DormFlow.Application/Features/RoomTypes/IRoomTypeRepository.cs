using DormFlow.Application.Common.Pagination;
using DormFlow.Contracts.Common;
using DormFlow.Domain.Entities;
using DormFlow.Domain.Enums;

namespace DormFlow.Application.Features.RoomTypes;

/// <summary>ການກັ່ນຕອງສໍາລັບການດຶງບັນຊີລາຍຊື່ paginated ຂອງປະເພດຫ້ອງ</summary>
public sealed record RoomTypeListFilter(
    Guid? OrganizationId,
    Guid? DormitoryId,
    RoomTypeStatus? Status,
    IReadOnlyCollection<Guid>? RestrictToDormitoryIds);

/// <summary>ເຂົ້າເຖິງຂໍ້ມູນປະເພດຫ້ອງໃນຖານຂໍ້ມູນ</summary>
public interface IRoomTypeRepository
{
    /// <summary>ອ່ານປະເພດຫ້ອງຈາກລະຫັດອ້າງອີງ</summary>
    Task<RoomType?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>ດຶງເອົາຊື່ປະເພດຫ້ອງເປັນຄູ່ (ລະຫັດ, ຊື່) ອີງ ຕາມ ບັນ ຊີ ລາຍ ຊື່ ລະ ຫັດ ສໍາ ລັບ ການ ສະ ແດງ ໂດຍ ບໍ່ ມີ ການ join</summary>
    Task<IReadOnlyDictionary<Guid, string>> GetNamesByIdsAsync(
        IReadOnlyCollection<Guid> ids, CancellationToken cancellationToken = default);

    /// <summary>ກວດເບິ່ງວ່າມີປະເພດຫ້ອງທີ່ມີລະຫັດນີ້ຢູ່ໃນຫໍພັກດຽວກັນ</summary>
    Task<bool> ExistsByCodeAsync(
        Guid dormitoryId,
        string code,
        Guid? excludeRoomTypeId,
        CancellationToken cancellationToken = default);

    /// <summary>ດຶງເອົາລາຍຊື່ປະເພດຫ້ອງທີ່ມີ paginated ໂດຍອີງຕາມຕົວກອງທີ່ລະບຸໄວ້</summary>
    Task<PagedResult<RoomType>> GetPagedAsync(
        RoomTypeListFilter filter,
        PagedQuery query,
        CancellationToken cancellationToken = default);

    /// <summary>ເພີ່ມປະເພດຫ້ອງໃຝ່</summary>
    Task AddAsync(RoomType roomType, CancellationToken cancellationToken = default);

    /// <summary>ໝາຍປະເພດຫ້ອງລຶບ ລະບົບຈະປ່ຽນເປັນ Soft Delete ອັດຕະໂນມັດເມື່ອບັນທຶກ</summary>
    void Remove(RoomType roomType);

    /// <summary>ບັນທຶກການປ່ຽນແປງທັງໝົດໃສ່ຖານຂໍ້ມູນ</summary>
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
