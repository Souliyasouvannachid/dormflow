using DormFlow.Contracts.Common;
using DormFlow.Contracts.Notifications;

namespace DormFlow.Application.Features.Notifications;

/// <summary>Use Case ໂມດູນການແຈ້ງເຕືອນທັງໝົດ</summary>
public interface INotificationService
{
    /// <summary>ດຶງເອົາບັນຊີລາຍຊື່ paginated ຂອງການແຈ້ງເຕືອນຂອງຜູ້ໃຊ້ໃນປະຈຸບັນ</summary>
    Task<PagedResult<NotificationResponse>> GetMyPagedAsync(
        NotificationListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>ໝາຍວ່າອ່ານໄດ້ສະເພາະການແຈ້ງເຕືອນສຳລັບຜູ້ໃຊ້ປັດຈຸບັນເທົ່ານັ້ນທີ່ສາມາດເຮັດໄດ້</summary>
    Task<NotificationResponse> MarkAsReadAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>ສົ່ງການແຈ້ງເຕືອນໃຫ້ຜູ້ໃຊ້ຄົນດຽວ</summary>
    Task<NotificationResponse> SendAsync(
        SendNotificationRequest request,
        CancellationToken cancellationToken = default);
}
