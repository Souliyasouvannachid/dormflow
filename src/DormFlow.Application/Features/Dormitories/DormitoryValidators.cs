using DormFlow.Contracts.Dormitories;
using DormFlow.Domain.Enums;
using FluentValidation;

namespace DormFlow.Application.Features.Dormitories;

/// <summary>ตรวจสอบคำขอสร้างหอพักใหม่</summary>
public sealed class CreateDormitoryRequestValidator : AbstractValidator<CreateDormitoryRequest>
{
    /// <summary>สร้างตัวตรวจสอบ</summary>
    public CreateDormitoryRequestValidator()
    {
        RuleFor(request => request.Code)
            .NotEmpty().WithMessage("กรุณาระบุรหัสหอพัก")
            .MaximumLength(50).WithMessage("รหัสหอพักต้องยาวไม่เกิน 50 ตัวอักษร");

        RuleFor(request => request.Name)
            .NotEmpty().WithMessage("กรุณาระบุชื่อหอพัก")
            .MaximumLength(200).WithMessage("ชื่อหอพักต้องยาวไม่เกิน 200 ตัวอักษร");

        RuleFor(request => request.Email)
            .EmailAddress().WithMessage("รูปแบบอีเมลไม่ถูกต้อง")
            .When(request => !string.IsNullOrWhiteSpace(request.Email));

        RuleFor(request => request.BillingDay)
            .InclusiveBetween(1, 28).WithMessage("วันที่ออกใบแจ้งหนี้ต้องอยู่ระหว่าง 1 ถึง 28");

        RuleFor(request => request.PaymentDueDay)
            .InclusiveBetween(1, 31).WithMessage("วันที่ครบกำหนดชำระต้องอยู่ระหว่าง 1 ถึง 31");

        RuleFor(request => request.LateFeeType)
            .Must(value => Enum.TryParse<LateFeeType>(value, ignoreCase: true, out _))
            .WithMessage("รูปแบบค่าปรับชำระล่าช้าไม่ถูกต้อง ต้องเป็น Fixed หรือ Percentage");

        RuleFor(request => request.LateFeeAmount)
            .GreaterThanOrEqualTo(0).WithMessage("ค่าปรับชำระล่าช้าต้องไม่ติดลบ");
    }
}

/// <summary>ตรวจสอบคำขอแก้ไขข้อมูลหอพัก</summary>
public sealed class UpdateDormitoryRequestValidator : AbstractValidator<UpdateDormitoryRequest>
{
    /// <summary>สร้างตัวตรวจสอบ</summary>
    public UpdateDormitoryRequestValidator()
    {
        RuleFor(request => request.Code)
            .NotEmpty().WithMessage("กรุณาระบุรหัสหอพัก")
            .MaximumLength(50).WithMessage("รหัสหอพักต้องยาวไม่เกิน 50 ตัวอักษร");

        RuleFor(request => request.Name)
            .NotEmpty().WithMessage("กรุณาระบุชื่อหอพัก")
            .MaximumLength(200).WithMessage("ชื่อหอพักต้องยาวไม่เกิน 200 ตัวอักษร");

        RuleFor(request => request.Email)
            .EmailAddress().WithMessage("รูปแบบอีเมลไม่ถูกต้อง")
            .When(request => !string.IsNullOrWhiteSpace(request.Email));

        RuleFor(request => request.BillingDay)
            .InclusiveBetween(1, 28).WithMessage("วันที่ออกใบแจ้งหนี้ต้องอยู่ระหว่าง 1 ถึง 28");

        RuleFor(request => request.PaymentDueDay)
            .InclusiveBetween(1, 31).WithMessage("วันที่ครบกำหนดชำระต้องอยู่ระหว่าง 1 ถึง 31");

        RuleFor(request => request.LateFeeType)
            .Must(value => Enum.TryParse<LateFeeType>(value, ignoreCase: true, out _))
            .WithMessage("รูปแบบค่าปรับชำระล่าช้าไม่ถูกต้อง ต้องเป็น Fixed หรือ Percentage");

        RuleFor(request => request.LateFeeAmount)
            .GreaterThanOrEqualTo(0).WithMessage("ค่าปรับชำระล่าช้าต้องไม่ติดลบ");
    }
}
