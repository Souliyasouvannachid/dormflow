using Asp.Versioning;
using DormFlow.Api.Authorization;
using DormFlow.Application.Common.Authorization;
using DormFlow.Application.Features.Visitors;
using DormFlow.Contracts.Common;
using DormFlow.Contracts.Visitors;
using Microsoft.AspNetCore.Mvc;

namespace DormFlow.Api.Controllers.V1;

/// <summary>Endpoint ສໍາລັບການຄຸ້ມຄອງນັກທ່ອງທ່ຽວ</summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/visitors")]
[Produces("application/json")]
public sealed class VisitorsController(IVisitorService visitorService) : ControllerBase
{
    private readonly IVisitorService _visitorService = visitorService;

    /// <summary>ດຶງເອົາລາຍຊື່ຜູ້ເຂົ້າຊົມທີ່ມີ paginated</summary>
    [HttpGet]
    [RequirePermission(PermissionCodes.Visitors.View)]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<VisitorResponse>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<PagedResult<VisitorResponse>>>> GetList(
        [FromQuery] VisitorListRequest request,
        CancellationToken cancellationToken)
    {
        PagedResult<VisitorResponse> result = await _visitorService.GetPagedAsync(request, cancellationToken);
        return Ok(ApiResponse.Paged(result));
    }

    /// <summary>ອ່ານຂໍ້ມູນຂອງຜູ້ເຂົ້າຊົມ</summary>
    [HttpGet("{id:guid}")]
    [RequirePermission(PermissionCodes.Visitors.View)]
    [ProducesResponseType(typeof(ApiResponse<VisitorResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<VisitorResponse>>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        VisitorResponse result = await _visitorService.GetByIdAsync(id, cancellationToken);
        return Ok(ApiResponse.Ok(result, ApiMessages.Retrieved));
    }

    /// <summary>ບັນທຶກການເຂົ້າມາຂອງນັກທ່ອງທ່ຽວໃຝ່</summary>
    [HttpPost]
    [RequirePermission(PermissionCodes.Visitors.CheckIn)]
    [ProducesResponseType(typeof(ApiResponse<VisitorResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<VisitorResponse>>> CheckIn(
        [FromBody] CheckInVisitorRequest request,
        CancellationToken cancellationToken)
    {
        VisitorResponse result = await _visitorService.CheckInAsync(request, cancellationToken);
        return Ok(ApiResponse.Ok(result, ApiMessages.Created));
    }

    /// <summary>ບັນທຶກຜູ້ເຂົ້າຊົມ</summary>
    [HttpPost("{id:guid}/checkout")]
    [RequirePermission(PermissionCodes.Visitors.CheckOut)]
    [ProducesResponseType(typeof(ApiResponse<VisitorResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ApiResponse<VisitorResponse>>> CheckOut(
        Guid id,
        CancellationToken cancellationToken)
    {
        VisitorResponse result = await _visitorService.CheckOutAsync(id, cancellationToken);
        return Ok(ApiResponse.Ok(result, ApiMessages.Success));
    }
}
