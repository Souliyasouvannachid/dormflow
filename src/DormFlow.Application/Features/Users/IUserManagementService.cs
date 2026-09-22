using DormFlow.Contracts.Common;
using DormFlow.Contracts.Users;

namespace DormFlow.Application.Features.Users;

/// <summary>Use Case ໂມດູນການຄຸ້ມຄອງຜູ້ໃຊ້ທັງຝົດ</summary>
public interface IUserManagementService
{
    /// <summary>ດຶງເອົາລາຍຊື່ຜູ້ໃຊ້ທີ່ຕັ້ງໄວ້ແລ້ວ</summary>
    Task<PagedResult<UserResponse>> GetPagedAsync(
        UserListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>ອ່ານຂໍ້ມູນຜູ້ໃຊ້ຈາກລະຫັດອ້າງອີງ</summary>
    Task<UserResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>ສ້າງບັນຊີໃຝ່</summary>
    Task<UserResponse> CreateAsync(CreateUserRequest request, CancellationToken cancellationToken = default);

    /// <summary>ແກ້ໄຂຂໍ້ມູນໂປຣໄຟລ໌ຜູ້ໃຊ້</summary>
    Task<UserResponse> UpdateAsync(
        Guid id,
        UpdateUserRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>ປ່ຽນສະຖານະບັນຊີຜູ້ໃຊ້</summary>
    Task<UserResponse> ChangeStatusAsync(Guid id, string status, CancellationToken cancellationToken = default);

    /// <summary>ມອບໝາຍໜ້າທີ່ໃຫ້ຜູ້ໃຊ້ແທນໜ້າທີ່ທັງໝົດກ່ອນໜ້ານີ້</summary>
    Task<UserResponse> AssignRolesAsync(
        Guid id,
        AssignUserRolesRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>ມອບໝາຍໃຫ້ຜູ້ໃຊ້ຈັດການໜຶ່ງຫໍພັກ</summary>
    Task<UserResponse> AssignDormitoryAsync(
        Guid id,
        AssignUserDormitoryRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>ເອົາການມອບໝາຍຫໍພັກອອກຈາກຜູ້ໃຊ້</summary>
    Task RemoveDormitoryAsync(Guid id, Guid dormitoryId, CancellationToken cancellationToken = default);
}
