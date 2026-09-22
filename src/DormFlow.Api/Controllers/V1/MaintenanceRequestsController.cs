using Asp.Versioning;
using DormFlow.Api.Authorization;
using DormFlow.Application.Common.Authorization;
using DormFlow.Application.Features.Maintenance;
using DormFlow.Contracts.Common;
using DormFlow.Contracts.Maintenance;
using DormFlow.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace DormFlow.Api.Controllers.V1;

/// <summary>Endpoint ສໍາລັບການຄຸ້ມຄອງວຽກງານບໍາລຸງຮັກສາ</summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/maintenance-requests")]
[Produces("application/json")]
public sealed class MaintenanceRequestsController(IMaintenanceRequestService maintenanceRequestService) : ControllerBase
{
    private readonly IMaintenanceRequestService _maintenanceRequestService = maintenanceRequestService;

    /// <summary>ດຶງເອົາບັນຊີລາຍຊື່ paginated ຂອງການແຈ້ງເຕືອນວຽກງານບໍາລຸງຮັກສາ</summary>
    [HttpGet]
    [RequirePermission(PermissionCodes.Maintenance.View)]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<MaintenanceRequestResponse>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<PagedResult<MaintenanceRequestResponse>>>> GetList(
        [FromQuery] MaintenanceRequestListRequest request,
        CancellationToken cancellationToken)
    {
        PagedResult<MaintenanceRequestResponse> result =
            await _maintenanceRequestService.GetPagedAsync(request, cancellationToken);
        return Ok(ApiResponse.Paged(result));
    }

    /// <summary>ອ່ານການແຈ້ງການວຽກບໍາລຸງຮັກສາຜຶ່ງ</summary>
    [HttpGet("{id:guid}")]
    [RequirePermission(PermissionCodes.Maintenance.View)]
    [ProducesResponseType(typeof(ApiResponse<MaintenanceRequestResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<MaintenanceRequestResponse>>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        MaintenanceRequestResponse result = await _maintenanceRequestService.GetByIdAsync(id, cancellationToken);
        return Ok(ApiResponse.Ok(result, ApiMessages.Retrieved));
    }

    /// <summary>ແຈ້ງການບໍາລຸງຮັກສາໃຝ່</summary>
    [HttpPost]
    [RequirePermission(PermissionCodes.Maintenance.UpdateStatus)]
    [ProducesResponseType(typeof(ApiResponse<MaintenanceRequestResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ApiResponse<MaintenanceRequestResponse>>> Create(
        [FromBody] CreateMaintenanceRequestRequest request,
        CancellationToken cancellationToken)
    {
        MaintenanceRequestResponse result = await _maintenanceRequestService.CreateAsync(request, cancellationToken);
        return Ok(ApiResponse.Ok(result, ApiMessages.Created));
    }

    /// <summary>ມອບໝາຍ (ຫຼືມອບໝາຍຄືນໃໝ່) ຊ່າງສ້ອມແປງສຳລັບວຽກນີ້</summary>
    [HttpPost("{id:guid}/assign")]
    [RequirePermission(PermissionCodes.Maintenance.Assign)]
    [ProducesResponseType(typeof(ApiResponse<MaintenanceRequestResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ApiResponse<MaintenanceRequestResponse>>> Assign(
        Guid id,
        [FromBody] AssignMaintenanceRequestRequest request,
        CancellationToken cancellationToken)
    {
        MaintenanceRequestResponse result = await _maintenanceRequestService.AssignAsync(id, request, cancellationToken);
        return Ok(ApiResponse.Ok(result, ApiMessages.Success));
    }

    /// <summary>ປ່ຽນສະຖານະວຽກບໍາລຸງຮັກສາ (ບໍ່ລວມເອົາການມອບໝາຍນັກວິຊາການ. ຕ້ອງການ Endpoint ມອບວຽກແທນ)</summary>
    [HttpPatch("{id:guid}/status")]
    [RequirePermission(PermissionCodes.Maintenance.UpdateStatus)]
    [ProducesResponseType(typeof(ApiResponse<MaintenanceRequestResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ApiResponse<MaintenanceRequestResponse>>> ChangeStatus(
        Guid id,
        [FromBody] ChangeStatusRequest<MaintenanceStatus> request,
        CancellationToken cancellationToken)
    {
        MaintenanceRequestResponse result =
            await _maintenanceRequestService.ChangeStatusAsync(id, request.Status.ToString(), cancellationToken);
        return Ok(ApiResponse.Ok(result, ApiMessages.Success));
    }
}
