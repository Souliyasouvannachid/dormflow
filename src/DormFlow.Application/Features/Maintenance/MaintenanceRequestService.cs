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
/// Use Case ໂມດູນການແຈ້ງເຕືອນການບຳລຸງຮັກສາທັງໝົດ
/// </summary>
/// <remarks>
/// ການແຕ່ງຕັ້ງນັກວິຊາການ (<see cref="AssignAsync"/>) ແຍກອອກຈາກການຫັນປ່ຽນຂອງລັດທົ່ວໄປ (<see cref="ChangeStatusAsync"/>)
/// ນີ້ແມ່ນຄວາມຕັ້ງໃຈເພາະວ່າມັນຕ້ອງໄດ້ຮັບການກວດສອບວ່າຜູ້ໃຊ້ທີ່ລະບຸນັ້ນແມ່ນຊ່າງສ້ອມແປງຫໍພັກແທ້ໆ
/// ແລະສິດທີ່ໃຊ້ແມ່ນແຕກຕ່າງກັນ (maintenance.assign ປຽບທຽບກັບ maintenance.update-status)
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
            TenantScope.GetOrganizationRestrictionForListing(_currentUser),
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
    public Task<IReadOnlyDictionary<MaintenanceStatus, int>> GetStatusCountsAsync(
        CancellationToken cancellationToken = default)
    {
        MaintenanceRequestListFilter filter = new(
            TenantScope.GetOrganizationRestrictionForListing(_currentUser),
            DormitoryId: null,
            RoomId: null,
            Status: null,
            Priority: null,
            AssignedTechnicianId: null,
            TenantScope.GetDormitoryRestrictionForListing(_currentUser));

        return _repository.GetStatusCountsAsync(filter, cancellationToken);
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
            ?? throw new NotFoundException("ບໍ່ພົບຫ້ອງທີ່ລະບຸ");

        TenantScope.EnsureDormitoryAccess(
            _currentUser,
            room.OrganizationId,
            room.DormitoryId.GetValueOrDefault(),
            "ບໍ່ພົບຫ້ອງທີ່ລະບຸ");

        if (room.Status == RoomStatus.Inactive)
        {
            throw new ConflictException("ຫ້ອງນີ້ປິດໃຊ້ງານຢູ່ ບໍ່ສາມາດແຈ້ງສ້ອມແປງໄດ້");
        }

        MaintenanceRequest maintenanceRequest = new()
        {
            OrganizationId = room.OrganizationId,
            DormitoryId = room.DormitoryId,
            RoomId = room.Id,
            TenantId = request.TenantId,
            Category = request.Category.Trim(),
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
            throw new ConflictException("ງານນີ້ປິດແລ້ວ ບໍ່ສາມາດມອບໝາຍຊ່າງໄດ້");
        }

        bool isEligibleTechnician = await _userAdministrationRepository.HasRoleForDormitoryAsync(
            request.TechnicianId,
            DormFlowRoles.Technician,
            maintenanceRequest.OrganizationId,
            maintenanceRequest.DormitoryId.GetValueOrDefault(),
            cancellationToken);

        if (!isEligibleTechnician)
        {
            throw new ValidationFailedException("ບໍ່ພົບຊ່າງສ້ອມແປງທີ່ລະບຸ ຫຼືຊ່າງບໍ່ໄດ້ຮັບມອບໝາຍໃຫ້ດູແລຫໍພັກນີ້");
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
            throw new ValidationFailedException($"ສະຖານະ '{status}' ບໍ່ຖືກຕ້ອງ");
        }

        if (newStatus == MaintenanceStatus.Assigned)
        {
            throw new ValidationFailedException("ຕ້ອງມອບໝາຍຊ່າງຜ່ານ Endpoint ມອບໝາຍງານເທົ່ານັ້ນ");
        }

        MaintenanceRequest maintenanceRequest = await FindAccessibleAsync(id, cancellationToken);

        if (!IsAllowedTransition(maintenanceRequest.Status, newStatus))
        {
            throw new ConflictException(
                $"ບໍ່ສາມາດປ່ຽນສະຖານະຈາກ {maintenanceRequest.Status} ເປັນ {newStatus} ໄດ້");
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
            ?? throw new NotFoundException("ບໍ່ພົບຂໍ້ມູນງານແຈ້ງສ້ອມແປງທີ່ຕ້ອງການ");

        TenantScope.EnsureDormitoryAccess(
            _currentUser,
            maintenanceRequest.OrganizationId,
            maintenanceRequest.DormitoryId.GetValueOrDefault(),
            "ບໍ່ພົບຂໍ້ມູນງານແຈ້ງສ້ອມແປງທີ່ຕ້ອງການ");

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
        TenantId = maintenanceRequest.TenantId,
        Category = maintenanceRequest.Category,
        ActualCost = maintenanceRequest.ActualCost,
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
