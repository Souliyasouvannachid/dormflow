using Asp.Versioning;
using DormFlow.Api.Authorization;
using DormFlow.Application.Common.Authorization;
using DormFlow.Application.Features.Floors;
using DormFlow.Contracts.Common;
using DormFlow.Contracts.Floors;
using DormFlow.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace DormFlow.Api.Controllers.V1;

/// <summary>Endpoint สำหรับจัดการชั้นของอาคาร</summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/floors")]
[Produces("application/json")]
public sealed class FloorsController(IFloorService floorService) : ControllerBase
{
    private readonly IFloorService _floorService = floorService;

    /// <summary>ดึงรายการชั้นแบบแบ่งหน้า</summary>
    [HttpGet]
    [RequirePermission(PermissionCodes.Floors.View)]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<FloorResponse>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<PagedResult<FloorResponse>>>> GetList(
        [FromQuery] FloorListRequest request,
        CancellationToken cancellationToken)
    {
        PagedResult<FloorResponse> result = await _floorService.GetPagedAsync(request, cancellationToken);
        return Ok(ApiResponse.Paged(result));
    }

    /// <summary>อ่านข้อมูลชั้นหนึ่งชั้น</summary>
    [HttpGet("{id:guid}")]
    [RequirePermission(PermissionCodes.Floors.View)]
    [ProducesResponseType(typeof(ApiResponse<FloorResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<FloorResponse>>> GetById(Guid id, CancellationToken cancellationToken)
    {
        FloorResponse result = await _floorService.GetByIdAsync(id, cancellationToken);
        return Ok(ApiResponse.Ok(result, ApiMessages.Retrieved));
    }

    /// <summary>สร้างชั้นใหม่</summary>
    [HttpPost]
    [RequirePermission(PermissionCodes.Floors.Create)]
    [ProducesResponseType(typeof(ApiResponse<FloorResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ApiResponse<FloorResponse>>> Create(
        [FromBody] CreateFloorRequest request,
        CancellationToken cancellationToken)
    {
        FloorResponse result = await _floorService.CreateAsync(request, cancellationToken);
        return Ok(ApiResponse.Ok(result, ApiMessages.Created));
    }

    /// <summary>แก้ไขข้อมูลชั้น</summary>
    [HttpPut("{id:guid}")]
    [RequirePermission(PermissionCodes.Floors.Update)]
    [ProducesResponseType(typeof(ApiResponse<FloorResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ApiResponse<FloorResponse>>> Update(
        Guid id,
        [FromBody] UpdateFloorRequest request,
        CancellationToken cancellationToken)
    {
        FloorResponse result = await _floorService.UpdateAsync(id, request, cancellationToken);
        return Ok(ApiResponse.Ok(result, ApiMessages.Updated));
    }

    /// <summary>เปลี่ยนสถานะชั้น</summary>
    [HttpPatch("{id:guid}/status")]
    [RequirePermission(PermissionCodes.Floors.Update)]
    [ProducesResponseType(typeof(ApiResponse<FloorResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<FloorResponse>>> ChangeStatus(
        Guid id,
        [FromBody] ChangeStatusRequest<FloorStatus> request,
        CancellationToken cancellationToken)
    {
        FloorResponse result = await _floorService.ChangeStatusAsync(id, request.Status.ToString(), cancellationToken);
        return Ok(ApiResponse.Ok(result, ApiMessages.Success));
    }

    /// <summary>ลบชั้น</summary>
    [HttpDelete("{id:guid}")]
    [RequirePermission(PermissionCodes.Floors.Delete)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse>> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _floorService.DeleteAsync(id, cancellationToken);
        return Ok(ApiResponse.Ok(ApiMessages.Deleted));
    }
}
