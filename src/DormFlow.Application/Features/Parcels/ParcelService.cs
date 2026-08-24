using DormFlow.Application.Common.Abstractions;
using DormFlow.Application.Common.Authorization;
using DormFlow.Application.Common.Exceptions;
using DormFlow.Application.Common.Pagination;
using DormFlow.Application.Features.Rooms;
using DormFlow.Application.Features.Tenants;
using DormFlow.Contracts.Common;
using DormFlow.Contracts.Parcels;
using DormFlow.Domain.Entities;
using DormFlow.Domain.Enums;
using FluentValidation;

namespace DormFlow.Application.Features.Parcels;

/// <summary>Use Case ทั้งหมดของโมดูลพัสดุ</summary>
public sealed class ParcelService(
    IParcelRepository repository,
    IRoomRepository roomRepository,
    ITenantRepository tenantRepository,
    ICurrentUser currentUser,
    IDateTimeProvider dateTimeProvider,
    IAuditLogWriter auditLogWriter,
    IValidator<LogParcelRequest> logValidator,
    IValidator<PickupParcelRequest> pickupValidator) : IParcelService
{
    private const string AuditEntityName = "Parcel";
    private static readonly string[] AllowedSortFields = ["status", "receivedAt", "createdAt"];

    private readonly IParcelRepository _repository = repository;
    private readonly IRoomRepository _roomRepository = roomRepository;
    private readonly ITenantRepository _tenantRepository = tenantRepository;
    private readonly ICurrentUser _currentUser = currentUser;
    private readonly IDateTimeProvider _dateTimeProvider = dateTimeProvider;
    private readonly IAuditLogWriter _auditLogWriter = auditLogWriter;
    private readonly IValidator<LogParcelRequest> _logValidator = logValidator;
    private readonly IValidator<PickupParcelRequest> _pickupValidator = pickupValidator;

    /// <inheritdoc />
    public async Task<PagedResult<ParcelResponse>> GetPagedAsync(
        ParcelListRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        PagedQuery query = PagedQueryNormalizer.Normalize(request, AllowedSortFields, defaultSortBy: "receivedAt");

        ParcelListFilter filter = new(
            TenantScope.GetOrganizationRestrictionForListing(_currentUser),
            request.DormitoryId,
            request.RoomId,
            ParseStatusOrNull(request.Status),
            TenantScope.GetDormitoryRestrictionForListing(_currentUser));

        PagedResult<Parcel> result = await _repository.GetPagedAsync(filter, query, cancellationToken);

        return PagedResult.Create(
            (IReadOnlyList<ParcelResponse>)[.. result.Items.Select(ToResponse)],
            result.Page,
            result.PageSize,
            result.TotalItems);
    }

    /// <inheritdoc />
    public async Task<ParcelResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => ToResponse(await FindAccessibleAsync(id, cancellationToken));

    /// <inheritdoc />
    public async Task<ParcelResponse> LogAsync(
        LogParcelRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        await _logValidator.ValidateAndThrowAsync(request, cancellationToken);

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

        Parcel parcel = new()
        {
            OrganizationId = room.OrganizationId,
            DormitoryId = room.DormitoryId,
            RoomId = room.Id,
            TenantId = request.TenantId,
            CourierName = request.CourierName,
            TrackingNumber = request.TrackingNumber,
            Description = request.Description,
            ReceivedAt = _dateTimeProvider.UtcNow,
            Notes = request.Notes,
        };

        await _repository.AddAsync(parcel, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        await _auditLogWriter.WriteAsync(
            AuditAction.Create,
            AuditEntityName,
            parcel.Id.ToString(),
            organizationId: parcel.OrganizationId,
            cancellationToken: cancellationToken);

        return ToResponse(parcel);
    }

    /// <inheritdoc />
    public async Task<ParcelResponse> PickupAsync(
        Guid id,
        PickupParcelRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        await _pickupValidator.ValidateAndThrowAsync(request, cancellationToken);

        Parcel parcel = await FindAccessibleAsync(id, cancellationToken);

        if (parcel.Status != ParcelStatus.Received)
        {
            throw new ConflictException("พัสดุนี้ถูกรับไปแล้ว");
        }

        parcel.Status = ParcelStatus.PickedUp;
        parcel.PickedUpAt = _dateTimeProvider.UtcNow;
        parcel.PickedUpByName = request.PickedUpByName;

        await _repository.SaveChangesAsync(cancellationToken);

        await _auditLogWriter.WriteAsync(
            AuditAction.ChangeStatus,
            AuditEntityName,
            parcel.Id.ToString(),
            organizationId: parcel.OrganizationId,
            newValues: $$"""{"status":"{{ParcelStatus.PickedUp}}"}""",
            cancellationToken: cancellationToken);

        return ToResponse(parcel);
    }

    private async Task<Parcel> FindAccessibleAsync(Guid id, CancellationToken cancellationToken)
    {
        Parcel parcel = await _repository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException("ไม่พบข้อมูลพัสดุที่ต้องการ");

        TenantScope.EnsureDormitoryAccess(
            _currentUser,
            parcel.OrganizationId,
            parcel.DormitoryId.GetValueOrDefault(),
            "ไม่พบข้อมูลพัสดุที่ต้องการ");

        return parcel;
    }

    private static ParcelStatus? ParseStatusOrNull(string? status)
        => !string.IsNullOrWhiteSpace(status) && Enum.TryParse(status, ignoreCase: true, out ParcelStatus parsed)
            ? parsed
            : null;

    private static ParcelResponse ToResponse(Parcel parcel) => new()
    {
        Id = parcel.Id,
        OrganizationId = parcel.OrganizationId,
        DormitoryId = parcel.DormitoryId.GetValueOrDefault(),
        RoomId = parcel.RoomId,
        TenantId = parcel.TenantId,
        CourierName = parcel.CourierName,
        TrackingNumber = parcel.TrackingNumber,
        Description = parcel.Description,
        ReceivedAt = parcel.ReceivedAt,
        Status = parcel.Status.ToString(),
        PickedUpAt = parcel.PickedUpAt,
        PickedUpByName = parcel.PickedUpByName,
        Notes = parcel.Notes,
        CreatedAt = parcel.CreatedAt,
    };
}
