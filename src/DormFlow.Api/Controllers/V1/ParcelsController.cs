using Asp.Versioning;
using DormFlow.Api.Authorization;
using DormFlow.Application.Common.Authorization;
using DormFlow.Application.Features.Parcels;
using DormFlow.Contracts.Common;
using DormFlow.Contracts.Parcels;
using Microsoft.AspNetCore.Mvc;

namespace DormFlow.Api.Controllers.V1;

/// <summary>Endpoint สำหรับจัดการพัสดุ</summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/parcels")]
[Produces("application/json")]
public sealed class ParcelsController(IParcelService parcelService) : ControllerBase
{
    private readonly IParcelService _parcelService = parcelService;

    /// <summary>ดึงรายการพัสดุแบบแบ่งหน้า</summary>
    [HttpGet]
    [RequirePermission(PermissionCodes.Parcels.View)]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<ParcelResponse>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<PagedResult<ParcelResponse>>>> GetList(
        [FromQuery] ParcelListRequest request,
        CancellationToken cancellationToken)
    {
        PagedResult<ParcelResponse> result = await _parcelService.GetPagedAsync(request, cancellationToken);
        return Ok(ApiResponse.Paged(result));
    }

    /// <summary>อ่านข้อมูลพัสดุหนึ่งรายการ</summary>
    [HttpGet("{id:guid}")]
    [RequirePermission(PermissionCodes.Parcels.View)]
    [ProducesResponseType(typeof(ApiResponse<ParcelResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<ParcelResponse>>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        ParcelResponse result = await _parcelService.GetByIdAsync(id, cancellationToken);
        return Ok(ApiResponse.Ok(result, ApiMessages.Retrieved));
    }

    /// <summary>บันทึกพัสดุที่มาถึงใหม่</summary>
    [HttpPost]
    [RequirePermission(PermissionCodes.Parcels.Log)]
    [ProducesResponseType(typeof(ApiResponse<ParcelResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<ParcelResponse>>> Log(
        [FromBody] LogParcelRequest request,
        CancellationToken cancellationToken)
    {
        ParcelResponse result = await _parcelService.LogAsync(request, cancellationToken);
        return Ok(ApiResponse.Ok(result, ApiMessages.Created));
    }

    /// <summary>บันทึกการรับพัสดุ</summary>
    [HttpPost("{id:guid}/pickup")]
    [RequirePermission(PermissionCodes.Parcels.Pickup)]
    [ProducesResponseType(typeof(ApiResponse<ParcelResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ApiResponse<ParcelResponse>>> Pickup(
        Guid id,
        [FromBody] PickupParcelRequest request,
        CancellationToken cancellationToken)
    {
        ParcelResponse result = await _parcelService.PickupAsync(id, request, cancellationToken);
        return Ok(ApiResponse.Ok(result, ApiMessages.Success));
    }
}
