using Asp.Versioning;
using DormFlow.Api.Authorization;
using DormFlow.Application.Common.Authorization;
using DormFlow.Application.Features.Dormitories;
using DormFlow.Contracts.Common;
using DormFlow.Contracts.Dormitories;
using DormFlow.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace DormFlow.Api.Controllers.V1;

/// <summary>Endpoint ສໍາລັບການຄຸ້ມຄອງຫໍພັກ</summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/dormitories")]
[Produces("application/json")]
public sealed class DormitoriesController(IDormitoryService dormitoryService) : ControllerBase
{
    private readonly IDormitoryService _dormitoryService = dormitoryService;

    /// <summary>ດຶງເອົາລາຍຊື່ຫໍພັກທີ່ຕັ້ງໄວ້ແລ້ວ</summary>
    /// <param name="request">ຕົວກໍານົດການຊອກຫາ</param>
    /// <param name="cancellationToken">Token ສໍາລັບການຍົກເລີກຄໍາສັ່ງ</param>
    /// <response code="200">ລາຍຊື່ຫໍພັກ</response>
    [HttpGet]
    [RequirePermission(PermissionCodes.Dormitories.View)]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<DormitoryResponse>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<PagedResult<DormitoryResponse>>>> GetList(
        [FromQuery] DormitoryListRequest request,
        CancellationToken cancellationToken)
    {
        PagedResult<DormitoryResponse> result = await _dormitoryService.GetPagedAsync(request, cancellationToken);
        return Ok(ApiResponse.Paged(result));
    }

    /// <summary>ອ່ານຂໍ້ມູນກ່ຽວກັບຫໍພັກໜຶ່ງ</summary>
    /// <param name="id">ລະຫັດຫໍພັກ</param>
    /// <param name="cancellationToken">Token ສໍາລັບການຍົກເລີກຄໍາສັ່ງ</param>
    /// <response code="200">ຂໍ້ມູນຫໍພັກ</response>
    /// <response code="404">ບໍ່ພົບຫໍພັກທີ່ລະບຸໄວ້</response>
    [HttpGet("{id:guid}")]
    [RequirePermission(PermissionCodes.Dormitories.View)]
    [ProducesResponseType(typeof(ApiResponse<DormitoryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<DormitoryResponse>>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        DormitoryResponse result = await _dormitoryService.GetByIdAsync(id, cancellationToken);
        return Ok(ApiResponse.Ok(result, ApiMessages.Retrieved));
    }

    /// <summary>ສ້າງຫໍພັກໃໝ່</summary>
    /// <param name="request">ຂໍ້ມູນຫໍພັກ</param>
    /// <param name="cancellationToken">Token ສໍາລັບການຍົກເລີກຄໍາສັ່ງ</param>
    /// <response code="200">ສຳເລັດການສ້າງຫໍພັກ</response>
    /// <response code="400">ຂໍ້ມູນບໍ່ຖືກຕ້ອງ</response>
    /// <response code="409">ລະຫັດຫໍພັກແມ່ນຊໍ້າກັນຂອງລະຫັດຫໍພັກທີ່ມີຢູ່ແລ້ວໃນອົງກອນດຽວກັນ</response>
    [HttpPost]
    [RequirePermission(PermissionCodes.Dormitories.Create)]
    [ProducesResponseType(typeof(ApiResponse<DormitoryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ApiResponse<DormitoryResponse>>> Create(
        [FromBody] CreateDormitoryRequest request,
        CancellationToken cancellationToken)
    {
        DormitoryResponse result = await _dormitoryService.CreateAsync(request, cancellationToken);
        return Ok(ApiResponse.Ok(result, ApiMessages.Created));
    }

    /// <summary>ແກ້ໄຂຂໍ້ມູນຫໍພັກ</summary>
    /// <param name="id">ລະຫັດຫໍພັກທີ່ທ່ານຕ້ອງການແກ້ໄຂ</param>
    /// <param name="request">ຂໍ້ມູນໃຝ່</param>
    /// <param name="cancellationToken">Token ສໍາລັບການຍົກເລີກຄໍາສັ່ງ</param>
    /// <response code="200">ແກ້ໄຂສຳເລັດແລ້ວ</response>
    /// <response code="400">ຂໍ້ມູນບໍ່ຖືກຕ້ອງ</response>
    /// <response code="404">ບໍ່ພົບຫໍພັກທີ່ລະບຸໄວ້</response>
    /// <response code="409">ລະຫັດຫໍພັກແມ່ນຊໍ້າກັນຂອງລະຫັດຫໍພັກທີ່ມີຢູ່ແລ້ວໃນອົງກອນດຽວກັນ</response>
    [HttpPut("{id:guid}")]
    [RequirePermission(PermissionCodes.Dormitories.Update)]
    [ProducesResponseType(typeof(ApiResponse<DormitoryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ApiResponse<DormitoryResponse>>> Update(
        Guid id,
        [FromBody] UpdateDormitoryRequest request,
        CancellationToken cancellationToken)
    {
        DormitoryResponse result = await _dormitoryService.UpdateAsync(id, request, cancellationToken);
        return Ok(ApiResponse.Ok(result, ApiMessages.Updated));
    }

    /// <summary>ປ່ຽນສະຖານະຫໍພັກ</summary>
    /// <param name="id">ລະຫັດຫໍພັກທີ່ທ່ານຕ້ອງການປ່ຽນສະຖານະ</param>
    /// <param name="request">ສະຖານະໃຝ່</param>
    /// <param name="cancellationToken">Token ສໍາລັບການຍົກເລີກຄໍາສັ່ງ</param>
    /// <response code="200">ປ່ຽນສະຖານະສຳເລັດແລ້ວ</response>
    /// <response code="400">ສະຖານະທີ່ລະບຸບໍ່ຖືກຕ້ອງ</response>
    /// <response code="404">ບໍ່ພົບຫໍພັກທີ່ລະບຸໄວ້</response>
    [HttpPatch("{id:guid}/status")]
    [RequirePermission(PermissionCodes.Dormitories.Update)]
    [ProducesResponseType(typeof(ApiResponse<DormitoryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<DormitoryResponse>>> ChangeStatus(
        Guid id,
        [FromBody] ChangeStatusRequest<DormitoryStatus> request,
        CancellationToken cancellationToken)
    {
        DormitoryResponse result = await _dormitoryService.ChangeStatusAsync(
            id,
            request.Status.ToString(),
            cancellationToken);

        return Ok(ApiResponse.Ok(result, ApiMessages.Success));
    }

    /// <summary>ລຶບຫໍພັກ</summary>
    /// <param name="id">ລະຫັດຫໍພັກທີ່ທ່ານຕ້ອງການລຶບ</param>
    /// <param name="cancellationToken">Token ສໍາລັບການຍົກເລີກຄໍາສັ່ງ</param>
    /// <response code="200">ລຶບສຳເລັດແລ້ວ</response>
    /// <response code="404">ບໍ່ພົບຫໍພັກທີ່ລະບຸໄວ້</response>
    [HttpDelete("{id:guid}")]
    [RequirePermission(PermissionCodes.Dormitories.Delete)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse>> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _dormitoryService.DeleteAsync(id, cancellationToken);
        return Ok(ApiResponse.Ok(ApiMessages.Deleted));
    }
}
