using DormFlow.Application.Common.Abstractions;
using DormFlow.Application.Common.Authorization;
using DormFlow.Application.Common.Exceptions;
using DormFlow.Application.Common.Pagination;
using DormFlow.Application.Features.Rooms;
using DormFlow.Application.Features.Tenants;
using DormFlow.Contracts.Common;
using DormFlow.Contracts.Parking;
using DormFlow.Domain.Entities;
using DormFlow.Domain.Enums;
using FluentValidation;

namespace DormFlow.Application.Features.Parking;

/// <summary>Use Case ทั้งหมดของโมดูลจดทะเบียนที่จอดรถ</summary>
public sealed class ParkingRegistrationService(
    IParkingRegistrationRepository repository,
    IRoomRepository roomRepository,
    ITenantRepository tenantRepository,
    ICurrentUser currentUser,
    IDateTimeProvider dateTimeProvider,
    IAuditLogWriter auditLogWriter,
    IValidator<RegisterParkingRequest> registerValidator) : IParkingRegistrationService
{
    private const string AuditEntityName = "ParkingRegistration";
    private static readonly string[] AllowedSortFields = ["status", "registeredAt", "createdAt"];

    private readonly IParkingRegistrationRepository _repository = repository;
    private readonly IRoomRepository _roomRepository = roomRepository;
    private readonly ITenantRepository _tenantRepository = tenantRepository;
    private readonly ICurrentUser _currentUser = currentUser;
    private readonly IDateTimeProvider _dateTimeProvider = dateTimeProvider;
    private readonly IAuditLogWriter _auditLogWriter = auditLogWriter;
    private readonly IValidator<RegisterParkingRequest> _registerValidator = registerValidator;

    /// <inheritdoc />
    public async Task<PagedResult<ParkingRegistrationResponse>> GetPagedAsync(
        ParkingRegistrationListRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        PagedQuery query = PagedQueryNormalizer.Normalize(request, AllowedSortFields, defaultSortBy: "registeredAt");

        ParkingRegistrationListFilter filter = new(
            TenantScope.GetOrganizationRestrictionForListing(_currentUser),
            request.DormitoryId,
            request.TenantId,
            ParseStatusOrNull(request.Status),
            TenantScope.GetDormitoryRestrictionForListing(_currentUser));

        PagedResult<ParkingRegistration> result = await _repository.GetPagedAsync(filter, query, cancellationToken);

        return PagedResult.Create(
            (IReadOnlyList<ParkingRegistrationResponse>)[.. result.Items.Select(ToResponse)],
            result.Page,
            result.PageSize,
            result.TotalItems);
    }

    /// <inheritdoc />
    public async Task<ParkingRegistrationResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => ToResponse(await FindAccessibleAsync(id, cancellationToken));

    /// <inheritdoc />
    public async Task<ParkingRegistrationResponse> RegisterAsync(
        RegisterParkingRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        await _registerValidator.ValidateAndThrowAsync(request, cancellationToken);

        Room room = await _roomRepository.GetByIdAsync(request.RoomId, cancellationToken)
            ?? throw new NotFoundException("ไม่พบห้องที่ระบุ");

        TenantScope.EnsureDormitoryAccess(
            _currentUser,
            room.OrganizationId,
            room.DormitoryId.GetValueOrDefault(),
            "ไม่พบห้องที่ระบุ");

        Tenant tenant = await _tenantRepository.GetByIdAsync(request.TenantId, cancellationToken)
            ?? throw new NotFoundException("ไม่พบข้อมูลผู้เช่าที่ระบุ");

        if (tenant.OrganizationId != room.OrganizationId)
        {
            throw new NotFoundException("ไม่พบข้อมูลผู้เช่าที่ระบุ");
        }

        string licensePlate = request.LicensePlate.Trim();

        if (await _repository.HasActiveByLicensePlateAsync(room.OrganizationId, licensePlate, cancellationToken))
        {
            throw new ConflictException($"ทะเบียนรถ '{licensePlate}' มีการจดทะเบียนที่จอดรถที่ยังไม่สิ้นสุดอยู่แล้ว");
        }

        ParkingRegistration registration = new()
        {
            OrganizationId = room.OrganizationId,
            DormitoryId = room.DormitoryId,
            RoomId = room.Id,
            TenantId = tenant.Id,
            LicensePlate = licensePlate,
            VehicleType = Enum.Parse<VehicleType>(request.VehicleType, ignoreCase: true),
            Brand = request.Brand,
            Color = request.Color,
            ParkingSpotLabel = request.ParkingSpotLabel,
            RegisteredAt = _dateTimeProvider.UtcNow,
            Notes = request.Notes,
        };

        await _repository.AddAsync(registration, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        await _auditLogWriter.WriteAsync(
            AuditAction.Create,
            AuditEntityName,
            registration.Id.ToString(),
            organizationId: registration.OrganizationId,
            cancellationToken: cancellationToken);

        return ToResponse(registration);
    }

    /// <inheritdoc />
    public async Task<ParkingRegistrationResponse> ReleaseAsync(Guid id, CancellationToken cancellationToken = default)
    {
        ParkingRegistration registration = await FindAccessibleAsync(id, cancellationToken);

        if (registration.Status != ParkingRegistrationStatus.Active)
        {
            throw new ConflictException("การจดทะเบียนนี้สิ้นสุดไปแล้ว");
        }

        registration.Status = ParkingRegistrationStatus.Ended;
        registration.EndedAt = _dateTimeProvider.UtcNow;

        await _repository.SaveChangesAsync(cancellationToken);

        await _auditLogWriter.WriteAsync(
            AuditAction.ChangeStatus,
            AuditEntityName,
            registration.Id.ToString(),
            organizationId: registration.OrganizationId,
            newValues: $$"""{"status":"{{ParkingRegistrationStatus.Ended}}"}""",
            cancellationToken: cancellationToken);

        return ToResponse(registration);
    }

    private async Task<ParkingRegistration> FindAccessibleAsync(Guid id, CancellationToken cancellationToken)
    {
        ParkingRegistration registration = await _repository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException("ไม่พบข้อมูลการจดทะเบียนที่จอดรถที่ต้องการ");

        TenantScope.EnsureDormitoryAccess(
            _currentUser,
            registration.OrganizationId,
            registration.DormitoryId.GetValueOrDefault(),
            "ไม่พบข้อมูลการจดทะเบียนที่จอดรถที่ต้องการ");

        return registration;
    }

    private static ParkingRegistrationStatus? ParseStatusOrNull(string? status)
        => !string.IsNullOrWhiteSpace(status) && Enum.TryParse(status, ignoreCase: true, out ParkingRegistrationStatus parsed)
            ? parsed
            : null;

    private static ParkingRegistrationResponse ToResponse(ParkingRegistration registration) => new()
    {
        Id = registration.Id,
        OrganizationId = registration.OrganizationId,
        DormitoryId = registration.DormitoryId.GetValueOrDefault(),
        RoomId = registration.RoomId,
        TenantId = registration.TenantId,
        LicensePlate = registration.LicensePlate,
        VehicleType = registration.VehicleType.ToString(),
        Brand = registration.Brand,
        Color = registration.Color,
        ParkingSpotLabel = registration.ParkingSpotLabel,
        Status = registration.Status.ToString(),
        RegisteredAt = registration.RegisteredAt,
        EndedAt = registration.EndedAt,
        Notes = registration.Notes,
        CreatedAt = registration.CreatedAt,
    };
}
