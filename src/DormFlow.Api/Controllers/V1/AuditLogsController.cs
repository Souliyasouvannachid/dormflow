using Asp.Versioning;
using DormFlow.Api.Authorization;
using DormFlow.Application.Common.Authorization;
using DormFlow.Application.Features.AuditLogs;
using DormFlow.Contracts.AuditLogs;
using DormFlow.Contracts.Common;
using Microsoft.AspNetCore.Mvc;

namespace DormFlow.Api.Controllers.V1;

/// <summary>Endpoint ສໍາລັບການເບິ່ງ Audit Log ຂອງລະບົບ</summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/audit-logs")]
[Produces("application/json")]
[RequirePermission(PermissionCodes.Administration.ViewAuditLogs)]
public sealed class AuditLogsController(IAuditLogQueryService auditLogQueryService) : ControllerBase
{
    private readonly IAuditLogQueryService _auditLogQueryService = auditLogQueryService;

    /// <summary>ດຶງເອົາລາຍການ Audit Log Paginated, ຈັດຮຽງຈາກເຫດການຫຼ້າສຸດໄປຫາເກົ່າທີ່ສຸດຕາມຄ່າເລີ່ມຕົ້ນ</summary>
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
