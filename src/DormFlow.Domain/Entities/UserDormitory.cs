namespace DormFlow.Domain.Entities;

/// <summary>
/// การมอบหมายผู้ใช้ให้ดูแลหอพักหนึ่งแห่ง
/// </summary>
/// <remarks>
/// ผู้ใช้หนึ่งคนดูแลได้หลายหอพัก และหอพักหนึ่งแห่งมีผู้ดูแลได้หลายคน
/// ใช้ Id ของตัวเองแทน Composite Key เพื่อให้มีคอลัมน์ assigned_at และ assigned_by ต่อการมอบหมายแต่ละครั้งได้ชัดเจน
/// ไม่ใช้ Soft Delete เพราะการถอนสิทธิ์ดูแลหอพักควรลบแถวออกจริง
/// </remarks>
public class UserDormitory
{
    /// <summary>รหัสอ้างอิงของการมอบหมาย</summary>
    public Guid Id { get; init; } = Guid.CreateVersion7();

    /// <summary>ผู้ใช้ที่ได้รับมอบหมาย</summary>
    public Guid UserId { get; set; }

    /// <summary>หอพักที่ได้รับมอบหมาย</summary>
    public Guid DormitoryId { get; set; }

    /// <summary>เป็นหอพักหลักของผู้ใช้รายนี้หรือไม่</summary>
    public bool IsPrimary { get; set; }

    /// <summary>เวลาที่มอบหมาย</summary>
    public DateTimeOffset AssignedAt { get; set; }

    /// <summary>ผู้มอบหมาย</summary>
    public Guid? AssignedBy { get; set; }
}
