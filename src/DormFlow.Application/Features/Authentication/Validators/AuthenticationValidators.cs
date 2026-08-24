using DormFlow.Contracts.Authentication;
using FluentValidation;

namespace DormFlow.Application.Features.Authentication.Validators;

/// <summary>ข้อกำหนดของรหัสผ่านที่ใช้ร่วมกันทุกจุดที่ผู้ใช้ตั้งรหัสผ่าน</summary>
public static class PasswordRules
{
    /// <summary>ความยาวขั้นต่ำของรหัสผ่าน</summary>
    public const int MinimumLength = 8;

    /// <summary>ความยาวสูงสุดของรหัสผ่าน</summary>
    public const int MaximumLength = 128;

    /// <summary>
    /// ใส่กฎของรหัสผ่านให้กับฟิลด์ที่ระบุ
    /// </summary>
    /// <typeparam name="T">ชนิดของ request ที่กำลังตรวจสอบ</typeparam>
    /// <param name="rule">กฎของฟิลด์รหัสผ่าน</param>
    /// <remarks>
    /// กฎชุดนี้ต้องสอดคล้องกับค่าที่ตั้งไว้ใน ASP.NET Core Identity
    /// มีไว้เพื่อให้ผู้ใช้ได้รับข้อความภาษาไทยที่ชัดเจนก่อนที่คำขอจะไปถึงชั้น Identity
    /// </remarks>
    public static IRuleBuilderOptions<T, string> ApplyPasswordPolicy<T>(this IRuleBuilder<T, string> rule)
    {
        ArgumentNullException.ThrowIfNull(rule);

        return rule
            .NotEmpty().WithMessage("กรุณาระบุรหัสผ่าน")
            .MinimumLength(MinimumLength).WithMessage($"รหัสผ่านต้องยาวอย่างน้อย {MinimumLength} ตัวอักษร")
            .MaximumLength(MaximumLength).WithMessage($"รหัสผ่านต้องยาวไม่เกิน {MaximumLength} ตัวอักษร")
            .Matches("[A-Z]").WithMessage("รหัสผ่านต้องมีตัวอักษรพิมพ์ใหญ่อย่างน้อยหนึ่งตัว")
            .Matches("[a-z]").WithMessage("รหัสผ่านต้องมีตัวอักษรพิมพ์เล็กอย่างน้อยหนึ่งตัว")
            .Matches("[0-9]").WithMessage("รหัสผ่านต้องมีตัวเลขอย่างน้อยหนึ่งตัว");
    }
}

/// <summary>ตรวจสอบข้อมูลการเข้าสู่ระบบ</summary>
public sealed class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    /// <summary>สร้างตัวตรวจสอบ</summary>
    public LoginRequestValidator()
    {
        RuleFor(request => request.Email)
            .NotEmpty().WithMessage("กรุณาระบุอีเมล")
            .MaximumLength(256).WithMessage("อีเมลต้องยาวไม่เกิน 256 ตัวอักษร")
            .EmailAddress().WithMessage("รูปแบบอีเมลไม่ถูกต้อง");

        // ตอนเข้าสู่ระบบไม่บังคับความซับซ้อนของรหัสผ่าน
        // เพราะบัญชีเก่าอาจตั้งรหัสไว้ก่อนที่นโยบายปัจจุบันจะมีผล
        RuleFor(request => request.Password)
            .NotEmpty().WithMessage("กรุณาระบุรหัสผ่าน")
            .MaximumLength(PasswordRules.MaximumLength)
            .WithMessage($"รหัสผ่านต้องยาวไม่เกิน {PasswordRules.MaximumLength} ตัวอักษร");
    }
}

/// <summary>ตรวจสอบคำขอต่ออายุโทเคน</summary>
public sealed class RefreshTokenRequestValidator : AbstractValidator<RefreshTokenRequest>
{
    /// <summary>สร้างตัวตรวจสอบ</summary>
    public RefreshTokenRequestValidator()
    {
        RuleFor(request => request.RefreshToken)
            .NotEmpty().WithMessage("กรุณาระบุ Refresh Token")
            .MaximumLength(512).WithMessage("Refresh Token ไม่ถูกต้อง");
    }
}

/// <summary>ตรวจสอบคำขอออกจากระบบ</summary>
public sealed class LogoutRequestValidator : AbstractValidator<LogoutRequest>
{
    /// <summary>สร้างตัวตรวจสอบ</summary>
    public LogoutRequestValidator()
    {
        RuleFor(request => request.RefreshToken)
            .NotEmpty().WithMessage("กรุณาระบุ Refresh Token")
            .MaximumLength(512).WithMessage("Refresh Token ไม่ถูกต้อง");
    }
}

/// <summary>ตรวจสอบคำขอลิงก์ตั้งรหัสผ่านใหม่</summary>
public sealed class ForgotPasswordRequestValidator : AbstractValidator<ForgotPasswordRequest>
{
    /// <summary>สร้างตัวตรวจสอบ</summary>
    public ForgotPasswordRequestValidator()
    {
        RuleFor(request => request.Email)
            .NotEmpty().WithMessage("กรุณาระบุอีเมล")
            .MaximumLength(256).WithMessage("อีเมลต้องยาวไม่เกิน 256 ตัวอักษร")
            .EmailAddress().WithMessage("รูปแบบอีเมลไม่ถูกต้อง");
    }
}

/// <summary>ตรวจสอบคำขอตั้งรหัสผ่านใหม่</summary>
public sealed class ResetPasswordRequestValidator : AbstractValidator<ResetPasswordRequest>
{
    /// <summary>สร้างตัวตรวจสอบ</summary>
    public ResetPasswordRequestValidator()
    {
        RuleFor(request => request.Email)
            .NotEmpty().WithMessage("กรุณาระบุอีเมล")
            .EmailAddress().WithMessage("รูปแบบอีเมลไม่ถูกต้อง");

        RuleFor(request => request.Token)
            .NotEmpty().WithMessage("กรุณาระบุโทเคนสำหรับตั้งรหัสผ่านใหม่");

        RuleFor(request => request.NewPassword).ApplyPasswordPolicy();

        RuleFor(request => request.ConfirmPassword)
            .Equal(request => request.NewPassword)
            .WithMessage("รหัสผ่านใหม่และการยืนยันรหัสผ่านไม่ตรงกัน");
    }
}

/// <summary>ตรวจสอบคำขอเปลี่ยนรหัสผ่าน</summary>
public sealed class ChangePasswordRequestValidator : AbstractValidator<ChangePasswordRequest>
{
    /// <summary>สร้างตัวตรวจสอบ</summary>
    public ChangePasswordRequestValidator()
    {
        RuleFor(request => request.CurrentPassword)
            .NotEmpty().WithMessage("กรุณาระบุรหัสผ่านปัจจุบัน");

        RuleFor(request => request.NewPassword)
            .ApplyPasswordPolicy()
            .NotEqual(request => request.CurrentPassword)
            .WithMessage("รหัสผ่านใหม่ต้องไม่ซ้ำกับรหัสผ่านปัจจุบัน");

        RuleFor(request => request.ConfirmPassword)
            .Equal(request => request.NewPassword)
            .WithMessage("รหัสผ่านใหม่และการยืนยันรหัสผ่านไม่ตรงกัน");
    }
}
