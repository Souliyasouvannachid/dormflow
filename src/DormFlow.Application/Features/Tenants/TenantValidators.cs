using DormFlow.Contracts.Tenants;
using FluentValidation;

namespace DormFlow.Application.Features.Tenants;

/// <summary>ตรวจสอบคำขอสร้างข้อมูลผู้เช่าใหม่</summary>
public sealed class CreateTenantRequestValidator : AbstractValidator<CreateTenantRequest>
{
    /// <summary>สร้างตัวตรวจสอบ</summary>
    public CreateTenantRequestValidator()
    {
        RuleFor(request => request.FirstName)
            .NotEmpty().WithMessage("กรุณาระบุชื่อ")
            .MaximumLength(100).WithMessage("ชื่อต้องยาวไม่เกิน 100 ตัวอักษร");

        RuleFor(request => request.LastName)
            .NotEmpty().WithMessage("กรุณาระบุนามสกุล")
            .MaximumLength(100).WithMessage("นามสกุลต้องยาวไม่เกิน 100 ตัวอักษร");

        RuleFor(request => request.Email)
            .EmailAddress().WithMessage("รูปแบบอีเมลไม่ถูกต้อง")
            .When(request => !string.IsNullOrWhiteSpace(request.Email));

        RuleFor(request => request.NationalId)
            .MaximumLength(20).WithMessage("เลขบัตรประจำตัวต้องยาวไม่เกิน 20 ตัวอักษร");

        RuleFor(request => request.DateOfBirth!.Value)
            .LessThan(DateOnly.FromDateTime(DateTime.UtcNow))
            .WithMessage("วันเดือนปีเกิดต้องไม่ใช่วันที่ในอนาคต")
            .When(request => request.DateOfBirth.HasValue);
    }
}

/// <summary>ตรวจสอบคำขอแก้ไขข้อมูลผู้เช่า</summary>
public sealed class UpdateTenantRequestValidator : AbstractValidator<UpdateTenantRequest>
{
    /// <summary>สร้างตัวตรวจสอบ</summary>
    public UpdateTenantRequestValidator()
    {
        RuleFor(request => request.FirstName)
            .NotEmpty().WithMessage("กรุณาระบุชื่อ")
            .MaximumLength(100).WithMessage("ชื่อต้องยาวไม่เกิน 100 ตัวอักษร");

        RuleFor(request => request.LastName)
            .NotEmpty().WithMessage("กรุณาระบุนามสกุล")
            .MaximumLength(100).WithMessage("นามสกุลต้องยาวไม่เกิน 100 ตัวอักษร");

        RuleFor(request => request.Email)
            .EmailAddress().WithMessage("รูปแบบอีเมลไม่ถูกต้อง")
            .When(request => !string.IsNullOrWhiteSpace(request.Email));

        RuleFor(request => request.NationalId)
            .MaximumLength(20).WithMessage("เลขบัตรประจำตัวต้องยาวไม่เกิน 20 ตัวอักษร");

        RuleFor(request => request.DateOfBirth!.Value)
            .LessThan(DateOnly.FromDateTime(DateTime.UtcNow))
            .WithMessage("วันเดือนปีเกิดต้องไม่ใช่วันที่ในอนาคต")
            .When(request => request.DateOfBirth.HasValue);
    }
}
