namespace DormFlow.Domain.Enums;

/// <summary>ເຫດຜົນວ່າເປັນຫຍັງ Refresh Token ຖອນຄືນ</summary>
public enum RefreshTokenRevocationReason
{
    /// <summary>ມັນໄດ້ຖືກປ່ຽນແທນກົນຈັກດ້ວຍ token ໃໝ່ Token Rotation</summary>
    ReplacedByNewToken = 1,

    /// <summary>ຜູ້ໃຊ້ອອກຈາກລະບົບ</summary>
    Logout = 2,

    /// <summary>ຜູ້ໃຊ້ ຫຼືຜູ້ບໍລິຫານສັ່ງໃຫ້ຖອນຕົວ</summary>
    RevokedManually = 3,

    /// <summary>ກວດພົບການໃຊ້ໂທເຄັນຄືນໃໝ່ທີ່ໄດ້ຖືກ spun ແລ້ວ. ດັ່ງນັ້ນ, ລະບົບຕ່ອງໂສ້ token ທັງຝົດໄດ້ຖືກຖອນຄືນ</summary>
    ReuseDetected = 4,

    /// <summary>ຜູ້ໃຊ້ປ່ຽນ ຫຼືຕັ້ງລະຫັດຜ່ານໃໝ່</summary>
    PasswordChanged = 5,

    /// <summary>ບັນຊີຖືກລະງັບ ຫຼືປິດການນຳໃຊ້</summary>
    AccountDisabled = 6,
}
