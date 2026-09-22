using Microsoft.AspNetCore.Identity;

namespace DormFlow.Infrastructure.Identity;

/// <summary>ແປງຂໍ້ຜິດພາດຈາກ ASP.NET Core Identity ໃຫ້ເປັນຂໍ້ຄວາມພາສາລາວ</summary>
internal static class IdentityErrorTranslator
{
    public static string Translate(IdentityError error) => error.Code switch
    {
        "PasswordMismatch" => "ລະຫັດຜ່ານປັດຈຸບັນບໍ່ຖືກຕ້ອງ",
        "InvalidToken" => "ໂທເຄນບໍ່ຖືກຕ້ອງ ຫຼື ໝົດອາຍຸແລ້ວ ກະລຸນາຂໍລິ້ງຕັ້ງລະຫັດຜ່ານໃໝ່ອີກຄັ້ງ",
        "PasswordTooShort" => "ລະຫັດຜ່ານສັ້ນເກີນໄປ",
        "PasswordRequiresDigit" => "ລະຫັດຜ່ານຕ້ອງມີໂຕເລກຢ່າງໜ້ອຍໜຶ່ງໂຕ",
        "PasswordRequiresLower" => "ລະຫັດຜ່ານຕ້ອງມີໂຕອັກສອນພິມນ້ອຍຢ່າງໜ້ອຍໜຶ່ງໂຕ",
        "PasswordRequiresUpper" => "ລະຫັດຜ່ານຕ້ອງມີໂຕອັກສອນພິມໃຫຍ່ຢ່າງໜ້ອຍໜຶ່ງໂຕ",
        "PasswordRequiresUniqueChars" => "ລະຫັດຜ່ານຕ້ອງມີໂຕອັກສອນທີ່ບໍ່ຊ້ຳກັນຫຼາຍກວ່ານີ້",
        "PasswordRequiresNonAlphanumeric" => "ລະຫັດຜ່ານຕ້ອງມີສັນຍາລັກຢ່າງໜ້ອຍໜຶ່ງໂຕ",
        "UserLockedOut" => "ບັນຊີນີ້ຖືກລັອກຊົ່ວຄາວ ກະລຸນາລອງໃໝ່ພາຍຫຼັງ",
        "DuplicateUserName" => "ຊື່ຜູ້ໃຊ້ນີ້ມີຢູ່ແລ້ວ",
        "DuplicateEmail" => "ອີເມວນີ້ມີຢູ່ແລ້ວ",
        "InvalidUserName" => "ຊື່ຜູ້ໃຊ້ບໍ່ຖືກຕ້ອງ",
        "UserAlreadyHasPassword" => "ຜູ້ໃຊ້ນີ້ມີລະຫັດຜ່ານຢູ່ແລ້ວ",
        "UserNotInRole" => "ຜູ້ໃຊ້ບໍ່ມີບົດບາດທີ່ລະບຸ",
        "UserAlreadyInRole" => "ຜູ້ໃຊ້ມີບົດບາດນີ້ຢູ່ແລ້ວ",
        "InvalidRoleName" => "ຊື່ບົດບາດບໍ່ຖືກຕ້ອງ",
        "DuplicateRoleName" => "ຊື່ບົດບາດນີ້ມີຢູ່ແລ້ວ",
        "RoleNotFound" => "ບໍ່ພົບບົດບາດທີ່ລະບຸ",
        "ConcurrencyFailure" => "ຂໍ້ມູນຖືກແກ້ໄຂໂດຍຜູ້ໃຊ້ອື່ນ ກະລຸນາໂຫຼດຂໍ້ມູນໃໝ່ແລ້ວລອງອີກຄັ້ງ",
        "LoginAlreadyAssociated" => "ບັນຊີນີ້ຜູກກັບການເຂົ້າລະບົບນີ້ຢູ່ແລ້ວ",
        "RecoveryCodeRedemptionFailed" => "ລະຫັດກູ້ຄືນບໍ່ຖືກຕ້ອງ ຫຼືຖືກໃຊ້ໄປແລ້ວ",
        _ => "ການດຳເນີນການບັນຊີບໍ່ສຳເລັດ",
    };
}
