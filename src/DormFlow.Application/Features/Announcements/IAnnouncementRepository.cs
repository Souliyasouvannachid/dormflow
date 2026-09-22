using DormFlow.Application.Common.Pagination;
using DormFlow.Contracts.Common;
using DormFlow.Domain.Entities;
using DormFlow.Domain.Enums;

namespace DormFlow.Application.Features.Announcements;

/// <summary>ການກັ່ນຕອງສໍາລັບການດຶງບັນຊີລາຍຊື່ paginated ຂອງການປະກາດ</summary>
public sealed record AnnouncementListFilter(
    Guid? OrganizationId,
    Guid? DormitoryId,
    AnnouncementStatus? Status,
    IReadOnlyCollection<Guid>? RestrictToDormitoryIds);

/// <summary>ເຂົ້າເຖິງຂໍ້ມູນການປະກາດໃນຖານຂໍ້ມູນ</summary>
public interface IAnnouncementRepository
{
    /// <summary>ອ່ານຂໍ້ມູນການປະກາດຈາກລະຫັດການສົ່ງຕໍ່</summary>
    Task<Announcement?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>ດຶງເອົາລາຍຊື່ການປະກາດແບບ paginated ໂດຍອີງໃສ່ຕົວກອງທີ່ລະບຸ</summary>
    Task<PagedResult<Announcement>> GetPagedAsync(
        AnnouncementListFilter filter,
        PagedQuery query,
        CancellationToken cancellationToken = default);

    /// <summary>ເພີ່ມການປະກາດໃໝ່</summary>
    Task AddAsync(Announcement announcement, CancellationToken cancellationToken = default);

    /// <summary>ບັນທຶກການປ່ຽນແປງທັງໝົດໃສ່ຖານຂໍ້ມູນ</summary>
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
