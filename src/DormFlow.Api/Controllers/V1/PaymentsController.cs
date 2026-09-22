using Asp.Versioning;
using DormFlow.Api.Authorization;
using DormFlow.Application.Common.Authorization;
using DormFlow.Application.Features.Payments;
using DormFlow.Contracts.Common;
using DormFlow.Contracts.Payments;
using Microsoft.AspNetCore.Mvc;

namespace DormFlow.Api.Controllers.V1;

/// <summary>Endpoint ສໍາລັບການຄຸ້ມຄອງການຈ່າຍເງິນ</summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/payments")]
[Produces("application/json")]
public sealed class PaymentsController(IPaymentService paymentService) : ControllerBase
{
    private readonly IPaymentService _paymentService = paymentService;

    /// <summary>ດຶງເອົາລາຍການການຈ່າຍເງິນທີ່ໄດ້ວາງໄວ້</summary>
    [HttpGet]
    [RequirePermission(PermissionCodes.Payments.View)]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<PaymentResponse>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<PagedResult<PaymentResponse>>>> GetList(
        [FromQuery] PaymentListRequest request,
        CancellationToken cancellationToken)
    {
        PagedResult<PaymentResponse> result = await _paymentService.GetPagedAsync(request, cancellationToken);
        return Ok(ApiResponse.Paged(result));
    }

    /// <summary>ອ່ານຂໍ້ມູນກ່ຽວກັບໜຶ່ງລາຍການການຈ່າຍເງິນ</summary>
    [HttpGet("{id:guid}")]
    [RequirePermission(PermissionCodes.Payments.View)]
    [ProducesResponseType(typeof(ApiResponse<PaymentResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<PaymentResponse>>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        PaymentResponse result = await _paymentService.GetByIdAsync(id, cancellationToken);
        return Ok(ApiResponse.Ok(result, ApiMessages.Retrieved));
    }

    /// <summary>ບັນທຶກການຈ່າຍເງິນໃໝ່ໃນສະຖານະການຢືນຢັນທີ່ຍັງຄ້າງຢູ່</summary>
    [HttpPost]
    [RequirePermission(PermissionCodes.Payments.Create)]
    [ProducesResponseType(typeof(ApiResponse<PaymentResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ApiResponse<PaymentResponse>>> Create(
        [FromBody] CreatePaymentRequest request,
        CancellationToken cancellationToken)
    {
        PaymentResponse result = await _paymentService.CreateAsync(request, cancellationToken);
        return Ok(ApiResponse.Ok(result, ApiMessages.Created));
    }

    /// <summary>ຢືນຢັນລາຍການການຈ່າຍເງິນ ເອົາຈໍານວນເພື່ອຫັກໃບແຈ້ງໜີ້</summary>
    [HttpPost("{id:guid}/confirm")]
    [RequirePermission(PermissionCodes.Payments.Approve)]
    [ProducesResponseType(typeof(ApiResponse<PaymentResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ApiResponse<PaymentResponse>>> Confirm(
        Guid id,
        CancellationToken cancellationToken)
    {
        PaymentResponse result = await _paymentService.ConfirmAsync(id, cancellationToken);
        return Ok(ApiResponse.Ok(result, ApiMessages.Success));
    }

    /// <summary>ເງິນຄືນ</summary>
    [HttpPost("{id:guid}/refund")]
    [RequirePermission(PermissionCodes.Payments.Refund)]
    [ProducesResponseType(typeof(ApiResponse<PaymentResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ApiResponse<PaymentResponse>>> Refund(
        Guid id,
        [FromBody] RefundPaymentRequest request,
        CancellationToken cancellationToken)
    {
        PaymentResponse result = await _paymentService.RefundAsync(id, request, cancellationToken);
        return Ok(ApiResponse.Ok(result, ApiMessages.Success));
    }
}
