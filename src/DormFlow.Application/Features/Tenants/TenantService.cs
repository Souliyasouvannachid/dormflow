using DormFlow.Application.Common.Abstractions;
using DormFlow.Application.Common.Authorization;
using DormFlow.Application.Common.Exceptions;
using DormFlow.Application.Common.Pagination;
using DormFlow.Contracts.Common;
using DormFlow.Contracts.Tenants;
using DormFlow.Domain.Entities;
using DormFlow.Domain.Enums;
using FluentValidation;

namespace DormFlow.Application.Features.Tenants;

/// <summary>
/// Use Case ทั้งหมดของโมดูลผู้เช่า
/// </summary>
/// <remarks>
/// มองเห็นได้ระดับองค์กรเท่านั้น ไม่จำกัดตามหอพัก เพราะผู้เช่าไม่ได้ผูกกับหอพักใดหอพักหนึ่งถาวร
/// (ดูคำอธิบายที่ <see cref="Tenant"/>) และไม่มีคำสั่งลบเพราะสิทธิ์ tenants.delete ไม่มีอยู่ในระบบ
/// ตามที่ระบุไว้ในเอกสารต้นฉบับ
/// </remarks>
public sealed class TenantService(
    ITenantRepository repository,
    ICurrentUser currentUser,
    IAuditLogWriter auditLogWriter,
    IValidator<CreateTenantRequest> createValidator,
    IValidator<UpdateTenantRequest> updateValidator) : ITenantService
{
    private const string AuditEntityName = "Tenant";
    private static readonly string[] AllowedSortFields = ["firstName", "lastName", "status", "createdAt"];

    private readonly ITenantRepository _repository = repository;
    private readonly ICurrentUser _currentUser = currentUser;
    private readonly IAuditLogWriter _auditLogWriter = auditLogWriter;
    private readonly IValidator<CreateTenantRequest> _createValidator = createValidator;
    private readonly IValidator<UpdateTenantRequest> _updateValidator = updateValidator;

    /// <inheritdoc />
    public async Task<PagedResult<TenantResponse>> GetPagedAsync(
        TenantListRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        PagedQuery query = PagedQueryNormalizer.Normalize(request, AllowedSortFields, defaultSortBy: "firstName");

        Guid? organizationId = _currentUser.IsSuperAdmin ? request.OrganizationId : _currentUser.OrganizationId;
        TenantListFilter filter = new(organizationId, ParseStatusOrNull(request.Status));

        PagedResult<Tenant> result = await _repository.GetPagedAsync(filter, query, cancellationToken);

        return PagedResult.Create(
            (IReadOnlyList<TenantResponse>)[.. result.Items.Select(ToResponse)],
            result.Page,
            result.PageSize,
            result.TotalItems);
    }

    /// <inheritdoc />
    public async Task<TenantResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => ToResponse(await FindAccessibleAsync(id, cancellationToken));

    /// <inheritdoc />
    public async Task<TenantResponse> CreateAsync(
        CreateTenantRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        await _createValidator.ValidateAndThrowAsync(request, cancellationToken);

        Guid organizationId = TenantScope.ResolveOrganizationIdForCreate(_currentUser, request.OrganizationId);

        if (!string.IsNullOrWhiteSpace(request.NationalId)
            && await _repository.ExistsByNationalIdAsync(organizationId, request.NationalId, null, cancellationToken))
        {
            throw new ConflictException($"เลขบัตรประจำตัว '{request.NationalId}' ถูกใช้ในองค์กรนี้แล้ว");
        }

        Tenant tenant = new()
        {
            OrganizationId = organizationId,
            FirstName = request.FirstName.Trim(),
            LastName = request.LastName.Trim(),
            Email = request.Email,
            Phone = request.Phone,
            NationalId = request.NationalId,
            DateOfBirth = request.DateOfBirth,
            Address = request.Address,
            EmergencyContactName = request.EmergencyContactName,
            EmergencyContactPhone = request.EmergencyContactPhone,
        };

        await _repository.AddAsync(tenant, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        await _auditLogWriter.WriteAsync(
            AuditAction.Create,
            AuditEntityName,
            tenant.Id.ToString(),
            organizationId: tenant.OrganizationId,
            cancellationToken: cancellationToken);

        return ToResponse(tenant);
    }

    /// <inheritdoc />
    public async Task<TenantResponse> UpdateAsync(
        Guid id,
        UpdateTenantRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        await _updateValidator.ValidateAndThrowAsync(request, cancellationToken);

        Tenant tenant = await FindAccessibleAsync(id, cancellationToken);

        if (!string.IsNullOrWhiteSpace(request.NationalId)
            && await _repository.ExistsByNationalIdAsync(tenant.OrganizationId, request.NationalId, tenant.Id, cancellationToken))
        {
            throw new ConflictException($"เลขบัตรประจำตัว '{request.NationalId}' ถูกใช้ในองค์กรนี้แล้ว");
        }

        tenant.FirstName = request.FirstName.Trim();
        tenant.LastName = request.LastName.Trim();
        tenant.Email = request.Email;
        tenant.Phone = request.Phone;
        tenant.NationalId = request.NationalId;
        tenant.DateOfBirth = request.DateOfBirth;
        tenant.Address = request.Address;
        tenant.EmergencyContactName = request.EmergencyContactName;
        tenant.EmergencyContactPhone = request.EmergencyContactPhone;

        await _repository.SaveChangesAsync(cancellationToken);

        await _auditLogWriter.WriteAsync(
            AuditAction.Update,
            AuditEntityName,
            tenant.Id.ToString(),
            organizationId: tenant.OrganizationId,
            cancellationToken: cancellationToken);

        return ToResponse(tenant);
    }

    /// <inheritdoc />
    public async Task<TenantResponse> ChangeStatusAsync(
        Guid id,
        string status,
        CancellationToken cancellationToken = default)
    {
        if (!Enum.TryParse(status, ignoreCase: true, out TenantStatus newStatus))
        {
            throw new ValidationFailedException($"สถานะ '{status}' ไม่ถูกต้อง");
        }

        Tenant tenant = await FindAccessibleAsync(id, cancellationToken);
        tenant.Status = newStatus;

        await _repository.SaveChangesAsync(cancellationToken);

        await _auditLogWriter.WriteAsync(
            AuditAction.ChangeStatus,
            AuditEntityName,
            tenant.Id.ToString(),
            organizationId: tenant.OrganizationId,
            newValues: $$"""{"status":"{{newStatus}}"}""",
            cancellationToken: cancellationToken);

        return ToResponse(tenant);
    }

    private async Task<Tenant> FindAccessibleAsync(Guid id, CancellationToken cancellationToken)
    {
        Tenant tenant = await _repository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException("ไม่พบข้อมูลผู้เช่าที่ต้องการ");

        TenantScope.EnsureOrganizationAccess(_currentUser, tenant.OrganizationId, "ไม่พบข้อมูลผู้เช่าที่ต้องการ");

        return tenant;
    }

    private static TenantStatus? ParseStatusOrNull(string? status)
        => !string.IsNullOrWhiteSpace(status) && Enum.TryParse(status, ignoreCase: true, out TenantStatus parsed)
            ? parsed
            : null;

    private static TenantResponse ToResponse(Tenant tenant) => new()
    {
        Id = tenant.Id,
        OrganizationId = tenant.OrganizationId,
        FirstName = tenant.FirstName,
        LastName = tenant.LastName,
        Email = tenant.Email,
        Phone = tenant.Phone,
        NationalId = tenant.NationalId,
        DateOfBirth = tenant.DateOfBirth,
        Address = tenant.Address,
        EmergencyContactName = tenant.EmergencyContactName,
        EmergencyContactPhone = tenant.EmergencyContactPhone,
        Status = tenant.Status.ToString(),
        CreatedAt = tenant.CreatedAt,
        UpdatedAt = tenant.UpdatedAt,
    };
}
