namespace DormFlow.Contracts.Announcements;

/// <summary>คำขอสร้างประกาศใหม่ (สร้างเป็นสถานะร่างเสมอ)</summary>
public class CreateAnnouncementRequest
{
    /// <summary>หอพักที่ประกาศนี้เกี่ยวข้อง</summary>
    public Guid DormitoryId { get; set; }

    /// <summary>หัวข้อประกาศ</summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>เนื้อหาประกาศ</summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>เวลาที่ประกาศหมดอายุ ไม่ระบุได้</summary>
    public DateTimeOffset? ExpiresAt { get; set; }

    /// <summary>หมายเหตุเพิ่มเติม</summary>
    public string? Notes { get; set; }
}
