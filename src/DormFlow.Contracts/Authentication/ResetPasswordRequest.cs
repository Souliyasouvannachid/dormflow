namespace DormFlow.Contracts.Authentication;

/// <summary>ຂໍ້ມູນສໍາລັບການຕັ້ງລະຫັດຜ່ານໃຝ່</summary>
public class ResetPasswordRequest
{
    /// <summary>ອີເມວບັນຊີ</summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>ໂທເຄັນໄດ້ຮັບຈາກອີເມວຣີເຊັດລະຫັດຜ່ານ</summary>
    public string Token { get; set; } = string.Empty;

    /// <summary>ລະຫັດຜ່ານໃໝ່</summary>
    public string NewPassword { get; set; } = string.Empty;

    /// <summary>ຢືນຢັນລະຫັດຜ່ານໃໝ່</summary>
    public string ConfirmPassword { get; set; } = string.Empty;
}
