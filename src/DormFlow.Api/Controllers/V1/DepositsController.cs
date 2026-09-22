using Asp.Versioning;
using DormFlow.Api.Authorization;
using DormFlow.Application.Common.Authorization;
using DormFlow.Application.Features.Deposits;
using DormFlow.Contracts.Common;
using DormFlow.Contracts.Deposits;
using Microsoft.AspNetCore.Mvc;

namespace DormFlow.Api.Controllers.V1;

/// <summary>Endpoint ສໍາລັບການຄຸ້ມຄອງເງິນຝາກ</summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/deposits")]
[Produces("application/json")]
public sealed class DepositsController(IDepositService depositService) : ControllerBase
{
    private readonly IDepositService _depositService = depositService;

    /// <summary>ດຶງເອົາລາຍຊື່ເງິນຝາກທີ່ແບ່ງອອກເປັນໜ້າ</summary>
    [HttpGet]
    [RequirePermission(PermissionCodes.Deposits.View)]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<DepositResponse>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<PagedResult<DepositResponse>>>> GetList(
        [FromQuery] DepositListRequest request,
        CancellationToken cancellationToken)
    {
        PagedResult<DepositResponse> result = await _depositService.GetPagedAsync(request, cancellationToken);
        return Ok(ApiResponse.Paged(result));
    }

    /// <summary>ອ່ານຂໍ້ມູນເງິນຝາກຜຶ່ງ</summary>
    [HttpGet("{id:guid}")]
    [RequirePermission(PermissionCodes.Deposits.View)]
    [ProducesResponseType(typeof(ApiResponse<DepositResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<DepositResponse>>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        DepositResponse result = await _depositService.GetByIdAsync(id, cancellationToken);
        return Ok(ApiResponse.Ok(result, ApiMessages.Retrieved));
    }

    /// <summary>ບັນທຶກການຮັບເງິນຝາກໃໝ່</summary>
    [HttpPost]
    [RequirePermission(PermissionCodes.Deposits.Collect)]
    [ProducesResponseType(typeof(ApiResponse<DepositResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ApiResponse<DepositResponse>>> Collect(
        [FromBody] CollectDepositRequest request,
        CancellationToken cancellationToken)
    {
        DepositResponse result = await _depositService.CollectAsync(request, cancellationToken);
        return Ok(ApiResponse.Ok(result, ApiMessages.Created));
    }

    /// <summary>ຝາກເງິນຄືນ</summary>
    [HttpPost("{id:guid}/refund")]
    [RequirePermission(PermissionCodes.Deposits.Refund)]
    [ProducesResponseType(typeof(ApiResponse<DepositResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ApiResponse<DepositResponse>>> Refund(
        Guid id,
        [FromBody] RefundDepositRequest request,
        CancellationToken cancellationToken)
    {
        DepositResponse result = await _depositService.RefundAsync(id, request, cancellationToken);
        return Ok(ApiResponse.Ok(result, ApiMessages.Success));
    }
}
