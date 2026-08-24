using DormFlow.Application.Common.Abstractions;
using DormFlow.Application.Common.Authorization;
using DormFlow.Application.Common.Exceptions;
using DormFlow.Application.Common.Pagination;
using DormFlow.Application.Features.Buildings;
using DormFlow.Application.Features.Floors;
using DormFlow.Application.Features.RoomTypes;
using DormFlow.Contracts.Common;
using DormFlow.Contracts.Rooms;
using DormFlow.Domain.Entities;
using DormFlow.Domain.Enums;
using FluentValidation;

namespace DormFlow.Application.Features.Rooms;

/// <summary>Use Case ทั้งหมดของโมดูลห้องพัก</summary>
public sealed class RoomService(
    IRoomRepository repository,
    IBuildingRepository buildingRepository,
    IFloorRepository floorRepository,
    IRoomTypeRepository roomTypeRepository,
    ICurrentUser currentUser,
    IAuditLogWriter auditLogWriter,
    IValidator<CreateRoomRequest> createValidator,
    IValidator<UpdateRoomRequest> updateValidator) : IRoomService
{
    private const string AuditEntityName = "Room";
    private static readonly string[] AllowedSortFields = ["roomNumber", "monthlyRent", "status", "createdAt"];

    private readonly IRoomRepository _repository = repository;
    private readonly IBuildingRepository _buildingRepository = buildingRepository;
    private readonly IFloorRepository _floorRepository = floorRepository;
    private readonly IRoomTypeRepository _roomTypeRepository = roomTypeRepository;
    private readonly ICurrentUser _currentUser = currentUser;
    private readonly IAuditLogWriter _auditLogWriter = auditLogWriter;
    private readonly IValidator<CreateRoomRequest> _createValidator = createValidator;
    private readonly IValidator<UpdateRoomRequest> _updateValidator = updateValidator;

    /// <inheritdoc />
    public async Task<PagedResult<RoomResponse>> GetPagedAsync(
        RoomListRequest request,
        CancellationToken cancellationToken = default)
        => await QueryAsync(request, forceAvailableOnly: false, cancellationToken);

    /// <inheritdoc />
    public async Task<PagedResult<RoomResponse>> GetAvailableAsync(
        RoomListRequest request,
        CancellationToken cancellationToken = default)
        => await QueryAsync(request, forceAvailableOnly: true, cancellationToken);

    /// <inheritdoc />
    public async Task<RoomSummaryResponse> GetSummaryAsync(
        RoomSummaryRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        RoomSummaryFilter filter = new(
            TenantScope.GetOrganizationRestrictionForListing(_currentUser),
            request.DormitoryId,
            request.BuildingId,
            TenantScope.GetDormitoryRestrictionForListing(_currentUser));

        IReadOnlyDictionary<RoomStatus, int> counts = await _repository.GetStatusCountsAsync(filter, cancellationToken);

        return new RoomSummaryResponse
        {
            TotalRooms = counts.Values.Sum(),
            Available = counts.GetValueOrDefault(RoomStatus.Available),
            Reserved = counts.GetValueOrDefault(RoomStatus.Reserved),
            Occupied = counts.GetValueOrDefault(RoomStatus.Occupied),
            Cleaning = counts.GetValueOrDefault(RoomStatus.Cleaning),
            Maintenance = counts.GetValueOrDefault(RoomStatus.Maintenance),
            Inactive = counts.GetValueOrDefault(RoomStatus.Inactive),
        };
    }

    /// <inheritdoc />
    public async Task<RoomResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        Room room = await FindAccessibleAsync(id, cancellationToken);
        return ToResponse(room);
    }

    /// <inheritdoc />
    public async Task<RoomResponse> CreateAsync(
        CreateRoomRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        await _createValidator.ValidateAndThrowAsync(request, cancellationToken);

        Building building = await _buildingRepository.GetByIdAsync(request.BuildingId, cancellationToken)
            ?? throw new NotFoundException("ไม่พบอาคารที่ระบุ");

        TenantScope.EnsureDormitoryAccess(
            _currentUser,
            building.OrganizationId,
            building.DormitoryId.GetValueOrDefault(),
            "ไม่พบอาคารที่ระบุ");

        Floor floor = await _floorRepository.GetByIdAsync(request.FloorId, cancellationToken)
            ?? throw new NotFoundException("ไม่พบชั้นที่ระบุ");

        if (floor.BuildingId != building.Id)
        {
            throw new ValidationFailedException("ชั้นที่ระบุไม่ได้อยู่ในอาคารที่เลือก");
        }

        RoomType roomType = await _roomTypeRepository.GetByIdAsync(request.RoomTypeId, cancellationToken)
            ?? throw new NotFoundException("ไม่พบประเภทห้องที่ระบุ");

        if (roomType.DormitoryId != building.DormitoryId)
        {
            throw new ValidationFailedException("ประเภทห้องที่ระบุไม่ได้อยู่ในหอพักเดียวกับอาคารที่เลือก");
        }

        string roomNumber = request.RoomNumber.Trim();

        if (await _repository.ExistsByRoomNumberAsync(building.Id, roomNumber, null, cancellationToken))
        {
            throw new ConflictException($"เลขห้อง '{roomNumber}' ถูกใช้ในอาคารนี้แล้ว");
        }

        Room room = new()
        {
            OrganizationId = building.OrganizationId,
            DormitoryId = building.DormitoryId,
            BuildingId = building.Id,
            FloorId = floor.Id,
            RoomTypeId = roomType.Id,
            RoomNumber = roomNumber,
            MonthlyRent = request.MonthlyRent,
            DepositAmount = request.DepositAmount,
            MaximumOccupants = request.MaximumOccupants,
            Description = request.Description,
        };

        await _repository.AddAsync(room, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        await _auditLogWriter.WriteAsync(
            AuditAction.Create,
            AuditEntityName,
            room.Id.ToString(),
            organizationId: room.OrganizationId,
            cancellationToken: cancellationToken);

        return ToResponse(room);
    }

    /// <inheritdoc />
    public async Task<RoomResponse> UpdateAsync(
        Guid id,
        UpdateRoomRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        await _updateValidator.ValidateAndThrowAsync(request, cancellationToken);

        Room room = await FindAccessibleAsync(id, cancellationToken);

        Floor floor = await _floorRepository.GetByIdAsync(request.FloorId, cancellationToken)
            ?? throw new NotFoundException("ไม่พบชั้นที่ระบุ");

        if (floor.BuildingId != room.BuildingId)
        {
            throw new ValidationFailedException("ชั้นที่ระบุไม่ได้อยู่ในอาคารเดียวกับห้องนี้");
        }

        RoomType roomType = await _roomTypeRepository.GetByIdAsync(request.RoomTypeId, cancellationToken)
            ?? throw new NotFoundException("ไม่พบประเภทห้องที่ระบุ");

        if (roomType.DormitoryId != room.DormitoryId)
        {
            throw new ValidationFailedException("ประเภทห้องที่ระบุไม่ได้อยู่ในหอพักเดียวกับห้องนี้");
        }

        string roomNumber = request.RoomNumber.Trim();

        if (await _repository.ExistsByRoomNumberAsync(room.BuildingId, roomNumber, room.Id, cancellationToken))
        {
            throw new ConflictException($"เลขห้อง '{roomNumber}' ถูกใช้ในอาคารนี้แล้ว");
        }

        room.FloorId = floor.Id;
        room.RoomTypeId = roomType.Id;
        room.RoomNumber = roomNumber;
        room.MonthlyRent = request.MonthlyRent;
        room.DepositAmount = request.DepositAmount;
        room.MaximumOccupants = request.MaximumOccupants;
        room.Description = request.Description;

        await _repository.SaveChangesAsync(cancellationToken);

        await _auditLogWriter.WriteAsync(
            AuditAction.Update,
            AuditEntityName,
            room.Id.ToString(),
            organizationId: room.OrganizationId,
            cancellationToken: cancellationToken);

        return ToResponse(room);
    }

    /// <inheritdoc />
    public async Task<RoomResponse> ChangeStatusAsync(
        Guid id,
        string status,
        CancellationToken cancellationToken = default)
    {
        if (!Enum.TryParse(status, ignoreCase: true, out RoomStatus newStatus))
        {
            throw new ValidationFailedException($"สถานะ '{status}' ไม่ถูกต้อง");
        }

        Room room = await FindAccessibleAsync(id, cancellationToken);
        room.Status = newStatus;

        await _repository.SaveChangesAsync(cancellationToken);

        await _auditLogWriter.WriteAsync(
            AuditAction.ChangeStatus,
            AuditEntityName,
            room.Id.ToString(),
            organizationId: room.OrganizationId,
            newValues: $$"""{"status":"{{newStatus}}"}""",
            cancellationToken: cancellationToken);

        return ToResponse(room);
    }

    /// <inheritdoc />
    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        Room room = await FindAccessibleAsync(id, cancellationToken);

        _repository.Remove(room);
        await _repository.SaveChangesAsync(cancellationToken);

        await _auditLogWriter.WriteAsync(
            AuditAction.Delete,
            AuditEntityName,
            room.Id.ToString(),
            organizationId: room.OrganizationId,
            cancellationToken: cancellationToken);
    }

    private async Task<PagedResult<RoomResponse>> QueryAsync(
        RoomListRequest request,
        bool forceAvailableOnly,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        PagedQuery query = PagedQueryNormalizer.Normalize(request, AllowedSortFields, defaultSortBy: "roomNumber");

        RoomStatus? status = forceAvailableOnly ? RoomStatus.Available : ParseStatusOrNull(request.Status);

        RoomListFilter filter = new(
            TenantScope.GetOrganizationRestrictionForListing(_currentUser),
            request.DormitoryId,
            request.BuildingId,
            request.FloorId,
            request.RoomTypeId,
            status,
            TenantScope.GetDormitoryRestrictionForListing(_currentUser));

        PagedResult<Room> result = await _repository.GetPagedAsync(filter, query, cancellationToken);

        return PagedResult.Create(
            (IReadOnlyList<RoomResponse>)[.. result.Items.Select(ToResponse)],
            result.Page,
            result.PageSize,
            result.TotalItems);
    }

    private async Task<Room> FindAccessibleAsync(Guid id, CancellationToken cancellationToken)
    {
        Room room = await _repository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException("ไม่พบข้อมูลห้องพักที่ต้องการ");

        TenantScope.EnsureDormitoryAccess(
            _currentUser,
            room.OrganizationId,
            room.DormitoryId.GetValueOrDefault(),
            "ไม่พบข้อมูลห้องพักที่ต้องการ");

        return room;
    }

    private static RoomStatus? ParseStatusOrNull(string? status)
        => !string.IsNullOrWhiteSpace(status) && Enum.TryParse(status, ignoreCase: true, out RoomStatus parsed)
            ? parsed
            : null;

    private static RoomResponse ToResponse(Room room) => new()
    {
        Id = room.Id,
        OrganizationId = room.OrganizationId,
        DormitoryId = room.DormitoryId.GetValueOrDefault(),
        BuildingId = room.BuildingId,
        FloorId = room.FloorId,
        RoomTypeId = room.RoomTypeId,
        RoomNumber = room.RoomNumber,
        MonthlyRent = room.MonthlyRent,
        DepositAmount = room.DepositAmount,
        MaximumOccupants = room.MaximumOccupants,
        Status = room.Status.ToString(),
        Description = room.Description,
        CreatedAt = room.CreatedAt,
        UpdatedAt = room.UpdatedAt,
    };
}
