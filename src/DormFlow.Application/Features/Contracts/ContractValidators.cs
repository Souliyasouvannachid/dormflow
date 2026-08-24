using DormFlow.Contracts.Leases;
using FluentValidation;

namespace DormFlow.Application.Features.Contracts;

/// <summary>ตรวจสอบคำขอสร้างสัญญาเช่าใหม่</summary>
public sealed class CreateContractRequestValidator : AbstractValidator<CreateContractRequest>
{
    /// <summary>สร้างตัวตรวจสอบ</summary>
    public CreateContractRequestValidator()
    {
        RuleFor(request => request.RoomId)
            .NotEmpty().WithMessage("กรุณาระบุห้องที่จะทำสัญญา");

        RuleFor(request => request.TenantId)
            .NotEmpty().WithMessage("กรุณาระบุผู้เช่าตามสัญญา");

        RuleFor(request => request.EndDate)
            .GreaterThan(request => request.StartDate)
            .WithMessage("วันที่สิ้นสุดสัญญาต้องอยู่หลังวันที่เริ่มสัญญา");

        RuleFor(request => request.MonthlyRent)
            .GreaterThanOrEqualTo(0).WithMessage("ค่าเช่ารายเดือนต้องไม่ติดลบ")
            .When(request => request.MonthlyRent.HasValue);

        RuleFor(request => request.DepositAmount)
            .GreaterThanOrEqualTo(0).WithMessage("ค่ามัดจำต้องไม่ติดลบ")
            .When(request => request.DepositAmount.HasValue);

        RuleFor(request => request.Notes)
            .MaximumLength(1000).WithMessage("หมายเหตุต้องยาวไม่เกิน 1000 ตัวอักษร");
    }
}

/// <summary>ตรวจสอบคำขอสิ้นสุดสัญญาเช่า</summary>
public sealed class TerminateContractRequestValidator : AbstractValidator<TerminateContractRequest>
{
    /// <summary>สร้างตัวตรวจสอบ</summary>
    public TerminateContractRequestValidator()
    {
        RuleFor(request => request.Reason)
            .MaximumLength(500).WithMessage("สาเหตุต้องยาวไม่เกิน 500 ตัวอักษร");
    }
}
