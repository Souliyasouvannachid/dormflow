using DormFlow.Application.Common.Abstractions;
using DormFlow.Application.Common.Authorization;
using DormFlow.Application.Common.Exceptions;
using DormFlow.Application.Common.Pagination;
using DormFlow.Application.Features.Rooms;
using DormFlow.Application.Features.Tenants;
using DormFlow.Contracts.Common;
using DormFlow.Contracts.Visitors;
using DormFlow.Domain.Entities;
using DormFlow.Domain.Enums;
using FluentValidation;

namespace DormFlow.Application.Features.Visitors;

/// <summary>Use Case ทั้งหมดของโมดูลผู้มาเยือน</summary>
public sealed class VisitorService(
    IVisitorRepository repository,
    IRoomRepository roomRepository,
    ITenantRepository tenantRepository,
    ICurrentUser currentUser,
    IDateTimeProvider dateTimeProvider,
    IAuditLogWriter auditLogWriter,
    IValidator<CheckInVisitorRequest> checkInValidator) : IVisitorService
{
    private const string AuditEntityName = "Visitor";
    private static readonly string[] AllowedSortFields = ["status", "checkInAt", "createdAt"];

    private readonly IVisitorRepository _repository = repository;
    private readonly IRoomRepository _roomRepository = roomRepository;
    private readonly ITenantRepository _tenantRepository = tenantRepository;
    private readonly ICurrentUser _currentUser = currentUser;
    private readonly IDateTimeProvider _dateTimeProvider = dateTimeProvider;
    private readonly IAuditLogWriter _auditLogWriter = auditLogWriter;
    private readonly IValidator<CheckInVisitorRequest> _checkInValidator = checkInValidator;

    /// <inheritdoc />
    public async Task<PagedResult<VisitorResponse>> GetPagedAsync(
        VisitorListRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        PagedQuery query = PagedQueryNormalizer.Normalize(request, AllowedSortFields, defaultSortBy: "checkInAt");

        VisitorListFilter filter = new(
            TenantScope.GetOrganizationRestrictionForListing(_currentUser),
            request.DormitoryId,
            request.RoomId,
            ParseStatusOrNull(request.Status),
            TenantScope.GetDormitoryRestrictionForListing(_currentUser));

        PagedResult<Visitor> result = await _repository.GetPagedAsync(filter, query, cancellationToken);

        return PagedResult.Create(
            (IReadOnlyList<VisitorResponse>)[.. result.Items.Select(ToResponse)],
            result.Page,
            result.PageSize,
            result.TotalItems);
    }

    /// <inheritdoc />
    public async Task<VisitorResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => ToResponse(await FindAccessibleAsync(id, cancellationToken));

    /// <inheritdoc />
    public async Task<VisitorResponse> CheckInAsync(
        CheckInVisitorRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        await _checkInValidator.ValidateAndThrowAsync(request, cancellationToken);

        Room room = await _roomRepository.GetByIdAsync(request.RoomId, cancellationToken)
            ?? throw new NotFoundException("ไม่พบห้องที่ระบุ");

        TenantScope.EnsureDormitoryAccess(
            _currentUser,
            room.OrganizationId,
            room.DormitoryId.GetValueOrDefault(),
            "ไม่พบห้องที่ระบุ");

        if (request.TenantId.HasValue)
        {
            Tenant tenant = await _tenantRepository.GetByIdAsync(request.TenantId.Value, cancellationToken)
                ?? throw new NotFoundException("ไม่พบข้อมูลผู้เช่าที่ระบุ");

            if (tenant.OrganizationId != room.OrganizationId)
            {
                throw new NotFoundException("ไม่พบข้อมูลผู้เช่าที่ระบุ");
            }
        }

        Visitor visitor = new()
        {
            OrganizationId = room.OrganizationId,
            DormitoryId = room.DormitoryId,
            RoomId = room.Id,
            TenantId = request.TenantId,
            VisitorName = request.VisitorName.Trim(),
            VisitorPhone = request.VisitorPhone,
            IdNumber = request.IdNumber,
            Purpose = request.Purpose,
            CheckInAt = _dateTimeProvider.UtcNow,
            Notes = request.Notes,
        };

        await _repository.AddAsync(visitor, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        await _auditLogWriter.WriteAsync(
            AuditAction.Create,
            AuditEntityName,
            visitor.Id.ToString(),
            organizationId: visitor.OrganizationId,
            cancellationToken: cancellationToken);

        return ToResponse(visitor);
    }

    /// <inheritdoc />
    public async Task<VisitorResponse> CheckOutAsync(Guid id, CancellationToken cancellationToken = default)
    {
        Visitor visitor = await FindAccessibleAsync(id, cancellationToken);

        if (visitor.Status != VisitorStatus.CheckedIn)
        {
            throw new ConflictException("ผู้มาเยือนรายนี้ออกจากหอพักไปแล้ว");
        }

        visitor.Status = VisitorStatus.CheckedOut;
        visitor.CheckOutAt = _dateTimeProvider.UtcNow;

        await _repository.SaveChangesAsync(cancellationToken);

        await _auditLogWriter.WriteAsync(
            AuditAction.ChangeStatus,
            AuditEntityName,
            visitor.Id.ToString(),
            organizationId: visitor.OrganizationId,
            newValues: $$"""{"status":"{{VisitorStatus.CheckedOut}}"}""",
            cancellationToken: cancellationToken);

        return ToResponse(visitor);
    }

    private async Task<Visitor> FindAccessibleAsync(Guid id, CancellationToken cancellationToken)
    {
        Visitor visitor = await _repository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException("ไม่พบข้อมูลผู้มาเยือนที่ต้องการ");

        TenantScope.EnsureDormitoryAccess(
            _currentUser,
            visitor.OrganizationId,
            visitor.DormitoryId.GetValueOrDefault(),
            "ไม่พบข้อมูลผู้มาเยือนที่ต้องการ");

        return visitor;
    }

    private static VisitorStatus? ParseStatusOrNull(string? status)
        => !string.IsNullOrWhiteSpace(status) && Enum.TryParse(status, ignoreCase: true, out VisitorStatus parsed)
            ? parsed
            : null;

    private static VisitorResponse ToResponse(Visitor visitor) => new()
    {
        Id = visitor.Id,
        OrganizationId = visitor.OrganizationId,
        DormitoryId = visitor.DormitoryId.GetValueOrDefault(),
        RoomId = visitor.RoomId,
        TenantId = visitor.TenantId,
        VisitorName = visitor.VisitorName,
        VisitorPhone = visitor.VisitorPhone,
        IdNumber = visitor.IdNumber,
        Purpose = visitor.Purpose,
        CheckInAt = visitor.CheckInAt,
        Status = visitor.Status.ToString(),
        CheckOutAt = visitor.CheckOutAt,
        Notes = visitor.Notes,
        CreatedAt = visitor.CreatedAt,
    };
}
