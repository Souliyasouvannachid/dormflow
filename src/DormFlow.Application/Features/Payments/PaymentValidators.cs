using DormFlow.Contracts.Payments;
using DormFlow.Domain.Enums;
using FluentValidation;

namespace DormFlow.Application.Features.Payments;

/// <summary>ตรวจสอบคำขอบันทึกรายการชำระเงิน</summary>
public sealed class CreatePaymentRequestValidator : AbstractValidator<CreatePaymentRequest>
{
    /// <summary>สร้างตัวตรวจสอบ</summary>
    public CreatePaymentRequestValidator()
    {
        RuleFor(request => request.InvoiceId)
            .NotEmpty().WithMessage("กรุณาระบุใบแจ้งหนี้ที่ชำระ");

        RuleFor(request => request.Amount)
            .GreaterThan(0).WithMessage("จำนวนเงินที่ชำระต้องมากกว่าศูนย์");

        RuleFor(request => request.Method)
            .Must(value => Enum.TryParse<PaymentMethod>(value, ignoreCase: true, out _))
            .WithMessage("ช่องทางการชำระเงินไม่ถูกต้อง ต้องเป็น Cash, BankTransfer, QrCode หรือ Other");

        RuleFor(request => request.ReferenceNumber)
            .MaximumLength(100).WithMessage("เลขอ้างอิงต้องยาวไม่เกิน 100 ตัวอักษร");

        RuleFor(request => request.Notes)
            .MaximumLength(1000).WithMessage("หมายเหตุต้องยาวไม่เกิน 1000 ตัวอักษร");
    }
}

/// <summary>ตรวจสอบคำขอคืนเงิน</summary>
public sealed class RefundPaymentRequestValidator : AbstractValidator<RefundPaymentRequest>
{
    /// <summary>สร้างตัวตรวจสอบ</summary>
    public RefundPaymentRequestValidator()
    {
        RuleFor(request => request.Reason)
            .MaximumLength(500).WithMessage("สาเหตุการคืนเงินต้องยาวไม่เกิน 500 ตัวอักษร");
    }
}
