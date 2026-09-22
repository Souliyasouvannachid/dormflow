using DormFlow.Application.Common.Abstractions;
using DormFlow.Application.Common.Authorization;
using DormFlow.Application.Common.Exceptions;
using DormFlow.Application.Common.Pagination;
using DormFlow.Contracts.Common;
using DormFlow.Contracts.Dormitories;
using DormFlow.Domain.Entities;
using DormFlow.Domain.Enums;
using FluentValidation;

namespace DormFlow.Application.Features.Dormitories;

/// <summary>Use Case ໂມດູນຫໍພັກທັງໝົດ</summary>
public sealed class DormitoryService(
    IDormitoryRepository repository,
    ICurrentUser currentUser,
    IAuditLogWriter auditLogWriter,
    IValidator<CreateDormitoryRequest> createValidator,
    IValidator<UpdateDormitoryRequest> updateValidator) : IDormitoryService
{
    /// <summary>ຊື່ Entity ໃຊ້ເພື່ອຊີ້ບອກເຫດການໃນ Audit Log</summary>
    private const string AuditEntityName = "Dormitory";

    private static readonly string[] AllowedSortFields = ["code", "name", "status", "createdAt"];

    private readonly IDormitoryRepository _repository = repository;
    private readonly ICurrentUser _currentUser = currentUser;
    private readonly IAuditLogWriter _auditLogWriter = auditLogWriter;
    private readonly IValidator<CreateDormitoryRequest> _createValidator = createValidator;
    private readonly IValidator<UpdateDormitoryRequest> _updateValidator = updateValidator;

    /// <inheritdoc />
    public async Task<PagedResult<DormitoryResponse>> GetPagedAsync(
        DormitoryListRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        PagedQuery query = PagedQueryNormalizer.Normalize(request, AllowedSortFields, defaultSortBy: "name");

        Guid? organizationId = _currentUser.IsSuperAdmin ? request.OrganizationId : _currentUser.OrganizationId;
        DormitoryStatus? status = ParseStatusOrNull(request.Status);

        DormitoryListFilter filter = new(
            organizationId,
            status,
            TenantScope.GetDormitoryRestrictionForListing(_currentUser));

        PagedResult<Dormitory> result = await _repository.GetPagedAsync(filter, query, cancellationToken);

        return PagedResult.Create(
            (IReadOnlyList<DormitoryResponse>)[.. result.Items.Select(ToResponse)],
            result.Page,
            result.PageSize,
            result.TotalItems);
    }

    /// <inheritdoc />
    public async Task<DormitoryResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        Dormitory dormitory = await FindAccessibleAsync(id, cancellationToken);
        return ToResponse(dormitory);
    }

    /// <inheritdoc />
    public async Task<DormitoryResponse> CreateAsync(
        CreateDormitoryRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        await _createValidator.ValidateAndThrowAsync(request, cancellationToken);

        Guid organizationId = TenantScope.ResolveOrganizationIdForCreate(_currentUser, request.OrganizationId);
        string code = request.Code.Trim();

        if (await _repository.ExistsByCodeAsync(organizationId, code, null, cancellationToken))
        {
            throw new ConflictException($"ລະຫັດຫໍພັກ '{code}' ຖືກໃຊ້ໃນອົງກອນນີ້ແລ້ວ");
        }

        Dormitory dormitory = new()
        {
            OrganizationId = organizationId,
            Code = code,
            Name = request.Name.Trim(),
            Description = request.Description,
            Phone = request.Phone,
            Email = request.Email,
            Address = request.Address,
            Province = request.Province,
            District = request.District,
            Subdistrict = request.Subdistrict,
            PostalCode = request.PostalCode,
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            BillingDay = request.BillingDay,
            PaymentDueDay = request.PaymentDueDay,
            LateFeeType = Enum.Parse<LateFeeType>(request.LateFeeType, ignoreCase: true),
            LateFeeAmount = request.LateFeeAmount,
            WaterRatePerUnit = request.WaterRatePerUnit,
            ElectricityRatePerUnit = request.ElectricityRatePerUnit,
        };

        await _repository.AddAsync(dormitory, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        await _auditLogWriter.WriteAsync(
            AuditAction.Create,
            AuditEntityName,
            dormitory.Id.ToString(),
            organizationId: dormitory.OrganizationId,
            cancellationToken: cancellationToken);

        return ToResponse(dormitory);
    }

    /// <inheritdoc />
    public async Task<DormitoryResponse> UpdateAsync(
        Guid id,
        UpdateDormitoryRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        await _updateValidator.ValidateAndThrowAsync(request, cancellationToken);

        Dormitory dormitory = await FindAccessibleAsync(id, cancellationToken);
        string code = request.Code.Trim();

        if (await _repository.ExistsByCodeAsync(dormitory.OrganizationId, code, dormitory.Id, cancellationToken))
        {
            throw new ConflictException($"ລະຫັດຫໍພັກ '{code}' ຖືກໃຊ້ໃນອົງກອນນີ້ແລ້ວ");
        }

        dormitory.Code = code;
        dormitory.Name = request.Name.Trim();
        dormitory.Description = request.Description;
        dormitory.Phone = request.Phone;
        dormitory.Email = request.Email;
        dormitory.Address = request.Address;
        dormitory.Province = request.Province;
        dormitory.District = request.District;
        dormitory.Subdistrict = request.Subdistrict;
        dormitory.PostalCode = request.PostalCode;
        dormitory.Latitude = request.Latitude;
        dormitory.Longitude = request.Longitude;
        dormitory.BillingDay = request.BillingDay;
        dormitory.PaymentDueDay = request.PaymentDueDay;
        dormitory.LateFeeType = Enum.Parse<LateFeeType>(request.LateFeeType, ignoreCase: true);
        dormitory.LateFeeAmount = request.LateFeeAmount;
        dormitory.WaterRatePerUnit = request.WaterRatePerUnit;
        dormitory.ElectricityRatePerUnit = request.ElectricityRatePerUnit;

        await _repository.SaveChangesAsync(cancellationToken);

        await _auditLogWriter.WriteAsync(
            AuditAction.Update,
            AuditEntityName,
            dormitory.Id.ToString(),
            organizationId: dormitory.OrganizationId,
            cancellationToken: cancellationToken);

        return ToResponse(dormitory);
    }

    /// <inheritdoc />
    public async Task<DormitoryResponse> ChangeStatusAsync(
        Guid id,
        string status,
        CancellationToken cancellationToken = default)
    {
        if (!Enum.TryParse(status, ignoreCase: true, out DormitoryStatus newStatus))
        {
            throw new ValidationFailedException($"ສະຖານະ '{status}' ບໍ່ຖືກຕ້ອງ");
        }

        Dormitory dormitory = await FindAccessibleAsync(id, cancellationToken);
        dormitory.Status = newStatus;

        await _repository.SaveChangesAsync(cancellationToken);

        await _auditLogWriter.WriteAsync(
            AuditAction.ChangeStatus,
            AuditEntityName,
            dormitory.Id.ToString(),
            organizationId: dormitory.OrganizationId,
            newValues: $$"""{"status":"{{newStatus}}"}""",
            cancellationToken: cancellationToken);

        return ToResponse(dormitory);
    }

    /// <inheritdoc />
    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        Dormitory dormitory = await FindAccessibleAsync(id, cancellationToken);

        _repository.Remove(dormitory);
        await _repository.SaveChangesAsync(cancellationToken);

        await _auditLogWriter.WriteAsync(
            AuditAction.Delete,
            AuditEntityName,
            dormitory.Id.ToString(),
            organizationId: dormitory.OrganizationId,
            cancellationToken: cancellationToken);
    }

    private async Task<Dormitory> FindAccessibleAsync(Guid id, CancellationToken cancellationToken)
    {
        Dormitory dormitory = await _repository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException("ບໍ່ພົບຂໍ້ມູນຫໍພັກທີ່ຕ້ອງການ");

        TenantScope.EnsureDormitoryAccess(
            _currentUser,
            dormitory.OrganizationId,
            dormitory.Id,
            "ບໍ່ພົບຂໍ້ມູນຫໍພັກທີ່ຕ້ອງການ");

        return dormitory;
    }

    private static DormitoryStatus? ParseStatusOrNull(string? status)
        => !string.IsNullOrWhiteSpace(status) && Enum.TryParse(status, ignoreCase: true, out DormitoryStatus parsed)
            ? parsed
            : null;

    private static DormitoryResponse ToResponse(Dormitory dormitory) => new()
    {
        Id = dormitory.Id,
        OrganizationId = dormitory.OrganizationId,
        Code = dormitory.Code,
        Name = dormitory.Name,
        Description = dormitory.Description,
        Phone = dormitory.Phone,
        Email = dormitory.Email,
        Address = dormitory.Address,
        Province = dormitory.Province,
        District = dormitory.District,
        Subdistrict = dormitory.Subdistrict,
        PostalCode = dormitory.PostalCode,
        Latitude = dormitory.Latitude,
        Longitude = dormitory.Longitude,
        BillingDay = dormitory.BillingDay,
        PaymentDueDay = dormitory.PaymentDueDay,
        LateFeeType = dormitory.LateFeeType.ToString(),
        LateFeeAmount = dormitory.LateFeeAmount,
        WaterRatePerUnit = dormitory.WaterRatePerUnit,
        ElectricityRatePerUnit = dormitory.ElectricityRatePerUnit,
        Status = dormitory.Status.ToString(),
        CreatedAt = dormitory.CreatedAt,
        UpdatedAt = dormitory.UpdatedAt,
    };
}
