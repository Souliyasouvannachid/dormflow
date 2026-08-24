using Asp.Versioning;
using DormFlow.Api.Authorization;
using DormFlow.Application.Common.Authorization;
using DormFlow.Application.Features.Rooms;
using DormFlow.Contracts.Common;
using DormFlow.Contracts.Rooms;
using DormFlow.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace DormFlow.Api.Controllers.V1;

/// <summary>Endpoint สำหรับจัดการห้องพัก</summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/rooms")]
[Produces("application/json")]
public sealed class RoomsController(IRoomService roomService) : ControllerBase
{
    private readonly IRoomService _roomService = roomService;

    /// <summary>ดึงรายการห้องพักแบบแบ่งหน้า</summary>
    [HttpGet]
    [RequirePermission(PermissionCodes.Rooms.View)]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<RoomResponse>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<PagedResult<RoomResponse>>>> GetList(
        [FromQuery] RoomListRequest request,
        CancellationToken cancellationToken)
    {
        PagedResult<RoomResponse> result = await _roomService.GetPagedAsync(request, cancellationToken);
        return Ok(ApiResponse.Paged(result));
    }

    /// <summary>ดึงรายการห้องว่างที่พร้อมให้เช่า</summary>
    [HttpGet("available")]
    [RequirePermission(PermissionCodes.Rooms.View)]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<RoomResponse>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<PagedResult<RoomResponse>>>> GetAvailable(
        [FromQuery] RoomListRequest request,
        CancellationToken cancellationToken)
    {
        PagedResult<RoomResponse> result = await _roomService.GetAvailableAsync(request, cancellationToken);
        return Ok(ApiResponse.Paged(result));
    }

    /// <summary>สรุปจำนวนห้องแยกตามสถานะ</summary>
    [HttpGet("summary")]
    [RequirePermission(PermissionCodes.Rooms.View)]
    [ProducesResponseType(typeof(ApiResponse<RoomSummaryResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<RoomSummaryResponse>>> GetSummary(
        [FromQuery] RoomSummaryRequest request,
        CancellationToken cancellationToken)
    {
        RoomSummaryResponse result = await _roomService.GetSummaryAsync(request, cancellationToken);
        return Ok(ApiResponse.Ok(result, ApiMessages.Retrieved));
    }

    /// <summary>อ่านข้อมูลห้องพักหนึ่งห้อง</summary>
    [HttpGet("{id:guid}")]
    [RequirePermission(PermissionCodes.Rooms.View)]
    [ProducesResponseType(typeof(ApiResponse<RoomResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<RoomResponse>>> GetById(Guid id, CancellationToken cancellationToken)
    {
        RoomResponse result = await _roomService.GetByIdAsync(id, cancellationToken);
        return Ok(ApiResponse.Ok(result, ApiMessages.Retrieved));
    }

    /// <summary>สร้างห้องพักใหม่</summary>
    [HttpPost]
    [RequirePermission(PermissionCodes.Rooms.Create)]
    [ProducesResponseType(typeof(ApiResponse<RoomResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ApiResponse<RoomResponse>>> Create(
        [FromBody] CreateRoomRequest request,
        CancellationToken cancellationToken)
    {
        RoomResponse result = await _roomService.CreateAsync(request, cancellationToken);
        return Ok(ApiResponse.Ok(result, ApiMessages.Created));
    }

    /// <summary>แก้ไขข้อมูลห้องพัก</summary>
    [HttpPut("{id:guid}")]
    [RequirePermission(PermissionCodes.Rooms.Update)]
    [ProducesResponseType(typeof(ApiResponse<RoomResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ApiResponse<RoomResponse>>> Update(
        Guid id,
        [FromBody] UpdateRoomRequest request,
        CancellationToken cancellationToken)
    {
        RoomResponse result = await _roomService.UpdateAsync(id, request, cancellationToken);
        return Ok(ApiResponse.Ok(result, ApiMessages.Updated));
    }

    /// <summary>เปลี่ยนสถานะห้องพัก</summary>
    [HttpPatch("{id:guid}/status")]
    [RequirePermission(PermissionCodes.Rooms.ChangeStatus)]
    [ProducesResponseType(typeof(ApiResponse<RoomResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<RoomResponse>>> ChangeStatus(
        Guid id,
        [FromBody] ChangeStatusRequest<RoomStatus> request,
        CancellationToken cancellationToken)
    {
        RoomResponse result = await _roomService.ChangeStatusAsync(id, request.Status.ToString(), cancellationToken);
        return Ok(ApiResponse.Ok(result, ApiMessages.Success));
    }

    /// <summary>ลบห้องพัก</summary>
    [HttpDelete("{id:guid}")]
    [RequirePermission(PermissionCodes.Rooms.Delete)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse>> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _roomService.DeleteAsync(id, cancellationToken);
        return Ok(ApiResponse.Ok(ApiMessages.Deleted));
    }
}
