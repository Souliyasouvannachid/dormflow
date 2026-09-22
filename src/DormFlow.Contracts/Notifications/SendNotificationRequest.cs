namespace DormFlow.Contracts.Notifications;

/// <summary>ຮ້ອງຂໍໃຫ້ສົ່ງການແຈ້ງເຕືອນໃຫ້ຜູ້ໃຊ້ຜຶ່ງ</summary>
public class SendNotificationRequest
{
    /// <summary>ຜູ້ໃຊ້ຜູ້ຮັບ</summary>
    public Guid RecipientUserId { get; set; }

    /// <summary>ຫົວຂໍ້ແຈ້ງການ</summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>ເນື້ອໃນການແຈ້ງເຕືອນ</summary>
    public string Message { get; set; } = string.Empty;
}
