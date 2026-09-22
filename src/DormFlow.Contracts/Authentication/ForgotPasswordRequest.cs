namespace DormFlow.Contracts.Authentication;

/// <summary>ຂໍ້ມູນສໍາລັບການຮ້ອງຂໍການເຊື່ອມຕໍ່ເພື່ອປັບລະຫັດຜ່ານຂອງທ່ານ</summary>
public class ForgotPasswordRequest
{
    /// <summary>ທີ່ຢູ່ອີເມວຂອງບັນຊີລືມລະຫັດຜ່ານ</summary>
    public string Email { get; set; } = string.Empty;
}
