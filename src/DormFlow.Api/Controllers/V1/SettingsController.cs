using Asp.Versioning;
using DormFlow.Api.Authorization;
using DormFlow.Application.Common.Authorization;
using DormFlow.Application.Features.Settings;
using DormFlow.Contracts.Common;
using DormFlow.Contracts.Settings;
using Microsoft.AspNetCore.Mvc;

namespace DormFlow.Api.Controllers.V1;

/// <summary>Endpoint ສໍາລັບການຄຸ້ມຄອງການຕັ້ງຄ່າອົງກອນ</summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/settings")]
[Produces("application/json")]
public sealed class SettingsController(ISettingService settingService) : ControllerBase
{
    private readonly ISettingService _settingService = settingService;

    /// <summary>ດຶງເອົາບັນຊີລາຍຊື່ paginated ຂອງການຕັ້ງຄ່າ</summary>
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

    /// <summary>ອ່ານການຕັ້ງຄ່າຈາກຄີ</summary>
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

    /// <summary>ສ້າງ ຫຼືແກ້ໄຂການຕັ້ງຄ່າໂດຍອີງໃສ່ລະຫັດທີ່ລະບຸ</summary>
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

    /// <summary>ລຶບການຕັ້ງຄ່າໂດຍອີງໃສ່ລະຫັດທີ່ລະບຸ</summary>
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
