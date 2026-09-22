using System.ComponentModel.DataAnnotations;

namespace DormFlow.Infrastructure.Options;

/// <summary>
/// ການຕັ້ງຄ່າ JWT ອ່ານຈາກ configuration section ຊື່ Jwt
/// </summary>
/// <remarks>
/// SigningKey ມັນເປັນຄວາມລັບແລະຕ້ອງໄດ້ຮັບການກໍານົດ environment variable ຊື່ Jwt__SigningKey
/// ຫຼື Secret Manager ເທົ່ານັ້ນ. ຢ່າໃສ່ຄ່າຕົວຈິງໃນ appsettings.json
/// </remarks>
public sealed class JwtOptions
{
    /// <summary>ຊື່ section ໃນ appsettings</summary>
    public const string SectionName = "Jwt";

    /// <summary>ຄວາມຍາວກະແຈຕໍ່າສຸດ HMAC-SHA256 ຫົວໜ່ວຍແມ່ນຕົວອັກສອນ, ທຽບເທົ່າ 256 ນ້ອຍ</summary>
    public const int MinimumSigningKeyLength = 32;

    /// <summary>ກະແຈທີ່ໃຊ້ເພື່ອເຊັນ ແລະຢັ້ງຢືນລາຍເຊັນຂອງໂທເຄັນ</summary>
    [Required(ErrorMessage = "ຕ້ອງກຳນົດ Jwt:SigningKey ຜ່ານ environment variable ຫຼື Secret Manager")]
    [MinLength(MinimumSigningKeyLength, ErrorMessage = "Jwt:SigningKey ຕ້ອງມີຄວາມຍາວຢ່າງໜ້ອຍ 32 ຕົວອັກສອນ")]
    public string SigningKey { get; set; } = string.Empty;

    /// <summary>ຜູ້ອອກໂທເຄັນ</summary>
    [Required]
    public string Issuer { get; set; } = "DormFlow";

    /// <summary>ຜູ້ຮັບໂທເຄັນ</summary>
    [Required]
    public string Audience { get; set; } = "DormFlow.Client";

    /// <summary>ອາຍຸຂອງ Access Token ຜ່ວຍບໍລິການແມ່ນນາທີ</summary>
    [Range(1, 120)]
    public int AccessTokenMinutes { get; set; } = 15;

    /// <summary>ອາຍຸຂອງ Refresh Token ຜ່ວຍບໍລິການແມ່ນມື້</summary>
    [Range(1, 365)]
    public int RefreshTokenDays { get; set; } = 7;

    /// <summary>
    /// ຄວາມທົນທານທີ່ໃຊ້ເວລາທີ່ຍອມຮັບໄດ້ ຜ່ວຍບໍລິການແມ່ນວິນາທີ
    /// ຕັ້ງເປັນ 0 ເພື່ອໃຫ້ tokens ໝົດອາຍຸຕາມເວລາ. ມັນບໍ່ໄດ້ຍືດຍາວອີກຕໍ່ໄປ 5 ນາທີຕາມຄ່າເລີ່ມຕົ້ນຂອງຫ້ອງສະໝຸດ
    /// </summary>
    [Range(0, 300)]
    public int ClockSkewSeconds { get; set; }
}
