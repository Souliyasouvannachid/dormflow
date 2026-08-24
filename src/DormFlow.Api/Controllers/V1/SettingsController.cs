using Asp.Versioning;
using DormFlow.Api.Authorization;
using DormFlow.Application.Common.Authorization;
using DormFlow.Application.Features.Settings;
using DormFlow.Contracts.Common;
using DormFlow.Contracts.Settings;
using Microsoft.AspNetCore.Mvc;

namespace DormFlow.Api.Controllers.V1;

/// <summary>Endpoint สำหรับจัดการค่าตั้งขององค์กร</summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/settings")]
[Produces("application/json")]
public sealed class SettingsController(ISettingService settingService) : ControllerBase
{
    private readonly ISettingService _settingService = settingService;

    /// <summary>ดึงรายการค่าตั้งแบบแบ่งหน้า</summary>
    [HttpGet]
    [RequirePermission(PermissionCodes.Administration.ManageSettings)]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<SettingResponse>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<PagedResult<SettingResponse>>>> GetList(
        [FromQuery] SettingListRequest request,
        CancellationToken cancellationToken)
    {
        PagedResult<SettingResponse> result = await _settingService.GetPagedAsync(request, cancellationToken);
        return Ok(ApiResponse.Paged(result));
    }

    /// <summary>อ่านค่าตั้งจากคีย์</summary>
    [HttpGet("{key}")]
    [RequirePermission(PermissionCodes.Administration.ManageSettings)]
    [ProducesResponseType(typeof(ApiResponse<SettingResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<SettingResponse>>> GetByKey(
        string key,
        [FromQuery] Guid? organizationId,
        CancellationToken cancellationToken)
    {
        SettingResponse result = await _settingService.GetByKeyAsync(key, organizationId, cancellationToken);
        return Ok(ApiResponse.Ok(result, ApiMessages.Retrieved));
    }

    /// <summary>สร้างหรือแก้ไขค่าตั้งตามคีย์ที่ระบุ</summary>
    [HttpPut("{key}")]
    [RequirePermission(PermissionCodes.Administration.ManageSettings)]
    [ProducesResponseType(typeof(ApiResponse<SettingResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<SettingResponse>>> Upsert(
        string key,
        [FromBody] UpsertSettingRequest request,
        CancellationToken cancellationToken)
    {
        SettingResponse result = await _settingService.UpsertAsync(key, request, cancellationToken);
        return Ok(ApiResponse.Ok(result, ApiMessages.Success));
    }

    /// <summary>ลบค่าตั้งตามคีย์ที่ระบุ</summary>
    [HttpDelete("{key}")]
    [RequirePermission(PermissionCodes.Administration.ManageSettings)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse>> Delete(
        string key,
        [FromQuery] Guid? organizationId,
        CancellationToken cancellationToken)
    {
        await _settingService.DeleteAsync(key, organizationId, cancellationToken);
        return Ok(ApiResponse.Ok(ApiMessages.Deleted));
    }
}
