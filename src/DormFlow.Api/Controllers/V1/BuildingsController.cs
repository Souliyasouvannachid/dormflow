using Asp.Versioning;
using DormFlow.Api.Authorization;
using DormFlow.Application.Common.Authorization;
using DormFlow.Application.Features.Buildings;
using DormFlow.Contracts.Buildings;
using DormFlow.Contracts.Common;
using DormFlow.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace DormFlow.Api.Controllers.V1;

/// <summary>Endpoint ສໍາລັບການຄຸ້ມຄອງອາຄານ</summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/buildings")]
[Produces("application/json")]
public sealed class BuildingsController(IBuildingService buildingService) : ControllerBase
{
    private readonly IBuildingService _buildingService = buildingService;

    /// <summary>ດຶງຂໍ້ມູນລາຍຊື່ຕຶກອາຄານ</summary>
    [HttpGet]
    [RequirePermission(PermissionCodes.Buildings.View)]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<BuildingResponse>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<PagedResult<BuildingResponse>>>> GetList(
        [FromQuery] BuildingListRequest request,
        CancellationToken cancellationToken)
    {
        PagedResult<BuildingResponse> result = await _buildingService.GetPagedAsync(request, cancellationToken);
        return Ok(ApiResponse.Paged(result));
    }

    /// <summary>ອ່ານຂໍ້ມູນກ່ຽວກັບອາຄານ</summary>
    [HttpGet("{id:guid}")]
    [RequirePermission(PermissionCodes.Buildings.View)]
    [ProducesResponseType(typeof(ApiResponse<BuildingResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<BuildingResponse>>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        BuildingResponse result = await _buildingService.GetByIdAsync(id, cancellationToken);
        return Ok(ApiResponse.Ok(result, ApiMessages.Retrieved));
    }

    /// <summary>ສ້າງອາຄານໃຝ່</summary>
    [HttpPost]
    [RequirePermission(PermissionCodes.Buildings.Create)]
    [ProducesResponseType(typeof(ApiResponse<BuildingResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ApiResponse<BuildingResponse>>> Create(
        [FromBody] CreateBuildingRequest request,
        CancellationToken cancellationToken)
    {
        BuildingResponse result = await _buildingService.CreateAsync(request, cancellationToken);
        return Ok(ApiResponse.Ok(result, ApiMessages.Created));
    }

    /// <summary>ແກ້ໄຂຂໍ້ມູນອາຄານ</summary>
    [HttpPut("{id:guid}")]
    [RequirePermission(PermissionCodes.Buildings.Update)]
    [ProducesResponseType(typeof(ApiResponse<BuildingResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ApiResponse<BuildingResponse>>> Update(
        Guid id,
        [FromBody] UpdateBuildingRequest request,
        CancellationToken cancellationToken)
    {
        BuildingResponse result = await _buildingService.UpdateAsync(id, request, cancellationToken);
        return Ok(ApiResponse.Ok(result, ApiMessages.Updated));
    }

    /// <summary>ປ່ຽນສະຖານະການກໍ່ສ້າງ</summary>
    [HttpPatch("{id:guid}/status")]
    [RequirePermission(PermissionCodes.Buildings.Update)]
    [ProducesResponseType(typeof(ApiResponse<BuildingResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<BuildingResponse>>> ChangeStatus(
        Guid id,
        [FromBody] ChangeStatusRequest<BuildingStatus> request,
        CancellationToken cancellationToken)
    {
        BuildingResponse result = await _buildingService.ChangeStatusAsync(
            id,
            request.Status.ToString(),
            cancellationToken);

        return Ok(ApiResponse.Ok(result, ApiMessages.Success));
    }

    /// <summary>ລຶບອາຄານ</summary>
    [HttpDelete("{id:guid}")]
    [RequirePermission(PermissionCodes.Buildings.Delete)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse>> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _buildingService.DeleteAsync(id, cancellationToken);
        return Ok(ApiResponse.Ok(ApiMessages.Deleted));
    }
}
