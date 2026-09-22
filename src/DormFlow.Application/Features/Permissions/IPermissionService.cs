using DormFlow.Contracts.Permissions;

namespace DormFlow.Application.Features.Permissions;

/// <summary>Use Case ໃບອະນຸຍາດໂມດູນທັງໝົດ</summary>
public interface IPermissionService
{
    /// <summary>ດຶງລາຍຊື່ການອະນຸຍາດລະບົບທັງໝົດ</summary>
    Task<IReadOnlyList<PermissionResponse>> GetAllAsync(CancellationToken cancellationToken = default);
}
