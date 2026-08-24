using Asp.Versioning;
using DormFlow.Api.Authorization;
using DormFlow.Application.Common.Authorization;
using DormFlow.Application.Features.MeterReadings;
using DormFlow.Contracts.Common;
using DormFlow.Contracts.MeterReadings;
using Microsoft.AspNetCore.Mvc;

namespace DormFlow.Api.Controllers.V1;

/// <summary>Endpoint สำหรับจัดการค่ามิเตอร์</summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/meter-readings")]
[Produces("application/json")]
public sealed class MeterReadingsController(IMeterReadingService meterReadingService) : ControllerBase
{
    private readonly IMeterReadingService _meterReadingService = meterReadingService;

    /// <summary>ดึงรายการค่ามิเตอร์แบบแบ่งหน้า</summary>
    [HttpGet]
    [RequirePermission(PermissionCodes.MeterReadings.View)]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<MeterReadingResponse>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<PagedResult<MeterReadingResponse>>>> GetList(
        [FromQuery] MeterReadingListRequest request,
        CancellationToken cancellationToken)
    {
        PagedResult<MeterReadingResponse> result = await _meterReadingService.GetPagedAsync(request, cancellationToken);
        return Ok(ApiResponse.Paged(result));
    }

    /// <summary>อ่านค่ามิเตอร์หนึ่งรายการ</summary>
    [HttpGet("{id:guid}")]
    [RequirePermission(PermissionCodes.MeterReadings.View)]
    [ProducesResponseType(typeof(ApiResponse<MeterReadingResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<MeterReadingResponse>>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        MeterReadingResponse result = await _meterReadingService.GetByIdAsync(id, cancellationToken);
        return Ok(ApiResponse.Ok(result, ApiMessages.Retrieved));
    }

    /// <summary>บันทึกค่ามิเตอร์ใหม่</summary>
    [HttpPost]
    [RequirePermission(PermissionCodes.MeterReadings.Create)]
    [ProducesResponseType(typeof(ApiResponse<MeterReadingResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ApiResponse<MeterReadingResponse>>> Create(
        [FromBody] CreateMeterReadingRequest request,
        CancellationToken cancellationToken)
    {
        MeterReadingResponse result = await _meterReadingService.CreateAsync(request, cancellationToken);
        return Ok(ApiResponse.Ok(result, ApiMessages.Created));
    }
}
