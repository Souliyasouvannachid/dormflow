namespace DormFlow.Contracts.Authentication;

/// <summary>ຂໍ້ມູນສໍາລັບການອອກຈາກລະບົບ</summary>
public class LogoutRequest
{
    /// <summary>Refresh Token ທີ່ທ່ານຕ້ອງການຖອນຄືນ</summary>
    public string RefreshToken { get; set; } = string.Empty;
}
