using Asp.Versioning;
using DormFlow.Api.Authorization;
using DormFlow.Application.Common.Authorization;
using DormFlow.Application.Features.Dormitories;
using DormFlow.Contracts.Common;
using DormFlow.Contracts.Dormitories;
using DormFlow.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace DormFlow.Api.Controllers.V1;

/// <summary>Endpoint สำหรับจัดการหอพัก</summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/dormitories")]
[Produces("application/json")]
public sealed class DormitoriesController(IDormitoryService dormitoryService) : ControllerBase
{
    private readonly IDormitoryService _dormitoryService = dormitoryService;

    /// <summary>ดึงรายการหอพักแบบแบ่งหน้า</summary>
    /// <param name="request">พารามิเตอร์การค้นหา</param>
    /// <param name="cancellationToken">โทเคนสำหรับยกเลิกคำสั่ง</param>
    /// <response code="200">รายการหอพัก</response>
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

    /// <summary>อ่านข้อมูลหอพักหนึ่งแห่ง</summary>
    /// <param name="id">รหัสหอพัก</param>
    /// <param name="cancellationToken">โทเคนสำหรับยกเลิกคำสั่ง</param>
    /// <response code="200">ข้อมูลหอพัก</response>
    /// <response code="404">ไม่พบหอพักที่ระบุ</response>
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

    /// <summary>สร้างหอพักใหม่</summary>
    /// <param name="request">ข้อมูลหอพัก</param>
    /// <param name="cancellationToken">โทเคนสำหรับยกเลิกคำสั่ง</param>
    /// <response code="200">สร้างหอพักสำเร็จ</response>
    /// <response code="400">ข้อมูลไม่ถูกต้อง</response>
    /// <response code="409">รหัสหอพักซ้ำกับที่มีอยู่แล้วในองค์กรเดียวกัน</response>
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

    /// <summary>แก้ไขข้อมูลหอพัก</summary>
    /// <param name="id">รหัสหอพักที่ต้องการแก้ไข</param>
    /// <param name="request">ข้อมูลใหม่</param>
    /// <param name="cancellationToken">โทเคนสำหรับยกเลิกคำสั่ง</param>
    /// <response code="200">แก้ไขสำเร็จ</response>
    /// <response code="400">ข้อมูลไม่ถูกต้อง</response>
    /// <response code="404">ไม่พบหอพักที่ระบุ</response>
    /// <response code="409">รหัสหอพักซ้ำกับที่มีอยู่แล้วในองค์กรเดียวกัน</response>
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

    /// <summary>เปลี่ยนสถานะหอพัก</summary>
    /// <param name="id">รหัสหอพักที่ต้องการเปลี่ยนสถานะ</param>
    /// <param name="request">สถานะใหม่</param>
    /// <param name="cancellationToken">โทเคนสำหรับยกเลิกคำสั่ง</param>
    /// <response code="200">เปลี่ยนสถานะสำเร็จ</response>
    /// <response code="400">สถานะที่ระบุไม่ถูกต้อง</response>
    /// <response code="404">ไม่พบหอพักที่ระบุ</response>
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

    /// <summary>ลบหอพัก</summary>
    /// <param name="id">รหัสหอพักที่ต้องการลบ</param>
    /// <param name="cancellationToken">โทเคนสำหรับยกเลิกคำสั่ง</param>
    /// <response code="200">ลบสำเร็จ</response>
    /// <response code="404">ไม่พบหอพักที่ระบุ</response>
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
