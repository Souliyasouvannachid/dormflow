using DormFlow.Domain.Entities;
using DormFlow.Domain.Enums;

namespace DormFlow.Application.Common.Abstractions;

/// <summary>บันทึกเหตุการณ์ลง Audit Log</summary>
public interface IAuditLogWriter
{
    /// <summary>บันทึกเหตุการณ์ที่เตรียมไว้แล้ว</summary>
    /// <param name="auditLog">เหตุการณ์ที่ต้องการบันทึก</param>
    /// <param name="cancellationToken">โทเคนสำหรับยกเลิกคำสั่ง</param>
    Task WriteAsync(AuditLog auditLog, CancellationToken cancellationToken = default);

    /// <summary>
    /// บันทึกเหตุการณ์โดยเติมข้อมูลบริบทของคำขอปัจจุบันให้อัตโนมัติ
    /// ได้แก่ ผู้ใช้ องค์กร หมายเลข IP, User Agent, เส้นทางของคำขอ และเวลา
    /// </summary>
    /// <param name="action">ประเภทของเหตุการณ์</param>
    /// <param name="entityName">ชื่อ Entity ที่ถูกกระทำ</param>
    /// <param name="entityId">รหัสของ Entity ที่ถูกกระทำ</param>
    /// <param name="userId">ผู้ใช้ที่ก่อให้เกิดเหตุการณ์ ระบุเมื่อยังไม่มีข้อมูลใน Claim เช่น ตอนเข้าสู่ระบบ</param>
    /// <param name="organizationId">องค์กรที่เกี่ยวข้อง ระบุเมื่อยังไม่มีข้อมูลใน Claim</param>
    /// <param name="oldValues">ค่าเดิมก่อนแก้ไขในรูปแบบ JSON</param>
    /// <param name="newValues">ค่าใหม่หลังแก้ไขในรูปแบบ JSON</param>
    /// <param name="cancellationToken">โทเคนสำหรับยกเลิกคำสั่ง</param>
    Task WriteAsync(
        AuditAction action,
        string entityName,
        string? entityId = null,
        Guid? userId = null,
        Guid? organizationId = null,
        string? oldValues = null,
        string? newValues = null,
        CancellationToken cancellationToken = default);
}
