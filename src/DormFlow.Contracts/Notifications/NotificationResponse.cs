namespace DormFlow.Contracts.Notifications;

/// <summary>ข้อมูลการแจ้งเตือนหนึ่งรายการ</summary>
public class NotificationResponse
{
    /// <summary>รหัสอ้างอิงของการแจ้งเตือน</summary>
    public Guid Id { get; init; }

    /// <summary>ผู้ใช้ที่เป็นผู้รับ</summary>
    public Guid RecipientUserId { get; init; }

    /// <summary>หัวข้อการแจ้งเตือน</summary>
    public string Title { get; init; } = string.Empty;

    /// <summary>เนื้อหาการแจ้งเตือน</summary>
    public string Message { get; init; } = string.Empty;

    /// <summary>อ่านแล้วหรือยัง</summary>
    public bool IsRead { get; init; }

    /// <summary>เวลาที่อ่าน</summary>
    public DateTimeOffset? ReadAt { get; init; }

    /// <summary>เวลาที่สร้างข้อมูล</summary>
    public DateTimeOffset CreatedAt { get; init; }
}
