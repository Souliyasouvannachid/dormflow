namespace DormFlow.Domain.Enums;

/// <summary>ປະເພດຂອງເຫດການທີ່ບັນທຶກໄວ້ Audit Log</summary>
public enum AuditAction
{
    /// <summary>ສ້າງຂໍ້ມູນໃຝ່</summary>
    Create = 1,

    /// <summary>ແກ້ໄຂຂໍ້ມູນ</summary>
    Update = 2,

    /// <summary>ລຶບຂໍ້ມູນ</summary>
    Delete = 3,

    /// <summary>ປ່ຽນສະຖານະຂໍ້ມູນ</summary>
    ChangeStatus = 4,

    /// <summary>ກໍານົດບົດບາດໃຫ້ຜູ້ໃຊ້</summary>
    AssignRole = 5,

    /// <summary>ມອບຫໍພັກໃຫ້ຜູ້ໃຊ້</summary>
    AssignDormitory = 6,

    /// <summary>ເຂົ້າສູ່ລະບົບສຳເລັດແລ້ວ</summary>
    Login = 7,

    /// <summary>ເຂົ້າສູ່ລະບົບລົ້ມເຫຼວ</summary>
    LoginFailed = 8,

    /// <summary>ອອກຈາກລະບົບ</summary>
    Logout = 9,

    /// <summary>ຕໍ່ອາຍຸ Access Token ຄືກັນ Refresh Token</summary>
    RefreshToken = 10,

    /// <summary>ຖອນຄືນ Refresh Token</summary>
    RevokeToken = 11,

    /// <summary>ກວດພົບການປະພຶດ Refresh Token ທີ່ໄດ້ຖືກນໍາໃຊ້ແລະຖືກນໍາໃຊ້ຄືນ</summary>
    RefreshTokenReuseDetected = 12,

    /// <summary>ປ່ຽນລະຫັດຜ່ານຂອງທ່ານດ້ວຍຕົນເອງ</summary>
    ChangePassword = 13,

    /// <summary>ຂໍລິ້ງເພື່ອຣີເຊັດລະຫັດຜ່ານຂອງທ່ານ</summary>
    ForgotPassword = 14,

    /// <summary>ຕັ້ງລະຫັດຜ່ານໃໝ່ດ້ວຍໂທເຄັນທີ່ໄດ້ຮັບ</summary>
    ResetPassword = 15,
}
