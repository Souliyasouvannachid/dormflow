using DormFlow.Contracts.Authentication;

namespace DormFlow.Application.Common.Abstractions;

/// <summary>Use Case ทั้งหมดที่เกี่ยวกับการยืนยันตัวตน</summary>
public interface IAuthenticationService
{
    /// <summary>เข้าสู่ระบบด้วยอีเมลและรหัสผ่าน</summary>
    /// <param name="request">ข้อมูลเข้าสู่ระบบ</param>
    /// <param name="cancellationToken">โทเคนสำหรับยกเลิกคำสั่ง</param>
    Task<AuthTokenResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);

    /// <summary>ขอ Access Token ใบใหม่ด้วย Refresh Token พร้อมหมุนโทเคนใบเดิมทิ้ง</summary>
    /// <param name="request">Refresh Token ที่ถืออยู่</param>
    /// <param name="cancellationToken">โทเคนสำหรับยกเลิกคำสั่ง</param>
    Task<AuthTokenResponse> RefreshTokenAsync(RefreshTokenRequest request, CancellationToken cancellationToken = default);

    /// <summary>ออกจากระบบและเพิกถอน Refresh Token ที่ส่งมา</summary>
    /// <param name="request">Refresh Token ที่ต้องการเพิกถอน</param>
    /// <param name="cancellationToken">โทเคนสำหรับยกเลิกคำสั่ง</param>
    Task LogoutAsync(LogoutRequest request, CancellationToken cancellationToken = default);

    /// <summary>ขอลิงก์ตั้งรหัสผ่านใหม่</summary>
    /// <param name="request">อีเมลของบัญชี</param>
    /// <param name="cancellationToken">โทเคนสำหรับยกเลิกคำสั่ง</param>
    Task ForgotPasswordAsync(ForgotPasswordRequest request, CancellationToken cancellationToken = default);

    /// <summary>ตั้งรหัสผ่านใหม่ด้วยโทเคนที่ได้รับทางอีเมล</summary>
    /// <param name="request">ข้อมูลสำหรับตั้งรหัสผ่านใหม่</param>
    /// <param name="cancellationToken">โทเคนสำหรับยกเลิกคำสั่ง</param>
    Task ResetPasswordAsync(ResetPasswordRequest request, CancellationToken cancellationToken = default);

    /// <summary>เปลี่ยนรหัสผ่านของผู้ใช้ที่เข้าสู่ระบบอยู่</summary>
    /// <param name="request">ข้อมูลสำหรับเปลี่ยนรหัสผ่าน</param>
    /// <param name="cancellationToken">โทเคนสำหรับยกเลิกคำสั่ง</param>
    Task ChangePasswordAsync(ChangePasswordRequest request, CancellationToken cancellationToken = default);

    /// <summary>อ่านข้อมูลของผู้ใช้ที่เข้าสู่ระบบอยู่</summary>
    /// <param name="cancellationToken">โทเคนสำหรับยกเลิกคำสั่ง</param>
    Task<CurrentUserResponse> GetCurrentUserAsync(CancellationToken cancellationToken = default);
}
