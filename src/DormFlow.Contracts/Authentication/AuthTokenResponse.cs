namespace DormFlow.Contracts.Authentication;

/// <summary>ຜົນໄດ້ຮັບຂອງການເຂົ້າສູ່ລະບົບຫຼືການຕໍ່ອາຍຸ tokens</summary>
public class AuthTokenResponse
{
    /// <summary>Access Token ຕົວແບບ JWT ໃຊ້ຕິດຢູ່ໃນ Authorization header</summary>
    public string AccessToken { get; init; } = string.Empty;

    /// <summary>ປະເພດຂອງ token, ການນໍາໃຊ້ມູນຄ່າ Bearer ສະເໝີ</summary>
    public string TokenType { get; init; } = "Bearer";

    /// <summary>ຊີວິດທີ່ຍັງເຫຼືອຂອງ Access Token ຜ່ວຍບໍລິການແມ່ນວິນາທີ</summary>
    public int ExpiresIn { get; init; }

    /// <summary>ເວລາຝົດອາຍຸຂອງ Access Token</summary>
    public DateTimeOffset ExpiresAt { get; init; }

    /// <summary>
    /// Refresh Token ເຄີຍຖາມ Access Token ໃບໃໝ່
    /// ທຸກໆຄັ້ງທີ່ໃຊ້ລະບົບຈະອອກອັນໃໝ່ ແລະ ຖອນອັນເກົ່າທັນທີ
    /// </summary>
    public string RefreshToken { get; init; } = string.Empty;

    /// <summary>ເວລາຝົດອາຍຸຂອງ Refresh Token</summary>
    public DateTimeOffset RefreshTokenExpiresAt { get; init; }

    /// <summary>ເຂົ້າສູ່ລະບົບຂໍ້ມູນຜູ້ໃຊ້</summary>
    public CurrentUserResponse User { get; init; } = new();
}
