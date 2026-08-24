using Asp.Versioning;
using DormFlow.Api.Authorization;
using DormFlow.Application.Common.Authorization;
using DormFlow.Application.Features.Permissions;
using DormFlow.Contracts.Common;
using DormFlow.Contracts.Permissions;
using Microsoft.AspNetCore.Mvc;

namespace DormFlow.Api.Controllers.V1;

/// <summary>Endpoint สำหรับดูรายการสิทธิ์ทั้งหมดของระบบ</summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/permissions")]
[Produces("application/json")]
[RequirePermission(PermissionCodes.Administration.ManageRoles)]
public sealed class PermissionsController(IPermissionService permissionService) : ControllerBase
{
    private readonly IPermissionService _permissionService = permissionService;

    /// <summary>ดึงรายการสิทธิ์ทั้งหมดของระบบ ใช้ประกอบหน้าจอกำหนดสิทธิ์ให้บทบาท</summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<PermissionResponse>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<PermissionResponse>>>> GetAll(
        CancellationToken cancellationToken)
    {
        IReadOnlyList<PermissionResponse> result = await _permissionService.GetAllAsync(cancellationToken);
        return Ok(ApiResponse.Ok(result, ApiMessages.Retrieved));
    }
}
