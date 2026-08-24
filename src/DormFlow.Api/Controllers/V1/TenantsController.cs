using Asp.Versioning;
using DormFlow.Api.Authorization;
using DormFlow.Application.Common.Authorization;
using DormFlow.Application.Features.Tenants;
using DormFlow.Contracts.Common;
using DormFlow.Contracts.Tenants;
using DormFlow.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace DormFlow.Api.Controllers.V1;

/// <summary>Endpoint สำหรับจัดการข้อมูลผู้เช่า</summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/tenants")]
[Produces("application/json")]
public sealed class TenantsController(ITenantService tenantService) : ControllerBase
{
    private readonly ITenantService _tenantService = tenantService;

    /// <summary>ดึงรายการผู้เช่าแบบแบ่งหน้า</summary>
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

    /// <summary>อ่านข้อมูลผู้เช่าหนึ่งคน</summary>
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

    /// <summary>สร้างข้อมูลผู้เช่าใหม่</summary>
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

    /// <summary>แก้ไขข้อมูลผู้เช่า</summary>
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

    /// <summary>เปลี่ยนสถานะผู้เช่า</summary>
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
