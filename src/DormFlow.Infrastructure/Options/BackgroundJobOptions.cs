namespace DormFlow.Infrastructure.Options;

/// <summary>
/// ການຕັ້ງຄ່າ Background Job ອ່ານຈາກ configuration section ຊື່ BackgroundJobs
/// </summary>
public sealed class BackgroundJobOptions
{
    /// <summary>ຊື່ section ໃນ appsettings</summary>
    public const string SectionName = "BackgroundJobs";

    /// <summary>
    /// ເປີດໃຊ້ Background Job ຫຼືບໍ່ (Dashboard ແລະ Recurring Job ທັງໝົດ)
    /// ສາມາດປິດໄດ້ໂດຍບໍ່ມີການ deploy ໃຝ່ໃນກໍລະນີທີ່ທ່ານຕ້ອງການທີ່ຈະຢຸດຊົ່ວຄາວສໍາລັບການບໍາລຸງຮັກສາ
    /// </summary>
    public bool Enabled { get; set; } = true;
}
