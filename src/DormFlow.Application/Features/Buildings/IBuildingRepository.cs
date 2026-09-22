using DormFlow.Application.Common.Pagination;
using DormFlow.Contracts.Common;
using DormFlow.Domain.Entities;
using DormFlow.Domain.Enums;

namespace DormFlow.Application.Features.Buildings;

/// <summary>ການກັ່ນຕອງສໍາລັບການດຶງບັນຊີລາຍຊື່ paginated ຂອງອາຄານ</summary>
/// <param name="OrganizationId">ຈໍາກັດຜົນໄດ້ຮັບພຽງແຕ່ອາຄານຂອງອົງກອນນີ້ null ເທົ່ານັ້ນ SuperAdmin ເທົ່ານັ້ນ</param>
/// <param name="DormitoryId">ກັ່ນຕອງສະເພາະອາຄານຫໍພັກທີ່ລະບຸເປັນ null ໃນເວລາທີ່ບໍ່ຈໍາກັດ</param>
/// <param name="Status">ກັ່ນຕອງສະເພາະອາຄານທີ່ມີສະຖານະທີ່ກົງກັນ null ໃນເວລາທີ່ບໍ່ຈໍາກັດ</param>
/// <param name="RestrictToDormitoryIds">ຈໍາກັດຜົນໄດ້ຮັບພຽງແຕ່ອາຄານຫໍພັກໃນບັນຊີລາຍຊື່ນີ້ null ໃນເວລາທີ່ບໍ່ຈໍາກັດ</param>
public sealed record BuildingListFilter(
    Guid? OrganizationId,
    Guid? DormitoryId,
    BuildingStatus? Status,
    IReadOnlyCollection<Guid>? RestrictToDormitoryIds);

/// <summary>ເຂົ້າເຖິງຂໍ້ມູນການກໍ່ສ້າງໃນຖານຂໍ້ມູນ</summary>
public interface IBuildingRepository
{
    /// <summary>ອ່ານການສ້າງຈາກລະຫັດອ້າງອີງ</summary>
    Task<Building?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>ດຶງຊື່ອາຄານເປັນຄູ່ (ລະຫັດ, ຊື່) ອີງ ຕາມ ບັນ ຊີ ລາຍ ຊື່ ລະ ຫັດ ສໍາ ລັບ ການ ສະ ແດງ ໂດຍ ບໍ່ ມີ ການ join</summary>
    Task<IReadOnlyDictionary<Guid, string>> GetNamesByIdsAsync(
        IReadOnlyCollection<Guid> ids, CancellationToken cancellationToken = default);

    /// <summary>ກວດເບິ່ງວ່າມີອາຄານທີ່ມີລະຫັດນີ້ຢູ່ໃນຫໍພັກດຽວກັນ</summary>
    Task<bool> ExistsByCodeAsync(
        Guid dormitoryId,
        string code,
        Guid? excludeBuildingId,
        CancellationToken cancellationToken = default);

    /// <summary>ດຶງເອົາບັນຊີລາຍຊື່ຂອງອາຄານ paginated ໂດຍອີງໃສ່ການກັ່ນຕອງທີ່ລະບຸໄວ້</summary>
    Task<PagedResult<Building>> GetPagedAsync(
        BuildingListFilter filter,
        PagedQuery query,
        CancellationToken cancellationToken = default);

    /// <summary>ເພີ່ມອາຄານໃໝ່</summary>
    Task AddAsync(Building building, CancellationToken cancellationToken = default);

    /// <summary>Mark ລຶບອາຄານ ລະບົບຈະປ່ຽນເປັນ Soft Delete ອັດຕະໂນມັດເມື່ອບັນທຶກ</summary>
    void Remove(Building building);

    /// <summary>ບັນທຶກການປ່ຽນແປງທັງໝົດໃສ່ຖານຂໍ້ມູນ</summary>
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
