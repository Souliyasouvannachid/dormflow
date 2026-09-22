using DormFlow.Application.Common.Pagination;
using DormFlow.Contracts.Common;
using DormFlow.Domain.Entities;
using DormFlow.Domain.Enums;

namespace DormFlow.Application.Features.Floors;

/// <summary>ການກັ່ນຕອງສໍາລັບການດຶງຂໍ້ມູນລາຍຊື່ຊັ້ນໃນ paginated</summary>
/// <param name="OrganizationId">ຈໍາກັດຜົນໄດ້ຮັບພຽງແຕ່ຊັ້ນອົງກອນນີ້ເພື່ອ null ເທົ່ານັ້ນ SuperAdmin ເທົ່ານັ້ນ</param>
/// <param name="BuildingId">ການກັ່ນຕອງພຽງແຕ່ຊັ້ນຂອງອາຄານທີ່ລະບຸໄວ້ເປັນ null ໃນເວລາທີ່ບໍ່ຈໍາກັດ</param>
/// <param name="Status">ກັ່ນຕອງພຽງແຕ່ຊັ້ນຂໍ້ມູນທີ່ມີສະຖານະທີ່ກົງກັນເປັນ null ໃນເວລາທີ່ບໍ່ຈໍາກັດ</param>
/// <param name="RestrictToDormitoryIds">ຈໍາກັດຜົນໄດ້ຮັບພຽງແຕ່ຊັ້ນຫໍພັກໃນບັນຊີລາຍຊື່ນີ້ null ໃນເວລາທີ່ບໍ່ຈໍາກັດ</param>
public sealed record FloorListFilter(
    Guid? OrganizationId,
    Guid? BuildingId,
    FloorStatus? Status,
    IReadOnlyCollection<Guid>? RestrictToDormitoryIds);

/// <summary>ເຂົ້າເຖິງຂໍ້ມູນຊັ້ນອາຄານໃນຖານຂໍ້ມູນ</summary>
public interface IFloorRepository
{
    /// <summary>ອ່ານຊັ້ນຂໍ້ມູນຈາກລະຫັດອ້າງອີງ</summary>
    Task<Floor?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// ດຶງຊື່ຫ້ອງຮຽນເປັນຄູ່ (ລະຫັດ, ຊື່) ອີງ ຕາມ ບັນ ຊີ ລາຍ ຊື່ ລະ ຫັດ ສໍາ ລັບ ການ ສະ ແດງ ໂດຍ ບໍ່ ມີ ການ join
    /// ບໍ່ມີຊັ້ນ <see cref="Floor.Name"/> ລໍາດັບຊັ້ນຈະຖືກນໍາໃຊ້ (<see cref="Floor.FloorNumber"/>) ແທນ
    /// </summary>
    Task<IReadOnlyDictionary<Guid, string>> GetNamesByIdsAsync(
        IReadOnlyCollection<Guid> ids, CancellationToken cancellationToken = default);

    /// <summary>ກວດເບິ່ງວ່າຊັ້ນທີ່ໃຊ້ລໍາດັບນີ້ມີຢູ່ໃນອາຄານດຽວກັນຫຼືບໍ່</summary>
    Task<bool> ExistsByFloorNumberAsync(
        Guid buildingId,
        int floorNumber,
        Guid? excludeFloorId,
        CancellationToken cancellationToken = default);

    /// <summary>ດຶງລາຍຊື່ຂອງຊັ້ນຂໍ້ມູນ paginated ໂດຍອີງໃສ່ຕົວກອງທີ່ລະບຸ</summary>
    Task<PagedResult<Floor>> GetPagedAsync(
        FloorListFilter filter,
        PagedQuery query,
        CancellationToken cancellationToken = default);

    /// <summary>ເພີ່ມຊັ້ນຂໍ້ມູນໃໝ່</summary>
    Task AddAsync(Floor floor, CancellationToken cancellationToken = default);

    /// <summary>Mark ລຶບ layer ລະບົບຈະປ່ຽນເປັນ Soft Delete ອັດຕະໂນມັດເມື່ອບັນທຶກ</summary>
    void Remove(Floor floor);

    /// <summary>ບັນທຶກການປ່ຽນແປງທັງໝົດໃສ່ຖານຂໍ້ມູນ</summary>
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
