using Asp.Versioning;
using DormFlow.Api.Authorization;
using DormFlow.Application.Common.Authorization;
using DormFlow.Application.Features.Users;
using DormFlow.Contracts.Common;
using DormFlow.Contracts.Users;
using DormFlow.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace DormFlow.Api.Controllers.V1;

/// <summary>Endpoint ສໍາລັບການຄຸ້ມຄອງບັນຊີຜູ້ໃຊ້</summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/users")]
[Produces("application/json")]
[RequirePermission(PermissionCodes.Administration.ManageUsers)]
public sealed class UsersController(IUserManagementService userManagementService) : ControllerBase
{
    private readonly IUserManagementService _userManagementService = userManagementService;

    /// <summary>ດຶງເອົາລາຍຊື່ຜູ້ໃຊ້ທີ່ຕັ້ງໄວ້ແລ້ວ</summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<UserResponse>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<PagedResult<UserResponse>>>> GetList(
        [FromQuery] UserListRequest request,
        CancellationToken cancellationToken)
    {
        PagedResult<UserResponse> result = await _userManagementService.GetPagedAsync(request, cancellationToken);
        return Ok(ApiResponse.Paged(result));
    }

    /// <summary>ອ່ານຂໍ້ມູນຂອງບັນຊີຜູ້ໃຊ້ຜຶ່ງ</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<UserResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<UserResponse>>> GetById(Guid id, CancellationToken cancellationToken)
    {
        UserResponse result = await _userManagementService.GetByIdAsync(id, cancellationToken);
        return Ok(ApiResponse.Ok(result, ApiMessages.Retrieved));
    }

    /// <summary>ສ້າງບັນຊີໃຝ່</summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<UserResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ApiResponse<UserResponse>>> Create(
        [FromBody] CreateUserRequest request,
        CancellationToken cancellationToken)
    {
        UserResponse result = await _userManagementService.CreateAsync(request, cancellationToken);
        return Ok(ApiResponse.Ok(result, ApiMessages.Created));
    }

    /// <summary>ແກ້ໄຂຂໍ້ມູນໂປຣໄຟລ໌ຜູ້ໃຊ້</summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<UserResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<UserResponse>>> Update(
        Guid id,
        [FromBody] UpdateUserRequest request,
        CancellationToken cancellationToken)
    {
        UserResponse result = await _userManagementService.UpdateAsync(id, request, cancellationToken);
        return Ok(ApiResponse.Ok(result, ApiMessages.Updated));
    }

    /// <summary>ປ່ຽນສະຖານະບັນຊີຜູ້ໃຊ້</summary>
    [HttpPatch("{id:guid}/status")]
    [ProducesResponseType(typeof(ApiResponse<UserResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<UserResponse>>> ChangeStatus(
        Guid id,
        [FromBody] ChangeStatusRequest<UserStatus> request,
        CancellationToken cancellationToken)
    {
        UserResponse result = await _userManagementService.ChangeStatusAsync(
            id,
            request.Status.ToString(),
            cancellationToken);

        return Ok(ApiResponse.Ok(result, ApiMessages.Success));
    }

    /// <summary>ມອບໝາຍໜ້າທີ່ໃຫ້ຜູ້ໃຊ້ແທນໜ້າທີ່ທັງໝົດກ່ອນໜ້ານີ້</summary>
    [HttpPost("{id:guid}/roles")]
    [ProducesResponseType(typeof(ApiResponse<UserResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<UserResponse>>> AssignRoles(
        Guid id,
        [FromBody] AssignUserRolesRequest request,
        CancellationToken cancellationToken)
    {
        UserResponse result = await _userManagementService.AssignRolesAsync(id, request, cancellationToken);
        return Ok(ApiResponse.Ok(result, ApiMessages.Success));
    }

    /// <summary>ມອບໝາຍໃຫ້ຜູ້ໃຊ້ຈັດການໜຶ່ງຫໍພັກ</summary>
    [HttpPost("{id:guid}/dormitories")]
    [ProducesResponseType(typeof(ApiResponse<UserResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ApiResponse<UserResponse>>> AssignDormitory(
        Guid id,
        [FromBody] AssignUserDormitoryRequest request,
        CancellationToken cancellationToken)
    {
        UserResponse result = await _userManagementService.AssignDormitoryAsync(id, request, cancellationToken);
        return Ok(ApiResponse.Ok(result, ApiMessages.Success));
    }

    /// <summary>ເອົາການມອບໝາຍຫໍພັກອອກຈາກຜູ້ໃຊ້</summary>
    [HttpDelete("{id:guid}/dormitories/{dormitoryId:guid}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse>> RemoveDormitory(
        Guid id,
        Guid dormitoryId,
        CancellationToken cancellationToken)
    {
        await _userManagementService.RemoveDormitoryAsync(id, dormitoryId, cancellationToken);
        return Ok(ApiResponse.Ok(ApiMessages.Deleted));
    }
}
