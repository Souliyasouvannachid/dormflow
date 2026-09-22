using DormFlow.Application.Common.Pagination;
using DormFlow.Contracts.Common;
using DormFlow.Domain.Entities;
using DormFlow.Domain.Enums;

namespace DormFlow.Application.Features.Rooms;

/// <summary>ການກັ່ນຕອງສໍາລັບການດຶງບັນຊີລາຍຊື່ paginated ຂອງຫ້ອງ</summary>
public sealed record RoomListFilter(
    Guid? OrganizationId,
    Guid? DormitoryId,
    Guid? BuildingId,
    Guid? FloorId,
    Guid? RoomTypeId,
    RoomStatus? Status,
    IReadOnlyCollection<Guid>? RestrictToDormitoryIds);

/// <summary>ການກັ່ນຕອງສໍາລັບການສະຫຼຸບຈໍານວນຂອງຫ້ອງໂດຍສະຖານະ</summary>
public sealed record RoomSummaryFilter(
    Guid? OrganizationId,
    Guid? DormitoryId,
    Guid? BuildingId,
    IReadOnlyCollection<Guid>? RestrictToDormitoryIds);

/// <summary>ເຂົ້າເຖິງຂໍ້ມູນຫ້ອງໃນຖານຂໍ້ມູນ</summary>
public interface IRoomRepository
{
    /// <summary>ອ່ານຫ້ອງຈາກລະຫັດການສົ່ງຕໍ່</summary>
    Task<Room?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>ອ່ານຫ້ອງຫຼາຍຫ້ອງໃນ query ດຽວ ສຳລັບ background job</summary>
    Task<IReadOnlyDictionary<Guid, Room>> GetByIdsAsync(
        IReadOnlyCollection<Guid> ids, CancellationToken cancellationToken = default);

    /// <summary>ກວດເບິ່ງວ່າມີຫ້ອງທີ່ມີຕົວເລກນີ້ຢູ່ໃນອາຄານດຽວກັນ</summary>
    Task<bool> ExistsByRoomNumberAsync(
        Guid buildingId,
        string roomNumber,
        Guid? excludeRoomId,
        CancellationToken cancellationToken = default);

    /// <summary>ດຶງເອົາບັນຊີລາຍຊື່ຂອງຫ້ອງ paginated ໂດຍອີງຕາມຕົວກອງທີ່ລະບຸ</summary>
    Task<PagedResult<Room>> GetPagedAsync(
        RoomListFilter filter,
        PagedQuery query,
        CancellationToken cancellationToken = default);

    /// <summary>ນັບຫ້ອງຕາມສະຖານະໂດຍອີງໃສ່ຕົວກອງທີ່ລະບຸ</summary>
    Task<IReadOnlyDictionary<RoomStatus, int>> GetStatusCountsAsync(
        RoomSummaryFilter filter,
        CancellationToken cancellationToken = default);

    /// <summary>ເພີ່ມຫ້ອງໃຝ່</summary>
    Task AddAsync(Room room, CancellationToken cancellationToken = default);

    /// <summary>ຝາຍລຶບຫ້ອງ ລະບົບຈະປ່ຽນເປັນ Soft Delete ອັດຕະໂນມັດເມື່ອບັນທຶກ</summary>
    void Remove(Room room);

    /// <summary>ບັນທຶກການປ່ຽນແປງທັງໝົດໃສ່ຖານຂໍ້ມູນ</summary>
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
