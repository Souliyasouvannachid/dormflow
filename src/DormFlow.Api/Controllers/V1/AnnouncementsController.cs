using Asp.Versioning;
using DormFlow.Api.Authorization;
using DormFlow.Application.Common.Authorization;
using DormFlow.Application.Features.Announcements;
using DormFlow.Contracts.Announcements;
using DormFlow.Contracts.Common;
using Microsoft.AspNetCore.Mvc;

namespace DormFlow.Api.Controllers.V1;

/// <summary>Endpoint ສໍາລັບການຄຸ້ມຄອງການປະກາດ</summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/announcements")]
[Produces("application/json")]
public sealed class AnnouncementsController(IAnnouncementService announcementService) : ControllerBase
{
    private readonly IAnnouncementService _announcementService = announcementService;

    /// <summary>ດຶງເອົາລາຍຊື່ການປະກາດແບບ paginated</summary>
    [HttpGet]
    [RequirePermission(PermissionCodes.Announcements.View)]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<AnnouncementResponse>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<PagedResult<AnnouncementResponse>>>> GetList(
        [FromQuery] AnnouncementListRequest request,
        CancellationToken cancellationToken)
    {
        PagedResult<AnnouncementResponse> result = await _announcementService.GetPagedAsync(request, cancellationToken);
        return Ok(ApiResponse.Paged(result));
    }

    /// <summary>ອ່ານປະກາດຜຶ່ງ</summary>
    [HttpGet("{id:guid}")]
    [RequirePermission(PermissionCodes.Announcements.View)]
    [ProducesResponseType(typeof(ApiResponse<AnnouncementResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<AnnouncementResponse>>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        AnnouncementResponse result = await _announcementService.GetByIdAsync(id, cancellationToken);
        return Ok(ApiResponse.Ok(result, ApiMessages.Retrieved));
    }

    /// <summary>ສ້າງການປະກາດໃໝ່ໃນສະຖານະສະບັບຮ່າງ</summary>
    [HttpPost]
    [RequirePermission(PermissionCodes.Announcements.Create)]
    [ProducesResponseType(typeof(ApiResponse<AnnouncementResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<AnnouncementResponse>>> Create(
        [FromBody] CreateAnnouncementRequest request,
        CancellationToken cancellationToken)
    {
        AnnouncementResponse result = await _announcementService.CreateAsync(request, cancellationToken);
        return Ok(ApiResponse.Ok(result, ApiMessages.Created));
    }

    /// <summary>ປະກາດ</summary>
    [HttpPost("{id:guid}/publish")]
    [RequirePermission(PermissionCodes.Announcements.Publish)]
    [ProducesResponseType(typeof(ApiResponse<AnnouncementResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ApiResponse<AnnouncementResponse>>> Publish(
        Guid id,
        CancellationToken cancellationToken)
    {
        AnnouncementResponse result = await _announcementService.PublishAsync(id, cancellationToken);
        return Ok(ApiResponse.Ok(result, ApiMessages.Success));
    }

    /// <summary>ປະກາດການເກັບຮັກສາໃນສາງ</summary>
    [HttpPost("{id:guid}/archive")]
    [RequirePermission(PermissionCodes.Announcements.Publish)]
    [ProducesResponseType(typeof(ApiResponse<AnnouncementResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ApiResponse<AnnouncementResponse>>> Archive(
        Guid id,
        CancellationToken cancellationToken)
    {
        AnnouncementResponse result = await _announcementService.ArchiveAsync(id, cancellationToken);
        return Ok(ApiResponse.Ok(result, ApiMessages.Success));
    }
}
