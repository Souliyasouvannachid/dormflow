using Asp.Versioning;
using DormFlow.Api.Authorization;
using DormFlow.Application.Common.Authorization;
using DormFlow.Application.Features.Reservations;
using DormFlow.Contracts.Common;
using DormFlow.Contracts.Reservations;
using Microsoft.AspNetCore.Mvc;

namespace DormFlow.Api.Controllers.V1;

/// <summary>Endpoint สำหรับจัดการการจองห้องพัก</summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/reservations")]
[Produces("application/json")]
public sealed class ReservationsController(IReservationService reservationService) : ControllerBase
{
    private readonly IReservationService _reservationService = reservationService;

    /// <summary>ดึงรายการจองแบบแบ่งหน้า</summary>
    [HttpGet]
    [RequirePermission(PermissionCodes.Reservations.View)]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<ReservationResponse>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<PagedResult<ReservationResponse>>>> GetList(
        [FromQuery] ReservationListRequest request,
        CancellationToken cancellationToken)
    {
        PagedResult<ReservationResponse> result = await _reservationService.GetPagedAsync(request, cancellationToken);
        return Ok(ApiResponse.Paged(result));
    }

    /// <summary>อ่านข้อมูลการจองหนึ่งรายการ</summary>
    [HttpGet("{id:guid}")]
    [RequirePermission(PermissionCodes.Reservations.View)]
    [ProducesResponseType(typeof(ApiResponse<ReservationResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<ReservationResponse>>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        ReservationResponse result = await _reservationService.GetByIdAsync(id, cancellationToken);
        return Ok(ApiResponse.Ok(result, ApiMessages.Retrieved));
    }

    /// <summary>จองห้องพัก</summary>
    [HttpPost]
    [RequirePermission(PermissionCodes.Reservations.Create)]
    [ProducesResponseType(typeof(ApiResponse<ReservationResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ApiResponse<ReservationResponse>>> Create(
        [FromBody] CreateReservationRequest request,
        CancellationToken cancellationToken)
    {
        ReservationResponse result = await _reservationService.CreateAsync(request, cancellationToken);
        return Ok(ApiResponse.Ok(result, ApiMessages.Created));
    }

    /// <summary>ยกเลิกการจอง</summary>
    [HttpPost("{id:guid}/cancel")]
    [RequirePermission(PermissionCodes.Reservations.Cancel)]
    [ProducesResponseType(typeof(ApiResponse<ReservationResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ApiResponse<ReservationResponse>>> Cancel(
        Guid id,
        CancellationToken cancellationToken)
    {
        ReservationResponse result = await _reservationService.CancelAsync(id, cancellationToken);
        return Ok(ApiResponse.Ok(result, ApiMessages.Success));
    }
}
