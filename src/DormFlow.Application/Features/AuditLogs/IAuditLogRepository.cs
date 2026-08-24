using DormFlow.Application.Common.Pagination;
using DormFlow.Contracts.Common;
using DormFlow.Domain.Entities;
using DormFlow.Domain.Enums;

namespace DormFlow.Application.Features.AuditLogs;

/// <summary>ตัวกรองสำหรับดึงรายการ Audit Log แบบแบ่งหน้า</summary>
public sealed record AuditLogListFilter(
    Guid? OrganizationId,
    Guid? DormitoryId,
    Guid? UserId,
    AuditAction? Action,
    string? EntityName,
    DateTimeOffset? From,
    DateTimeOffset? To,
    IReadOnlyCollection<Guid>? RestrictToDormitoryIds);

/// <summary>เข้าถึงข้อมูล Audit Log ในฐานข้อมูล</summary>
public interface IAuditLogRepository
{
    /// <summary>ดึงรายการ Audit Log แบบแบ่งหน้าตามตัวกรองที่ระบุ เรียงจากเหตุการณ์ล่าสุดไปเก่าสุด</summary>
    Task<PagedResult<AuditLog>> GetPagedAsync(
        AuditLogListFilter filter,
        PagedQuery query,
        CancellationToken cancellationToken = default);
}
