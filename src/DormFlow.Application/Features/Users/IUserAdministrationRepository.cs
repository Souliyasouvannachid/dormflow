using DormFlow.Application.Common.Models;
using DormFlow.Application.Common.Pagination;
using DormFlow.Contracts.Common;
using DormFlow.Domain.Enums;

namespace DormFlow.Application.Features.Users;

/// <summary>ການກັ່ນຕອງສໍາລັບການດຶງບັນຊີລາຍຊື່ paginated ຂອງຜູ້ໃຊ້</summary>
/// <param name="OrganizationId">ການກັ່ນຕອງພຽງແຕ່ຜູ້ໃຊ້ຂອງອົງກອນທີ່ລະບຸໄວ້ເປັນ null ໃນເວລາທີ່ບໍ່ຈໍາກັດ</param>
/// <param name="DormitoryId">ການກັ່ນຕອງພຽງແຕ່ຜູ້ໃຊ້ມອບຝາຍໃຫ້ຫໍພັກທີ່ລະບຸໄວ້ເປັນ null ໃນເວລາທີ່ບໍ່ຈໍາກັດ</param>
/// <param name="Status">ກັ່ນຕອງສະເພາະຜູ້ໃຊ້ທີ່ສະຖານະກົງກັບ null ໃນເວລາທີ່ບໍ່ຈໍາກັດ</param>
/// <param name="RestrictToDormitoryIds">
/// ຈໍາກັດຜົນໄດ້ຮັບກັບຜູ້ໃຊ້ທີ່ຖືກມອບຝາຍໃຫ້ຢ່າງຜ້ອຍຜຶ່ງຫໍພັກໃນບັນຊີລາຍຊື່ນີ້
/// ແມ່ນ null ເມື່ອຜູ້ໂທສາມາດເຫັນໄດ້ໂດຍຜູ້ໃຊ້ທຸກຄົນໃນອົງກອນຂອງພວກເຂົາ (SuperAdmin ແລະ Owner)
/// </param>
public sealed record UserListFilter(
    Guid? OrganizationId,
    Guid? DormitoryId,
    UserStatus? Status,
    IReadOnlyCollection<Guid>? RestrictToDormitoryIds);

/// <summary>ຜົນໄດ້ຮັບຂອງການສ້າງບັນຊີຜູ້ໃຊ້ໃຝ່</summary>
/// <param name="Succeeded">ປະສົບຜົນສໍາເລັດຫຼືບໍ່?</param>
/// <param name="UserId">ສ້າງ ID ຜູ້ໃຊ້ສົບຜົນສໍາເລັດມັນມີມູນຄ່າພຽງແຕ່ໃນເວລາທີ່ສໍາເລັດ</param>
/// <param name="Errors">ຂໍ້ຄວາມອະທິບາຍເຫດຜົນສໍາລັບຄວາມລົ້ມເຫຼວ</param>
public sealed record UserCreationResult(bool Succeeded, Guid? UserId, IReadOnlyList<string> Errors)
{
    /// <summary>ສ້າງຜົນສໍາເລັດ</summary>
    public static UserCreationResult Success(Guid userId) => new(true, userId, []);

    /// <summary>ຜະລິດຜົນໄດ້ຮັບທີ່ບໍ່ສໍາເລັດ</summary>
    public static UserCreationResult Failed(params string[] errors) => new(false, null, errors);
}

/// <summary>
/// ເຂົ້າເຖິງ ແລະຈັດການບັນຊີຜູ້ໃຊ້ໃນຖານຂໍ້ມູນ. ສໍາລັບຜ້າຈໍການດູແລຜູ້ໃຊ້ຂອງຜູ້ບໍລິຫານ
/// </summary>
/// <remarks>
/// ແຍກອອກຈາກ <see cref="Common.Abstractions.IUserAccountService"/> ເນື່ອງຈາກວ່າມັນເປັນຄວາມຮັບຜິດຊອບຂອງແຕ່ລະຄົນ
/// ບຸກຄົນນັ້ນເບິ່ງແຍງການກວດສອບຕົວຕົນແລະລະຫັດຜ່ານຂອງຜູ້ໃຊ້ບັນຊີ
/// ພາກນີ້ເບິ່ງແຍງການຈັດການບັນຊີຜູ້ໃຊ້ອື່ນໂດຍຜູ້ເບິ່ງແຍງລະບົບ
/// </remarks>
public interface IUserAdministrationRepository
{
    /// <summary>ກວດເບິ່ງວ່າມີບັນຊີທີ່ມີທີ່ຢູ່ອີເມວນີ້ຢູ່ກ່ອນແລ້ວ</summary>
    Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>ອ່ານຂໍ້ມູນຜູ້ໃຊ້ຈາກລະຫັດອ້າງອີງ</summary>
    Task<UserSecurityProfile?> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>ດຶງເອົາລາຍຊື່ຜູ້ໃຊ້ທີ່ມີ paginated ໂດຍອີງໃສ່ຕົວກອງທີ່ລະບຸ</summary>
    Task<PagedResult<UserSecurityProfile>> GetPagedAsync(
        UserListFilter filter,
        PagedQuery query,
        CancellationToken cancellationToken = default);

    /// <summary>ສ້າງບັນຊີໃຝ່</summary>
    Task<UserCreationResult> CreateAsync(
        Guid? organizationId,
        string email,
        string password,
        string firstName,
        string lastName,
        string? phone,
        CancellationToken cancellationToken = default);

    /// <summary>ແກ້ໄຂຂໍ້ມູນໂປຣໄຟລ໌ຜູ້ໃຊ້ Restore false ເມື່ອຜູ້ໃຊ້ບໍ່ພົບ</summary>
    Task<bool> UpdateProfileAsync(
        Guid userId,
        string firstName,
        string lastName,
        string? phone,
        string? profileImage,
        CancellationToken cancellationToken = default);

    /// <summary>ປ່ຽນສະຖານະບັນຊີຜູ້ໃຊ້ຄືນ false ເມື່ອຜູ້ໃຊ້ບໍ່ພົບ</summary>
    Task<bool> ChangeStatusAsync(
        Guid userId,
        UserStatus status,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// ມອບໝາຍໜ້າທີ່ໃຫ້ຜູ້ໃຊ້ແທນໜ້າທີ່ເດີມທັງໝົດດ້ວຍໜ້າທີ່ທີ່ລະບຸໄວ້
    /// ຟື້ນຟູ false ເມື່ອຜູ້ໃຊ້ບໍ່ພົບ ຫຼືພົບຊື່ບົດບາດທີ່ບໍ່ມີຢູ່ໃນລະບົບ
    /// </summary>
    Task<bool> SetRolesAsync(
        Guid userId,
        IReadOnlyList<string> roleNames,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// ມອບໃຫ້ຜູ້ໃຊ້ຢູ່ໃນຫໍພັກຜຶ່ງ. ຟື້ນຟູ false ເມື່ອຜູ້ໃຊ້ບໍ່ພົບຫຼືຫໍພັກບໍ່ພົບ
    /// ໃນເວລາທີ່ມັນໄດ້ຮັບການສ້າງຕັ້ງເປັນຫໍພັກຕົ້ນຕໍຂອງຜູ້ໃຊ້ນີ້ຫໍພັກຕົ້ນສະບັບ (ຖ້າມີ) ສະຖານະຫໍພັກຫຼັກຈະຖືກຍົກເລີກໂດຍອັດຕະໂນມັດ
    /// </summary>
    Task<bool> AssignDormitoryAsync(
        Guid userId,
        Guid dormitoryId,
        bool isPrimary,
        Guid? assignedBy,
        DateTimeOffset assignedAt,
        CancellationToken cancellationToken = default);

    /// <summary>ເອົາການມອບຝາຍຫໍພັກອອກຈາກຜູ້ໃຊ້ Restore false ເມື່ອການມອບໝາຍນັ້ນບໍ່ພົບ</summary>
    Task<bool> RemoveDormitoryAsync(
        Guid userId,
        Guid dormitoryId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// ກວດສອບວ່າຜູ້ໃຊ້ທີ່ລະບຸນັ້ນເປັນຂອງອົງກອນນີ້. ມີບົດບາດທີ່ກໍານົດໄວ້ແລະໄດ້ຮັບການມອບຝາຍໃຫ້ເບິ່ງແຍງຫໍພັກທີ່ກໍານົດໄວ້ຫຼືບໍ່
    /// ໃຊ້ໃນການກວດສອບກ່ອນການມອບໝາຍວຽກເຊັ່ນ: ມອບໝາຍໃຫ້ຊ່າງສ້ອມແປງເພື່ອເບິ່ງແຍງວຽກງານສ້ອມແປງໃນຫໍພັກໂດຍສະເພາະ
    /// </summary>
    Task<bool> HasRoleForDormitoryAsync(
        Guid userId,
        string roleName,
        Guid organizationId,
        Guid dormitoryId,
        CancellationToken cancellationToken = default);
}
