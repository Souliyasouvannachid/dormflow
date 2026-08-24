using DormFlow.Contracts.Organizations;
using FluentValidation;

namespace DormFlow.Application.Features.Organizations;

/// <summary>ตรวจสอบคำขอสร้างองค์กรใหม่</summary>
public sealed class CreateOrganizationRequestValidator : AbstractValidator<CreateOrganizationRequest>
{
    /// <summary>สร้างตัวตรวจสอบ</summary>
    public CreateOrganizationRequestValidator()
    {
        RuleFor(request => request.Code)
            .NotEmpty().WithMessage("กรุณาระบุรหัสองค์กร")
            .MaximumLength(50).WithMessage("รหัสองค์กรต้องยาวไม่เกิน 50 ตัวอักษร");

        RuleFor(request => request.Name)
            .NotEmpty().WithMessage("กรุณาระบุชื่อองค์กร")
            .MaximumLength(200).WithMessage("ชื่อองค์กรต้องยาวไม่เกิน 200 ตัวอักษร");

        RuleFor(request => request.Email)
            .EmailAddress().WithMessage("รูปแบบอีเมลไม่ถูกต้อง")
            .When(request => !string.IsNullOrWhiteSpace(request.Email));

        RuleFor(request => request.TaxId)
            .MaximumLength(20).WithMessage("เลขประจำตัวผู้เสียภาษีต้องยาวไม่เกิน 20 ตัวอักษร");
    }
}

/// <summary>ตรวจสอบคำขอแก้ไขข้อมูลองค์กร</summary>
public sealed class UpdateOrganizationRequestValidator : AbstractValidator<UpdateOrganizationRequest>
{
    /// <summary>สร้างตัวตรวจสอบ</summary>
    public UpdateOrganizationRequestValidator()
    {
        RuleFor(request => request.Code)
            .NotEmpty().WithMessage("กรุณาระบุรหัสองค์กร")
            .MaximumLength(50).WithMessage("รหัสองค์กรต้องยาวไม่เกิน 50 ตัวอักษร");

        RuleFor(request => request.Name)
            .NotEmpty().WithMessage("กรุณาระบุชื่อองค์กร")
            .MaximumLength(200).WithMessage("ชื่อองค์กรต้องยาวไม่เกิน 200 ตัวอักษร");

        RuleFor(request => request.Email)
            .EmailAddress().WithMessage("รูปแบบอีเมลไม่ถูกต้อง")
            .When(request => !string.IsNullOrWhiteSpace(request.Email));

        RuleFor(request => request.TaxId)
            .MaximumLength(20).WithMessage("เลขประจำตัวผู้เสียภาษีต้องยาวไม่เกิน 20 ตัวอักษร");
    }
}
