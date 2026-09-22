using DormFlow.Application.Common.Models;
using Microsoft.Extensions.Logging;

namespace DormFlow.Application.Features.Authentication;

/// <summary>
/// ຂໍ້ຄວາມ Log ຂອງ Use Case ການລະບຸຕົວຕົນ
/// </summary>
/// <remarks>
/// ໃຊ້ <see cref="LoggerMessage"/> ຮູບແບບການກໍ່ສ້າງ delegate ລ່ວງໜ້າ
/// ໃນຄໍາສັ່ງທີ່ຈະບໍ່ມີ box ຄ່າແລະບໍ່ປະເມີນພາລາມິເຕີເມື່ອລະດັບ Log ປິດ
/// ຂໍ້ຄວາມທັງໝົດໃນໄຟລ໌ນີ້ບໍ່ສາມາດມີລະຫັດຜ່ານ, ໂທເຄັນ ຫຼືທີ່ຢູ່ອີເມວຂອງຜູ້ໃຊ້ໄດ້
/// </remarks>
internal static class AuthenticationLog
{
    private static readonly Action<ILogger, string?, UserAuthenticationStatus, Exception?> LoginFailedCallback =
        LoggerMessage.Define<string?, UserAuthenticationStatus>(
            LogLevel.Warning,
            new EventId(2000, nameof(LoginFailed)),
            "ເຂົ້າສູ່ລະບົບບໍ່ສຳເລັດຈາກ {IpAddress} ສາເຫດ {Reason}");

    private static readonly Action<ILogger, Guid?, string?, Exception?> RefreshTokenReuseDetectedCallback =
        LoggerMessage.Define<Guid?, string?>(
            LogLevel.Warning,
            new EventId(2001, nameof(RefreshTokenReuseDetected)),
            "ກວດພົບການໃຊ້ Refresh Token ຊ້ຳຂອງຜູ້ໃຊ້ {UserId} ຈາກ {IpAddress} ລະບົບໄດ້ຍົກເລີກໂທເຄນທັງໝົດຂອງຜູ້ໃຊ້ລາຍນີ້ແລ້ວ");

    private static readonly Action<ILogger, Exception?> LogoutWithInactiveTokenCallback =
        LoggerMessage.Define(
            LogLevel.Information,
            new EventId(2002, nameof(LogoutWithInactiveToken)),
            "ໄດ້ຮັບຄຳຮ້ອງອອກຈາກລະບົບດ້ວຍ Refresh Token ທີ່ໃຊ້ງານບໍ່ໄດ້ແລ້ວ");

    private static readonly Action<ILogger, Exception?> ForgotPasswordForUnknownAccountCallback =
        LoggerMessage.Define(
            LogLevel.Information,
            new EventId(2003, nameof(ForgotPasswordForUnknownAccount)),
            "ມີຄຳຮ້ອງຕັ້ງລະຫັດຜ່ານໃໝ່ສຳລັບອີເມວທີ່ບໍ່ມີບັນຊີຢູ່ໃນລະບົບ");

    /// <summary>ບັນທຶກຄວາມພະຍາຍາມເຂົ້າສູ່ລະບົບທີ່ບໍ່ສໍາເລັດ</summary>
    public static void LoginFailed(ILogger logger, string? ipAddress, UserAuthenticationStatus reason)
        => LoginFailedCallback(logger, ipAddress, reason, null);

    /// <summary>ໃຊ້ບັນທຶກການກວດສອບ Refresh Token ເຮັດຊ້ຳ</summary>
    public static void RefreshTokenReuseDetected(ILogger logger, Guid? userId, string? ipAddress)
        => RefreshTokenReuseDetectedCallback(logger, userId, ipAddress, null);

    /// <summary>ການຮ້ອງຂໍອອກຈາກລະບົບດ້ວຍ tokens ທີ່ແຕກຫັກແມ່ນບໍ່ມີໃຫ້ແລ້ວ</summary>
    public static void LogoutWithInactiveToken(ILogger logger)
        => LogoutWithInactiveTokenCallback(logger, null);

    /// <summary>ບັນທຶກການຮ້ອງຂໍການຕັ້ງລະຫັດຜ່ານໃຝ່ສໍາລັບທີ່ຢູ່ອີເມວທີ່ບໍ່ມີຕົວຈິງແລ້ວ</summary>
    public static void ForgotPasswordForUnknownAccount(ILogger logger)
        => ForgotPasswordForUnknownAccountCallback(logger, null);
}
