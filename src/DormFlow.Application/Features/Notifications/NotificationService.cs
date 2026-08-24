using DormFlow.Application.Common.Abstractions;
using DormFlow.Application.Common.Exceptions;
using DormFlow.Application.Common.Models;
using DormFlow.Application.Common.Pagination;
using DormFlow.Application.Features.Users;
using DormFlow.Contracts.Common;
using DormFlow.Contracts.Notifications;
using DormFlow.Domain.Entities;
using DormFlow.Domain.Enums;
using FluentValidation;

namespace DormFlow.Application.Features.Notifications;

/// <summary>Use Case ทั้งหมดของโมดูลการแจ้งเตือน</summary>
public sealed class NotificationService(
    INotificationRepository repository,
    IUserAdministrationRepository userAdministrationRepository,
    ICurrentUser currentUser,
    IDateTimeProvider dateTimeProvider,
    IAuditLogWriter auditLogWriter,
    IValidator<SendNotificationRequest> sendValidator) : INotificationService
{
    private const string AuditEntityName = "Notification";
    private static readonly string[] AllowedSortFields = ["isRead", "createdAt"];

    private readonly INotificationRepository _repository = repository;
    private readonly IUserAdministrationRepository _userAdministrationRepository = userAdministrationRepository;
    private readonly ICurrentUser _currentUser = currentUser;
    private readonly IDateTimeProvider _dateTimeProvider = dateTimeProvider;
    private readonly IAuditLogWriter _auditLogWriter = auditLogWriter;
    private readonly IValidator<SendNotificationRequest> _sendValidator = sendValidator;

    /// <inheritdoc />
    public async Task<PagedResult<NotificationResponse>> GetMyPagedAsync(
        NotificationListRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        Guid recipientUserId = RequireAuthenticatedUserId();
        PagedQuery query = PagedQueryNormalizer.Normalize(request, AllowedSortFields, defaultSortBy: "createdAt");

        NotificationListFilter filter = new(recipientUserId, request.IsRead);

        PagedResult<Notification> result = await _repository.GetPagedAsync(filter, query, cancellationToken);

        return PagedResult.Create(
            (IReadOnlyList<NotificationResponse>)[.. result.Items.Select(ToResponse)],
            result.Page,
            result.PageSize,
            result.TotalItems);
    }

    /// <inheritdoc />
    public async Task<NotificationResponse> MarkAsReadAsync(Guid id, CancellationToken cancellationToken = default)
    {
        Guid recipientUserId = RequireAuthenticatedUserId();

        Notification notification = await _repository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException("ไม่พบการแจ้งเตือนที่ต้องการ");

        if (notification.RecipientUserId != recipientUserId)
        {
            throw new NotFoundException("ไม่พบการแจ้งเตือนที่ต้องการ");
        }

        if (!notification.IsRead)
        {
            notification.IsRead = true;
            notification.ReadAt = _dateTimeProvider.UtcNow;

            await _repository.SaveChangesAsync(cancellationToken);
        }

        return ToResponse(notification);
    }

    /// <inheritdoc />
    public async Task<NotificationResponse> SendAsync(
        SendNotificationRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        await _sendValidator.ValidateAndThrowAsync(request, cancellationToken);

        UserSecurityProfile recipient = await _userAdministrationRepository.GetByIdAsync(
                request.RecipientUserId,
                cancellationToken)
            ?? throw new NotFoundException("ไม่พบผู้ใช้ที่ระบุ");

        if (!_currentUser.IsSuperAdmin && recipient.OrganizationId != _currentUser.OrganizationId)
        {
            throw new NotFoundException("ไม่พบผู้ใช้ที่ระบุ");
        }

        Notification notification = new()
        {
            OrganizationId = recipient.OrganizationId,
            RecipientUserId = recipient.UserId,
            Title = request.Title.Trim(),
            Message = request.Message.Trim(),
        };

        await _repository.AddAsync(notification, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        await _auditLogWriter.WriteAsync(
            AuditAction.Create,
            AuditEntityName,
            notification.Id.ToString(),
            organizationId: notification.OrganizationId,
            cancellationToken: cancellationToken);

        return ToResponse(notification);
    }

    private Guid RequireAuthenticatedUserId()
        => _currentUser.UserId ?? throw new UnauthorizedAccessException("กรุณาเข้าสู่ระบบก่อนใช้งานคำสั่งนี้");

    private static NotificationResponse ToResponse(Notification notification) => new()
    {
        Id = notification.Id,
        RecipientUserId = notification.RecipientUserId,
        Title = notification.Title,
        Message = notification.Message,
        IsRead = notification.IsRead,
        ReadAt = notification.ReadAt,
        CreatedAt = notification.CreatedAt,
    };
}
