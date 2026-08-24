using Asp.Versioning;
using DormFlow.Api.Authorization;
using DormFlow.Application.Common.Authorization;
using DormFlow.Application.Features.RoomTypes;
using DormFlow.Contracts.Common;
using DormFlow.Contracts.RoomTypes;
using DormFlow.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace DormFlow.Api.Controllers.V1;

/// <summary>Endpoint สำหรับจัดการประเภทห้อง</summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/room-types")]
[Produces("application/json")]
public sealed class RoomTypesController(IRoomTypeService roomTypeService) : ControllerBase
{
    private readonly IRoomTypeService _roomTypeService = roomTypeService;

    /// <summary>ดึงรายการประเภทห้องแบบแบ่งหน้า</summary>
    [HttpGet]
    [RequirePermission(PermissionCodes.RoomTypes.View)]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<RoomTypeResponse>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<PagedResult<RoomTypeResponse>>>> GetList(
        [FromQuery] RoomTypeListRequest request,
        CancellationToken cancellationToken)
    {
        PagedResult<RoomTypeResponse> result = await _roomTypeService.GetPagedAsync(request, cancellationToken);
        return Ok(ApiResponse.Paged(result));
    }

    /// <summary>อ่านข้อมูลประเภทห้องหนึ่งแบบ</summary>
    [HttpGet("{id:guid}")]
    [RequirePermission(PermissionCodes.RoomTypes.View)]
    [ProducesResponseType(typeof(ApiResponse<RoomTypeResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<RoomTypeResponse>>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        RoomTypeResponse result = await _roomTypeService.GetByIdAsync(id, cancellationToken);
        return Ok(ApiResponse.Ok(result, ApiMessages.Retrieved));
    }

    /// <summary>สร้างประเภทห้องใหม่</summary>
    [HttpPost]
    [RequirePermission(PermissionCodes.RoomTypes.Create)]
    [ProducesResponseType(typeof(ApiResponse<RoomTypeResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ApiResponse<RoomTypeResponse>>> Create(
        [FromBody] CreateRoomTypeRequest request,
        CancellationToken cancellationToken)
    {
        RoomTypeResponse result = await _roomTypeService.CreateAsync(request, cancellationToken);
        return Ok(ApiResponse.Ok(result, ApiMessages.Created));
    }

    /// <summary>แก้ไขข้อมูลประเภทห้อง</summary>
    [HttpPut("{id:guid}")]
    [RequirePermission(PermissionCodes.RoomTypes.Update)]
    [ProducesResponseType(typeof(ApiResponse<RoomTypeResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ApiResponse<RoomTypeResponse>>> Update(
        Guid id,
        [FromBody] UpdateRoomTypeRequest request,
        CancellationToken cancellationToken)
    {
        RoomTypeResponse result = await _roomTypeService.UpdateAsync(id, request, cancellationToken);
        return Ok(ApiResponse.Ok(result, ApiMessages.Updated));
    }

    /// <summary>เปลี่ยนสถานะประเภทห้อง</summary>
    [HttpPatch("{id:guid}/status")]
    [RequirePermission(PermissionCodes.RoomTypes.Update)]
    [ProducesResponseType(typeof(ApiResponse<RoomTypeResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<RoomTypeResponse>>> ChangeStatus(
        Guid id,
        [FromBody] ChangeStatusRequest<RoomTypeStatus> request,
        CancellationToken cancellationToken)
    {
        RoomTypeResponse result = await _roomTypeService.ChangeStatusAsync(
            id,
            request.Status.ToString(),
            cancellationToken);

        return Ok(ApiResponse.Ok(result, ApiMessages.Success));
    }

    /// <summary>ลบประเภทห้อง</summary>
    [HttpDelete("{id:guid}")]
    [RequirePermission(PermissionCodes.RoomTypes.Delete)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse>> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _roomTypeService.DeleteAsync(id, cancellationToken);
        return Ok(ApiResponse.Ok(ApiMessages.Deleted));
    }
}
