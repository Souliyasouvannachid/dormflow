namespace DormFlow.Domain.Enums;

/// <summary>สถานะของประกาศ</summary>
public enum AnnouncementStatus
{
    /// <summary>ร่าง ยังไม่เผยแพร่</summary>
    Draft = 1,

    /// <summary>เผยแพร่แล้ว</summary>
    Published = 2,

    /// <summary>เก็บเข้าคลัง ไม่แสดงอีกต่อไป</summary>
    Archived = 3,
}
