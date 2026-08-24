using DormFlow.Contracts.Permissions;

namespace DormFlow.Application.Features.Permissions;

/// <summary>Use Case ทั้งหมดของโมดูลสิทธิ์การใช้งาน</summary>
public interface IPermissionService
{
    /// <summary>ดึงรายการสิทธิ์ทั้งหมดของระบบ</summary>
    Task<IReadOnlyList<PermissionResponse>> GetAllAsync(CancellationToken cancellationToken = default);
}
