using Asp.Versioning;
using DormFlow.Api.Authorization;
using DormFlow.Application.Common.Authorization;
using DormFlow.Application.Features.Roles;
using DormFlow.Contracts.Common;
using DormFlow.Contracts.Roles;
using Microsoft.AspNetCore.Mvc;

namespace DormFlow.Api.Controllers.V1;

/// <summary>Endpoint ສໍາລັບການຄຸ້ມຄອງບົດບາດແລະການອະນຸຍາດບົດບາດ</summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/roles")]
[Produces("application/json")]
[RequirePermission(PermissionCodes.Administration.ManageRoles)]
public sealed class RolesController(IRoleService roleService) : ControllerBase
{
    private readonly IRoleService _roleService = roleService;

    /// <summary>ດຶງເອົາລາຍຊື່ບົດບາດໜ້າວຽກ</summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<RoleResponse>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<PagedResult<RoleResponse>>>> GetList(
        [FromQuery] RoleListRequest request,
        CancellationToken cancellationToken)
    {
        PagedResult<RoleResponse> result = await _roleService.GetPagedAsync(request, cancellationToken);
        return Ok(ApiResponse.Paged(result));
    }

    /// <summary>ອ່ານຂໍ້ມູນກ່ຽວກັບບົດບາດຜຶ່ງ</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<RoleResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<RoleResponse>>> GetById(Guid id, CancellationToken cancellationToken)
    {
        RoleResponse result = await _roleService.GetByIdAsync(id, cancellationToken);
        return Ok(ApiResponse.Ok(result, ApiMessages.Retrieved));
    }

    /// <summary>ສ້າງບົດບາດໃຝ່</summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<RoleResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ApiResponse<RoleResponse>>> Create(
        [FromBody] CreateRoleRequest request,
        CancellationToken cancellationToken)
    {
        RoleResponse result = await _roleService.CreateAsync(request, cancellationToken);
        return Ok(ApiResponse.Ok(result, ApiMessages.Created));
    }

    /// <summary>ແກ້ໄຂຂໍ້ມູນບົດບາດ</summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<RoleResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ApiResponse<RoleResponse>>> Update(
        Guid id,
        [FromBody] UpdateRoleRequest request,
        CancellationToken cancellationToken)
    {
        RoleResponse result = await _roleService.UpdateAsync(id, request, cancellationToken);
        return Ok(ApiResponse.Ok(result, ApiMessages.Updated));
    }

    /// <summary>ລຶບບົດບາດ</summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ApiResponse>> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _roleService.DeleteAsync(id, cancellationToken);
        return Ok(ApiResponse.Ok(ApiMessages.Deleted));
    }

    /// <summary>ມອບສິດໃຫ້ໜ້າທີ່ແທນສິດຕົ້ນສະບັບທັງໝົດ</summary>
    [HttpPost("{id:guid}/permissions")]
    [ProducesResponseType(typeof(ApiResponse<RoleResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<RoleResponse>>> AssignPermissions(
        Guid id,
        [FromBody] AssignRolePermissionsRequest request,
        CancellationToken cancellationToken)
    {
        RoleResponse result = await _roleService.AssignPermissionsAsync(id, request, cancellationToken);
        return Ok(ApiResponse.Ok(result, ApiMessages.Success));
    }
}
