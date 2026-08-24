using DormFlow.Application.Common.Models;

namespace DormFlow.Application.Common.Abstractions;

/// <summary>
/// การดำเนินการกับบัญชีผู้ใช้ที่ต้องพึ่ง ASP.NET Core Identity
/// </summary>
/// <remarks>
/// ชั้น Application เรียกผ่าน interface นี้เท่านั้น จึงไม่ต้องรู้จัก UserManager
/// และไม่มีทางเผลอทำงานกับแฮชรหัสผ่านโดยตรง
/// </remarks>
public interface IUserAccountService
{
    /// <summary>ตรวจสอบอีเมลและรหัสผ่าน พร้อมนับความพยายามที่ผิดพลาดเพื่อล็อกบัญชี</summary>
    /// <param name="email">อีเมลที่ใช้เข้าสู่ระบบ</param>
    /// <param name="password">รหัสผ่าน</param>
    /// <param name="cancellationToken">โทเคนสำหรับยกเลิกคำสั่ง</param>
    Task<UserAuthenticationResult> AuthenticateAsync(
        string email,
        string password,
        CancellationToken cancellationToken = default);

    /// <summary>อ่านข้อมูลผู้ใช้พร้อมบทบาทและสิทธิ์ทั้งหมด</summary>
    /// <param name="userId">รหัสผู้ใช้</param>
    /// <param name="cancellationToken">โทเคนสำหรับยกเลิกคำสั่ง</param>
    /// <returns>null เมื่อไม่พบผู้ใช้ หรือผู้ใช้ถูกลบไปแล้ว</returns>
    Task<UserSecurityProfile?> GetSecurityProfileAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    /// <summary>อ่านข้อมูลผู้ใช้จากอีเมล</summary>
    /// <param name="email">อีเมลของบัญชี</param>
    /// <param name="cancellationToken">โทเคนสำหรับยกเลิกคำสั่ง</param>
    /// <returns>null เมื่อไม่พบผู้ใช้ หรือผู้ใช้ถูกลบไปแล้ว</returns>
    Task<UserSecurityProfile?> GetSecurityProfileByEmailAsync(
        string email,
        CancellationToken cancellationToken = default);

    /// <summary>ตรวจว่าบัญชีและองค์กรของผู้ใช้ยังอยู่ในสถานะที่ใช้งานระบบได้</summary>
    /// <param name="userId">รหัสผู้ใช้</param>
    /// <param name="cancellationToken">โทเคนสำหรับยกเลิกคำสั่ง</param>
    Task<bool> IsAccountUsableAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>บันทึกเวลาเข้าสู่ระบบล่าสุด</summary>
    /// <param name="userId">รหัสผู้ใช้</param>
    /// <param name="loggedInAt">เวลาที่เข้าสู่ระบบ</param>
    /// <param name="cancellationToken">โทเคนสำหรับยกเลิกคำสั่ง</param>
    Task RecordSuccessfulLoginAsync(
        Guid userId,
        DateTimeOffset loggedInAt,
        CancellationToken cancellationToken = default);

    /// <summary>สร้างโทเคนสำหรับตั้งรหัสผ่านใหม่</summary>
    /// <param name="email">อีเมลของบัญชี</param>
    /// <param name="cancellationToken">โทเคนสำหรับยกเลิกคำสั่ง</param>
    /// <returns>null เมื่อไม่พบบัญชีนั้น ผู้เรียกต้องตอบข้อความเดิมเสมอเพื่อไม่ให้เดาได้ว่ามีอีเมลนี้อยู่จริง</returns>
    Task<string?> GeneratePasswordResetTokenAsync(
        string email,
        CancellationToken cancellationToken = default);

    /// <summary>ตั้งรหัสผ่านใหม่ด้วยโทเคนที่ได้รับ</summary>
    /// <param name="email">อีเมลของบัญชี</param>
    /// <param name="token">โทเคนที่ได้รับทางอีเมล</param>
    /// <param name="newPassword">รหัสผ่านใหม่</param>
    /// <param name="cancellationToken">โทเคนสำหรับยกเลิกคำสั่ง</param>
    Task<PasswordOperationResult> ResetPasswordAsync(
        string email,
        string token,
        string newPassword,
        CancellationToken cancellationToken = default);

    /// <summary>เปลี่ยนรหัสผ่านของบัญชีที่เข้าสู่ระบบอยู่</summary>
    /// <param name="userId">รหัสผู้ใช้</param>
    /// <param name="currentPassword">รหัสผ่านปัจจุบัน</param>
    /// <param name="newPassword">รหัสผ่านใหม่</param>
    /// <param name="cancellationToken">โทเคนสำหรับยกเลิกคำสั่ง</param>
    Task<PasswordOperationResult> ChangePasswordAsync(
        Guid userId,
        string currentPassword,
        string newPassword,
        CancellationToken cancellationToken = default);
}
