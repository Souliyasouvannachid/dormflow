using Asp.Versioning;
using DormFlow.Api.Authorization;
using DormFlow.Application.Common.Authorization;
using DormFlow.Application.Features.AuditLogs;
using DormFlow.Contracts.AuditLogs;
using DormFlow.Contracts.Common;
using Microsoft.AspNetCore.Mvc;

namespace DormFlow.Api.Controllers.V1;

/// <summary>Endpoint สำหรับดู Audit Log ของระบบ</summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/audit-logs")]
[Produces("application/json")]
[RequirePermission(PermissionCodes.Administration.ViewAuditLogs)]
public sealed class AuditLogsController(IAuditLogQueryService auditLogQueryService) : ControllerBase
{
    private readonly IAuditLogQueryService _auditLogQueryService = auditLogQueryService;

    /// <summary>ดึงรายการ Audit Log แบบแบ่งหน้า เรียงจากเหตุการณ์ล่าสุดไปเก่าสุดเป็นค่าเริ่มต้น</summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<AuditLogResponse>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<PagedResult<AuditLogResponse>>>> GetList(
        [FromQuery] AuditLogListRequest request,
        CancellationToken cancellationToken)
    {
        PagedResult<AuditLogResponse> result = await _auditLogQueryService.GetPagedAsync(request, cancellationToken);
        return Ok(ApiResponse.Paged(result));
    }
}
