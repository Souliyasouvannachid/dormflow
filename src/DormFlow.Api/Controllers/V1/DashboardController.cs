using Asp.Versioning;
using DormFlow.Api.Authorization;
using DormFlow.Application.Common.Authorization;
using DormFlow.Application.Features.Dashboard;
using DormFlow.Contracts.Common;
using DormFlow.Contracts.Dashboard;
using Microsoft.AspNetCore.Mvc;

namespace DormFlow.Api.Controllers.V1;

/// <summary>Endpoint ສໍາລັບພາບລວມຂອງ dashboard ໄດ້</summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/dashboard")]
[Produces("application/json")]
public sealed class DashboardController(IDashboardService dashboardService) : ControllerBase
{
    private readonly IDashboardService _dashboardService = dashboardService;

    /// <summary>ດຶງຂໍ້ມູນພາບລວມຂອງ dashboard ສໍາລັບຜູ້ໃຊ້ທີ່ເຂົ້າສູ່ລະບົບ</summary>
    /// <param name="cancellationToken">Token ສໍາລັບການຍົກເລີກຄໍາສັ່ງ</param>
    /// <response code="200">ພາບລວມຂອງ Dashboard</response>
    [HttpGet("summary")]
    [RequirePermission(PermissionCodes.Dashboard.View)]
    [ProducesResponseType(typeof(ApiResponse<DashboardSummaryResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<DashboardSummaryResponse>>> GetSummary(
        CancellationToken cancellationToken)
    {
        DashboardSummaryResponse result = await _dashboardService.GetSummaryAsync(cancellationToken);
        return Ok(ApiResponse.Ok(result, ApiMessages.Retrieved));
    }
}
