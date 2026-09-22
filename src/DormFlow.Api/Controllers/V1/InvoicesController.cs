using Asp.Versioning;
using DormFlow.Api.Authorization;
using DormFlow.Application.Common.Authorization;
using DormFlow.Application.Features.Invoices;
using DormFlow.Contracts.Common;
using DormFlow.Contracts.Invoices;
using Microsoft.AspNetCore.Mvc;

namespace DormFlow.Api.Controllers.V1;

/// <summary>Endpoint ສໍາລັບການຄຸ້ມຄອງໃບແຈ້ງຜີ້</summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/invoices")]
[Produces("application/json")]
public sealed class InvoicesController(IInvoiceService invoiceService) : ControllerBase
{
    private readonly IInvoiceService _invoiceService = invoiceService;

    /// <summary>ດຶງເອົາລາຍຊື່ໃບແຈ້ງໜີ້ທີ່ໄດ້ວາງໄວ້ແລ້ວ</summary>
    [HttpGet]
    [RequirePermission(PermissionCodes.Invoices.View)]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<InvoiceResponse>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<PagedResult<InvoiceResponse>>>> GetList(
        [FromQuery] InvoiceListRequest request,
        CancellationToken cancellationToken)
    {
        PagedResult<InvoiceResponse> result = await _invoiceService.GetPagedAsync(request, cancellationToken);
        return Ok(ApiResponse.Paged(result));
    }

    /// <summary>ອ່ານຂໍ້ມູນໃບແຈ້ງຜີ້</summary>
    [HttpGet("{id:guid}")]
    [RequirePermission(PermissionCodes.Invoices.View)]
    [ProducesResponseType(typeof(ApiResponse<InvoiceResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<InvoiceResponse>>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        InvoiceResponse result = await _invoiceService.GetByIdAsync(id, cancellationToken);
        return Ok(ApiResponse.Ok(result, ApiMessages.Retrieved));
    }

    /// <summary>ສ້າງໃບແຈ້ງໜີ້ໃໝ່ໃນສະຖານະສະບັບຮ່າງ</summary>
    [HttpPost]
    [RequirePermission(PermissionCodes.Invoices.Create)]
    [ProducesResponseType(typeof(ApiResponse<InvoiceResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ApiResponse<InvoiceResponse>>> Create(
        [FromBody] CreateInvoiceRequest request,
        CancellationToken cancellationToken)
    {
        InvoiceResponse result = await _invoiceService.CreateAsync(request, cancellationToken);
        return Ok(ApiResponse.Ok(result, ApiMessages.Created));
    }

    /// <summary>ອອກໃບແຈ້ງໜີ້ໃຫ້ຜູ້ເຊົ່າ</summary>
    [HttpPost("{id:guid}/issue")]
    [RequirePermission(PermissionCodes.Invoices.Issue)]
    [ProducesResponseType(typeof(ApiResponse<InvoiceResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ApiResponse<InvoiceResponse>>> Issue(
        Guid id,
        CancellationToken cancellationToken)
    {
        InvoiceResponse result = await _invoiceService.IssueAsync(id, cancellationToken);
        return Ok(ApiResponse.Ok(result, ApiMessages.Success));
    }

    /// <summary>ຍົກເລີກໃບແຈ້ງໜີ້</summary>
    [HttpPost("{id:guid}/cancel")]
    [RequirePermission(PermissionCodes.Invoices.Cancel)]
    [ProducesResponseType(typeof(ApiResponse<InvoiceResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ApiResponse<InvoiceResponse>>> Cancel(
        Guid id,
        CancellationToken cancellationToken)
    {
        InvoiceResponse result = await _invoiceService.CancelAsync(id, cancellationToken);
        return Ok(ApiResponse.Ok(result, ApiMessages.Success));
    }
}
