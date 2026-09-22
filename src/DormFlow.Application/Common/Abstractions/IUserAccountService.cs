using DormFlow.Application.Common.Models;

namespace DormFlow.Application.Common.Abstractions;

/// <summary>
/// ການດໍາເນີນງານກັບບັນຊີຜູ້ໃຊ້ທີ່ຂຶ້ນກັບ ASP.NET Core Identity
/// </summary>
/// <remarks>
/// ຊັ້ນ Application ໂທຜ່ານ interface ພຽງແຕ່ນີ້, ສະນັ້ນທ່ານບໍ່ຈໍາເປັນຕ້ອງຮູ້ UserManager
/// ແລະບໍ່ມີທາງທີ່ຈະເຮັດວຽກໂດຍບັງເອີນກັບ hashes ລະຫັດຜ່ານໂດຍກົງ
/// </remarks>
public interface IUserAccountService
{
    /// <summary>ຢືນຢັນອີເມວ ແລະລະຫັດຜ່ານ ບວກກັບຄວາມພະຍາຍາມລັອກບັນຊີທີ່ບໍ່ຖືກຕ້ອງຖືກນັບ</summary>
    /// <param name="email">ອີເມວທີ່ໃຊ້ເພື່ອເຂົ້າສູ່ລະບົບ</param>
    /// <param name="password">ລະຫັດຜ່ານ</param>
    /// <param name="cancellationToken">Token ສໍາລັບການຍົກເລີກຄໍາສັ່ງ</param>
    Task<UserAuthenticationResult> AuthenticateAsync(
        string email,
        string password,
        CancellationToken cancellationToken = default);

    /// <summary>ອ່ານຂໍ້ມູນຜູ້ໃຊ້ທີ່ມີບົດບາດແລະການອະນຸຍາດທັງຝົດ</summary>
    /// <param name="userId">ID ຜູ້ໃຊ້</param>
    /// <param name="cancellationToken">Token ສໍາລັບການຍົກເລີກຄໍາສັ່ງ</param>
    /// <returns>null ເມື່ອຜູ້ໃຊ້ບໍ່ພົບຫຼືຜູ້ໃຊ້ໄດ້ຖືກລຶບຖິ້ມ</returns>
    Task<UserSecurityProfile?> GetSecurityProfileAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    /// <summary>ອ່ານຂໍ້ມູນຜູ້ໃຊ້ຈາກອີເມວ</summary>
    /// <param name="email">ອີເມວບັນຊີ</param>
    /// <param name="cancellationToken">Token ສໍາລັບການຍົກເລີກຄໍາສັ່ງ</param>
    /// <returns>null ເມື່ອຜູ້ໃຊ້ບໍ່ພົບຫຼືຜູ້ໃຊ້ໄດ້ຖືກລຶບຖິ້ມ</returns>
    Task<UserSecurityProfile?> GetSecurityProfileByEmailAsync(
        string email,
        CancellationToken cancellationToken = default);

    /// <summary>ກວດສອບວ່າບັນຊີ ແລະອົງກອນຂອງຜູ້ໃຊ້ຍັງເຮັດວຽກຢູ່</summary>
    /// <param name="userId">ID ຜູ້ໃຊ້</param>
    /// <param name="cancellationToken">Token ສໍາລັບການຍົກເລີກຄໍາສັ່ງ</param>
    Task<bool> IsAccountUsableAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>ບັນທຶກເວລາເຂົ້າສູ່ລະບົບຄັ້ງສຸດທ້າຍ</summary>
    /// <param name="userId">ID ຜູ້ໃຊ້</param>
    /// <param name="loggedInAt">ເວລາເຂົ້າສູ່ລະບົບ</param>
    /// <param name="cancellationToken">Token ສໍາລັບການຍົກເລີກຄໍາສັ່ງ</param>
    Task RecordSuccessfulLoginAsync(
        Guid userId,
        DateTimeOffset loggedInAt,
        CancellationToken cancellationToken = default);

    /// <summary>ສ້າງ token ຣີເຊັດລະຫັດຜ່ານ</summary>
    /// <param name="email">ອີເມວບັນຊີ</param>
    /// <param name="cancellationToken">Token ສໍາລັບການຍົກເລີກຄໍາສັ່ງ</param>
    /// <returns>null ເມື່ອບັນຊີນັ້ນບໍ່ພົບຜູ້ໂທຕ້ອງຕອບກັບຂໍ້ຄວາມດຽວກັນສະເໝີເພື່ອຫຼີກເວັ້ນການຄາດເດົາວ່າອີເມວນັ້ນມີຢູ່</returns>
    Task<string?> GeneratePasswordResetTokenAsync(
        string email,
        CancellationToken cancellationToken = default);

    /// <summary>ຕັ້ງລະຫັດຜ່ານໃໝ່ດ້ວຍໂທເຄັນທີ່ໄດ້ຮັບ</summary>
    /// <param name="email">ອີເມວບັນຊີ</param>
    /// <param name="token">Token ໄດ້ຮັບຜ່ານທາງອີເມວ</param>
    /// <param name="newPassword">ລະຫັດຜ່ານໃໝ່</param>
    /// <param name="cancellationToken">Token ສໍາລັບການຍົກເລີກຄໍາສັ່ງ</param>
    Task<PasswordOperationResult> ResetPasswordAsync(
        string email,
        string token,
        string newPassword,
        CancellationToken cancellationToken = default);

    /// <summary>ປ່ຽນລະຫັດຜ່ານສຳລັບບັນຊີທີ່ເຈົ້າເຂົ້າສູ່ລະບົບ</summary>
    /// <param name="userId">ID ຜູ້ໃຊ້</param>
    /// <param name="currentPassword">ລະຫັດຜ່ານປັດຈຸບັນ</param>
    /// <param name="newPassword">ລະຫັດຜ່ານໃໝ່</param>
    /// <param name="cancellationToken">Token ສໍາລັບການຍົກເລີກຄໍາສັ່ງ</param>
    Task<PasswordOperationResult> ChangePasswordAsync(
        Guid userId,
        string currentPassword,
        string newPassword,
        CancellationToken cancellationToken = default);
}
