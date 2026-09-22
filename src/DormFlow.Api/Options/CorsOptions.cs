namespace DormFlow.Api.Options;

/// <summary>ການຕັ້ງຄ່າ CORS ອ່ານຈາກ configuration section ຊື່ Cors</summary>
public sealed class CorsOptions
{
    /// <summary>ຊື່ section ໃນ appsettings</summary>
    public const string SectionName = "Cors";

    /// <summary>ຊື່ policy ໃຊ້ໃນທໍ່</summary>
    public const string PolicyName = "DormFlowCors";

    /// <summary>
    /// ລາຍການ Origin ຕ້ອງລະບຸຢ່າງໜ້ອຍໜຶ່ງຄ່າເມື່ອແລ່ນເຂົ້າ Production
    /// </summary>
    public IList<string> AllowedOrigins { get; } = [];
}
