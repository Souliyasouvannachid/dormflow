using DormFlow.Contracts.Permissions;
using DormFlow.Domain.Entities;

namespace DormFlow.Application.Features.Permissions;

/// <summary>Use Case ໃບອະນຸຍາດໂມດູນທັງໝົດ</summary>
public sealed class PermissionService(IPermissionRepository repository) : IPermissionService
{
    private readonly IPermissionRepository _repository = repository;

    /// <inheritdoc />
    public async Task<IReadOnlyList<PermissionResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        IReadOnlyList<Permission> permissions = await _repository.GetAllAsync(cancellationToken);

        return [.. permissions.Select(ToResponse)];
    }

    private static PermissionResponse ToResponse(Permission permission) => new()
    {
        Code = permission.Code,
        Name = permission.Name,
        Description = permission.Description,
        Module = permission.Module,
    };
}
