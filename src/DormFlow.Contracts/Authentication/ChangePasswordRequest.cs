namespace DormFlow.Contracts.Authentication;

/// <summary>ຂໍ້ມູນສໍາລັບການປ່ຽນລະຫັດຜ່ານສໍາລັບບັນຊີທີ່ທ່ານເຂົ້າສູ່ລະບົບ</summary>
public class ChangePasswordRequest
{
    /// <summary>ລະຫັດຜ່ານປັດຈຸບັນ</summary>
    public string CurrentPassword { get; set; } = string.Empty;

    /// <summary>ລະຫັດຜ່ານໃໝ່</summary>
    public string NewPassword { get; set; } = string.Empty;

    /// <summary>ຢືນຢັນລະຫັດຜ່ານໃໝ່</summary>
    public string ConfirmPassword { get; set; } = string.Empty;
}
