using DormFlow.Application.Common.Pagination;
using DormFlow.Contracts.Common;
using DormFlow.Domain.Entities;

namespace DormFlow.Application.Features.Notifications;

/// <summary>ການກັ່ນຕອງສໍາລັບການດຶງບັນຊີລາຍຊື່ paginated ຂອງການແຈ້ງເຕືອນຈາກຜູ້ຮັບດຽວ</summary>
public sealed record NotificationListFilter(Guid RecipientUserId, bool? IsRead);

/// <summary>ເຂົ້າເຖິງຂໍ້ມູນການແຈ້ງເຕືອນໃນຖານຂໍ້ມູນ</summary>
public interface INotificationRepository
{
    /// <summary>ອ່ານການແຈ້ງເຕືອນຈາກລະຫັດການສົ່ງຕໍ່</summary>
    Task<Notification?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>ດຶງເອົາລາຍຊື່ການແຈ້ງເຕືອນແບບ paginated ສໍາລັບຜູ້ຮັບດຽວໂດຍອີງໃສ່ການກັ່ນຕອງທີ່ລະບຸໄວ້</summary>
    Task<PagedResult<Notification>> GetPagedAsync(
        NotificationListFilter filter,
        PagedQuery query,
        CancellationToken cancellationToken = default);

    /// <summary>ເພີ່ມການແຈ້ງເຕືອນໃໝ່</summary>
    Task AddAsync(Notification notification, CancellationToken cancellationToken = default);

    /// <summary>ບັນທຶກການປ່ຽນແປງທັງໝົດໃສ່ຖານຂໍ້ມູນ</summary>
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
