using DormFlow.Application.Common.Models;
using DormFlow.Domain.Enums;

namespace DormFlow.Application.Common.Abstractions;

/// <summary>
/// ການຄຸ້ມຄອງວົງຈອນຊີວິດຂອງ Refresh Token ລວມທັງການອອກ, ກວດກາ, ໝູນວຽນ ແລະ ຖອນຄືນ
/// </summary>
public interface IRefreshTokenService
{
    /// <summary>ອອກ Refresh Token ໃບຮັບຮອງໃໝ່ສຳລັບຜູ້ໃຊ້</summary>
    /// <param name="userId">ເຈົ້າຂອງໂທເຄັນ</param>
    /// <param name="jwtId">ມູນຄ່າ jti ຂອງ Access Token ທີ່ອອກມາຮ່ວມກັນ</param>
    /// <param name="ipAddress">ເລກ IP ຕົ້ນກໍາເນີດ</param>
    /// <param name="userAgent">User Agent ຂອງຄໍາຮ້ອງສະຝັກ</param>
    /// <param name="cancellationToken">Token ສໍາລັບການຍົກເລີກຄໍາສັ່ງ</param>
    Task<IssuedRefreshToken> IssueAsync(
        Guid userId,
        string jwtId,
        string? ipAddress,
        string? userAgent,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// ກວດເບິ່ງວ່າໂທເຄັນທີ່ສົ່ງໂດຍຜູ້ໂທສາມາດຖືກນໍາໃຊ້ສໍາລັບການຕໍ່ອາຍຸ
    /// </summary>
    /// <remarks>
    /// ເມື່ອມັນໄດ້ຖືກຄົ້ນພົບວ່າ tokens ໄດ້ຖືກ spun ແລ້ວແຕ່ໄດ້ຖືກນໍາມາໃຊ້ຄືນ
    /// ວິທີການນີ້ທັນທີຖອນ tokens ທີ່ໃຊ້ວຽກທັງຝົດສໍາລັບຜູ້ໃຊ້ນັ້ນກ່ອນທີ່ຈະກັບຄືນຜົນໄດ້ຮັບ
    /// ເນື່ອງຈາກວ່າມັນຝາຍຄວາມວ່າ token ໄດ້ຕົກຢູ່ໃນມືຂອງຄົນອື່ນ
    /// </remarks>
    /// <param name="rawToken">ມູນຄ່າໂທເຄັນທີ່ສົ່ງໂດຍຜູ້ໂທ</param>
    /// <param name="ipAddress">ເລກ IP ຕົ້ນກໍາເນີດ</param>
    /// <param name="cancellationToken">Token ສໍາລັບການຍົກເລີກຄໍາສັ່ງ</param>
    Task<RefreshTokenValidationResult> ValidateAsync(
        string rawToken,
        string? ipAddress,
        CancellationToken cancellationToken = default);

    /// <summary>ຖອດຖອນໂທເຄັນເກົ່າ ແລະອອກອັນໃໝ່ໃນທຸລະກຳດຽວກັນ</summary>
    /// <param name="currentTokenId">ລະຫັດຂອງ token ຕົ້ນສະບັບ</param>
    /// <param name="jwtId">ມູນຄ່າ jti ຂອງ Access Token ໃບໃໝ່</param>
    /// <param name="ipAddress">ເລກ IP ຕົ້ນກໍາເນີດ</param>
    /// <param name="userAgent">User Agent ຂອງຄໍາຮ້ອງສະຝັກ</param>
    /// <param name="cancellationToken">Token ສໍາລັບການຍົກເລີກຄໍາສັ່ງ</param>
    Task<IssuedRefreshToken> RotateAsync(
        Guid currentTokenId,
        string jwtId,
        string? ipAddress,
        string? userAgent,
        CancellationToken cancellationToken = default);

    /// <summary>ຍົກເລີກໜຶ່ງໂທເຄັນ</summary>
    /// <param name="rawToken">ຄ່າຂອງໂທເຄັນທີ່ຈະຖືກຖອນຄືນ</param>
    /// <param name="reason">ເຫດຜົນສໍາລັບການຖອນ</param>
    /// <param name="ipAddress">ເລກ IP ຕົ້ນກໍາເນີດ</param>
    /// <param name="cancellationToken">Token ສໍາລັບການຍົກເລີກຄໍາສັ່ງ</param>
    /// <returns>true ເມື່ອພົບເຫັນແລະໄດ້ຍົກເລີກສໍາເລັດ</returns>
    Task<bool> RevokeAsync(
        string rawToken,
        RefreshTokenRevocationReason reason,
        string? ipAddress,
        CancellationToken cancellationToken = default);

    /// <summary>ຖອນ tokens ຜູ້ໃຊ້ທີ່ໃຊ້ວຽກທັງຝົດ</summary>
    /// <param name="userId">ເຈົ້າຂອງໂທເຄັນ</param>
    /// <param name="reason">ເຫດຜົນສໍາລັບການຖອນ</param>
    /// <param name="ipAddress">ເລກ IP ຕົ້ນກໍາເນີດ</param>
    /// <param name="cancellationToken">Token ສໍາລັບການຍົກເລີກຄໍາສັ່ງ</param>
    /// <returns>ຈຳນວນໂທເຄັນຖືກຖອນຄືນ</returns>
    Task<int> RevokeAllForUserAsync(
        Guid userId,
        RefreshTokenRevocationReason reason,
        string? ipAddress,
        CancellationToken cancellationToken = default);
}
