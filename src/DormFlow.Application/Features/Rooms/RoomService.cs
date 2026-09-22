using DormFlow.Application.Common.Abstractions;
using DormFlow.Application.Common.Authorization;
using DormFlow.Application.Common.Exceptions;
using DormFlow.Application.Common.Pagination;
using DormFlow.Application.Features.Buildings;
using DormFlow.Application.Features.Dormitories;
using DormFlow.Application.Features.Floors;
using DormFlow.Application.Features.RoomTypes;
using DormFlow.Contracts.Common;
using DormFlow.Contracts.Rooms;
using DormFlow.Domain.Entities;
using DormFlow.Domain.Enums;
using FluentValidation;

namespace DormFlow.Application.Features.Rooms;

/// <summary>Use Case ໂມດູນຫ້ອງທັງໝົດ</summary>
public sealed class RoomService(
    IRoomRepository repository,
    IBuildingRepository buildingRepository,
    IFloorRepository floorRepository,
    IRoomTypeRepository roomTypeRepository,
    IDormitoryRepository dormitoryRepository,
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
    private readonly IDormitoryRepository _dormitoryRepository = dormitoryRepository;
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
        return await ToResponseAsync(room, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<RoomResponse> CreateAsync(
        CreateRoomRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        await _createValidator.ValidateAndThrowAsync(request, cancellationToken);

        Building building = await _buildingRepository.GetByIdAsync(request.BuildingId, cancellationToken)
            ?? throw new NotFoundException("ບໍ່ພົບອາຄານທີ່ລະບຸ");

        TenantScope.EnsureDormitoryAccess(
            _currentUser,
            building.OrganizationId,
            building.DormitoryId.GetValueOrDefault(),
            "ບໍ່ພົບອາຄານທີ່ລະບຸ");

        Floor floor = await _floorRepository.GetByIdAsync(request.FloorId, cancellationToken)
            ?? throw new NotFoundException("ບໍ່ພົບຊັ້ນທີ່ລະບຸ");

        if (floor.BuildingId != building.Id)
        {
            throw new ValidationFailedException("ຊັ້ນທີ່ລະບຸບໍ່ໄດ້ຢູ່ໃນອາຄານທີ່ເລືອກ");
        }

        RoomType roomType = await _roomTypeRepository.GetByIdAsync(request.RoomTypeId, cancellationToken)
            ?? throw new NotFoundException("ບໍ່ພົບປະເພດຫ້ອງທີ່ລະບຸ");

        if (roomType.DormitoryId != building.DormitoryId)
        {
            throw new ValidationFailedException("ປະເພດຫ້ອງທີ່ລະບຸບໍ່ໄດ້ຢູ່ໃນຫໍພັກດຽວກັບອາຄານທີ່ເລືອກ");
        }

        string roomNumber = request.RoomNumber.Trim();

        if (await _repository.ExistsByRoomNumberAsync(building.Id, roomNumber, null, cancellationToken))
        {
            throw new ConflictException($"ເລກຫ້ອງ '{roomNumber}' ຖືກໃຊ້ໃນອາຄານນີ້ແລ້ວ");
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
            FloorPlanX = request.FloorPlanX,
            FloorPlanY = request.FloorPlanY,
            FloorPlanWidth = request.FloorPlanWidth,
            FloorPlanHeight = request.FloorPlanHeight,
        };

        await _repository.AddAsync(room, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        await _auditLogWriter.WriteAsync(
            AuditAction.Create,
            AuditEntityName,
            room.Id.ToString(),
            organizationId: room.OrganizationId,
            cancellationToken: cancellationToken);

        return await ToResponseAsync(room, cancellationToken);
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
            ?? throw new NotFoundException("ບໍ່ພົບຊັ້ນທີ່ລະບຸ");

        if (floor.BuildingId != room.BuildingId)
        {
            throw new ValidationFailedException("ຊັ້ນທີ່ລະບຸບໍ່ໄດ້ຢູ່ໃນອາຄານດຽວກັບຫ້ອງນີ້");
        }

        RoomType roomType = await _roomTypeRepository.GetByIdAsync(request.RoomTypeId, cancellationToken)
            ?? throw new NotFoundException("ບໍ່ພົບປະເພດຫ້ອງທີ່ລະບຸ");

        if (roomType.DormitoryId != room.DormitoryId)
        {
            throw new ValidationFailedException("ປະເພດຫ້ອງທີ່ລະບຸບໍ່ໄດ້ຢູ່ໃນຫໍພັກດຽວກັບຫ້ອງນີ້");
        }

        string roomNumber = request.RoomNumber.Trim();

        if (await _repository.ExistsByRoomNumberAsync(room.BuildingId, roomNumber, room.Id, cancellationToken))
        {
            throw new ConflictException($"ເລກຫ້ອງ '{roomNumber}' ຖືກໃຊ້ໃນອາຄານນີ້ແລ້ວ");
        }

        room.FloorId = floor.Id;
        room.RoomTypeId = roomType.Id;
        room.RoomNumber = roomNumber;
        room.MonthlyRent = request.MonthlyRent;
        room.DepositAmount = request.DepositAmount;
        room.MaximumOccupants = request.MaximumOccupants;
        room.Description = request.Description;
        room.FloorPlanX = request.FloorPlanX;
        room.FloorPlanY = request.FloorPlanY;
        room.FloorPlanWidth = request.FloorPlanWidth;
        room.FloorPlanHeight = request.FloorPlanHeight;

        await _repository.SaveChangesAsync(cancellationToken);

        await _auditLogWriter.WriteAsync(
            AuditAction.Update,
            AuditEntityName,
            room.Id.ToString(),
            organizationId: room.OrganizationId,
            cancellationToken: cancellationToken);

        return await ToResponseAsync(room, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<RoomResponse> ChangeStatusAsync(
        Guid id,
        string status,
        CancellationToken cancellationToken = default)
    {
        if (!Enum.TryParse(status, ignoreCase: true, out RoomStatus newStatus))
        {
            throw new ValidationFailedException($"ສະຖານະ '{status}' ບໍ່ຖືກຕ້ອງ");
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

        return await ToResponseAsync(room, cancellationToken);
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
        IReadOnlyList<RoomResponse> items = await ToResponsesAsync(result.Items, cancellationToken);

        return PagedResult.Create(items, result.Page, result.PageSize, result.TotalItems);
    }

    private async Task<Room> FindAccessibleAsync(Guid id, CancellationToken cancellationToken)
    {
        Room room = await _repository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException("ບໍ່ພົບຂໍ້ມູນຫ້ອງພັກທີ່ຕ້ອງການ");

        TenantScope.EnsureDormitoryAccess(
            _currentUser,
            room.OrganizationId,
            room.DormitoryId.GetValueOrDefault(),
            "ບໍ່ພົບຂໍ້ມູນຫ້ອງພັກທີ່ຕ້ອງການ");

        return room;
    }

    private static RoomStatus? ParseStatusOrNull(string? status)
        => !string.IsNullOrWhiteSpace(status) && Enum.TryParse(status, ignoreCase: true, out RoomStatus parsed)
            ? parsed
            : null;

    private async Task<RoomResponse> ToResponseAsync(Room room, CancellationToken cancellationToken)
    {
        IReadOnlyList<RoomResponse> responses = await ToResponsesAsync([room], cancellationToken);
        return responses[0];
    }

    /// <summary>
    /// ແປງ <see cref="Room"/> ແມ່ນ <see cref="RoomResponse"/> ກັບຊື່ຂອງຫໍພັກ/ອາຄານ/ຊັ້ນ/ປະເພດຫ້ອງ
    /// ເນື່ອງຈາກວ່າ <see cref="Room"/> ຮັກສາລະຫັດການອ້າງອີງເທົ່ານັ້ນ (ບໍ່ມີ navigation property) ດັ່ງນັ້ນ, ຊື່ຕ້ອງໄດ້ຮັບການສະກັດແຍກຕ່າງຫາກ
    /// query ແຕ່ເຮັດໃຫ້ພຽງແຕ່ຜຶ່ງ batch ຕໍ່ລາຍການ (ບໍ່ query ແຖວຕໍ່ແຖວ) ເພື່ອຫຼຸດຜ່ອນຈໍານວນຂອງຮອບກັບຖານຂໍ້ມູນ
    /// </summary>
    private async Task<IReadOnlyList<RoomResponse>> ToResponsesAsync(
        IReadOnlyList<Room> rooms,
        CancellationToken cancellationToken)
    {
        Guid[] dormitoryIds = [.. rooms.Select(room => room.DormitoryId.GetValueOrDefault()).Distinct()];
        Guid[] buildingIds = [.. rooms.Select(room => room.BuildingId).Distinct()];
        Guid[] floorIds = [.. rooms.Select(room => room.FloorId).Distinct()];
        Guid[] roomTypeIds = [.. rooms.Select(room => room.RoomTypeId).Distinct()];

        IReadOnlyDictionary<Guid, string> dormitoryNames =
            await _dormitoryRepository.GetNamesByIdsAsync(dormitoryIds, cancellationToken);
        IReadOnlyDictionary<Guid, string> buildingNames =
            await _buildingRepository.GetNamesByIdsAsync(buildingIds, cancellationToken);
        IReadOnlyDictionary<Guid, string> floorNames =
            await _floorRepository.GetNamesByIdsAsync(floorIds, cancellationToken);
        IReadOnlyDictionary<Guid, string> roomTypeNames =
            await _roomTypeRepository.GetNamesByIdsAsync(roomTypeIds, cancellationToken);

        return [.. rooms.Select(room => new RoomResponse
        {
            Id = room.Id,
            OrganizationId = room.OrganizationId,
            DormitoryId = room.DormitoryId.GetValueOrDefault(),
            DormitoryName = dormitoryNames.GetValueOrDefault(room.DormitoryId.GetValueOrDefault(), string.Empty),
            BuildingId = room.BuildingId,
            BuildingName = buildingNames.GetValueOrDefault(room.BuildingId, string.Empty),
            FloorId = room.FloorId,
            FloorName = floorNames.GetValueOrDefault(room.FloorId, string.Empty),
            RoomTypeId = room.RoomTypeId,
            RoomTypeName = roomTypeNames.GetValueOrDefault(room.RoomTypeId, string.Empty),
            RoomNumber = room.RoomNumber,
            MonthlyRent = room.MonthlyRent,
            DepositAmount = room.DepositAmount,
            MaximumOccupants = room.MaximumOccupants,
            Status = room.Status.ToString(),
            Description = room.Description,
            FloorPlanX = room.FloorPlanX,
            FloorPlanY = room.FloorPlanY,
            FloorPlanWidth = room.FloorPlanWidth,
            FloorPlanHeight = room.FloorPlanHeight,
            CreatedAt = room.CreatedAt,
            UpdatedAt = room.UpdatedAt,
        })];
    }
}
