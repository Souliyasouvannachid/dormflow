using DormFlow.Domain.Common;
using DormFlow.Domain.Enums;

namespace DormFlow.Domain.Entities;

/// <summary>
/// บันทึกเหตุการณ์สำคัญที่เกิดขึ้นในระบบ
/// </summary>
/// <remarks>
/// ตารางนี้เป็นแบบเขียนอย่างเดียว ไม่มีการแก้ไขและไม่มี Soft Delete
/// จึงไม่สืบทอดจาก <see cref="BaseEntity"/> เพื่อไม่ให้มีคอลัมน์ที่ไม่มีวันถูกใช้
/// และเพื่อไม่ให้ Global Query Filter มากรองข้อมูลที่ต้องตรวจสอบย้อนหลังได้เสมอ
/// </remarks>
public class AuditLog : IEntity
{
    /// <inheritdoc />
    public Guid Id { get; init; } = Guid.CreateVersion7();

    /// <summary>องค์กรที่เกี่ยวข้อง เป็น null สำหรับเหตุการณ์ระดับระบบ</summary>
    public Guid? OrganizationId { get; init; }

    /// <summary>หอพักที่เกี่ยวข้อง</summary>
    public Guid? DormitoryId { get; init; }

    /// <summary>ผู้ใช้ที่ก่อให้เกิดเหตุการณ์ เป็น null เมื่อยังไม่ผ่านการยืนยันตัวตน</summary>
    public Guid? UserId { get; init; }

    /// <summary>ประเภทของเหตุการณ์</summary>
    public required AuditAction Action { get; init; }

    /// <summary>ชื่อ Entity ที่ถูกกระทำ</summary>
    public required string EntityName { get; init; }

    /// <summary>รหัสของ Entity ที่ถูกกระทำ</summary>
    public string? EntityId { get; init; }

    /// <summary>ค่าเดิมก่อนแก้ไข เก็บเป็น jsonb</summary>
    public string? OldValues { get; init; }

    /// <summary>ค่าใหม่หลังแก้ไข เก็บเป็น jsonb</summary>
    public string? NewValues { get; init; }

    /// <summary>หมายเลข IP ต้นทาง</summary>
    public string? IpAddress { get; init; }

    /// <summary>User Agent ของคำขอ</summary>
    public string? UserAgent { get; init; }

    /// <summary>เส้นทางของคำขอที่ทำให้เกิดเหตุการณ์</summary>
    public string? RequestPath { get; init; }

    /// <summary>เวลาที่เกิดเหตุการณ์</summary>
    public DateTimeOffset CreatedAt { get; init; }
}
