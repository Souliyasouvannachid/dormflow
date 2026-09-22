using DormFlow.Contracts.Common;
using DormFlow.Contracts.Roles;

namespace DormFlow.Application.Features.Roles;

/// <summary>Use Case ໂມດູນບົດບາດທັງໝົດ</summary>
public interface IRoleService
{
    /// <summary>ດຶງເອົາລາຍຊື່ບົດບາດໜ້າວຽກ</summary>
    Task<PagedResult<RoleResponse>> GetPagedAsync(
        RoleListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>ອ່ານຂໍ້ມູນບົດບາດຈາກລະຫັດການສົ່ງຕໍ່</summary>
    Task<RoleResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>ສ້າງບົດບາດໃຝ່</summary>
    Task<RoleResponse> CreateAsync(CreateRoleRequest request, CancellationToken cancellationToken = default);

    /// <summary>ແກ້ໄຂຂໍ້ມູນບົດບາດ</summary>
    Task<RoleResponse> UpdateAsync(
        Guid id,
        UpdateRoleRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>ລຶບບົດບາດ</summary>
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>ມອບສິດໃຫ້ໜ້າທີ່ແທນສິດຕົ້ນສະບັບທັງໝົດ</summary>
    Task<RoleResponse> AssignPermissionsAsync(
        Guid id,
        AssignRolePermissionsRequest request,
        CancellationToken cancellationToken = default);
}
