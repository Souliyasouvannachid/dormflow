using DormFlow.Contracts.Common;
using DormFlow.Contracts.Users;

namespace DormFlow.Application.Features.Users;

/// <summary>Use Case ทั้งหมดของโมดูลบริหารจัดการผู้ใช้งาน</summary>
public interface IUserManagementService
{
    /// <summary>ดึงรายการผู้ใช้แบบแบ่งหน้า</summary>
    Task<PagedResult<UserResponse>> GetPagedAsync(
        UserListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>อ่านข้อมูลผู้ใช้จากรหัสอ้างอิง</summary>
    Task<UserResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>สร้างบัญชีผู้ใช้ใหม่</summary>
    Task<UserResponse> CreateAsync(CreateUserRequest request, CancellationToken cancellationToken = default);

    /// <summary>แก้ไขข้อมูลโปรไฟล์ของผู้ใช้</summary>
    Task<UserResponse> UpdateAsync(
        Guid id,
        UpdateUserRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>เปลี่ยนสถานะบัญชีผู้ใช้</summary>
    Task<UserResponse> ChangeStatusAsync(Guid id, string status, CancellationToken cancellationToken = default);

    /// <summary>กำหนดบทบาทให้ผู้ใช้ แทนที่บทบาทเดิมทั้งหมด</summary>
    Task<UserResponse> AssignRolesAsync(
        Guid id,
        AssignUserRolesRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>มอบหมายให้ผู้ใช้ดูแลหอพักหนึ่งแห่ง</summary>
    Task<UserResponse> AssignDormitoryAsync(
        Guid id,
        AssignUserDormitoryRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>ถอนการมอบหมายหอพักออกจากผู้ใช้</summary>
    Task RemoveDormitoryAsync(Guid id, Guid dormitoryId, CancellationToken cancellationToken = default);
}
