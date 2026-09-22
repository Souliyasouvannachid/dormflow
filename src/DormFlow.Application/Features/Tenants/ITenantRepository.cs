using DormFlow.Application.Common.Pagination;
using DormFlow.Contracts.Common;
using DormFlow.Domain.Entities;
using DormFlow.Domain.Enums;

namespace DormFlow.Application.Features.Tenants;

/// <summary>ການກັ່ນຕອງສໍາລັບການດຶງບັນຊີລາຍຊື່ paginated ຂອງຜູ້ເຊົ່າ</summary>
public sealed record TenantListFilter(Guid? OrganizationId, TenantStatus? Status);

/// <summary>ເຂົ້າເຖິງຂໍ້ມູນຜູ້ເຊົ່າໃນຖານຂໍ້ມູນ</summary>
public interface ITenantRepository
{
    /// <summary>ອ່ານຜູ້ເຊົ່າຈາກລະຫັດອ້າງອີງ</summary>
    Task<Tenant?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>ກວດເບິ່ງວ່າມີຜູ້ເຊົ່າທີ່ໃຊ້ເລກບັດປະຈຳຕົວນີ້ຢູ່ໃນອົງກອນດຽວກັນຫຼືບໍ່</summary>
    Task<bool> ExistsByNationalIdAsync(
        Guid organizationId,
        string nationalId,
        Guid? excludeTenantId,
        CancellationToken cancellationToken = default);

    /// <summary>ດຶງເອົາລາຍຊື່ຜູ້ເຊົ່າແບບ paginated ໂດຍອີງໃສ່ຕົວກອງທີ່ລະບຸ</summary>
    Task<PagedResult<Tenant>> GetPagedAsync(
        TenantListFilter filter,
        PagedQuery query,
        CancellationToken cancellationToken = default);

    /// <summary>ເພີ່ມຜູ້ເຊົ່າໃໝ່</summary>
    Task AddAsync(Tenant tenant, CancellationToken cancellationToken = default);

    /// <summary>ບັນທຶກການປ່ຽນແປງທັງໝົດໃສ່ຖານຂໍ້ມູນ</summary>
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
