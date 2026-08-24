using DormFlow.Domain.Entities;

namespace DormFlow.Application.Features.Permissions;

/// <summary>เข้าถึงรายการสิทธิ์ทั้งหมดของระบบในฐานข้อมูล</summary>
public interface IPermissionRepository
{
    /// <summary>ดึงรายการสิทธิ์ทั้งหมด เรียงตามโมดูลแล้วตามรหัสสิทธิ์</summary>
    Task<IReadOnlyList<Permission>> GetAllAsync(CancellationToken cancellationToken = default);
}
