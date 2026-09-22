using Asp.Versioning;
using DormFlow.Api.Authorization;
using DormFlow.Application.Common.Authorization;
using DormFlow.Application.Features.Contracts;
using DormFlow.Contracts.Common;
using DormFlow.Contracts.Leases;
using Microsoft.AspNetCore.Mvc;

namespace DormFlow.Api.Controllers.V1;

/// <summary>
/// Endpoint ສໍາລັບການຄຸ້ມຄອງສັນຍາເຊົ່າ
/// </summary>
/// <remarks>
/// Check-in ແລະ check-out ແມ່ນເອີ້ນວ່າ <c>activate</c> ແລະ <c>terminate</c> ກ່ຽວກັບສັນຍາສະບັບນີ້
/// ບໍ່ມີ Endpoint ແຍກຕ່າງຫາກ, ເບິ່ງເຫດຜົນເພີ່ມເຕີມຢູ່ <see cref="Domain.Entities.Contract"/>
/// </remarks>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/contracts")]
[Produces("application/json")]
public sealed class ContractsController(IContractService contractService) : ControllerBase
{
    private readonly IContractService _contractService = contractService;

    /// <summary>ດຶງເອົາບັນຊີລາຍຊື່ paginated ຂອງສັນຍາເຊົ່າ</summary>
    [HttpGet]
    [RequirePermission(PermissionCodes.Contracts.View)]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<ContractResponse>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<PagedResult<ContractResponse>>>> GetList(
        [FromQuery] ContractListRequest request,
        CancellationToken cancellationToken)
    {
        PagedResult<ContractResponse> result = await _contractService.GetPagedAsync(request, cancellationToken);
        return Ok(ApiResponse.Paged(result));
    }

    /// <summary>ອ່ານສັນຍາເຊົ່າຜຶ່ງ</summary>
    [HttpGet("{id:guid}")]
    [RequirePermission(PermissionCodes.Contracts.View)]
    [ProducesResponseType(typeof(ApiResponse<ContractResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<ContractResponse>>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        ContractResponse result = await _contractService.GetByIdAsync(id, cancellationToken);
        return Ok(ApiResponse.Ok(result, ApiMessages.Retrieved));
    }

    /// <summary>ສ້າງສັນຍາເຊົ່າໃໝ່ຢູ່ໃນລັດສະບັບຮ່າງ</summary>
    [HttpPost]
    [RequirePermission(PermissionCodes.Contracts.Create)]
    [ProducesResponseType(typeof(ApiResponse<ContractResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ApiResponse<ContractResponse>>> Create(
        [FromBody] CreateContractRequest request,
        CancellationToken cancellationToken)
    {
        ContractResponse result = await _contractService.CreateAsync(request, cancellationToken);
        return Ok(ApiResponse.Ok(result, ApiMessages.Created));
    }

    /// <summary>ເປີດໃຊ້ຂໍ້ຕົກລົງການເຊົ່າ (ເຊັກອິນ)</summary>
    [HttpPost("{id:guid}/activate")]
    [RequirePermission(PermissionCodes.Contracts.Activate)]
    [ProducesResponseType(typeof(ApiResponse<ContractResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ApiResponse<ContractResponse>>> Activate(
        Guid id,
        CancellationToken cancellationToken)
    {
        ContractResponse result = await _contractService.ActivateAsync(id, cancellationToken);
        return Ok(ApiResponse.Ok(result, ApiMessages.Success));
    }

    /// <summary>ສິ້ນສຸດສັນຍາເຊົ່າ (ກວດເບິ່ງ)</summary>
    [HttpPost("{id:guid}/terminate")]
    [RequirePermission(PermissionCodes.Contracts.Terminate)]
    [ProducesResponseType(typeof(ApiResponse<ContractResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ApiResponse<ContractResponse>>> Terminate(
        Guid id,
        [FromBody] TerminateContractRequest request,
        CancellationToken cancellationToken)
    {
        ContractResponse result = await _contractService.TerminateAsync(id, request, cancellationToken);
        return Ok(ApiResponse.Ok(result, ApiMessages.Success));
    }
}
