namespace DormFlow.Application.Common.Models;

/// <summary>ສະຖານະຜົນການກວດກາ Refresh Token</summary>
public enum RefreshTokenValidationStatus
{
    /// <summary>ສາມາດນໍາໃຊ້ສໍາລັບການຕໍ່ອາຍຸ</summary>
    Valid = 1,

    /// <summary>ໂທເຄັນນີ້ບໍ່ພົບຢູ່ໃນລະບົບ</summary>
    NotFound = 2,

    /// <summary>ໂທເຄັນໝົດອາຍຸແລ້ວ</summary>
    Expired = 3,

    /// <summary>token ໄດ້ຖືກຖອນຄືນ, i.e. ຜູ້ໃຊ້ໄດ້ອອກຈາກລະບົບ</summary>
    Revoked = 4,

    /// <summary>
    /// ກວດສອບການນໍາໃຊ້ໃຝ່ຂອງ tokens ທີ່ໄດ້ຖືກ spun ແລ້ວ
    /// ໂທເຄັນທັງໝົດສຳລັບຜູ້ໃຊ້ນັ້ນຈະຖືກຖອນຄືນທັນທີ
    /// </summary>
    ReuseDetected = 5,

    /// <summary>ບັນຊີເຈົ້າຂອງໂທເຄັນຖືກປິດການນຳໃຊ້ແລ້ວ</summary>
    AccountDisabled = 6,
}

/// <summary>ຜົນການກວດກາ Refresh Token</summary>
/// <param name="Status">ສະຖານະຜົນໄດ້ຮັບ</param>
/// <param name="TokenId">ລະຫັດຂອງແຖວ token ມັນມີມູນຄ່າພຽງແຕ່ເມື່ອ token ຖືກພົບເຫັນຢູ່ໃນລະບົບ</param>
/// <param name="UserId">ເຈົ້າຂອງ token ມີມູນຄ່າພຽງແຕ່ເມື່ອ token ພົບເຫັນຢູ່ໃນລະບົບ</param>
public sealed record RefreshTokenValidationResult(
    RefreshTokenValidationStatus Status,
    Guid? TokenId = null,
    Guid? UserId = null)
{
    /// <summary>ສ້າງຜົນໄດ້ຮັບໃຝ່</summary>
    /// <param name="tokenId">ລະຫັດຂອງແຖວໂທເຄັນ</param>
    /// <param name="userId">ເຈົ້າຂອງໂທເຄັນ</param>
    public static RefreshTokenValidationResult Valid(Guid tokenId, Guid userId)
        => new(RefreshTokenValidationStatus.Valid, tokenId, userId);

    /// <summary>ສ້າງຜົນໄດ້ຮັບທີ່ບໍ່ສາມາດຂະຫຍາຍໄດ້</summary>
    /// <param name="status">ເຫດຜົນໃຊ້ບໍ່ໄດ້</param>
    /// <param name="userId">ເຈົ້າຂອງໂທເຄັນ, ຖ້າຮູ້ຈັກ</param>
    public static RefreshTokenValidationResult Invalid(
        RefreshTokenValidationStatus status,
        Guid? userId = null)
        => new(status, null, userId);
}
