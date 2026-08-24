using Asp.Versioning;
using DormFlow.Api.Authorization;
using DormFlow.Application.Common.Authorization;
using DormFlow.Application.Features.Contracts;
using DormFlow.Contracts.Common;
using DormFlow.Contracts.Leases;
using Microsoft.AspNetCore.Mvc;

namespace DormFlow.Api.Controllers.V1;

/// <summary>
/// Endpoint สำหรับจัดการสัญญาเช่า
/// </summary>
/// <remarks>
/// เช็คอินและเช็คเอาท์คือการเรียก <c>activate</c> และ <c>terminate</c> บนสัญญานี้
/// ไม่มี Endpoint แยกต่างหาก ดูเหตุผลเพิ่มเติมที่ <see cref="Domain.Entities.Contract"/>
/// </remarks>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/contracts")]
[Produces("application/json")]
public sealed class ContractsController(IContractService contractService) : ControllerBase
{
    private readonly IContractService _contractService = contractService;

    /// <summary>ดึงรายการสัญญาเช่าแบบแบ่งหน้า</summary>
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

    /// <summary>อ่านข้อมูลสัญญาเช่าหนึ่งฉบับ</summary>
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

    /// <summary>สร้างสัญญาเช่าใหม่ในสถานะร่าง</summary>
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

    /// <summary>เปิดใช้งานสัญญาเช่า (เช็คอิน)</summary>
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

    /// <summary>สิ้นสุดสัญญาเช่า (เช็คเอาท์)</summary>
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
