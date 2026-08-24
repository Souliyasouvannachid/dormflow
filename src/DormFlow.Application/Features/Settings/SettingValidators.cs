using DormFlow.Contracts.Settings;
using FluentValidation;

namespace DormFlow.Application.Features.Settings;

/// <summary>ตรวจสอบคำขอสร้างหรือแก้ไขค่าตั้ง</summary>
public sealed class UpsertSettingRequestValidator : AbstractValidator<UpsertSettingRequest>
{
    /// <summary>สร้างตัวตรวจสอบ</summary>
    public UpsertSettingRequestValidator()
    {
        RuleFor(request => request.Value)
            .NotEmpty().WithMessage("กรุณาระบุค่าของค่าตั้ง")
            .MaximumLength(2000).WithMessage("ค่าของค่าตั้งต้องยาวไม่เกิน 2000 ตัวอักษร");
    }
}
