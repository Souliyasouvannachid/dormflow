using Asp.Versioning;
using DormFlow.Api.Authorization;
using DormFlow.Application.Common.Authorization;
using DormFlow.Application.Features.Expenses;
using DormFlow.Contracts.Common;
using DormFlow.Contracts.Expenses;
using Microsoft.AspNetCore.Mvc;

namespace DormFlow.Api.Controllers.V1;

/// <summary>Endpoint ສໍາລັບການຄຸ້ມຄອງຄ່າໃຊ້ຈ່າຍໃນການດໍາເນີນງານ</summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/expenses")]
[Produces("application/json")]
public sealed class ExpensesController(IExpenseService expenseService) : ControllerBase
{
    private readonly IExpenseService _expenseService = expenseService;

    /// <summary>ດຶງເອົາບັນຊີລາຍຈ່າຍທີ່ເປັນ paginated</summary>
    [HttpGet]
    [RequirePermission(PermissionCodes.Expenses.View)]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<ExpenseResponse>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<PagedResult<ExpenseResponse>>>> GetList(
        [FromQuery] ExpenseListRequest request,
        CancellationToken cancellationToken)
    {
        PagedResult<ExpenseResponse> result = await _expenseService.GetPagedAsync(request, cancellationToken);
        return Ok(ApiResponse.Paged(result));
    }

    /// <summary>ອ່ານຂໍ້ມູນຄ່າໃຊ້ຈ່າຍຜຶ່ງ</summary>
    [HttpGet("{id:guid}")]
    [RequirePermission(PermissionCodes.Expenses.View)]
    [ProducesResponseType(typeof(ApiResponse<ExpenseResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<ExpenseResponse>>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        ExpenseResponse result = await _expenseService.GetByIdAsync(id, cancellationToken);
        return Ok(ApiResponse.Ok(result, ApiMessages.Retrieved));
    }

    /// <summary>ບັນທຶກຄ່າໃຊ້ຈ່າຍໃຝ່</summary>
    [HttpPost]
    [RequirePermission(PermissionCodes.Expenses.Create)]
    [ProducesResponseType(typeof(ApiResponse<ExpenseResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<ExpenseResponse>>> Create(
        [FromBody] CreateExpenseRequest request,
        CancellationToken cancellationToken)
    {
        ExpenseResponse result = await _expenseService.CreateAsync(request, cancellationToken);
        return Ok(ApiResponse.Ok(result, ApiMessages.Created));
    }
}
