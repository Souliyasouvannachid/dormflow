using DormFlow.Application.Common.Abstractions;
using DormFlow.Contracts.Users;
using FluentValidation;

namespace DormFlow.Application.Features.Users;

/// <summary>ตรวจสอบคำขอสร้างบัญชีผู้ใช้ใหม่</summary>
public sealed class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
{
    /// <summary>สร้างตัวตรวจสอบ</summary>
    public CreateUserRequestValidator()
    {
        RuleFor(request => request.Email)
            .NotEmpty().WithMessage("กรุณาระบุอีเมล")
            .MaximumLength(256).WithMessage("อีเมลต้องยาวไม่เกิน 256 ตัวอักษร")
            .EmailAddress().WithMessage("รูปแบบอีเมลไม่ถูกต้อง");

        RuleFor(request => request.Password).SetValidator(new PasswordRuleValidator());

        RuleFor(request => request.FirstName)
            .NotEmpty().WithMessage("กรุณาระบุชื่อ")
            .MaximumLength(100).WithMessage("ชื่อต้องยาวไม่เกิน 100 ตัวอักษร");

        RuleFor(request => request.LastName)
            .NotEmpty().WithMessage("กรุณาระบุนามสกุล")
            .MaximumLength(100).WithMessage("นามสกุลต้องยาวไม่เกิน 100 ตัวอักษร");
    }
}

/// <summary>ตรวจสอบคำขอแก้ไขข้อมูลโปรไฟล์ผู้ใช้</summary>
public sealed class UpdateUserRequestValidator : AbstractValidator<UpdateUserRequest>
{
    /// <summary>สร้างตัวตรวจสอบ</summary>
    public UpdateUserRequestValidator()
    {
        RuleFor(request => request.FirstName)
            .NotEmpty().WithMessage("กรุณาระบุชื่อ")
            .MaximumLength(100).WithMessage("ชื่อต้องยาวไม่เกิน 100 ตัวอักษร");

        RuleFor(request => request.LastName)
            .NotEmpty().WithMessage("กรุณาระบุนามสกุล")
            .MaximumLength(100).WithMessage("นามสกุลต้องยาวไม่เกิน 100 ตัวอักษร");
    }
}

/// <summary>ตรวจสอบคำขอกำหนดบทบาทให้ผู้ใช้</summary>
public sealed class AssignUserRolesRequestValidator : AbstractValidator<AssignUserRolesRequest>
{
    /// <summary>สร้างตัวตรวจสอบ</summary>
    public AssignUserRolesRequestValidator()
    {
        RuleFor(request => request.Roles)
            .NotEmpty().WithMessage("กรุณาระบุบทบาทอย่างน้อยหนึ่งรายการ");

        RuleForEach(request => request.Roles)
            .Must(DormFlowRoles.All.Contains)
            .WithMessage("พบชื่อบทบาทที่ไม่มีอยู่ในระบบ");
    }
}

/// <summary>ตรวจสอบคำขอมอบหมายหอพักให้ผู้ใช้</summary>
public sealed class AssignUserDormitoryRequestValidator : AbstractValidator<AssignUserDormitoryRequest>
{
    /// <summary>สร้างตัวตรวจสอบ</summary>
    public AssignUserDormitoryRequestValidator()
    {
        RuleFor(request => request.DormitoryId)
            .NotEmpty().WithMessage("กรุณาระบุหอพักที่ต้องการมอบหมาย");
    }
}

/// <summary>
/// กฎของรหัสผ่านที่ใช้ตอนสร้างบัญชีโดยผู้ดูแลระบบ
/// </summary>
/// <remarks>
/// แยกจาก PasswordRules ของโมดูล Authentication เพื่อไม่ให้ Features/Users ต้องอ้างอิงโมดูล Authentication
/// แต่ใช้เกณฑ์ชุดเดียวกันทุกประการ
/// </remarks>
internal sealed class PasswordRuleValidator : AbstractValidator<string>
{
    public PasswordRuleValidator()
    {
        RuleFor(password => password)
            .NotEmpty().WithMessage("กรุณาระบุรหัสผ่าน")
            .MinimumLength(8).WithMessage("รหัสผ่านต้องยาวอย่างน้อย 8 ตัวอักษร")
            .MaximumLength(128).WithMessage("รหัสผ่านต้องยาวไม่เกิน 128 ตัวอักษร")
            .Matches("[A-Z]").WithMessage("รหัสผ่านต้องมีตัวอักษรพิมพ์ใหญ่อย่างน้อยหนึ่งตัว")
            .Matches("[a-z]").WithMessage("รหัสผ่านต้องมีตัวอักษรพิมพ์เล็กอย่างน้อยหนึ่งตัว")
            .Matches("[0-9]").WithMessage("รหัสผ่านต้องมีตัวเลขอย่างน้อยหนึ่งตัว");
    }
}
