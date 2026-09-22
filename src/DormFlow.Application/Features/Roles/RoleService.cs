using DormFlow.Application.Common.Abstractions;
using DormFlow.Application.Common.Exceptions;
using DormFlow.Application.Common.Models;
using DormFlow.Application.Common.Pagination;
using DormFlow.Contracts.Common;
using DormFlow.Contracts.Roles;
using DormFlow.Domain.Enums;
using FluentValidation;

namespace DormFlow.Application.Features.Roles;

/// <summary>Use Case ໂມດູນບົດບາດທັງໝົດ</summary>
public sealed class RoleService(
    IRoleRepository repository,
    ICurrentUser currentUser,
    IDateTimeProvider dateTimeProvider,
    IAuditLogWriter auditLogWriter,
    IValidator<CreateRoleRequest> createValidator,
    IValidator<UpdateRoleRequest> updateValidator,
    IValidator<AssignRolePermissionsRequest> assignPermissionsValidator) : IRoleService
{
    private const string AuditEntityName = "Role";
    private static readonly string[] AllowedSortFields = ["name"];

    private readonly IRoleRepository _repository = repository;
    private readonly ICurrentUser _currentUser = currentUser;
    private readonly IDateTimeProvider _dateTimeProvider = dateTimeProvider;
    private readonly IAuditLogWriter _auditLogWriter = auditLogWriter;
    private readonly IValidator<CreateRoleRequest> _createValidator = createValidator;
    private readonly IValidator<UpdateRoleRequest> _updateValidator = updateValidator;
    private readonly IValidator<AssignRolePermissionsRequest> _assignPermissionsValidator = assignPermissionsValidator;

    /// <inheritdoc />
    public async Task<PagedResult<RoleResponse>> GetPagedAsync(
        RoleListRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        PagedQuery query = PagedQueryNormalizer.Normalize(request, AllowedSortFields, defaultSortBy: "name");
        PagedResult<RoleInfo> result = await _repository.GetPagedAsync(query, cancellationToken);

        return PagedResult.Create(
            (IReadOnlyList<RoleResponse>)[.. result.Items.Select(ToResponse)],
            result.Page,
            result.PageSize,
            result.TotalItems);
    }

    /// <inheritdoc />
    public async Task<RoleResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => ToResponse(await RequireByIdAsync(id, cancellationToken));

    /// <inheritdoc />
    public async Task<RoleResponse> CreateAsync(
        CreateRoleRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        await _createValidator.ValidateAndThrowAsync(request, cancellationToken);

        string name = request.Name.Trim();

        if (await _repository.ExistsByNameAsync(name, null, cancellationToken))
        {
            throw new ConflictException($"ຊື່ບົດບາດ '{name}' ຖືກໃຊ້ໃນລະບົບແລ້ວ");
        }

        RoleOperationResult result = await _repository.CreateAsync(name, request.Description, cancellationToken);

        if (!result.Succeeded || result.RoleId is null)
        {
            throw new ValidationFailedException(
                result.Errors.Count > 0 ? string.Join(" ", result.Errors) : "ສ້າງບົດບາດບໍ່ສຳເລັດ");
        }

        await _auditLogWriter.WriteAsync(
            AuditAction.Create,
            AuditEntityName,
            result.RoleId.Value.ToString(),
            cancellationToken: cancellationToken);

        return ToResponse(await RequireByIdAsync(result.RoleId.Value, cancellationToken));
    }

    /// <inheritdoc />
    public async Task<RoleResponse> UpdateAsync(
        Guid id,
        UpdateRoleRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        await _updateValidator.ValidateAndThrowAsync(request, cancellationToken);

        RoleInfo role = await RequireByIdAsync(id, cancellationToken);
        string name = request.Name.Trim();

        if (role.IsSystemRole && !string.Equals(role.Name, name, StringComparison.Ordinal))
        {
            throw new ConflictException("ບໍ່ສາມາດປ່ຽນຊື່ບົດບາດມາດຕະຖານຂອງລະບົບໄດ້");
        }

        if (!string.Equals(role.Name, name, StringComparison.Ordinal)
            && await _repository.ExistsByNameAsync(name, role.Id, cancellationToken))
        {
            throw new ConflictException($"ຊື່ບົດບາດ '{name}' ຖືກໃຊ້ໃນລະບົບແລ້ວ");
        }

        await _repository.UpdateAsync(role.Id, name, request.Description, cancellationToken);

        await _auditLogWriter.WriteAsync(
            AuditAction.Update,
            AuditEntityName,
            role.Id.ToString(),
            cancellationToken: cancellationToken);

        return ToResponse(await RequireByIdAsync(role.Id, cancellationToken));
    }

    /// <inheritdoc />
    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        RoleInfo role = await RequireByIdAsync(id, cancellationToken);

        if (role.IsSystemRole)
        {
            throw new ConflictException("ບໍ່ສາມາດລຶບບົດບາດມາດຕະຖານຂອງລະບົບໄດ້");
        }

        await _repository.DeleteAsync(role.Id, cancellationToken);

        await _auditLogWriter.WriteAsync(
            AuditAction.Delete,
            AuditEntityName,
            role.Id.ToString(),
            cancellationToken: cancellationToken);
    }

    /// <inheritdoc />
    public async Task<RoleResponse> AssignPermissionsAsync(
        Guid id,
        AssignRolePermissionsRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        await _assignPermissionsValidator.ValidateAndThrowAsync(request, cancellationToken);

        RoleInfo role = await RequireByIdAsync(id, cancellationToken);

        bool succeeded = await _repository.SetPermissionsAsync(
            role.Id,
            request.Permissions,
            _currentUser.UserId,
            _dateTimeProvider.UtcNow,
            cancellationToken);

        if (!succeeded)
        {
            throw new ValidationFailedException("ກຳນົດສິດບໍ່ສຳເລັດ ກະລຸນາກວດສອບລະຫັດສິດທີ່ລະບຸ");
        }

        await _auditLogWriter.WriteAsync(
            AuditAction.Update,
            AuditEntityName,
            role.Id.ToString(),
            newValues: $$"""{"permissions":[{{string.Join(",", request.Permissions.Select(code => $"\"{code}\""))}}]}""",
            cancellationToken: cancellationToken);

        return ToResponse(await RequireByIdAsync(role.Id, cancellationToken));
    }

    private async Task<RoleInfo> RequireByIdAsync(Guid id, CancellationToken cancellationToken)
        => await _repository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException("ບໍ່ພົບຂໍ້ມູນບົດບາດທີ່ຕ້ອງການ");

    private static RoleResponse ToResponse(RoleInfo role) => new()
    {
        Id = role.Id,
        Name = role.Name,
        Description = role.Description,
        IsSystemRole = role.IsSystemRole,
        Permissions = role.Permissions,
    };
}
