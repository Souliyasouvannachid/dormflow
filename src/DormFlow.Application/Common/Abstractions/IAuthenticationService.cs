using DormFlow.Contracts.Authentication;

namespace DormFlow.Application.Common.Abstractions;

/// <summary>Use Case ທັງໝົດກ່ຽວກັບການຢັ້ງຢືນຕົວຕົນ</summary>
public interface IAuthenticationService
{
    /// <summary>ເຂົ້າສູ່ລະບົບດ້ວຍອີເມວ ແລະລະຫັດຜ່ານ</summary>
    /// <param name="request">ຂໍ້ມູນການເຂົ້າສູ່ລະບົບ</param>
    /// <param name="cancellationToken">Token ສໍາລັບການຍົກເລີກຄໍາສັ່ງ</param>
    Task<AuthTokenResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);

    /// <summary>ຮ້ອງຂໍ Access Token ໃບໃຝ່ເຊັ່ນດຽວກັນ Refresh Token ພ້ອມທີ່ຈະໝຸນ ແລະຍົກເລີກ token ຕົ້ນສະບັບ</summary>
    /// <param name="request">Refresh Token ຖື</param>
    /// <param name="cancellationToken">Token ສໍາລັບການຍົກເລີກຄໍາສັ່ງ</param>
    Task<AuthTokenResponse> RefreshTokenAsync(RefreshTokenRequest request, CancellationToken cancellationToken = default);

    /// <summary>ອອກຈາກລະບົບແລະຖອນຄືນ Refresh Token ສົ່ງ</summary>
    /// <param name="request">Refresh Token ທີ່ທ່ານຕ້ອງການຖອນຄືນ</param>
    /// <param name="cancellationToken">Token ສໍາລັບການຍົກເລີກຄໍາສັ່ງ</param>
    Task LogoutAsync(LogoutRequest request, CancellationToken cancellationToken = default);

    /// <summary>ຂໍລິ້ງເພື່ອຣີເຊັດລະຫັດຜ່ານຂອງທ່ານ</summary>
    /// <param name="request">ອີເມວບັນຊີ</param>
    /// <param name="cancellationToken">Token ສໍາລັບການຍົກເລີກຄໍາສັ່ງ</param>
    Task ForgotPasswordAsync(ForgotPasswordRequest request, CancellationToken cancellationToken = default);

    /// <summary>ຕັ້ງລະຫັດຜ່ານໃໝ່ດ້ວຍໂທເຄັນທີ່ໄດ້ຮັບຜ່ານທາງອີເມວ</summary>
    /// <param name="request">ຂໍ້ມູນສໍາລັບການຕັ້ງລະຫັດຜ່ານໃຝ່</param>
    /// <param name="cancellationToken">Token ສໍາລັບການຍົກເລີກຄໍາສັ່ງ</param>
    Task ResetPasswordAsync(ResetPasswordRequest request, CancellationToken cancellationToken = default);

    /// <summary>ປ່ຽນລະຫັດຜ່ານຂອງຜູ້ໃຊ້ທີ່ເຂົ້າສູ່ລະບົບໃນປັດຈຸບັນ</summary>
    /// <param name="request">ຂໍ້ມູນສໍາລັບການປ່ຽນລະຫັດຜ່ານຂອງທ່ານ</param>
    /// <param name="cancellationToken">Token ສໍາລັບການຍົກເລີກຄໍາສັ່ງ</param>
    Task ChangePasswordAsync(ChangePasswordRequest request, CancellationToken cancellationToken = default);

    /// <summary>ອ່ານຂໍ້ມູນຂອງຜູ້ໃຊ້ທີ່ເຂົ້າສູ່ລະບົບໃນປັດຈຸບັນ</summary>
    /// <param name="cancellationToken">Token ສໍາລັບການຍົກເລີກຄໍາສັ່ງ</param>
    Task<CurrentUserResponse> GetCurrentUserAsync(CancellationToken cancellationToken = default);
}
