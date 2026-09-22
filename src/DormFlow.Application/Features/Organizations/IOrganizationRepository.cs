using DormFlow.Application.Common.Pagination;
using DormFlow.Contracts.Common;
using DormFlow.Domain.Entities;
using DormFlow.Domain.Enums;

namespace DormFlow.Application.Features.Organizations;

/// <summary>ການກັ່ນຕອງສໍາລັບການດຶງບັນຊີລາຍຊື່ paginated ຂອງອົງກອນ</summary>
/// <param name="OrganizationId">ຈໍາກັດຜົນໄດ້ຮັບພຽງແຕ່ອົງກອນນີ້ null ເມື່ອໃດ SuperAdmin ສາມາດເຫັນໄດ້ໃນທຸກອົງກອນ</param>
/// <param name="Status">ການກັ່ນຕອງພຽງແຕ່ສໍາລັບອົງກອນທີ່ມີສະຖານະທີ່ກົງກັນຂອງ null ໃນເວລາທີ່ບໍ່ຈໍາກັດ</param>
public sealed record OrganizationListFilter(Guid? OrganizationId, OrganizationStatus? Status);

/// <summary>ເຂົ້າເຖິງຂໍ້ມູນຂອງບໍລິສັດໃນຖານຂໍ້ມູນ</summary>
public interface IOrganizationRepository
{
    /// <summary>ອ່ານອົງກອນຈາກລະຫັດການສົ່ງຕໍ່</summary>
    Task<Organization?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>ກວດເບິ່ງວ່າອົງກອນໃດນຶ່ງໃຊ້ລະຫັດນີ້ແລ້ວ</summary>
    Task<bool> ExistsByCodeAsync(
        string code,
        Guid? excludeOrganizationId,
        CancellationToken cancellationToken = default);

    /// <summary>ດຶງເອົາບັນຊີລາຍຊື່ຂອງອົງກອນ paginated ໂດຍອີງໃສ່ການກັ່ນຕອງທີ່ລະບຸໄວ້</summary>
    Task<PagedResult<Organization>> GetPagedAsync(
        OrganizationListFilter filter,
        PagedQuery query,
        CancellationToken cancellationToken = default);

    /// <summary>ເພີ່ມອົງກອນໃໝ່</summary>
    Task AddAsync(Organization organization, CancellationToken cancellationToken = default);

    /// <summary>Mark ລຶບອົງກອນ ລະບົບຈະປ່ຽນເປັນ Soft Delete ອັດຕະໂນມັດເມື່ອບັນທຶກ</summary>
    void Remove(Organization organization);

    /// <summary>ບັນທຶກການປ່ຽນແປງທັງໝົດໃສ່ຖານຂໍ້ມູນ</summary>
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
