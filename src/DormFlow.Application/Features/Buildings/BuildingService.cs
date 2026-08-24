using DormFlow.Application.Common.Abstractions;
using DormFlow.Application.Common.Authorization;
using DormFlow.Application.Common.Exceptions;
using DormFlow.Application.Common.Pagination;
using DormFlow.Application.Features.Dormitories;
using DormFlow.Contracts.Buildings;
using DormFlow.Contracts.Common;
using DormFlow.Domain.Entities;
using DormFlow.Domain.Enums;
using FluentValidation;

namespace DormFlow.Application.Features.Buildings;

/// <summary>Use Case ทั้งหมดของโมดูลอาคาร</summary>
public sealed class BuildingService(
    IBuildingRepository repository,
    IDormitoryRepository dormitoryRepository,
    ICurrentUser currentUser,
    IAuditLogWriter auditLogWriter,
    IValidator<CreateBuildingRequest> createValidator,
    IValidator<UpdateBuildingRequest> updateValidator) : IBuildingService
{
    private const string AuditEntityName = "Building";
    private static readonly string[] AllowedSortFields = ["code", "name", "status", "createdAt"];

    private readonly IBuildingRepository _repository = repository;
    private readonly IDormitoryRepository _dormitoryRepository = dormitoryRepository;
    private readonly ICurrentUser _currentUser = currentUser;
    private readonly IAuditLogWriter _auditLogWriter = auditLogWriter;
    private readonly IValidator<CreateBuildingRequest> _createValidator = createValidator;
    private readonly IValidator<UpdateBuildingRequest> _updateValidator = updateValidator;

    /// <inheritdoc />
    public async Task<PagedResult<BuildingResponse>> GetPagedAsync(
        BuildingListRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        PagedQuery query = PagedQueryNormalizer.Normalize(request, AllowedSortFields, defaultSortBy: "name");

        BuildingListFilter filter = new(
            request.DormitoryId,
            ParseStatusOrNull(request.Status),
            TenantScope.GetDormitoryRestrictionForListing(_currentUser));

        PagedResult<Building> result = await _repository.GetPagedAsync(filter, query, cancellationToken);

        return PagedResult.Create(
            (IReadOnlyList<BuildingResponse>)[.. result.Items.Select(ToResponse)],
            result.Page,
            result.PageSize,
            result.TotalItems);
    }

    /// <inheritdoc />
    public async Task<BuildingResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        Building building = await FindAccessibleAsync(id, cancellationToken);
        return ToResponse(building);
    }

    /// <inheritdoc />
    public async Task<BuildingResponse> CreateAsync(
        CreateBuildingRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        await _createValidator.ValidateAndThrowAsync(request, cancellationToken);

        Dormitory dormitory = await _dormitoryRepository.GetByIdAsync(request.DormitoryId, cancellationToken)
            ?? throw new NotFoundException("ไม่พบหอพักที่ระบุ");

        TenantScope.EnsureDormitoryAccess(_currentUser, dormitory.OrganizationId, dormitory.Id, "ไม่พบหอพักที่ระบุ");

        string code = request.Code.Trim();

        if (await _repository.ExistsByCodeAsync(dormitory.Id, code, null, cancellationToken))
        {
            throw new ConflictException($"รหัสอาคาร '{code}' ถูกใช้ในหอพักนี้แล้ว");
        }

        Building building = new()
        {
            OrganizationId = dormitory.OrganizationId,
            DormitoryId = dormitory.Id,
            Code = code,
            Name = request.Name.Trim(),
            Description = request.Description,
            TotalFloors = request.TotalFloors,
        };

        await _repository.AddAsync(building, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        await _auditLogWriter.WriteAsync(
            AuditAction.Create,
            AuditEntityName,
            building.Id.ToString(),
            organizationId: building.OrganizationId,
            cancellationToken: cancellationToken);

        return ToResponse(building);
    }

    /// <inheritdoc />
    public async Task<BuildingResponse> UpdateAsync(
        Guid id,
        UpdateBuildingRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        await _updateValidator.ValidateAndThrowAsync(request, cancellationToken);

        Building building = await FindAccessibleAsync(id, cancellationToken);
        string code = request.Code.Trim();

        if (await _repository.ExistsByCodeAsync(building.DormitoryId.GetValueOrDefault(), code, building.Id, cancellationToken))
        {
            throw new ConflictException($"รหัสอาคาร '{code}' ถูกใช้ในหอพักนี้แล้ว");
        }

        building.Code = code;
        building.Name = request.Name.Trim();
        building.Description = request.Description;
        building.TotalFloors = request.TotalFloors;

        await _repository.SaveChangesAsync(cancellationToken);

        await _auditLogWriter.WriteAsync(
            AuditAction.Update,
            AuditEntityName,
            building.Id.ToString(),
            organizationId: building.OrganizationId,
            cancellationToken: cancellationToken);

        return ToResponse(building);
    }

    /// <inheritdoc />
    public async Task<BuildingResponse> ChangeStatusAsync(
        Guid id,
        string status,
        CancellationToken cancellationToken = default)
    {
        if (!Enum.TryParse(status, ignoreCase: true, out BuildingStatus newStatus))
        {
            throw new ValidationFailedException($"สถานะ '{status}' ไม่ถูกต้อง");
        }

        Building building = await FindAccessibleAsync(id, cancellationToken);
        building.Status = newStatus;

        await _repository.SaveChangesAsync(cancellationToken);

        await _auditLogWriter.WriteAsync(
            AuditAction.ChangeStatus,
            AuditEntityName,
            building.Id.ToString(),
            organizationId: building.OrganizationId,
            newValues: $$"""{"status":"{{newStatus}}"}""",
            cancellationToken: cancellationToken);

        return ToResponse(building);
    }

    /// <inheritdoc />
    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        Building building = await FindAccessibleAsync(id, cancellationToken);

        _repository.Remove(building);
        await _repository.SaveChangesAsync(cancellationToken);

        await _auditLogWriter.WriteAsync(
            AuditAction.Delete,
            AuditEntityName,
            building.Id.ToString(),
            organizationId: building.OrganizationId,
            cancellationToken: cancellationToken);
    }

    private async Task<Building> FindAccessibleAsync(Guid id, CancellationToken cancellationToken)
    {
        Building building = await _repository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException("ไม่พบข้อมูลอาคารที่ต้องการ");

        TenantScope.EnsureDormitoryAccess(
            _currentUser,
            building.OrganizationId,
            building.DormitoryId.GetValueOrDefault(),
            "ไม่พบข้อมูลอาคารที่ต้องการ");

        return building;
    }

    private static BuildingStatus? ParseStatusOrNull(string? status)
        => !string.IsNullOrWhiteSpace(status) && Enum.TryParse(status, ignoreCase: true, out BuildingStatus parsed)
            ? parsed
            : null;

    private static BuildingResponse ToResponse(Building building) => new()
    {
        Id = building.Id,
        OrganizationId = building.OrganizationId,
        DormitoryId = building.DormitoryId ?? Guid.Empty,
        Code = building.Code,
        Name = building.Name,
        Description = building.Description,
        TotalFloors = building.TotalFloors,
        Status = building.Status.ToString(),
        CreatedAt = building.CreatedAt,
        UpdatedAt = building.UpdatedAt,
    };
}
