using DormFlow.Application.Common.Abstractions;
using DormFlow.Application.Common.Authorization;
using DormFlow.Application.Common.Exceptions;
using DormFlow.Application.Common.Pagination;
using DormFlow.Application.Features.Dormitories;
using DormFlow.Contracts.Announcements;
using DormFlow.Contracts.Common;
using DormFlow.Domain.Entities;
using DormFlow.Domain.Enums;
using FluentValidation;

namespace DormFlow.Application.Features.Announcements;

/// <summary>Use Case ทั้งหมดของโมดูลประกาศ</summary>
public sealed class AnnouncementService(
    IAnnouncementRepository repository,
    IDormitoryRepository dormitoryRepository,
    ICurrentUser currentUser,
    IDateTimeProvider dateTimeProvider,
    IAuditLogWriter auditLogWriter,
    IValidator<CreateAnnouncementRequest> createValidator) : IAnnouncementService
{
    private const string AuditEntityName = "Announcement";
    private static readonly string[] AllowedSortFields = ["title", "status", "publishedAt", "createdAt"];

    private readonly IAnnouncementRepository _repository = repository;
    private readonly IDormitoryRepository _dormitoryRepository = dormitoryRepository;
    private readonly ICurrentUser _currentUser = currentUser;
    private readonly IDateTimeProvider _dateTimeProvider = dateTimeProvider;
    private readonly IAuditLogWriter _auditLogWriter = auditLogWriter;
    private readonly IValidator<CreateAnnouncementRequest> _createValidator = createValidator;

    /// <inheritdoc />
    public async Task<PagedResult<AnnouncementResponse>> GetPagedAsync(
        AnnouncementListRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        PagedQuery query = PagedQueryNormalizer.Normalize(request, AllowedSortFields, defaultSortBy: "createdAt");

        AnnouncementListFilter filter = new(
            TenantScope.GetOrganizationRestrictionForListing(_currentUser),
            request.DormitoryId,
            ParseStatusOrNull(request.Status),
            TenantScope.GetDormitoryRestrictionForListing(_currentUser));

        PagedResult<Announcement> result = await _repository.GetPagedAsync(filter, query, cancellationToken);

        return PagedResult.Create(
            (IReadOnlyList<AnnouncementResponse>)[.. result.Items.Select(ToResponse)],
            result.Page,
            result.PageSize,
            result.TotalItems);
    }

    /// <inheritdoc />
    public async Task<AnnouncementResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => ToResponse(await FindAccessibleAsync(id, cancellationToken));

    /// <inheritdoc />
    public async Task<AnnouncementResponse> CreateAsync(
        CreateAnnouncementRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        await _createValidator.ValidateAndThrowAsync(request, cancellationToken);

        Dormitory dormitory = await _dormitoryRepository.GetByIdAsync(request.DormitoryId, cancellationToken)
            ?? throw new NotFoundException("ไม่พบหอพักที่ระบุ");

        TenantScope.EnsureDormitoryAccess(_currentUser, dormitory.OrganizationId, dormitory.Id, "ไม่พบหอพักที่ระบุ");

        Announcement announcement = new()
        {
            OrganizationId = dormitory.OrganizationId,
            DormitoryId = dormitory.Id,
            Title = request.Title.Trim(),
            Content = request.Content.Trim(),
            ExpiresAt = request.ExpiresAt,
            Notes = request.Notes,
        };

        await _repository.AddAsync(announcement, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        await _auditLogWriter.WriteAsync(
            AuditAction.Create,
            AuditEntityName,
            announcement.Id.ToString(),
            organizationId: announcement.OrganizationId,
            cancellationToken: cancellationToken);

        return ToResponse(announcement);
    }

    /// <inheritdoc />
    public async Task<AnnouncementResponse> PublishAsync(Guid id, CancellationToken cancellationToken = default)
    {
        Announcement announcement = await FindAccessibleAsync(id, cancellationToken);

        if (announcement.Status != AnnouncementStatus.Draft)
        {
            throw new ConflictException("เผยแพร่ได้เฉพาะประกาศที่ยังเป็นร่างเท่านั้น");
        }

        announcement.Status = AnnouncementStatus.Published;
        announcement.PublishedAt = _dateTimeProvider.UtcNow;

        await _repository.SaveChangesAsync(cancellationToken);

        await _auditLogWriter.WriteAsync(
            AuditAction.ChangeStatus,
            AuditEntityName,
            announcement.Id.ToString(),
            organizationId: announcement.OrganizationId,
            newValues: $$"""{"status":"{{AnnouncementStatus.Published}}"}""",
            cancellationToken: cancellationToken);

        return ToResponse(announcement);
    }

    /// <inheritdoc />
    public async Task<AnnouncementResponse> ArchiveAsync(Guid id, CancellationToken cancellationToken = default)
    {
        Announcement announcement = await FindAccessibleAsync(id, cancellationToken);

        if (announcement.Status != AnnouncementStatus.Published)
        {
            throw new ConflictException("เก็บเข้าคลังได้เฉพาะประกาศที่เผยแพร่แล้วเท่านั้น");
        }

        announcement.Status = AnnouncementStatus.Archived;

        await _repository.SaveChangesAsync(cancellationToken);

        await _auditLogWriter.WriteAsync(
            AuditAction.ChangeStatus,
            AuditEntityName,
            announcement.Id.ToString(),
            organizationId: announcement.OrganizationId,
            newValues: $$"""{"status":"{{AnnouncementStatus.Archived}}"}""",
            cancellationToken: cancellationToken);

        return ToResponse(announcement);
    }

    private async Task<Announcement> FindAccessibleAsync(Guid id, CancellationToken cancellationToken)
    {
        Announcement announcement = await _repository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException("ไม่พบประกาศที่ต้องการ");

        TenantScope.EnsureDormitoryAccess(
            _currentUser,
            announcement.OrganizationId,
            announcement.DormitoryId.GetValueOrDefault(),
            "ไม่พบประกาศที่ต้องการ");

        return announcement;
    }

    private static AnnouncementStatus? ParseStatusOrNull(string? status)
        => !string.IsNullOrWhiteSpace(status) && Enum.TryParse(status, ignoreCase: true, out AnnouncementStatus parsed)
            ? parsed
            : null;

    private static AnnouncementResponse ToResponse(Announcement announcement) => new()
    {
        Id = announcement.Id,
        OrganizationId = announcement.OrganizationId,
        DormitoryId = announcement.DormitoryId.GetValueOrDefault(),
        Title = announcement.Title,
        Content = announcement.Content,
        Status = announcement.Status.ToString(),
        PublishedAt = announcement.PublishedAt,
        ExpiresAt = announcement.ExpiresAt,
        Notes = announcement.Notes,
        CreatedAt = announcement.CreatedAt,
    };
}
