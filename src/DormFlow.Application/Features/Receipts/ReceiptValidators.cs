using DormFlow.Contracts.Receipts;
using FluentValidation;

namespace DormFlow.Application.Features.Receipts;

/// <summary>ตรวจสอบคำขอออกใบเสร็จรับเงิน</summary>
public sealed class IssueReceiptRequestValidator : AbstractValidator<IssueReceiptRequest>
{
    /// <summary>สร้างตัวตรวจสอบ</summary>
    public IssueReceiptRequestValidator()
    {
        RuleFor(request => request.PaymentId)
            .NotEmpty().WithMessage("กรุณาระบุรายการชำระเงินที่ต้องการออกใบเสร็จ");

        RuleFor(request => request.Notes)
            .MaximumLength(1000).WithMessage("หมายเหตุต้องยาวไม่เกิน 1000 ตัวอักษร");
    }
}
