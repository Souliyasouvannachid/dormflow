namespace DormFlow.Application.Common.Models;

/// <summary>ສະຖານະຂອງຊື່ຜູ້ໃຊ້ ແລະຜົນການກວດສອບລະຫັດຜ່ານ</summary>
public enum UserAuthenticationStatus
{
    /// <summary>ການຢັ້ງຢືນຕົວຕົນສຳເລັດແລ້ວ</summary>
    Success = 1,

    /// <summary>ອີເມວ ຫຼືລະຫັດຜ່ານບໍ່ຖືກຕ້ອງ</summary>
    InvalidCredentials = 2,

    /// <summary>ບັນຊີຂອງທ່ານໄດ້ຖືກລັອກຊົ່ວຄາວເນື່ອງຈາກວ່າທ່ານໃສ່ລະຫັດຜ່ານຜິດພາດຫຼາຍກ່ວາຈໍານວນຄັ້ງທີ່ກໍານົດໄວ້</summary>
    LockedOut = 3,

    /// <summary>ບັນຊີຖືກປິດການນຳໃຊ້ ຫຼືຖືກລະງັບໄວ້</summary>
    AccountDisabled = 4,

    /// <summary>ອົງກອນທີ່ຜູ້ໃຊ້ເປັນເຈົ້າຂອງໄດ້ຖືກປິດໃຊ້ງານ ຫຼືຖືກລະງັບໄວ້</summary>
    OrganizationDisabled = 5,

    /// <summary>ການຢືນຢັນຂັ້ນຕອນທີສອງແມ່ນຕ້ອງການກ່ອນທີ່ຈະເຂົ້າສູ່ລະບົບ</summary>
    RequiresTwoFactor = 6,
}

/// <summary>ຜົນໄດ້ຮັບຂອງການກວດສອບຊື່ຜູ້ໃຊ້ແລະລະຫັດຜ່ານ</summary>
/// <param name="Status">ສະຖານະຜົນໄດ້ຮັບ</param>
/// <param name="Profile">ຂໍ້ມູນຜູ້ໃຊ້ແມ່ນມີຄຸນຄ່າພຽງແຕ່ຫຼັງຈາກການກວດສອບສົບຜົນສໍາເລັດ</param>
/// <param name="LockoutEndAt">ໃນເວລາທີ່ບັນຊີ lockout ຈະສິ້ນສຸດລົງມີມູນຄ່າພຽງແຕ່ໃນເວລາທີ່ບັນຊີໄດ້ຖືກລັອກ</param>
public sealed record UserAuthenticationResult(
    UserAuthenticationStatus Status,
    UserSecurityProfile? Profile = null,
    DateTimeOffset? LockoutEndAt = null)
{
    /// <summary>ສ້າງຜົນສໍາເລັດ</summary>
    /// <param name="profile">ຂໍ້ມູນຜູ້ໃຊ້</param>
    public static UserAuthenticationResult Success(UserSecurityProfile profile)
        => new(UserAuthenticationStatus.Success, profile);

    /// <summary>ຜະລິດຜົນໄດ້ຮັບທີ່ບໍ່ສໍາເລັດ</summary>
    /// <param name="status">ເຫດຜົນບໍ່ສໍາເລັດ</param>
    /// <param name="lockoutEndAt">ເມື່ອການປິດບັນຊີຈະສິ້ນສຸດລົງ</param>
    public static UserAuthenticationResult Failed(
        UserAuthenticationStatus status,
        DateTimeOffset? lockoutEndAt = null)
        => new(status, null, lockoutEndAt);
}
