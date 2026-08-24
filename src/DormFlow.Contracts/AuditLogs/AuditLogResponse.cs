namespace DormFlow.Contracts.AuditLogs;

/// <summary>เหตุการณ์หนึ่งรายการที่ถูกบันทึกไว้ใน Audit Log</summary>
public class AuditLogResponse
{
    /// <summary>รหัสอ้างอิงของเหตุการณ์</summary>
    public Guid Id { get; init; }

    /// <summary>องค์กรที่เกี่ยวข้อง</summary>
    public Guid? OrganizationId { get; init; }

    /// <summary>หอพักที่เกี่ยวข้อง</summary>
    public Guid? DormitoryId { get; init; }

    /// <summary>ผู้ใช้ที่ก่อให้เกิดเหตุการณ์</summary>
    public Guid? UserId { get; init; }

    /// <summary>ประเภทของเหตุการณ์</summary>
    public string Action { get; init; } = string.Empty;

    /// <summary>ชื่อ Entity ที่ถูกกระทำ</summary>
    public string EntityName { get; init; } = string.Empty;

    /// <summary>รหัสของ Entity ที่ถูกกระทำ</summary>
    public string? EntityId { get; init; }

    /// <summary>ค่าเดิมก่อนแก้ไข</summary>
    public string? OldValues { get; init; }

    /// <summary>ค่าใหม่หลังแก้ไข</summary>
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
