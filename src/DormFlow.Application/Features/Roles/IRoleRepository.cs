using DormFlow.Application.Common.Models;
using DormFlow.Application.Common.Pagination;
using DormFlow.Contracts.Common;

namespace DormFlow.Application.Features.Roles;

/// <summary>ຜົນໄດ້ຮັບຂອງການສ້າງຫຼືປ່ຽນຊື່ບົດບາດ</summary>
/// <param name="Succeeded">ປະສົບຜົນສໍາເລັດຫຼືບໍ່?</param>
/// <param name="RoleId">Role ID ມີມູນຄ່າພຽງແຕ່ກ່ຽວກັບຄວາມສໍາເລັດ</param>
/// <param name="Errors">ຂໍ້ຄວາມອະທິບາຍເຫດຜົນສໍາລັບຄວາມລົ້ມເຫຼວ</param>
public sealed record RoleOperationResult(bool Succeeded, Guid? RoleId, IReadOnlyList<string> Errors)
{
    /// <summary>ສ້າງຜົນສໍາເລັດ</summary>
    public static RoleOperationResult Success(Guid roleId) => new(true, roleId, []);

    /// <summary>ຜະລິດຜົນໄດ້ຮັບທີ່ບໍ່ສໍາເລັດ</summary>
    public static RoleOperationResult Failed(params string[] errors) => new(false, null, errors);
}

/// <summary>ເຂົ້າເຖິງ ແລະຈັດການບົດບາດໃນ ASP.NET Core Identity</summary>
public interface IRoleRepository
{
    /// <summary>ກວດເບິ່ງວ່າມີບົດບາດທີ່ມີຊື່ນີ້ຢູ່ແລ້ວ</summary>
    Task<bool> ExistsByNameAsync(string name, Guid? excludeRoleId, CancellationToken cancellationToken = default);

    /// <summary>ອ່ານຂໍ້ມູນບົດບາດຈາກລະຫັດການສົ່ງຕໍ່</summary>
    Task<RoleInfo?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>ດຶງເອົາລາຍຊື່ບົດບາດໜ້າວຽກ</summary>
    Task<PagedResult<RoleInfo>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default);

    /// <summary>ສ້າງບົດບາດໃຝ່</summary>
    Task<RoleOperationResult> CreateAsync(
        string name,
        string? description,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// ແກ້ໄຂຊື່ບົດບາດ ແລະຄຳອະທິບາຍ ການຟື້ນຟູ false ເມື່ອບໍ່ພົບບົດບາດ
    /// ຜູ້ໂທຕ້ອງກວດສອບກ່ອນວ່ານີ້ບໍ່ແມ່ນການປ່ຽນແປງຊື່ລະບົບມາດຕະຖານ
    /// </summary>
    Task<bool> UpdateAsync(
        Guid id,
        string name,
        string? description,
        CancellationToken cancellationToken = default);

    /// <summary>ລຶບການຟື້ນຟູບົດບາດ false ເມື່ອບໍ່ພົບບົດບາດ</summary>
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// ມອບສິດໃຫ້ໜ້າທີ່ແທນສິດຕົ້ນສະບັບທັງໝົດດ້ວຍອັນທີ່ລະບຸໄວ້
    /// ຟື້ນຟູ false ເມື່ອບົດບາດບໍ່ພົບຫຼືພົບເຫັນລະຫັດການອະນຸຍາດທີ່ບໍ່ມີຢູ່ໃນລະບົບ
    /// </summary>
    Task<bool> SetPermissionsAsync(
        Guid roleId,
        IReadOnlyList<string> permissionCodes,
        Guid? grantedBy,
        DateTimeOffset grantedAt,
        CancellationToken cancellationToken = default);
}
