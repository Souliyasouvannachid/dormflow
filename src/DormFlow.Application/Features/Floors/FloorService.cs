using DormFlow.Application.Common.Abstractions;
using DormFlow.Application.Common.Authorization;
using DormFlow.Application.Common.Exceptions;
using DormFlow.Application.Common.Pagination;
using DormFlow.Application.Features.Buildings;
using DormFlow.Contracts.Common;
using DormFlow.Contracts.Floors;
using DormFlow.Domain.Entities;
using DormFlow.Domain.Enums;
using FluentValidation;

namespace DormFlow.Application.Features.Floors;

/// <summary>Use Case ໂມດູນຊັ້ນກໍ່ສ້າງທັງໝົດ</summary>
public sealed class FloorService(
    IFloorRepository repository,
    IBuildingRepository buildingRepository,
    ICurrentUser currentUser,
    IAuditLogWriter auditLogWriter,
    IValidator<CreateFloorRequest> createValidator,
    IValidator<UpdateFloorRequest> updateValidator) : IFloorService
{
    private const string AuditEntityName = "Floor";
    private static readonly string[] AllowedSortFields = ["floorNumber", "status", "createdAt"];

    private readonly IFloorRepository _repository = repository;
    private readonly IBuildingRepository _buildingRepository = buildingRepository;
    private readonly ICurrentUser _currentUser = currentUser;
    private readonly IAuditLogWriter _auditLogWriter = auditLogWriter;
    private readonly IValidator<CreateFloorRequest> _createValidator = createValidator;
    private readonly IValidator<UpdateFloorRequest> _updateValidator = updateValidator;

    /// <inheritdoc />
    public async Task<PagedResult<FloorResponse>> GetPagedAsync(
        FloorListRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        PagedQuery query = PagedQueryNormalizer.Normalize(request, AllowedSortFields, defaultSortBy: "floorNumber");

        FloorListFilter filter = new(
            TenantScope.GetOrganizationRestrictionForListing(_currentUser),
            request.BuildingId,
            ParseStatusOrNull(request.Status),
            TenantScope.GetDormitoryRestrictionForListing(_currentUser));

        PagedResult<Floor> result = await _repository.GetPagedAsync(filter, query, cancellationToken);

        return PagedResult.Create(
            (IReadOnlyList<FloorResponse>)[.. result.Items.Select(ToResponse)],
            result.Page,
            result.PageSize,
            result.TotalItems);
    }

    /// <inheritdoc />
    public async Task<FloorResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        Floor floor = await FindAccessibleAsync(id, cancellationToken);
        return ToResponse(floor);
    }

    /// <inheritdoc />
    public async Task<FloorResponse> CreateAsync(
        CreateFloorRequest request,
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

        if (await _repository.ExistsByFloorNumberAsync(building.Id, request.FloorNumber, null, cancellationToken))
        {
            throw new ConflictException($"ຊັ້ນທີ {request.FloorNumber} ມີຢູ່ແລ້ວໃນອາຄານນີ້");
        }

        Floor floor = new()
        {
            OrganizationId = building.OrganizationId,
            DormitoryId = building.DormitoryId,
            BuildingId = building.Id,
            FloorNumber = request.FloorNumber,
            Name = request.Name,
            Description = request.Description,
        };

        await _repository.AddAsync(floor, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        await _auditLogWriter.WriteAsync(
            AuditAction.Create,
            AuditEntityName,
            floor.Id.ToString(),
            organizationId: floor.OrganizationId,
            cancellationToken: cancellationToken);

        return ToResponse(floor);
    }

    /// <inheritdoc />
    public async Task<FloorResponse> UpdateAsync(
        Guid id,
        UpdateFloorRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        await _updateValidator.ValidateAndThrowAsync(request, cancellationToken);

        Floor floor = await FindAccessibleAsync(id, cancellationToken);

        if (await _repository.ExistsByFloorNumberAsync(floor.BuildingId, request.FloorNumber, floor.Id, cancellationToken))
        {
            throw new ConflictException($"ຊັ້ນທີ {request.FloorNumber} ມີຢູ່ແລ້ວໃນອາຄານນີ້");
        }

        floor.FloorNumber = request.FloorNumber;
        floor.Name = request.Name;
        floor.Description = request.Description;

        await _repository.SaveChangesAsync(cancellationToken);

        await _auditLogWriter.WriteAsync(
            AuditAction.Update,
            AuditEntityName,
            floor.Id.ToString(),
            organizationId: floor.OrganizationId,
            cancellationToken: cancellationToken);

        return ToResponse(floor);
    }

    /// <inheritdoc />
    public async Task<FloorResponse> ChangeStatusAsync(
        Guid id,
        string status,
        CancellationToken cancellationToken = default)
    {
        if (!Enum.TryParse(status, ignoreCase: true, out FloorStatus newStatus))
        {
            throw new ValidationFailedException($"ສະຖານະ '{status}' ບໍ່ຖືກຕ້ອງ");
        }

        Floor floor = await FindAccessibleAsync(id, cancellationToken);
        floor.Status = newStatus;

        await _repository.SaveChangesAsync(cancellationToken);

        await _auditLogWriter.WriteAsync(
            AuditAction.ChangeStatus,
            AuditEntityName,
            floor.Id.ToString(),
            organizationId: floor.OrganizationId,
            newValues: $$"""{"status":"{{newStatus}}"}""",
            cancellationToken: cancellationToken);

        return ToResponse(floor);
    }

    /// <inheritdoc />
    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        Floor floor = await FindAccessibleAsync(id, cancellationToken);

        _repository.Remove(floor);
        await _repository.SaveChangesAsync(cancellationToken);

        await _auditLogWriter.WriteAsync(
            AuditAction.Delete,
            AuditEntityName,
            floor.Id.ToString(),
            organizationId: floor.OrganizationId,
            cancellationToken: cancellationToken);
    }

    private async Task<Floor> FindAccessibleAsync(Guid id, CancellationToken cancellationToken)
    {
        Floor floor = await _repository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException("ບໍ່ພົບຂໍ້ມູນຊັ້ນທີ່ຕ້ອງການ");

        TenantScope.EnsureDormitoryAccess(
            _currentUser,
            floor.OrganizationId,
            floor.DormitoryId.GetValueOrDefault(),
            "ບໍ່ພົບຂໍ້ມູນຊັ້ນທີ່ຕ້ອງການ");

        return floor;
    }

    private static FloorStatus? ParseStatusOrNull(string? status)
        => !string.IsNullOrWhiteSpace(status) && Enum.TryParse(status, ignoreCase: true, out FloorStatus parsed)
            ? parsed
            : null;

    private static FloorResponse ToResponse(Floor floor) => new()
    {
        Id = floor.Id,
        OrganizationId = floor.OrganizationId,
        DormitoryId = floor.DormitoryId.GetValueOrDefault(),
        BuildingId = floor.BuildingId,
        FloorNumber = floor.FloorNumber,
        Name = floor.Name,
        Description = floor.Description,
        Status = floor.Status.ToString(),
        CreatedAt = floor.CreatedAt,
        UpdatedAt = floor.UpdatedAt,
    };
}
