namespace DormFlow.Contracts.Authentication;

/// <summary>ຂໍ້ມູນການເຂົ້າສູ່ລະບົບ</summary>
public class LoginRequest
{
    /// <summary>ອີເມວທີ່ໃຊ້ເພື່ອເຂົ້າສູ່ລະບົບ</summary>
    /// <example>admin@dormflow.local</example>
    public string Email { get; set; } = string.Empty;

    /// <summary>ລະຫັດຜ່ານ</summary>
    public string Password { get; set; } = string.Empty;
}
