using Asp.Versioning;
using DormFlow.Api.Authorization;
using DormFlow.Application.Common.Authorization;
using DormFlow.Application.Features.Tenants;
using DormFlow.Contracts.Common;
using DormFlow.Contracts.Tenants;
using DormFlow.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace DormFlow.Api.Controllers.V1;

/// <summary>Endpoint ສໍາລັບການຄຸ້ມຄອງຂໍ້ມູນຜູ້ເຊົ່າ</summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/tenants")]
[Produces("application/json")]
public sealed class TenantsController(ITenantService tenantService) : ControllerBase
{
    private readonly ITenantService _tenantService = tenantService;

    /// <summary>ດຶງເອົາລາຍຊື່ຜູ້ເຊົ່າທີ່ຕັ້ງໄວ້ແລ້ວ</summary>
    [HttpGet]
    [RequirePermission(PermissionCodes.Tenants.View)]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<TenantResponse>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<PagedResult<TenantResponse>>>> GetList(
        [FromQuery] TenantListRequest request,
        CancellationToken cancellationToken)
    {
        PagedResult<TenantResponse> result = await _tenantService.GetPagedAsync(request, cancellationToken);
        return Ok(ApiResponse.Paged(result));
    }

    /// <summary>ອ່ານຂໍ້ມູນກ່ຽວກັບຜູ້ເຊົ່າຄົນໜຶ່ງ</summary>
    [HttpGet("{id:guid}")]
    [RequirePermission(PermissionCodes.Tenants.View)]
    [ProducesResponseType(typeof(ApiResponse<TenantResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<TenantResponse>>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        TenantResponse result = await _tenantService.GetByIdAsync(id, cancellationToken);
        return Ok(ApiResponse.Ok(result, ApiMessages.Retrieved));
    }

    /// <summary>ສ້າງຂໍ້ມູນຜູ້ເຊົ່າໃຝ່</summary>
    [HttpPost]
    [RequirePermission(PermissionCodes.Tenants.Create)]
    [ProducesResponseType(typeof(ApiResponse<TenantResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ApiResponse<TenantResponse>>> Create(
        [FromBody] CreateTenantRequest request,
        CancellationToken cancellationToken)
    {
        TenantResponse result = await _tenantService.CreateAsync(request, cancellationToken);
        return Ok(ApiResponse.Ok(result, ApiMessages.Created));
    }

    /// <summary>ແກ້ໄຂຂໍ້ມູນຜູ້ເຊົ່າ</summary>
    [HttpPut("{id:guid}")]
    [RequirePermission(PermissionCodes.Tenants.Update)]
    [ProducesResponseType(typeof(ApiResponse<TenantResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ApiResponse<TenantResponse>>> Update(
        Guid id,
        [FromBody] UpdateTenantRequest request,
        CancellationToken cancellationToken)
    {
        TenantResponse result = await _tenantService.UpdateAsync(id, request, cancellationToken);
        return Ok(ApiResponse.Ok(result, ApiMessages.Updated));
    }

    /// <summary>ປ່ຽນສະຖານະຜູ້ເຊົ່າ</summary>
    [HttpPatch("{id:guid}/status")]
    [RequirePermission(PermissionCodes.Tenants.Update)]
    [ProducesResponseType(typeof(ApiResponse<TenantResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<TenantResponse>>> ChangeStatus(
        Guid id,
        [FromBody] ChangeStatusRequest<TenantStatus> request,
        CancellationToken cancellationToken)
    {
        TenantResponse result = await _tenantService.ChangeStatusAsync(
            id,
            request.Status.ToString(),
            cancellationToken);

        return Ok(ApiResponse.Ok(result, ApiMessages.Success));
    }
}
