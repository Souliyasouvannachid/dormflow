namespace DormFlow.Contracts.Common;

/// <summary>
/// ຂໍ້ຄວາມມາດຕະຖານຂອງ API Response ລະບົບທັງຝົດ
/// ທັງຝົດຢູ່ໃນສະຖານທີ່ດຽວເພື່ອໃຫ້ຂໍ້ຄວາມທີ່ຜູ້ໃຊ້ເຫັນແມ່ນສອດຄ່ອງໃນທົ່ວພວກເຂົາທັງຝົດ Endpoint
/// </summary>
public static class ApiMessages
{
    /// <summary>ສຳເລັດແລ້ວ (ໃຊ້ກັບ Create / Update / Delete / Change Status)</summary>
    public const string Success = "ດຳເນີນການສຳເລັດ";

    /// <summary>ດຶງຂໍ້ມູນສຳເລັດແລ້ວ (ໃຊ້ກັບ Query)</summary>
    public const string Retrieved = "ດຶງຂໍ້ມູນສຳເລັດ";

    /// <summary>ສ້າງຂໍ້ມູນສຳເລັດແລ້ວ</summary>
    public const string Created = "ສ້າງຂໍ້ມູນສຳເລັດ";

    /// <summary>ແກ້ໄຂຂໍ້ມູນສຳເລັດແລ້ວ</summary>
    public const string Updated = "ແກ້ໄຂຂໍ້ມູນສຳເລັດ";

    /// <summary>ລຶບຂໍ້ມູນສຳເລັດແລ້ວ</summary>
    public const string Deleted = "ລຶບຂໍ້ມູນສຳເລັດ";

    /// <summary>ຂໍ້ມູນທີ່ສົ່ງມາຍັງບໍ່ໄດ້ຮັບການຢືນຢັນ (HTTP 400)</summary>
    public const string ValidationFailed = "ຂໍ້ມູນບໍ່ຖືກຕ້ອງ";

    /// <summary>ບໍ່ພົບຂໍ້ມູນທີ່ຮ້ອງຂໍ (HTTP 404)</summary>
    public const string NotFound = "ບໍ່ພົບຂໍ້ມູນທີ່ຕ້ອງການ";

    /// <summary>ຍັງບໍ່ໄດ້ເຂົ້າສູ່ລະບົບເທື່ອ? Token ບໍ່ຖືກຕ້ອງ (HTTP 401)</summary>
    public const string Unauthorized = "ກະລຸນາເຂົ້າສູ່ລະບົບ";

    /// <summary>ຂ້ອຍໄດ້ເຂົ້າສູ່ລະບົບແຕ່ບໍ່ມີການອະນຸຍາດ (HTTP 403)</summary>
    public const string Forbidden = "ທ່ານບໍ່ມີສິດເຮັດລາຍການນີ້";

    /// <summary>ຂໍ້ມູນຂັດກັບສະຖານະປັດຈຸບັນ (HTTP 409)</summary>
    public const string Conflict = "ຂໍ້ມູນຂັດແຍ້ງກັບຂໍ້ມູນທີ່ມີຢູ່ໃນລະບົບ";

    /// <summary>ຂໍ້ມູນໄດ້ຖືກແກ້ໄຂໂດຍຜູ້ໃຊ້ອື່ນຢູ່ຕາມທາງ (HTTP 409)</summary>
    public const string ConcurrencyConflict = "ຂໍ້ມູນຖືກແກ້ໄຂໂດຍຜູ້ໃຊ້ອື່ນ ກະລຸນາໂຫຼດຂໍ້ມູນໃໝ່ແລ້ວລອງອີກຄັ້ງ";

    /// <summary>ຮຽກຮ້ອງໃຫ້ນໍາໃຊ້ຫຼາຍກ່ວາລະບຸໄວ້ (HTTP 429)</summary>
    public const string TooManyRequests = "ມີການເອີ້ນໃຊ້ງານຖີ່ເກີນກຳນົດ ກະລຸນາລອງໃໝ່ອີກຄັ້ງພາຍຫຼັງ";

    /// <summary>ຄວາມຜິດພາດຂອງລະບົບທີ່ບໍ່ຄາດຄິດ (HTTP 500)</summary>
    public const string InternalServerError = "ເກີດຂໍ້ຜິດພາດພາຍໃນລະບົບ ກະລຸນາລອງໃໝ່ອີກຄັ້ງ";

    /// <summary>ການຮ້ອງຂໍໄດ້ຖືກຍົກເລີກໂດຍຜູ້ໂທ (HTTP 499)</summary>
    public const string RequestCancelled = "ຄຳຮ້ອງຖືກຍົກເລີກ";
}
