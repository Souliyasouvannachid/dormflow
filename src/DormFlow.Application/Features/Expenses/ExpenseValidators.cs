using DormFlow.Contracts.Expenses;
using DormFlow.Domain.Enums;
using FluentValidation;

namespace DormFlow.Application.Features.Expenses;

/// <summary>ตรวจสอบคำขอบันทึกค่าใช้จ่าย</summary>
public sealed class CreateExpenseRequestValidator : AbstractValidator<CreateExpenseRequest>
{
    /// <summary>สร้างตัวตรวจสอบ</summary>
    public CreateExpenseRequestValidator()
    {
        RuleFor(request => request.DormitoryId)
            .NotEmpty().WithMessage("กรุณาระบุหอพักที่เกี่ยวข้องกับค่าใช้จ่ายนี้");

        RuleFor(request => request.Category)
            .Must(value => Enum.TryParse<ExpenseCategory>(value, ignoreCase: true, out _))
            .WithMessage("หมวดหมู่ค่าใช้จ่ายไม่ถูกต้อง");

        RuleFor(request => request.Amount)
            .GreaterThan(0).WithMessage("จำนวนเงินต้องมากกว่าศูนย์");

        RuleFor(request => request.Description)
            .NotEmpty().WithMessage("กรุณาระบุคำอธิบายรายการ")
            .MaximumLength(500).WithMessage("คำอธิบายต้องยาวไม่เกิน 500 ตัวอักษร");

        RuleFor(request => request.PaidTo)
            .MaximumLength(200).WithMessage("ชื่อผู้รับเงินต้องยาวไม่เกิน 200 ตัวอักษร");

        RuleFor(request => request.Notes)
            .MaximumLength(1000).WithMessage("หมายเหตุต้องยาวไม่เกิน 1000 ตัวอักษร");
    }
}
