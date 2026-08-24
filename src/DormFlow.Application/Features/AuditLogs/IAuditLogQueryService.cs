using DormFlow.Contracts.AuditLogs;
using DormFlow.Contracts.Common;

namespace DormFlow.Application.Features.AuditLogs;

/// <summary>Use Case ทั้งหมดของโมดูล Audit Log</summary>
public interface IAuditLogQueryService
{
    /// <summary>ดึงรายการ Audit Log แบบแบ่งหน้า</summary>
    Task<PagedResult<AuditLogResponse>> GetPagedAsync(
        AuditLogListRequest request,
        CancellationToken cancellationToken = default);
}
