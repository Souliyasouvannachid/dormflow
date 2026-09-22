using DormFlow.Domain.Common;
using DormFlow.Domain.Enums;
using DormFlow.Domain.Exceptions;

namespace DormFlow.Domain.Entities;

/// <summary>
/// Refresh Token ປີ້ໜຶ່ງເກັບພຽງແຕ່ຄ່າ hash ຂອງໂທເຄັນ. ບໍ່ເກັບມູນຄ່າຕົວຈິງ
/// </summary>
/// <remarks>
/// ຖ້າຖານຂໍ້ມູນຖືກຮົ່ວ, ຜູ້ທີ່ໄດ້ຮັບຂໍ້ມູນຈະຮ້ອງຂໍໃຫ້ມີຄ່າ hash Access Token ບໍ່ສາມາດສືບຕໍ່ໄດ້
/// ເນື່ອງຈາກວ່າລະບົບກວດສອບໂດຍການ hashing ຄ່າທີ່ສົ່ງໂດຍຜູ້ໂທແລະປຽບທຽບໃຫ້ເຂົາເຈົ້າ. ມູນຄ່າທີ່ເກັບໄວ້ບໍ່ໄດ້ປຽບທຽບໂດຍກົງ
/// </remarks>
public class RefreshToken : BaseEntity
{
    /// <summary>ຜູ້ໃຊ້ທີ່ເປັນເຈົ້າຂອງ token</summary>
    public Guid UserId { get; set; }

    /// <summary>ຄ່າໂທເຄັນ hash</summary>
    public required string TokenHash { get; set; }

    /// <summary>ລະຫັດອ້າງອີງຂອງ Access Token ອອກມາຮ່ວມກັນ, ນໍາໃຊ້ສໍາລັບການກວດສອບຄືນຫຼັງ</summary>
    public required string JwtId { get; set; }

    /// <summary>ເວລາໝົດອາຍຸ</summary>
    public DateTimeOffset ExpiresAt { get; set; }

    /// <summary>ເລກ IP ຜູ້ທີ່ຮ້ອງຂໍ token ນີ້</summary>
    public string? CreatedByIp { get; set; }

    /// <summary>User Agent ຜູ້ທີ່ຮ້ອງຂໍ token ນີ້</summary>
    public string? UserAgent { get; set; }

    /// <summary>ຖອນເວລາ</summary>
    public DateTimeOffset? RevokedAt { get; set; }

    /// <summary>ເລກ IP ສັ່ງໃຫ້ຍົກເລີກ</summary>
    public string? RevokedByIp { get; set; }

    /// <summary>ໂທເຄັນໃໝ່ທີ່ປ່ຽນແທນອັນນີ້ດ້ວຍກົນຈັກ Token Rotation</summary>
    public Guid? ReplacedByTokenId { get; set; }

    /// <summary>ເຫດຜົນສໍາລັບການຖອນ</summary>
    public RefreshTokenRevocationReason? ReasonRevoked { get; set; }

    /// <summary>ມັນໄດ້ຖືກຖອນຄືນແລ້ວບໍ?</summary>
    public bool IsRevoked => RevokedAt.HasValue;

    /// <summary>ກວດເບິ່ງວ່າໂທເຄັນໝົດອາຍຸຫຼືບໍ່</summary>
    /// <param name="now">ເວລາປະຈຸບັນ</param>
    public bool IsExpired(DateTimeOffset now) => now >= ExpiresAt;

    /// <summary>ກວດເບິ່ງວ່າ token ແມ່ນຍັງໃຊ້ໄດ້ສໍາລັບການຕໍ່ອາຍຸ</summary>
    /// <param name="now">ເວລາປະຈຸບັນ</param>
    public bool IsActive(DateTimeOffset now) => !IsRevoked && !IsExpired(now);

    /// <summary>ຖອນໂທເຄັນນີ້</summary>
    /// <param name="now">ເວລາຖອນ</param>
    /// <param name="reason">ເຫດຜົນສໍາລັບການຖອນ</param>
    /// <param name="revokedByIp">ເລກ IP ສັ່ງໃຫ້ຍົກເລີກ</param>
    /// <param name="replacedByTokenId">ໂທເຄັນໃໝ່ທີ່ປ່ຽນແທນ ໃນກໍລະນີຂອງການຖອດຖອນເນື່ອງຈາກການໝຸນໂທເຄັນ</param>
    /// <exception cref="DomainException">ເມື່ອ token ໄດ້ຖືກຖອນຄືນ</exception>
    public void Revoke(
        DateTimeOffset now,
        RefreshTokenRevocationReason reason,
        string? revokedByIp = null,
        Guid? replacedByTokenId = null)
    {
        if (IsRevoked)
        {
            throw new DomainException("Refresh Token ໃບນີ້ຖືກເພີກຖອນໄປແລ້ວ");
        }

        RevokedAt = now;
        RevokedByIp = revokedByIp;
        ReasonRevoked = reason;
        ReplacedByTokenId = replacedByTokenId;
    }
}
