using DormFlow.Application.Common.Authorization;
using DormFlow.Contracts.Roles;
using FluentValidation;

namespace DormFlow.Application.Features.Roles;

/// <summary>ตรวจสอบคำขอสร้างบทบาทใหม่</summary>
public sealed class CreateRoleRequestValidator : AbstractValidator<CreateRoleRequest>
{
    /// <summary>สร้างตัวตรวจสอบ</summary>
    public CreateRoleRequestValidator()
    {
        RuleFor(request => request.Name)
            .NotEmpty().WithMessage("กรุณาระบุชื่อบทบาท")
            .MaximumLength(100).WithMessage("ชื่อบทบาทต้องยาวไม่เกิน 100 ตัวอักษร");

        RuleFor(request => request.Description)
            .MaximumLength(256).WithMessage("คำอธิบายต้องยาวไม่เกิน 256 ตัวอักษร");
    }
}

/// <summary>ตรวจสอบคำขอแก้ไขข้อมูลบทบาท</summary>
public sealed class UpdateRoleRequestValidator : AbstractValidator<UpdateRoleRequest>
{
    /// <summary>สร้างตัวตรวจสอบ</summary>
    public UpdateRoleRequestValidator()
    {
        RuleFor(request => request.Name)
            .NotEmpty().WithMessage("กรุณาระบุชื่อบทบาท")
            .MaximumLength(100).WithMessage("ชื่อบทบาทต้องยาวไม่เกิน 100 ตัวอักษร");

        RuleFor(request => request.Description)
            .MaximumLength(256).WithMessage("คำอธิบายต้องยาวไม่เกิน 256 ตัวอักษร");
    }
}

/// <summary>ตรวจสอบคำขอกำหนดสิทธิ์ให้บทบาท</summary>
public sealed class AssignRolePermissionsRequestValidator : AbstractValidator<AssignRolePermissionsRequest>
{
    /// <summary>สร้างตัวตรวจสอบ</summary>
    public AssignRolePermissionsRequestValidator()
    {
        RuleForEach(request => request.Permissions)
            .Must(PermissionCodes.Exists)
            .WithMessage("พบรหัสสิทธิ์ที่ไม่มีอยู่ในระบบ");
    }
}
