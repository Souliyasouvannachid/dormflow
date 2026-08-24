using DormFlow.Application.Common.Abstractions;
using DormFlow.Application.Common.Authorization;
using DormFlow.Application.Common.Exceptions;
using DormFlow.Application.Common.Models;
using DormFlow.Application.Common.Pagination;
using DormFlow.Application.Features.Dormitories;
using DormFlow.Contracts.Common;
using DormFlow.Contracts.Users;
using DormFlow.Domain.Entities;
using DormFlow.Domain.Enums;
using FluentValidation;

namespace DormFlow.Application.Features.Users;

/// <summary>Use Case ทั้งหมดของโมดูลบริหารจัดการผู้ใช้งาน</summary>
public sealed class UserManagementService(
    IUserAdministrationRepository repository,
    IDormitoryRepository dormitoryRepository,
    ICurrentUser currentUser,
    IDateTimeProvider dateTimeProvider,
    IAuditLogWriter auditLogWriter,
    IValidator<CreateUserRequest> createValidator,
    IValidator<UpdateUserRequest> updateValidator,
    IValidator<AssignUserRolesRequest> assignRolesValidator,
    IValidator<AssignUserDormitoryRequest> assignDormitoryValidator) : IUserManagementService
{
    private const string AuditEntityName = "User";
    private static readonly string[] AllowedSortFields = ["email", "firstName", "lastName", "status", "createdAt"];

    private readonly IUserAdministrationRepository _repository = repository;
    private readonly IDormitoryRepository _dormitoryRepository = dormitoryRepository;
    private readonly ICurrentUser _currentUser = currentUser;
    private readonly IDateTimeProvider _dateTimeProvider = dateTimeProvider;
    private readonly IAuditLogWriter _auditLogWriter = auditLogWriter;
    private readonly IValidator<CreateUserRequest> _createValidator = createValidator;
    private readonly IValidator<UpdateUserRequest> _updateValidator = updateValidator;
    private readonly IValidator<AssignUserRolesRequest> _assignRolesValidator = assignRolesValidator;
    private readonly IValidator<AssignUserDormitoryRequest> _assignDormitoryValidator = assignDormitoryValidator;

    /// <inheritdoc />
    public async Task<PagedResult<UserResponse>> GetPagedAsync(
        UserListRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        PagedQuery query = PagedQueryNormalizer.Normalize(request, AllowedSortFields, defaultSortBy: "firstName");

        Guid? organizationId = _currentUser.IsSuperAdmin ? request.OrganizationId : _currentUser.OrganizationId;

        UserListFilter filter = new(
            organizationId,
            request.DormitoryId,
            ParseStatusOrNull(request.Status),
            TenantScope.GetDormitoryRestrictionForListing(_currentUser));

        PagedResult<UserSecurityProfile> result = await _repository.GetPagedAsync(filter, query, cancellationToken);

        return PagedResult.Create(
            (IReadOnlyList<UserResponse>)[.. result.Items.Select(ToResponse)],
            result.Page,
            result.PageSize,
            result.TotalItems);
    }

    /// <inheritdoc />
    public async Task<UserResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        UserSecurityProfile profile = await FindAccessibleAsync(id, cancellationToken);
        return ToResponse(profile);
    }

    /// <inheritdoc />
    public async Task<UserResponse> CreateAsync(
        CreateUserRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        await _createValidator.ValidateAndThrowAsync(request, cancellationToken);

        Guid organizationId = TenantScope.ResolveOrganizationIdForCreate(_currentUser, request.OrganizationId);
        string email = request.Email.Trim();

        if (await _repository.ExistsByEmailAsync(email, cancellationToken))
        {
            throw new ConflictException($"อีเมล '{email}' ถูกใช้ในระบบแล้ว");
        }

        UserCreationResult result = await _repository.CreateAsync(
            organizationId,
            email,
            request.Password,
            request.FirstName.Trim(),
            request.LastName.Trim(),
            request.Phone,
            cancellationToken);

        if (!result.Succeeded || result.UserId is null)
        {
            throw new ValidationFailedException(
                result.Errors.Count > 0 ? string.Join(" ", result.Errors) : "สร้างบัญชีผู้ใช้ไม่สำเร็จ");
        }

        UserSecurityProfile profile = await _repository.GetByIdAsync(result.UserId.Value, cancellationToken)
            ?? throw new InvalidOperationException("สร้างบัญชีผู้ใช้สำเร็จแต่ไม่พบข้อมูลที่เพิ่งสร้าง");

        await _auditLogWriter.WriteAsync(
            AuditAction.Create,
            AuditEntityName,
            profile.UserId.ToString(),
            organizationId: profile.OrganizationId,
            cancellationToken: cancellationToken);

        return ToResponse(profile);
    }

    /// <inheritdoc />
    public async Task<UserResponse> UpdateAsync(
        Guid id,
        UpdateUserRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        await _updateValidator.ValidateAndThrowAsync(request, cancellationToken);

        UserSecurityProfile profile = await FindAccessibleAsync(id, cancellationToken);

        await _repository.UpdateProfileAsync(
            profile.UserId,
            request.FirstName.Trim(),
            request.LastName.Trim(),
            request.Phone,
            request.ProfileImage,
            cancellationToken);

        await _auditLogWriter.WriteAsync(
            AuditAction.Update,
            AuditEntityName,
            profile.UserId.ToString(),
            organizationId: profile.OrganizationId,
            cancellationToken: cancellationToken);

        return ToResponse(await RequireByIdAsync(profile.UserId, cancellationToken));
    }

    /// <inheritdoc />
    public async Task<UserResponse> ChangeStatusAsync(
        Guid id,
        string status,
        CancellationToken cancellationToken = default)
    {
        if (!Enum.TryParse(status, ignoreCase: true, out UserStatus newStatus))
        {
            throw new ValidationFailedException($"สถานะ '{status}' ไม่ถูกต้อง");
        }

        UserSecurityProfile profile = await FindAccessibleAsync(id, cancellationToken);

        await _repository.ChangeStatusAsync(profile.UserId, newStatus, cancellationToken);

        await _auditLogWriter.WriteAsync(
            AuditAction.ChangeStatus,
            AuditEntityName,
            profile.UserId.ToString(),
            organizationId: profile.OrganizationId,
            newValues: $$"""{"status":"{{newStatus}}"}""",
            cancellationToken: cancellationToken);

        return ToResponse(await RequireByIdAsync(profile.UserId, cancellationToken));
    }

    /// <inheritdoc />
    public async Task<UserResponse> AssignRolesAsync(
        Guid id,
        AssignUserRolesRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        await _assignRolesValidator.ValidateAndThrowAsync(request, cancellationToken);

        UserSecurityProfile profile = await FindAccessibleAsync(id, cancellationToken);

        bool succeeded = await _repository.SetRolesAsync(profile.UserId, request.Roles, cancellationToken);
        if (!succeeded)
        {
            throw new ValidationFailedException("กำหนดบทบาทไม่สำเร็จ กรุณาตรวจสอบชื่อบทบาทที่ระบุ");
        }

        await _auditLogWriter.WriteAsync(
            AuditAction.AssignRole,
            AuditEntityName,
            profile.UserId.ToString(),
            organizationId: profile.OrganizationId,
            newValues: $$"""{"roles":[{{string.Join(",", request.Roles.Select(role => $"\"{role}\""))}}]}""",
            cancellationToken: cancellationToken);

        return ToResponse(await RequireByIdAsync(profile.UserId, cancellationToken));
    }

    /// <inheritdoc />
    public async Task<UserResponse> AssignDormitoryAsync(
        Guid id,
        AssignUserDormitoryRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        await _assignDormitoryValidator.ValidateAndThrowAsync(request, cancellationToken);

        UserSecurityProfile profile = await FindAccessibleAsync(id, cancellationToken);

        Dormitory dormitory = await _dormitoryRepository.GetByIdAsync(request.DormitoryId, cancellationToken)
            ?? throw new NotFoundException("ไม่พบหอพักที่ระบุ");

        TenantScope.EnsureOrganizationAccess(
            _currentUser,
            dormitory.OrganizationId,
            "ไม่พบหอพักที่ระบุ");

        if (dormitory.OrganizationId != profile.OrganizationId)
        {
            throw new ValidationFailedException("ไม่สามารถมอบหมายหอพักต่างองค์กรให้ผู้ใช้รายนี้ได้");
        }

        bool succeeded = await _repository.AssignDormitoryAsync(
            profile.UserId,
            dormitory.Id,
            request.IsPrimary,
            _currentUser.UserId,
            _dateTimeProvider.UtcNow,
            cancellationToken);

        if (!succeeded)
        {
            throw new ConflictException("ผู้ใช้รายนี้ได้รับมอบหมายให้ดูแลหอพักนี้อยู่แล้ว");
        }

        await _auditLogWriter.WriteAsync(
            AuditAction.AssignDormitory,
            AuditEntityName,
            profile.UserId.ToString(),
            organizationId: profile.OrganizationId,
            newValues: $$"""{"dormitoryId":"{{dormitory.Id}}","isPrimary":{{(request.IsPrimary ? "true" : "false")}}}""",
            cancellationToken: cancellationToken);

        return ToResponse(await RequireByIdAsync(profile.UserId, cancellationToken));
    }

    /// <inheritdoc />
    public async Task RemoveDormitoryAsync(Guid id, Guid dormitoryId, CancellationToken cancellationToken = default)
    {
        UserSecurityProfile profile = await FindAccessibleAsync(id, cancellationToken);

        bool succeeded = await _repository.RemoveDormitoryAsync(profile.UserId, dormitoryId, cancellationToken);
        if (!succeeded)
        {
            throw new NotFoundException("ไม่พบการมอบหมายหอพักนี้ให้ผู้ใช้รายดังกล่าว");
        }

        await _auditLogWriter.WriteAsync(
            AuditAction.AssignDormitory,
            AuditEntityName,
            profile.UserId.ToString(),
            organizationId: profile.OrganizationId,
            newValues: $$"""{"removedDormitoryId":"{{dormitoryId}}"}""",
            cancellationToken: cancellationToken);
    }

    private async Task<UserSecurityProfile> FindAccessibleAsync(Guid id, CancellationToken cancellationToken)
    {
        UserSecurityProfile profile = await _repository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException("ไม่พบข้อมูลผู้ใช้ที่ต้องการ");

        TenantScope.EnsureOrganizationAccess(
            _currentUser,
            profile.OrganizationId.GetValueOrDefault(),
            "ไม่พบข้อมูลผู้ใช้ที่ต้องการ");

        return profile;
    }

    private async Task<UserSecurityProfile> RequireByIdAsync(Guid id, CancellationToken cancellationToken)
        => await _repository.GetByIdAsync(id, cancellationToken)
            ?? throw new InvalidOperationException($"ไม่พบผู้ใช้ {id} หลังจากบันทึกข้อมูลสำเร็จแล้ว");

    private static UserStatus? ParseStatusOrNull(string? status)
        => !string.IsNullOrWhiteSpace(status) && Enum.TryParse(status, ignoreCase: true, out UserStatus parsed)
            ? parsed
            : null;

    private static UserResponse ToResponse(UserSecurityProfile profile) => new()
    {
        Id = profile.UserId,
        Email = profile.Email,
        FirstName = profile.FirstName,
        LastName = profile.LastName,
        Phone = profile.Phone,
        ProfileImage = profile.ProfileImage,
        OrganizationId = profile.OrganizationId,
        Status = profile.Status.ToString(),
        LastLoginAt = profile.LastLoginAt,
        CreatedAt = profile.CreatedAt,
        Roles = profile.Roles,
        Dormitories =
        [
            .. profile.DormitoryAssignments.Select(assignment => new UserDormitoryAssignmentResponse
            {
                DormitoryId = assignment.DormitoryId,
                IsPrimary = assignment.IsPrimary,
                AssignedAt = assignment.AssignedAt,
            }),
        ],
    };
}
