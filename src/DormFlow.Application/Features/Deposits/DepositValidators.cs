using DormFlow.Contracts.Deposits;
using FluentValidation;

namespace DormFlow.Application.Features.Deposits;

/// <summary>ตรวจสอบคำขอบันทึกการรับเงินมัดจำ</summary>
public sealed class CollectDepositRequestValidator : AbstractValidator<CollectDepositRequest>
{
    /// <summary>สร้างตัวตรวจสอบ</summary>
    public CollectDepositRequestValidator()
    {
        RuleFor(request => request.ContractId)
            .NotEmpty().WithMessage("กรุณาระบุสัญญาเช่าที่รับเงินมัดจำ");

        RuleFor(request => request.Amount)
            .GreaterThanOrEqualTo(0).WithMessage("จำนวนเงินมัดจำต้องไม่ติดลบ")
            .When(request => request.Amount.HasValue);

        RuleFor(request => request.Notes)
            .MaximumLength(1000).WithMessage("หมายเหตุต้องยาวไม่เกิน 1000 ตัวอักษร");
    }
}

/// <summary>ตรวจสอบคำขอคืนเงินมัดจำ</summary>
public sealed class RefundDepositRequestValidator : AbstractValidator<RefundDepositRequest>
{
    /// <summary>สร้างตัวตรวจสอบ</summary>
    public RefundDepositRequestValidator()
    {
        RuleFor(request => request.RefundAmount)
            .GreaterThanOrEqualTo(0).WithMessage("จำนวนเงินที่คืนต้องไม่ติดลบ");

        RuleFor(request => request.DeductionReason)
            .MaximumLength(500).WithMessage("เหตุผลของส่วนที่หักไว้ต้องยาวไม่เกิน 500 ตัวอักษร");
    }
}
