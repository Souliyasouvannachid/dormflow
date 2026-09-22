namespace DormFlow.Contracts.Notifications;

/// <summary>ຂໍ້ມູນການແຈ້ງການຜຶ່ງ</summary>
public class NotificationResponse
{
    /// <summary>ລະຫັດອ້າງອີງການແຈ້ງເຕືອນ</summary>
    public Guid Id { get; init; }

    /// <summary>ຜູ້ໃຊ້ຜູ້ຮັບ</summary>
    public Guid RecipientUserId { get; init; }

    /// <summary>ຫົວຂໍ້ແຈ້ງການ</summary>
    public string Title { get; init; } = string.Empty;

    /// <summary>ເນື້ອໃນການແຈ້ງເຕືອນ</summary>
    public string Message { get; init; } = string.Empty;

    /// <summary>ເຈົ້າໄດ້ອ່ານແລ້ວບໍ?</summary>
    public bool IsRead { get; init; }

    /// <summary>ເວລາອ່ານ</summary>
    public DateTimeOffset? ReadAt { get; init; }

    /// <summary>ເວລາສ້າງຂໍ້ມູນ</summary>
    public DateTimeOffset CreatedAt { get; init; }
}
