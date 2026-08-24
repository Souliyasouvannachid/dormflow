using Asp.Versioning;
using DormFlow.Api.Authorization;
using DormFlow.Application.Common.Authorization;
using DormFlow.Application.Features.Parking;
using DormFlow.Contracts.Common;
using DormFlow.Contracts.Parking;
using Microsoft.AspNetCore.Mvc;

namespace DormFlow.Api.Controllers.V1;

/// <summary>Endpoint สำหรับจัดการการจดทะเบียนที่จอดรถ</summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/parking")]
[Produces("application/json")]
public sealed class ParkingController(IParkingRegistrationService parkingRegistrationService) : ControllerBase
{
    private readonly IParkingRegistrationService _parkingRegistrationService = parkingRegistrationService;

    /// <summary>ดึงรายการจดทะเบียนที่จอดรถแบบแบ่งหน้า</summary>
    [HttpGet]
    [RequirePermission(PermissionCodes.Parking.View)]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<ParkingRegistrationResponse>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<PagedResult<ParkingRegistrationResponse>>>> GetList(
        [FromQuery] ParkingRegistrationListRequest request,
        CancellationToken cancellationToken)
    {
        PagedResult<ParkingRegistrationResponse> result =
            await _parkingRegistrationService.GetPagedAsync(request, cancellationToken);
        return Ok(ApiResponse.Paged(result));
    }

    /// <summary>อ่านข้อมูลการจดทะเบียนที่จอดรถหนึ่งรายการ</summary>
    [HttpGet("{id:guid}")]
    [RequirePermission(PermissionCodes.Parking.View)]
    [ProducesResponseType(typeof(ApiResponse<ParkingRegistrationResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<ParkingRegistrationResponse>>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        ParkingRegistrationResponse result = await _parkingRegistrationService.GetByIdAsync(id, cancellationToken);
        return Ok(ApiResponse.Ok(result, ApiMessages.Retrieved));
    }

    /// <summary>จดทะเบียนที่จอดรถใหม่</summary>
    [HttpPost]
    [RequirePermission(PermissionCodes.Parking.Register)]
    [ProducesResponseType(typeof(ApiResponse<ParkingRegistrationResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ApiResponse<ParkingRegistrationResponse>>> Register(
        [FromBody] RegisterParkingRequest request,
        CancellationToken cancellationToken)
    {
        ParkingRegistrationResponse result = await _parkingRegistrationService.RegisterAsync(request, cancellationToken);
        return Ok(ApiResponse.Ok(result, ApiMessages.Created));
    }

    /// <summary>สิ้นสุดการจดทะเบียนที่จอดรถ</summary>
    [HttpPost("{id:guid}/release")]
    [RequirePermission(PermissionCodes.Parking.Release)]
    [ProducesResponseType(typeof(ApiResponse<ParkingRegistrationResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ApiResponse<ParkingRegistrationResponse>>> Release(
        Guid id,
        CancellationToken cancellationToken)
    {
        ParkingRegistrationResponse result = await _parkingRegistrationService.ReleaseAsync(id, cancellationToken);
        return Ok(ApiResponse.Ok(result, ApiMessages.Success));
    }
}
