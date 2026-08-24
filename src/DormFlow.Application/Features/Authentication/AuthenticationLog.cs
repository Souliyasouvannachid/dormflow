using DormFlow.Application.Common.Models;
using Microsoft.Extensions.Logging;

namespace DormFlow.Application.Features.Authentication;

/// <summary>
/// ข้อความ Log ของ Use Case การยืนยันตัวตน
/// </summary>
/// <remarks>
/// ใช้ <see cref="LoggerMessage"/> แบบสร้าง delegate ไว้ล่วงหน้า
/// เพื่อไม่ให้มีการ box ค่าและไม่ประเมินพารามิเตอร์เมื่อระดับ Log นั้นถูกปิดอยู่
/// ทุกข้อความในไฟล์นี้ต้องไม่มีรหัสผ่าน โทเคน หรืออีเมลของผู้ใช้
/// </remarks>
internal static class AuthenticationLog
{
    private static readonly Action<ILogger, string?, UserAuthenticationStatus, Exception?> LoginFailedCallback =
        LoggerMessage.Define<string?, UserAuthenticationStatus>(
            LogLevel.Warning,
            new EventId(2000, nameof(LoginFailed)),
            "เข้าสู่ระบบไม่สำเร็จจาก {IpAddress} สาเหตุ {Reason}");

    private static readonly Action<ILogger, Guid?, string?, Exception?> RefreshTokenReuseDetectedCallback =
        LoggerMessage.Define<Guid?, string?>(
            LogLevel.Warning,
            new EventId(2001, nameof(RefreshTokenReuseDetected)),
            "ตรวจพบการใช้ Refresh Token ซ้ำของผู้ใช้ {UserId} จาก {IpAddress} ระบบเพิกถอนโทเคนทั้งหมดของผู้ใช้รายนี้แล้ว");

    private static readonly Action<ILogger, Exception?> LogoutWithInactiveTokenCallback =
        LoggerMessage.Define(
            LogLevel.Information,
            new EventId(2002, nameof(LogoutWithInactiveToken)),
            "ได้รับคำขอออกจากระบบด้วย Refresh Token ที่ใช้งานไม่ได้แล้ว");

    private static readonly Action<ILogger, Exception?> ForgotPasswordForUnknownAccountCallback =
        LoggerMessage.Define(
            LogLevel.Information,
            new EventId(2003, nameof(ForgotPasswordForUnknownAccount)),
            "มีคำขอตั้งรหัสผ่านใหม่สำหรับอีเมลที่ไม่มีบัญชีอยู่ในระบบ");

    /// <summary>บันทึกความพยายามเข้าสู่ระบบที่ไม่สำเร็จ</summary>
    public static void LoginFailed(ILogger logger, string? ipAddress, UserAuthenticationStatus reason)
        => LoginFailedCallback(logger, ipAddress, reason, null);

    /// <summary>บันทึกการตรวจพบการใช้ Refresh Token ซ้ำ</summary>
    public static void RefreshTokenReuseDetected(ILogger logger, Guid? userId, string? ipAddress)
        => RefreshTokenReuseDetectedCallback(logger, userId, ipAddress, null);

    /// <summary>บันทึกคำขอออกจากระบบด้วยโทเคนที่ใช้งานไม่ได้แล้ว</summary>
    public static void LogoutWithInactiveToken(ILogger logger)
        => LogoutWithInactiveTokenCallback(logger, null);

    /// <summary>บันทึกคำขอตั้งรหัสผ่านใหม่ของอีเมลที่ไม่มีบัญชีอยู่จริง</summary>
    public static void ForgotPasswordForUnknownAccount(ILogger logger)
        => ForgotPasswordForUnknownAccountCallback(logger, null);
}
