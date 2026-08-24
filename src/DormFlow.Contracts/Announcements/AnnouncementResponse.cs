namespace DormFlow.Contracts.Announcements;

/// <summary>ข้อมูลประกาศหนึ่งฉบับ</summary>
public class AnnouncementResponse
{
    /// <summary>รหัสอ้างอิงของประกาศ</summary>
    public Guid Id { get; init; }

    /// <summary>องค์กรที่เกี่ยวข้อง</summary>
    public Guid OrganizationId { get; init; }

    /// <summary>หอพักที่เกี่ยวข้อง</summary>
    public Guid DormitoryId { get; init; }

    /// <summary>หัวข้อประกาศ</summary>
    public string Title { get; init; } = string.Empty;

    /// <summary>เนื้อหาประกาศ</summary>
    public string Content { get; init; } = string.Empty;

    /// <summary>สถานะของประกาศ</summary>
    public string Status { get; init; } = string.Empty;

    /// <summary>เวลาที่เผยแพร่</summary>
    public DateTimeOffset? PublishedAt { get; init; }

    /// <summary>เวลาที่ประกาศหมดอายุ</summary>
    public DateTimeOffset? ExpiresAt { get; init; }

    /// <summary>หมายเหตุเพิ่มเติม</summary>
    public string? Notes { get; init; }

    /// <summary>เวลาที่สร้างข้อมูล</summary>
    public DateTimeOffset CreatedAt { get; init; }
}
