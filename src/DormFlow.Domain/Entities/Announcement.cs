using DormFlow.Domain.Common;
using DormFlow.Domain.Enums;

namespace DormFlow.Domain.Entities;

/// <summary>
/// ประกาศของหอพักหนึ่งฉบับ
/// </summary>
/// <remarks>
/// ผูกกับหอพักเดียวเสมอ (DormitoryId บังคับ) ไม่รองรับประกาศระดับองค์กรที่แสดงพร้อมกันทุกหอพัก
/// เพราะสเปกไม่ได้ระบุรายละเอียดของโมดูลนี้ไว้เลย
/// </remarks>
public class Announcement : TenantEntity
{
    /// <summary>หัวข้อประกาศ</summary>
    public required string Title { get; set; }

    /// <summary>เนื้อหาประกาศ</summary>
    public required string Content { get; set; }

    /// <summary>สถานะของประกาศ</summary>
    public AnnouncementStatus Status { get; set; } = AnnouncementStatus.Draft;

    /// <summary>เวลาที่เผยแพร่ เป็น null เมื่อยังไม่เผยแพร่</summary>
    public DateTimeOffset? PublishedAt { get; set; }

    /// <summary>เวลาที่ประกาศหมดอายุ เป็น null เมื่อไม่กำหนด</summary>
    public DateTimeOffset? ExpiresAt { get; set; }

    /// <summary>หมายเหตุเพิ่มเติม</summary>
    public string? Notes { get; set; }
}
