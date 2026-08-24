using Asp.Versioning;
using DormFlow.Api.Authorization;
using DormFlow.Application.Common.Authorization;
using DormFlow.Application.Features.Organizations;
using DormFlow.Contracts.Common;
using DormFlow.Contracts.Organizations;
using DormFlow.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace DormFlow.Api.Controllers.V1;

/// <summary>Endpoint สำหรับจัดการองค์กร</summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/organizations")]
[Produces("application/json")]
public sealed class OrganizationsController(IOrganizationService organizationService) : ControllerBase
{
    private readonly IOrganizationService _organizationService = organizationService;

    /// <summary>ดึงรายการองค์กรแบบแบ่งหน้า</summary>
    [HttpGet]
    [RequirePermission(PermissionCodes.Organizations.View)]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<OrganizationResponse>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<PagedResult<OrganizationResponse>>>> GetList(
        [FromQuery] OrganizationListRequest request,
        CancellationToken cancellationToken)
    {
        PagedResult<OrganizationResponse> result = await _organizationService.GetPagedAsync(request, cancellationToken);
        return Ok(ApiResponse.Paged(result));
    }

    /// <summary>อ่านข้อมูลองค์กรหนึ่งแห่ง</summary>
    [HttpGet("{id:guid}")]
    [RequirePermission(PermissionCodes.Organizations.View)]
    [ProducesResponseType(typeof(ApiResponse<OrganizationResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<OrganizationResponse>>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        OrganizationResponse result = await _organizationService.GetByIdAsync(id, cancellationToken);
        return Ok(ApiResponse.Ok(result, ApiMessages.Retrieved));
    }

    /// <summary>สร้างองค์กรใหม่</summary>
    [HttpPost]
    [RequirePermission(PermissionCodes.Organizations.Create)]
    [ProducesResponseType(typeof(ApiResponse<OrganizationResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ApiResponse<OrganizationResponse>>> Create(
        [FromBody] CreateOrganizationRequest request,
        CancellationToken cancellationToken)
    {
        OrganizationResponse result = await _organizationService.CreateAsync(request, cancellationToken);
        return Ok(ApiResponse.Ok(result, ApiMessages.Created));
    }

    /// <summary>แก้ไขข้อมูลองค์กร</summary>
    [HttpPut("{id:guid}")]
    [RequirePermission(PermissionCodes.Organizations.Update)]
    [ProducesResponseType(typeof(ApiResponse<OrganizationResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ApiResponse<OrganizationResponse>>> Update(
        Guid id,
        [FromBody] UpdateOrganizationRequest request,
        CancellationToken cancellationToken)
    {
        OrganizationResponse result = await _organizationService.UpdateAsync(id, request, cancellationToken);
        return Ok(ApiResponse.Ok(result, ApiMessages.Updated));
    }

    /// <summary>เปลี่ยนสถานะองค์กร</summary>
    [HttpPatch("{id:guid}/status")]
    [RequirePermission(PermissionCodes.Organizations.Update)]
    [ProducesResponseType(typeof(ApiResponse<OrganizationResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<OrganizationResponse>>> ChangeStatus(
        Guid id,
        [FromBody] ChangeStatusRequest<OrganizationStatus> request,
        CancellationToken cancellationToken)
    {
        OrganizationResponse result = await _organizationService.ChangeStatusAsync(
            id,
            request.Status.ToString(),
            cancellationToken);

        return Ok(ApiResponse.Ok(result, ApiMessages.Success));
    }

    /// <summary>ลบองค์กร</summary>
    [HttpDelete("{id:guid}")]
    [RequirePermission(PermissionCodes.Organizations.Delete)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse>> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _organizationService.DeleteAsync(id, cancellationToken);
        return Ok(ApiResponse.Ok(ApiMessages.Deleted));
    }
}
