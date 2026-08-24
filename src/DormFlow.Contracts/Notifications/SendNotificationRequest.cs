namespace DormFlow.Contracts.Notifications;

/// <summary>คำขอส่งการแจ้งเตือนถึงผู้ใช้หนึ่งคน</summary>
public class SendNotificationRequest
{
    /// <summary>ผู้ใช้ที่เป็นผู้รับ</summary>
    public Guid RecipientUserId { get; set; }

    /// <summary>หัวข้อการแจ้งเตือน</summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>เนื้อหาการแจ้งเตือน</summary>
    public string Message { get; set; } = string.Empty;
}
