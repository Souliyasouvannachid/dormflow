using Asp.Versioning;
using DormFlow.Api.Authorization;
using DormFlow.Application.Common.Authorization;
using DormFlow.Application.Features.Receipts;
using DormFlow.Contracts.Common;
using DormFlow.Contracts.Receipts;
using Microsoft.AspNetCore.Mvc;

namespace DormFlow.Api.Controllers.V1;

/// <summary>Endpoint สำหรับจัดการใบเสร็จรับเงิน</summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/receipts")]
[Produces("application/json")]
public sealed class ReceiptsController(IReceiptService receiptService) : ControllerBase
{
    private readonly IReceiptService _receiptService = receiptService;

    /// <summary>ดึงรายการใบเสร็จรับเงินแบบแบ่งหน้า</summary>
    [HttpGet]
    [RequirePermission(PermissionCodes.Receipts.View)]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<ReceiptResponse>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<PagedResult<ReceiptResponse>>>> GetList(
        [FromQuery] ReceiptListRequest request,
        CancellationToken cancellationToken)
    {
        PagedResult<ReceiptResponse> result = await _receiptService.GetPagedAsync(request, cancellationToken);
        return Ok(ApiResponse.Paged(result));
    }

    /// <summary>อ่านข้อมูลใบเสร็จรับเงินหนึ่งฉบับ</summary>
    [HttpGet("{id:guid}")]
    [RequirePermission(PermissionCodes.Receipts.View)]
    [ProducesResponseType(typeof(ApiResponse<ReceiptResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<ReceiptResponse>>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        ReceiptResponse result = await _receiptService.GetByIdAsync(id, cancellationToken);
        return Ok(ApiResponse.Ok(result, ApiMessages.Retrieved));
    }

    /// <summary>ออกใบเสร็จรับเงินใหม่</summary>
    [HttpPost]
    [RequirePermission(PermissionCodes.Receipts.Issue)]
    [ProducesResponseType(typeof(ApiResponse<ReceiptResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ApiResponse<ReceiptResponse>>> Issue(
        [FromBody] IssueReceiptRequest request,
        CancellationToken cancellationToken)
    {
        ReceiptResponse result = await _receiptService.IssueAsync(request, cancellationToken);
        return Ok(ApiResponse.Ok(result, ApiMessages.Created));
    }
}
