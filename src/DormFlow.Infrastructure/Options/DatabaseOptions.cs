using System.ComponentModel.DataAnnotations;

namespace DormFlow.Infrastructure.Options;

/// <summary>
/// ການຕັ້ງຄ່າການເຊື່ອມຕໍ່ຖານຂໍ້ມູນອ່ານຈາກ configuration section ຊື່ Database
/// </summary>
public sealed class DatabaseOptions
{
    /// <summary>ຊື່ section ໃນ appsettings</summary>
    public const string SectionName = "Database";

    /// <summary>ຈຳນວນສູງສຸດຂອງຄວາມພະຍາຍາມເຊື່ອມຕໍ່ຄືນໃໝ່ເມື່ອພົບຂໍ້ຜິດພາດຊົ່ວຄາວ</summary>
    [Range(0, 10)]
    public int MaxRetryCount { get; set; } = 3;

    /// <summary>ເວລາສູງສຸດລະຫວ່າງການພະຍາຍາມ (ວິນາທີ)</summary>
    [Range(1, 120)]
    public int MaxRetryDelaySeconds { get; set; } = 10;

    /// <summary>ເວລາສູງສຸດທີ່ອະນຸຍາດໃຫ້ສໍາລັບຄໍາສັ່ງ SQL ຜຶ່ງຄໍາສັ່ງເຮັດວຽກ (ວິນາທີ)</summary>
    [Range(1, 600)]
    public int CommandTimeoutSeconds { get; set; } = 30;

    /// <summary>
    /// ສະແດງຕົວກໍານົດການຄໍາສັ່ງ SQL ໃນ Log ຫຼືບໍ່
    /// ຕ້ອງເປັນ false ຢູ່ໃນສະເຝີ Production ເພາະມັນເຮັດໃຫ້ຂໍ້ມູນສ່ວນຕົວຖືກຮົ່ວໄຫຼ Log
    /// </summary>
    public bool EnableSensitiveDataLogging { get; set; }

    /// <summary>ສະແດງລາຍລະອຽດຂໍ້ຜິດພາດ EF Core ເພີ່ມເຕີມຫຼືບໍ່ໃຊ້ພຽງແຕ່ໃນໄລຍະການພັດທະນາ</summary>
    public bool EnableDetailedErrors { get; set; }

    /// <summary>
    /// ແລ່ນ Migration ທີ່ລໍຖ້າອັດຕະໂນມັດເມື່ອແອັບພລິເຄຊັນເລີ່ມຕົ້ນຫຼືບໍ່
    /// ມັນຄວນຈະຖືກເປີດຢູ່ໃນເຄື່ອງພັດທະນາເທົ່ານັ້ນ Production ຄວນດໍາເນີນການ Migration ມັນເປັນຂັ້ນຕອນແຍກຕ່າງຫາກທໍາອິດ deploy
    /// </summary>
    public bool AutoMigrate { get; set; }

    /// <summary>ສ້າງຂໍ້ມູນເບື້ອງຕົ້ນເຊັ່ນການອະນຸຍາດແລະບົດບາດ. ເມື່ອແອັບພລິເຄຊັນເລີ່ມຕົ້ນຫຼືບໍ່?</summary>
    public bool SeedOnStartup { get; set; } = true;

    /// <summary>ຢຸດ startup ເມື່ອພົບ migration ທີ່ຍັງບໍ່ໄດ້ຮັນ</summary>
    public bool FailOnPendingMigrations { get; set; } = true;
}
