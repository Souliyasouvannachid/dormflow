using DormFlow.Domain.Common;

namespace DormFlow.Domain.Entities;

/// <summary>
/// การแจ้งเตือนหนึ่งรายการที่ส่งถึงผู้ใช้ในระบบหนึ่งคน
/// </summary>
/// <remarks>
/// ไม่สืบทอดจาก <see cref="TenantEntity"/> เพราะการแจ้งเตือนผูกกับผู้ใช้ผู้รับโดยตรง (RecipientUserId)
/// ไม่ใช่ข้อมูลที่ต้องกรองด้วยหอพักแบบ List Endpoint ทั่วไป — ผู้ใช้ทุกคนเห็นได้เฉพาะการแจ้งเตือนของตนเองเสมอ
/// </remarks>
public class Notification : BaseEntity
{
    /// <summary>องค์กรของผู้รับ เป็น null เมื่อผู้รับเป็น SuperAdmin ที่ไม่ผูกกับองค์กรใด</summary>
    public Guid? OrganizationId { get; set; }

    /// <summary>ผู้ใช้ที่เป็นผู้รับการแจ้งเตือนนี้</summary>
    public Guid RecipientUserId { get; set; }

    /// <summary>หัวข้อการแจ้งเตือน</summary>
    public required string Title { get; set; }

    /// <summary>เนื้อหาการแจ้งเตือน</summary>
    public required string Message { get; set; }

    /// <summary>อ่านแล้วหรือยัง</summary>
    public bool IsRead { get; set; }

    /// <summary>เวลาที่อ่าน เป็น null เมื่อยังไม่อ่าน</summary>
    public DateTimeOffset? ReadAt { get; set; }
}
