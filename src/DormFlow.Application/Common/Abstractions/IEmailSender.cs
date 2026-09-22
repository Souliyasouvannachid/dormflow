using DormFlow.Application.Common.Models;

namespace DormFlow.Application.Common.Abstractions;

/// <summary>ສົ່ງອີເມວຈາກລະບົບ</summary>
public interface IEmailSender
{
    /// <summary>ສົ່ງອີເມວຜຶ່ງ</summary>
    /// <param name="message">ຂໍ້ຄວາມທີ່ທ່ານຕ້ອງການສົ່ງ</param>
    /// <param name="cancellationToken">Token ສໍາລັບການຍົກເລີກຄໍາສັ່ງ</param>
    Task SendAsync(EmailMessage message, CancellationToken cancellationToken = default);
}
