using DormFlow.Contracts.Common;

namespace DormFlow.Contracts.AuditLogs;

/// <summary>พารามิเตอร์การค้นหารายการ Audit Log</summary>
public class AuditLogListRequest : PagedQueryRequest
{
    /// <summary>
    /// กรองเฉพาะเหตุการณ์ขององค์กรที่ระบุ มีผลเฉพาะเมื่อผู้เรียกเป็น SuperAdmin
    /// ผู้ใช้อื่นเห็นได้เฉพาะเหตุการณ์ในองค์กรของตนเองเสมอไม่ว่าจะระบุค่านี้หรือไม่
    /// </summary>
    public Guid? OrganizationId { get; set; }

    /// <summary>กรองเฉพาะเหตุการณ์ของหอพักที่ระบุ</summary>
    public Guid? DormitoryId { get; set; }

    /// <summary>กรองเฉพาะเหตุการณ์ที่ผู้ใช้รายนี้เป็นผู้กระทำ</summary>
    public Guid? UserId { get; set; }

    /// <summary>กรองเฉพาะเหตุการณ์ประเภทที่ระบุ</summary>
    public string? Action { get; set; }

    /// <summary>กรองเฉพาะเหตุการณ์ที่เกี่ยวข้องกับ Entity ชื่อนี้</summary>
    public string? EntityName { get; set; }

    /// <summary>กรองเฉพาะเหตุการณ์ที่เกิดตั้งแต่เวลานี้เป็นต้นไป</summary>
    public DateTimeOffset? From { get; set; }

    /// <summary>กรองเฉพาะเหตุการณ์ที่เกิดก่อนหรือเท่ากับเวลานี้</summary>
    public DateTimeOffset? To { get; set; }
}
