namespace DormFlow.Contracts.Authentication;

/// <summary>ຂໍ້ມູນທີ່ຈະຮ້ອງຂໍ Access Token ໃບໃໝ່</summary>
public class RefreshTokenRequest
{
    /// <summary>Refresh Token ໄດ້ຮັບໃນເວລາທີ່ເຂົ້າສູ່ລະບົບຫຼືຈາກການຕໍ່ອາຍຸທີ່ຜ່ານມາ</summary>
    public string RefreshToken { get; set; } = string.Empty;
}
