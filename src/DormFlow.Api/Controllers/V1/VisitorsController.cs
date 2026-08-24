using Asp.Versioning;
using DormFlow.Api.Authorization;
using DormFlow.Application.Common.Authorization;
using DormFlow.Application.Features.Visitors;
using DormFlow.Contracts.Common;
using DormFlow.Contracts.Visitors;
using Microsoft.AspNetCore.Mvc;

namespace DormFlow.Api.Controllers.V1;

/// <summary>Endpoint สำหรับจัดการผู้มาเยือน</summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/visitors")]
[Produces("application/json")]
public sealed class VisitorsController(IVisitorService visitorService) : ControllerBase
{
    private readonly IVisitorService _visitorService = visitorService;

    /// <summary>ดึงรายการผู้มาเยือนแบบแบ่งหน้า</summary>
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

    /// <summary>อ่านข้อมูลผู้มาเยือนหนึ่งรายการ</summary>
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

    /// <summary>บันทึกการเข้าของผู้มาเยือนใหม่</summary>
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

    /// <summary>บันทึกการออกของผู้มาเยือน</summary>
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
