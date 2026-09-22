using DormFlow.Application.Common.Pagination;
using DormFlow.Contracts.Common;
using DormFlow.Domain.Entities;
using DormFlow.Domain.Enums;

namespace DormFlow.Application.Features.Dormitories;

/// <summary>ການກັ່ນຕອງສໍາລັບການດຶງບັນຊີລາຍຊື່ paginated ຂອງຫໍພັກ</summary>
/// <param name="OrganizationId">ການກັ່ນຕອງພຽງແຕ່ຫໍພັກຂອງອົງກອນທີ່ລະບຸໄວ້ເປັນ null ໃນເວລາທີ່ບໍ່ຈໍາກັດ</param>
/// <param name="Status">ກັ່ນຕອງສະເພາະຫໍພັກທີ່ມີສະຖານະທີ່ກົງກັນກັບ null ໃນເວລາທີ່ບໍ່ຈໍາກັດ</param>
/// <param name="RestrictToIds">
/// ຈຳກັດຜົນໄດ້ຮັບໃສ່ ID ຫໍພັກໃນລາຍຊື່ນີ້. ນຳໃຊ້ກັບຜູ້ໃຊ້ທີ່ຖືກມອບໝາຍໃຫ້ເບິ່ງແຍງຫໍພັກສະເພາະເທົ່ານັ້ນ
/// ແມ່ນ null ເມື່ອຜູ້ໂທສາມາດເຫັນທຸກຫໍພັກໃນອົງກອນຂອງພວກເຂົາ (SuperAdmin ແລະ Owner)
/// </param>
public sealed record DormitoryListFilter(
    Guid? OrganizationId,
    DormitoryStatus? Status,
    IReadOnlyCollection<Guid>? RestrictToIds);

/// <summary>
/// ເຂົ້າເຖິງຂໍ້ມູນຫໍພັກໃນຖານຂໍ້ມູນ
/// </summary>
/// <remarks>
/// ແມ່ນ Port ຢູ່ເທິງພື້ນ Infrastructure ເປັນຜູ້ໃຫ້ບໍລິການ Application ສະນັ້ນເຮັດວຽກກັບ Domain Entity
/// ໂດຍບໍ່ມີການອ້າງອີງ EF Core ໂດຍກົງ
/// </remarks>
public interface IDormitoryRepository
{
    /// <summary>ອ່ານຫໍພັກຈາກລະຫັດການສົ່ງຕໍ່</summary>
    /// <param name="id">ລະຫັດຫໍພັກ</param>
    /// <param name="cancellationToken">Token ສໍາລັບການຍົກເລີກຄໍາສັ່ງ</param>
    Task<Dormitory?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>ອ່ານຫໍພັກຫຼາຍແຫ່ງໃນ query ດຽວ ສຳລັບຄິດຄ່າປັບ invoice</summary>
    Task<IReadOnlyDictionary<Guid, Dormitory>> GetByIdsAsync(
        IReadOnlyCollection<Guid> ids, CancellationToken cancellationToken = default);

    /// <summary>ດຶງຊື່ຫໍພັກເປັນຄູ່ (ລະຫັດ, ຊື່) ອີງ ຕາມ ບັນ ຊີ ລາຍ ຊື່ ລະ ຫັດ ສໍາ ລັບ ການ ສະ ແດງ ໂດຍ ບໍ່ ມີ ການ join</summary>
    /// <param name="ids">ລະຫັດຫໍພັກທີ່ຕ້ອງການ</param>
    /// <param name="cancellationToken">Token ສໍາລັບການຍົກເລີກຄໍາສັ່ງ</param>
    Task<IReadOnlyDictionary<Guid, string>> GetNamesByIdsAsync(
        IReadOnlyCollection<Guid> ids, CancellationToken cancellationToken = default);

    /// <summary>ກວດເບິ່ງວ່າມີຫໍພັກທີ່ມີລະຫັດນີ້ຢູ່ໃນອົງກອນດຽວກັນ</summary>
    /// <param name="organizationId">ອົງກອນທີ່ຈະໄດ້ຮັບການກວດສອບ</param>
    /// <param name="code">ລະຫັດຫໍພັກທີ່ທ່ານຕ້ອງການກວດສອບ</param>
    /// <param name="excludeDormitoryId">ລະຫັດຫໍພັກທີ່ບໍ່ຈໍາເປັນຕ້ອງຖືກນັບໃຊ້ໃນເວລາທີ່ດັດແກ້ຂໍ້ມູນສໍາລັບຫໍພັກຕົ້ນສະບັບ</param>
    /// <param name="cancellationToken">Token ສໍາລັບການຍົກເລີກຄໍາສັ່ງ</param>
    Task<bool> ExistsByCodeAsync(
        Guid organizationId,
        string code,
        Guid? excludeDormitoryId,
        CancellationToken cancellationToken = default);

    /// <summary>ດຶງເອົາລາຍຊື່ຫໍພັກທີ່ຕັ້ງໄວ້ໂດຍອີງຕາມຕົວກອງທີ່ລະບຸໄວ້</summary>
    /// <param name="filter">ການກັ່ນຕອງ</param>
    /// <param name="query">ປັບຕົວກໍານົດການ pagination</param>
    /// <param name="cancellationToken">Token ສໍາລັບການຍົກເລີກຄໍາສັ່ງ</param>
    Task<PagedResult<Dormitory>> GetPagedAsync(
        DormitoryListFilter filter,
        PagedQuery query,
        CancellationToken cancellationToken = default);

    /// <summary>ເພີ່ມຫໍພັກໃໝ່</summary>
    /// <param name="dormitory">ຫໍພັກທີ່ຕ້ອງການເພີ່ມ</param>
    /// <param name="cancellationToken">Token ສໍາລັບການຍົກເລີກຄໍາສັ່ງ</param>
    Task AddAsync(Dormitory dormitory, CancellationToken cancellationToken = default);

    /// <summary>Mark ລຶບຫໍພັກ ລະບົບຈະປ່ຽນເປັນ Soft Delete ອັດຕະໂນມັດເມື່ອບັນທຶກ</summary>
    /// <param name="dormitory">ຫໍພັກທີ່ເຈົ້າຕ້ອງການລຶບ</param>
    void Remove(Dormitory dormitory);

    /// <summary>ບັນທຶກການປ່ຽນແປງທັງໝົດໃສ່ຖານຂໍ້ມູນ</summary>
    /// <param name="cancellationToken">Token ສໍາລັບການຍົກເລີກຄໍາສັ່ງ</param>
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
