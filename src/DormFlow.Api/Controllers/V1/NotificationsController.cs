using Asp.Versioning;
using DormFlow.Api.Authorization;
using DormFlow.Application.Common.Authorization;
using DormFlow.Application.Features.Notifications;
using DormFlow.Contracts.Common;
using DormFlow.Contracts.Notifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DormFlow.Api.Controllers.V1;

/// <summary>Endpoint ສໍາລັບການແຈ້ງການ</summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/notifications")]
[Produces("application/json")]
public sealed class NotificationsController(INotificationService notificationService) : ControllerBase
{
    private readonly INotificationService _notificationService = notificationService;

    /// <summary>ດຶງລາຍຊື່ການແຈ້ງເຕືອນແບບ paginated ຂອງທ່ານເອງ</summary>
    [HttpGet]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<NotificationResponse>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<PagedResult<NotificationResponse>>>> GetMyList(
        [FromQuery] NotificationListRequest request,
        CancellationToken cancellationToken)
    {
        PagedResult<NotificationResponse> result = await _notificationService.GetMyPagedAsync(request, cancellationToken);
        return Ok(ApiResponse.Paged(result));
    }

    /// <summary>ໝາຍການແຈ້ງເຕືອນຂອງທ່ານວ່າອ່ານແລ້ວ</summary>
    [HttpPost("{id:guid}/read")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<NotificationResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<NotificationResponse>>> MarkAsRead(
        Guid id,
        CancellationToken cancellationToken)
    {
        NotificationResponse result = await _notificationService.MarkAsReadAsync(id, cancellationToken);
        return Ok(ApiResponse.Ok(result, ApiMessages.Success));
    }

    /// <summary>ສົ່ງການແຈ້ງເຕືອນໃຫ້ຜູ້ໃຊ້ຄົນດຽວ</summary>
    [HttpPost]
    [RequirePermission(PermissionCodes.Notifications.Send)]
    [ProducesResponseType(typeof(ApiResponse<NotificationResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<NotificationResponse>>> Send(
        [FromBody] SendNotificationRequest request,
        CancellationToken cancellationToken)
    {
        NotificationResponse result = await _notificationService.SendAsync(request, cancellationToken);
        return Ok(ApiResponse.Ok(result, ApiMessages.Created));
    }
}
