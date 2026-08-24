using DormFlow.Application.Common.Abstractions;
using DormFlow.Application.Common.Authorization;
using DormFlow.Application.Common.Exceptions;
using DormFlow.Application.Common.Pagination;
using DormFlow.Application.Features.Dormitories;
using DormFlow.Contracts.Common;
using DormFlow.Contracts.RoomTypes;
using DormFlow.Domain.Entities;
using DormFlow.Domain.Enums;
using FluentValidation;

namespace DormFlow.Application.Features.RoomTypes;

/// <summary>Use Case ทั้งหมดของโมดูลประเภทห้อง</summary>
public sealed class RoomTypeService(
    IRoomTypeRepository repository,
    IDormitoryRepository dormitoryRepository,
    ICurrentUser currentUser,
    IAuditLogWriter auditLogWriter,
    IValidator<CreateRoomTypeRequest> createValidator,
    IValidator<UpdateRoomTypeRequest> updateValidator) : IRoomTypeService
{
    private const string AuditEntityName = "RoomType";
    private static readonly string[] AllowedSortFields = ["code", "name", "monthlyRent", "status", "createdAt"];

    private readonly IRoomTypeRepository _repository = repository;
    private readonly IDormitoryRepository _dormitoryRepository = dormitoryRepository;
    private readonly ICurrentUser _currentUser = currentUser;
    private readonly IAuditLogWriter _auditLogWriter = auditLogWriter;
    private readonly IValidator<CreateRoomTypeRequest> _createValidator = createValidator;
    private readonly IValidator<UpdateRoomTypeRequest> _updateValidator = updateValidator;

    /// <inheritdoc />
    public async Task<PagedResult<RoomTypeResponse>> GetPagedAsync(
        RoomTypeListRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        PagedQuery query = PagedQueryNormalizer.Normalize(request, AllowedSortFields, defaultSortBy: "name");

        RoomTypeListFilter filter = new(
            request.DormitoryId,
            ParseStatusOrNull(request.Status),
            TenantScope.GetDormitoryRestrictionForListing(_currentUser));

        PagedResult<RoomType> result = await _repository.GetPagedAsync(filter, query, cancellationToken);

        return PagedResult.Create(
            (IReadOnlyList<RoomTypeResponse>)[.. result.Items.Select(ToResponse)],
            result.Page,
            result.PageSize,
            result.TotalItems);
    }

    /// <inheritdoc />
    public async Task<RoomTypeResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        RoomType roomType = await FindAccessibleAsync(id, cancellationToken);
        return ToResponse(roomType);
    }

    /// <inheritdoc />
    public async Task<RoomTypeResponse> CreateAsync(
        CreateRoomTypeRequest request,
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
            throw new ConflictException($"รหัสประเภทห้อง '{code}' ถูกใช้ในหอพักนี้แล้ว");
        }

        RoomType roomType = new()
        {
            OrganizationId = dormitory.OrganizationId,
            DormitoryId = dormitory.Id,
            Code = code,
            Name = request.Name.Trim(),
            Description = request.Description,
            MonthlyRent = request.MonthlyRent,
            DailyRent = request.DailyRent,
            DepositAmount = request.DepositAmount,
            MaximumOccupants = request.MaximumOccupants,
        };

        await _repository.AddAsync(roomType, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        await _auditLogWriter.WriteAsync(
            AuditAction.Create,
            AuditEntityName,
            roomType.Id.ToString(),
            organizationId: roomType.OrganizationId,
            cancellationToken: cancellationToken);

        return ToResponse(roomType);
    }

    /// <inheritdoc />
    public async Task<RoomTypeResponse> UpdateAsync(
        Guid id,
        UpdateRoomTypeRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        await _updateValidator.ValidateAndThrowAsync(request, cancellationToken);

        RoomType roomType = await FindAccessibleAsync(id, cancellationToken);
        string code = request.Code.Trim();

        if (await _repository.ExistsByCodeAsync(
                roomType.DormitoryId.GetValueOrDefault(), code, roomType.Id, cancellationToken))
        {
            throw new ConflictException($"รหัสประเภทห้อง '{code}' ถูกใช้ในหอพักนี้แล้ว");
        }

        roomType.Code = code;
        roomType.Name = request.Name.Trim();
        roomType.Description = request.Description;
        roomType.MonthlyRent = request.MonthlyRent;
        roomType.DailyRent = request.DailyRent;
        roomType.DepositAmount = request.DepositAmount;
        roomType.MaximumOccupants = request.MaximumOccupants;

        await _repository.SaveChangesAsync(cancellationToken);

        await _auditLogWriter.WriteAsync(
            AuditAction.Update,
            AuditEntityName,
            roomType.Id.ToString(),
            organizationId: roomType.OrganizationId,
            cancellationToken: cancellationToken);

        return ToResponse(roomType);
    }

    /// <inheritdoc />
    public async Task<RoomTypeResponse> ChangeStatusAsync(
        Guid id,
        string status,
        CancellationToken cancellationToken = default)
    {
        if (!Enum.TryParse(status, ignoreCase: true, out RoomTypeStatus newStatus))
        {
            throw new ValidationFailedException($"สถานะ '{status}' ไม่ถูกต้อง");
        }

        RoomType roomType = await FindAccessibleAsync(id, cancellationToken);
        roomType.Status = newStatus;

        await _repository.SaveChangesAsync(cancellationToken);

        await _auditLogWriter.WriteAsync(
            AuditAction.ChangeStatus,
            AuditEntityName,
            roomType.Id.ToString(),
            organizationId: roomType.OrganizationId,
            newValues: $$"""{"status":"{{newStatus}}"}""",
            cancellationToken: cancellationToken);

        return ToResponse(roomType);
    }

    /// <inheritdoc />
    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        RoomType roomType = await FindAccessibleAsync(id, cancellationToken);

        _repository.Remove(roomType);
        await _repository.SaveChangesAsync(cancellationToken);

        await _auditLogWriter.WriteAsync(
            AuditAction.Delete,
            AuditEntityName,
            roomType.Id.ToString(),
            organizationId: roomType.OrganizationId,
            cancellationToken: cancellationToken);
    }

    private async Task<RoomType> FindAccessibleAsync(Guid id, CancellationToken cancellationToken)
    {
        RoomType roomType = await _repository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException("ไม่พบข้อมูลประเภทห้องที่ต้องการ");

        TenantScope.EnsureDormitoryAccess(
            _currentUser,
            roomType.OrganizationId,
            roomType.DormitoryId.GetValueOrDefault(),
            "ไม่พบข้อมูลประเภทห้องที่ต้องการ");

        return roomType;
    }

    private static RoomTypeStatus? ParseStatusOrNull(string? status)
        => !string.IsNullOrWhiteSpace(status) && Enum.TryParse(status, ignoreCase: true, out RoomTypeStatus parsed)
            ? parsed
            : null;

    private static RoomTypeResponse ToResponse(RoomType roomType) => new()
    {
        Id = roomType.Id,
        OrganizationId = roomType.OrganizationId,
        DormitoryId = roomType.DormitoryId.GetValueOrDefault(),
        Code = roomType.Code,
        Name = roomType.Name,
        Description = roomType.Description,
        MonthlyRent = roomType.MonthlyRent,
        DailyRent = roomType.DailyRent,
        DepositAmount = roomType.DepositAmount,
        MaximumOccupants = roomType.MaximumOccupants,
        Status = roomType.Status.ToString(),
        CreatedAt = roomType.CreatedAt,
        UpdatedAt = roomType.UpdatedAt,
    };
}
