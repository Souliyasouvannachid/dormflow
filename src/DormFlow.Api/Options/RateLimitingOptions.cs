using System.ComponentModel.DataAnnotations;

namespace DormFlow.Api.Options;

/// <summary>ການຕັ້ງຄ່າ Rate Limiting ອ່ານຈາກ configuration section ຊື່ RateLimiting</summary>
public sealed class RateLimitingOptions
{
    /// <summary>ຊື່ section ໃນ appsettings</summary>
    public const string SectionName = "RateLimiting";

    /// <summary>ຊື່ policy ທີ່ຖືກນໍາໃຊ້ເປັນຄ່າເລີ່ມຕົ້ນສໍາລັບທຸກຄົນ Endpoint</summary>
    public const string GlobalPolicyName = "dormflow-global";

    /// <summary>ເປີດໃຊ້ Rate Limiting ຫຼືບໍ່</summary>
    public bool Enabled { get; set; } = true;

    /// <summary>ຈໍານວນການຮ້ອງຂໍສູງສຸດຕໍ່ໄລຍະເວລາ</summary>
    [Range(1, 100000)]
    public int PermitLimit { get; set; } = 100;

    /// <summary>ໄລຍະເວລາ (ວິນາທີ)</summary>
    [Range(1, 3600)]
    public int WindowSeconds { get; set; } = 60;

    /// <summary>ຈຳນວນການຮ້ອງຂໍທີ່ອະນຸຍາດໃຫ້ຈັດຄິວເມື່ອເກີນໂຄຕ້າ</summary>
    [Range(0, 1000)]
    public int QueueLimit { get; set; }
}
