using DormFlow.Contracts.AuditLogs;
using DormFlow.Contracts.Common;

namespace DormFlow.Application.Features.AuditLogs;

/// <summary>Use Case ໂມດູນທັງໝົດ Audit Log</summary>
public interface IAuditLogQueryService
{
    /// <summary>ດຶງເອົາລາຍການ Audit Log ປະເພດການແບ່ງໜ້າ</summary>
    Task<PagedResult<AuditLogResponse>> GetPagedAsync(
        AuditLogListRequest request,
        CancellationToken cancellationToken = default);
}
