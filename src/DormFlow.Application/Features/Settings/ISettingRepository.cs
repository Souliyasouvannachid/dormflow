using DormFlow.Application.Common.Pagination;
using DormFlow.Contracts.Common;
using DormFlow.Domain.Entities;

namespace DormFlow.Application.Features.Settings;

/// <summary>ການກັ່ນຕອງສໍາລັບການດຶງບັນຊີລາຍຊື່ paginated ຂອງການຕັ້ງຄ່າ</summary>
public sealed record SettingListFilter(Guid OrganizationId);

/// <summary>ເຂົ້າເຖິງຂໍ້ມູນການຕັ້ງຄ່າໃນຖານຂໍ້ມູນ</summary>
public interface ISettingRepository
{
    /// <summary>ອ່ານການຕັ້ງຄ່າຈາກລະຫັດອ້າງອີງ</summary>
    Task<Setting?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>ອ່ານການຕັ້ງຄ່າຈາກອົງກອນ ແລະກະແຈ</summary>
    Task<Setting?> GetByKeyAsync(Guid organizationId, string key, CancellationToken cancellationToken = default);

    /// <summary>ດຶງເອົາລາຍຊື່ການຕັ້ງຄ່າແບບ paginated ໂດຍອີງໃສ່ຕົວກອງທີ່ລະບຸ</summary>
    Task<PagedResult<Setting>> GetPagedAsync(
        SettingListFilter filter,
        PagedQuery query,
        CancellationToken cancellationToken = default);

    /// <summary>ເພີ່ມການຕັ້ງຄ່າໃໝ່</summary>
    Task AddAsync(Setting setting, CancellationToken cancellationToken = default);

    /// <summary>ຝາຍລົບການຕັ້ງຄ່າ ລະບົບຈະປ່ຽນເປັນ Soft Delete ອັດຕະໂນມັດເມື່ອບັນທຶກ</summary>
    void Remove(Setting setting);

    /// <summary>ບັນທຶກການປ່ຽນແປງທັງໝົດໃສ່ຖານຂໍ້ມູນ</summary>
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
