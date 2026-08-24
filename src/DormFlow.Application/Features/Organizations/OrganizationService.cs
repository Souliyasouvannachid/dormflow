using DormFlow.Application.Common.Abstractions;
using DormFlow.Application.Common.Exceptions;
using DormFlow.Application.Common.Pagination;
using DormFlow.Contracts.Common;
using DormFlow.Contracts.Organizations;
using DormFlow.Domain.Entities;
using DormFlow.Domain.Enums;
using FluentValidation;

namespace DormFlow.Application.Features.Organizations;

/// <summary>
/// Use Case ทั้งหมดของโมดูลองค์กร
/// </summary>
/// <remarks>
/// สร้างและลบองค์กรทำได้เฉพาะ SuperAdmin เท่านั้น (บังคับด้วยสิทธิ์ที่ระดับ Controller)
/// ส่วนการดูและแก้ไขข้อมูล Owner ทำได้เฉพาะองค์กรของตนเอง
/// </remarks>
public sealed class OrganizationService(
    IOrganizationRepository repository,
    ICurrentUser currentUser,
    IAuditLogWriter auditLogWriter,
    IValidator<CreateOrganizationRequest> createValidator,
    IValidator<UpdateOrganizationRequest> updateValidator) : IOrganizationService
{
    private const string AuditEntityName = "Organization";
    private static readonly string[] AllowedSortFields = ["code", "name", "status", "createdAt"];

    private readonly IOrganizationRepository _repository = repository;
    private readonly ICurrentUser _currentUser = currentUser;
    private readonly IAuditLogWriter _auditLogWriter = auditLogWriter;
    private readonly IValidator<CreateOrganizationRequest> _createValidator = createValidator;
    private readonly IValidator<UpdateOrganizationRequest> _updateValidator = updateValidator;

    /// <inheritdoc />
    public async Task<PagedResult<OrganizationResponse>> GetPagedAsync(
        OrganizationListRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        PagedQuery query = PagedQueryNormalizer.Normalize(request, AllowedSortFields, defaultSortBy: "name");

        OrganizationListFilter filter = new(
            _currentUser.IsSuperAdmin ? null : _currentUser.OrganizationId,
            ParseStatusOrNull(request.Status));

        PagedResult<Organization> result = await _repository.GetPagedAsync(filter, query, cancellationToken);

        return PagedResult.Create(
            (IReadOnlyList<OrganizationResponse>)[.. result.Items.Select(ToResponse)],
            result.Page,
            result.PageSize,
            result.TotalItems);
    }

    /// <inheritdoc />
    public async Task<OrganizationResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => ToResponse(await FindAccessibleAsync(id, cancellationToken));

    /// <inheritdoc />
    public async Task<OrganizationResponse> CreateAsync(
        CreateOrganizationRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        await _createValidator.ValidateAndThrowAsync(request, cancellationToken);

        string code = request.Code.Trim();

        if (await _repository.ExistsByCodeAsync(code, null, cancellationToken))
        {
            throw new ConflictException($"รหัสองค์กร '{code}' ถูกใช้ในระบบแล้ว");
        }

        Organization organization = new()
        {
            Code = code,
            Name = request.Name.Trim(),
            TaxId = request.TaxId,
            Phone = request.Phone,
            Email = request.Email,
            Address = request.Address,
        };

        await _repository.AddAsync(organization, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        await _auditLogWriter.WriteAsync(
            AuditAction.Create,
            AuditEntityName,
            organization.Id.ToString(),
            organizationId: organization.Id,
            cancellationToken: cancellationToken);

        return ToResponse(organization);
    }

    /// <inheritdoc />
    public async Task<OrganizationResponse> UpdateAsync(
        Guid id,
        UpdateOrganizationRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        await _updateValidator.ValidateAndThrowAsync(request, cancellationToken);

        Organization organization = await FindAccessibleAsync(id, cancellationToken);
        string code = request.Code.Trim();

        if (await _repository.ExistsByCodeAsync(code, organization.Id, cancellationToken))
        {
            throw new ConflictException($"รหัสองค์กร '{code}' ถูกใช้ในระบบแล้ว");
        }

        organization.Code = code;
        organization.Name = request.Name.Trim();
        organization.TaxId = request.TaxId;
        organization.Phone = request.Phone;
        organization.Email = request.Email;
        organization.Address = request.Address;

        await _repository.SaveChangesAsync(cancellationToken);

        await _auditLogWriter.WriteAsync(
            AuditAction.Update,
            AuditEntityName,
            organization.Id.ToString(),
            organizationId: organization.Id,
            cancellationToken: cancellationToken);

        return ToResponse(organization);
    }

    /// <inheritdoc />
    public async Task<OrganizationResponse> ChangeStatusAsync(
        Guid id,
        string status,
        CancellationToken cancellationToken = default)
    {
        if (!Enum.TryParse(status, ignoreCase: true, out OrganizationStatus newStatus))
        {
            throw new ValidationFailedException($"สถานะ '{status}' ไม่ถูกต้อง");
        }

        Organization organization = await FindAccessibleAsync(id, cancellationToken);
        organization.Status = newStatus;

        await _repository.SaveChangesAsync(cancellationToken);

        await _auditLogWriter.WriteAsync(
            AuditAction.ChangeStatus,
            AuditEntityName,
            organization.Id.ToString(),
            organizationId: organization.Id,
            newValues: $$"""{"status":"{{newStatus}}"}""",
            cancellationToken: cancellationToken);

        return ToResponse(organization);
    }

    /// <inheritdoc />
    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        Organization organization = await FindAccessibleAsync(id, cancellationToken);

        _repository.Remove(organization);
        await _repository.SaveChangesAsync(cancellationToken);

        await _auditLogWriter.WriteAsync(
            AuditAction.Delete,
            AuditEntityName,
            organization.Id.ToString(),
            organizationId: organization.Id,
            cancellationToken: cancellationToken);
    }

    private async Task<Organization> FindAccessibleAsync(Guid id, CancellationToken cancellationToken)
    {
        Organization organization = await _repository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException("ไม่พบข้อมูลองค์กรที่ต้องการ");

        if (!_currentUser.IsSuperAdmin && _currentUser.OrganizationId != organization.Id)
        {
            throw new NotFoundException("ไม่พบข้อมูลองค์กรที่ต้องการ");
        }

        return organization;
    }

    private static OrganizationStatus? ParseStatusOrNull(string? status)
        => !string.IsNullOrWhiteSpace(status) && Enum.TryParse(status, ignoreCase: true, out OrganizationStatus parsed)
            ? parsed
            : null;

    private static OrganizationResponse ToResponse(Organization organization) => new()
    {
        Id = organization.Id,
        Code = organization.Code,
        Name = organization.Name,
        TaxId = organization.TaxId,
        Phone = organization.Phone,
        Email = organization.Email,
        Address = organization.Address,
        Status = organization.Status.ToString(),
        CreatedAt = organization.CreatedAt,
        UpdatedAt = organization.UpdatedAt,
    };
}
