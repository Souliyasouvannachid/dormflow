using DormFlow.Application.Common.Abstractions;
using DormFlow.Application.Common.Authorization;
using DormFlow.Application.Common.Exceptions;
using DormFlow.Application.Common.Pagination;
using DormFlow.Application.Features.Rooms;
using DormFlow.Application.Features.Users;
using DormFlow.Contracts.Common;
using DormFlow.Contracts.Maintenance;
using DormFlow.Domain.Entities;
using DormFlow.Domain.Enums;
using FluentValidation;

namespace DormFlow.Application.Features.Maintenance;

/// <summary>
/// Use Case ทั้งหมดของโมดูลงานแจ้งซ่อมบำรุง
/// </summary>
/// <remarks>
/// การมอบหมายช่าง (<see cref="AssignAsync"/>) แยกออกจากการเปลี่ยนสถานะทั่วไป (<see cref="ChangeStatusAsync"/>)
/// โดยตั้งใจ เพราะต้องผ่านการตรวจสอบว่าผู้ใช้ที่ระบุเป็นช่างซ่อมบำรุงของหอพักนี้จริง
/// และสิทธิ์ที่ใช้ก็ต่างกัน (maintenance.assign เทียบกับ maintenance.update-status)
/// </remarks>
public sealed class MaintenanceRequestService(
    IMaintenanceRequestRepository repository,
    IRoomRepository roomRepository,
    IUserAdministrationRepository userAdministrationRepository,
    ICurrentUser currentUser,
    IDateTimeProvider dateTimeProvider,
    IAuditLogWriter auditLogWriter,
    IValidator<CreateMaintenanceRequestRequest> createValidator,
    IValidator<AssignMaintenanceRequestRequest> assignValidator) : IMaintenanceRequestService
{
    private const string AuditEntityName = "MaintenanceRequest";
    private static readonly string[] AllowedSortFields = ["priority", "status", "createdAt"];

    private readonly IMaintenanceRequestRepository _repository = repository;
    private readonly IRoomRepository _roomRepository = roomRepository;
    private readonly IUserAdministrationRepository _userAdministrationRepository = userAdministrationRepository;
    private readonly ICurrentUser _currentUser = currentUser;
    private readonly IDateTimeProvider _dateTimeProvider = dateTimeProvider;
    private readonly IAuditLogWriter _auditLogWriter = auditLogWriter;
    private readonly IValidator<CreateMaintenanceRequestRequest> _createValidator = createValidator;
    private readonly IValidator<AssignMaintenanceRequestRequest> _assignValidator = assignValidator;

    /// <inheritdoc />
    public async Task<PagedResult<MaintenanceRequestResponse>> GetPagedAsync(
        MaintenanceRequestListRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        PagedQuery query = PagedQueryNormalizer.Normalize(request, AllowedSortFields, defaultSortBy: "createdAt");

        MaintenanceRequestListFilter filter = new(
            request.DormitoryId,
            request.RoomId,
            ParseStatusOrNull(request.Status),
            ParsePriorityOrNull(request.Priority),
            request.AssignedTechnicianId,
            TenantScope.GetDormitoryRestrictionForListing(_currentUser));

        PagedResult<MaintenanceRequest> result = await _repository.GetPagedAsync(filter, query, cancellationToken);

        return PagedResult.Create(
            (IReadOnlyList<MaintenanceRequestResponse>)[.. result.Items.Select(ToResponse)],
            result.Page,
            result.PageSize,
            result.TotalItems);
    }

    /// <inheritdoc />
    public async Task<MaintenanceRequestResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => ToResponse(await FindAccessibleAsync(id, cancellationToken));

    /// <inheritdoc />
    public async Task<MaintenanceRequestResponse> CreateAsync(
        CreateMaintenanceRequestRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        await _createValidator.ValidateAndThrowAsync(request, cancellationToken);

        Room room = await _roomRepository.GetByIdAsync(request.RoomId, cancellationToken)
            ?? throw new NotFoundException("ไม่พบห้องที่ระบุ");

        TenantScope.EnsureDormitoryAccess(
            _currentUser,
            room.OrganizationId,
            room.DormitoryId.GetValueOrDefault(),
            "ไม่พบห้องที่ระบุ");

        if (room.Status == RoomStatus.Inactive)
        {
            throw new ConflictException("ห้องนี้ปิดใช้งานอยู่ ไม่สามารถแจ้งซ่อมได้");
        }

        MaintenanceRequest maintenanceRequest = new()
        {
            OrganizationId = room.OrganizationId,
            DormitoryId = room.DormitoryId,
            RoomId = room.Id,
            Title = request.Title.Trim(),
            Description = request.Description,
            Priority = Enum.Parse<MaintenancePriority>(request.Priority, ignoreCase: true),
        };

        await _repository.AddAsync(maintenanceRequest, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        await _auditLogWriter.WriteAsync(
            AuditAction.Create,
            AuditEntityName,
            maintenanceRequest.Id.ToString(),
            organizationId: maintenanceRequest.OrganizationId,
            cancellationToken: cancellationToken);

        return ToResponse(maintenanceRequest);
    }

    /// <inheritdoc />
    public async Task<MaintenanceRequestResponse> AssignAsync(
        Guid id,
        AssignMaintenanceRequestRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        await _assignValidator.ValidateAndThrowAsync(request, cancellationToken);

        MaintenanceRequest maintenanceRequest = await FindAccessibleAsync(id, cancellationToken);

        if (maintenanceRequest.Status is MaintenanceStatus.Completed or MaintenanceStatus.Cancelled)
        {
            throw new ConflictException("งานนี้ปิดแล้ว ไม่สามารถมอบหมายช่างได้");
        }

        bool isEligibleTechnician = await _userAdministrationRepository.HasRoleForDormitoryAsync(
            request.TechnicianId,
            DormFlowRoles.Technician,
            maintenanceRequest.OrganizationId,
            maintenanceRequest.DormitoryId.GetValueOrDefault(),
            cancellationToken);

        if (!isEligibleTechnician)
        {
            throw new ValidationFailedException("ไม่พบช่างซ่อมบำรุงที่ระบุ หรือช่างไม่ได้รับมอบหมายดูแลหอพักนี้");
        }

        DateTimeOffset now = _dateTimeProvider.UtcNow;
        maintenanceRequest.AssignedTechnicianId = request.TechnicianId;
        maintenanceRequest.AssignedAt = now;

        if (maintenanceRequest.Status == MaintenanceStatus.Reported)
        {
            maintenanceRequest.Status = MaintenanceStatus.Assigned;
        }

        Room? room = await _roomRepository.GetByIdAsync(maintenanceRequest.RoomId, cancellationToken);
        if (room is not null && room.Status == RoomStatus.Available)
        {
            room.Status = RoomStatus.Maintenance;
        }

        await _repository.SaveChangesAsync(cancellationToken);

        await _auditLogWriter.WriteAsync(
            AuditAction.ChangeStatus,
            AuditEntityName,
            maintenanceRequest.Id.ToString(),
            organizationId: maintenanceRequest.OrganizationId,
            newValues: $$"""{"status":"{{maintenanceRequest.Status}}","assignedTechnicianId":"{{request.TechnicianId}}"}""",
            cancellationToken: cancellationToken);

        return ToResponse(maintenanceRequest);
    }

    /// <inheritdoc />
    public async Task<MaintenanceRequestResponse> ChangeStatusAsync(
        Guid id,
        string status,
        CancellationToken cancellationToken = default)
    {
        if (!Enum.TryParse(status, ignoreCase: true, out MaintenanceStatus newStatus))
        {
            throw new ValidationFailedException($"สถานะ '{status}' ไม่ถูกต้อง");
        }

        if (newStatus == MaintenanceStatus.Assigned)
        {
            throw new ValidationFailedException("ต้องมอบหมายช่างผ่าน Endpoint มอบหมายงานเท่านั้น");
        }

        MaintenanceRequest maintenanceRequest = await FindAccessibleAsync(id, cancellationToken);

        if (!IsAllowedTransition(maintenanceRequest.Status, newStatus))
        {
            throw new ConflictException(
                $"ไม่สามารถเปลี่ยนสถานะจาก {maintenanceRequest.Status} เป็น {newStatus} ได้");
        }

        maintenanceRequest.Status = newStatus;

        DateTimeOffset now = _dateTimeProvider.UtcNow;
        if (newStatus == MaintenanceStatus.Completed)
        {
            maintenanceRequest.CompletedAt = now;
        }

        if (newStatus is MaintenanceStatus.Completed or MaintenanceStatus.Cancelled)
        {
            Room? room = await _roomRepository.GetByIdAsync(maintenanceRequest.RoomId, cancellationToken);
            if (room is not null && room.Status == RoomStatus.Maintenance
                && !await _repository.HasOpenRequestForRoomAsync(room.Id, maintenanceRequest.Id, cancellationToken))
            {
                room.Status = RoomStatus.Available;
            }
        }

        await _repository.SaveChangesAsync(cancellationToken);

        await _auditLogWriter.WriteAsync(
            AuditAction.ChangeStatus,
            AuditEntityName,
            maintenanceRequest.Id.ToString(),
            organizationId: maintenanceRequest.OrganizationId,
            newValues: $$"""{"status":"{{newStatus}}"}""",
            cancellationToken: cancellationToken);

        return ToResponse(maintenanceRequest);
    }

    private static bool IsAllowedTransition(MaintenanceStatus current, MaintenanceStatus target)
        => (current, target) switch
        {
            (MaintenanceStatus.Reported, MaintenanceStatus.Cancelled) => true,
            (MaintenanceStatus.Assigned, MaintenanceStatus.InProgress) => true,
            (MaintenanceStatus.Assigned, MaintenanceStatus.Cancelled) => true,
            (MaintenanceStatus.InProgress, MaintenanceStatus.Completed) => true,
            (MaintenanceStatus.InProgress, MaintenanceStatus.Cancelled) => true,
            _ => false,
        };

    private async Task<MaintenanceRequest> FindAccessibleAsync(Guid id, CancellationToken cancellationToken)
    {
        MaintenanceRequest maintenanceRequest = await _repository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException("ไม่พบข้อมูลงานแจ้งซ่อมบำรุงที่ต้องการ");

        TenantScope.EnsureDormitoryAccess(
            _currentUser,
            maintenanceRequest.OrganizationId,
            maintenanceRequest.DormitoryId.GetValueOrDefault(),
            "ไม่พบข้อมูลงานแจ้งซ่อมบำรุงที่ต้องการ");

        return maintenanceRequest;
    }

    private static MaintenanceStatus? ParseStatusOrNull(string? status)
        => !string.IsNullOrWhiteSpace(status) && Enum.TryParse(status, ignoreCase: true, out MaintenanceStatus parsed)
            ? parsed
            : null;

    private static MaintenancePriority? ParsePriorityOrNull(string? priority)
        => !string.IsNullOrWhiteSpace(priority) && Enum.TryParse(priority, ignoreCase: true, out MaintenancePriority parsed)
            ? parsed
            : null;

    private static MaintenanceRequestResponse ToResponse(MaintenanceRequest maintenanceRequest) => new()
    {
        Id = maintenanceRequest.Id,
        OrganizationId = maintenanceRequest.OrganizationId,
        DormitoryId = maintenanceRequest.DormitoryId.GetValueOrDefault(),
        RoomId = maintenanceRequest.RoomId,
        Title = maintenanceRequest.Title,
        Description = maintenanceRequest.Description,
        Priority = maintenanceRequest.Priority.ToString(),
        Status = maintenanceRequest.Status.ToString(),
        AssignedTechnicianId = maintenanceRequest.AssignedTechnicianId,
        AssignedAt = maintenanceRequest.AssignedAt,
        CompletedAt = maintenanceRequest.CompletedAt,
        ResolutionNotes = maintenanceRequest.ResolutionNotes,
        CreatedAt = maintenanceRequest.CreatedAt,
    };
}
