using DormFlow.Contracts.Invoices;
using FluentValidation;

namespace DormFlow.Application.Features.Invoices;

/// <summary>ตรวจสอบคำขอสร้างใบแจ้งหนี้</summary>
public sealed class CreateInvoiceRequestValidator : AbstractValidator<CreateInvoiceRequest>
{
    /// <summary>สร้างตัวตรวจสอบ</summary>
    public CreateInvoiceRequestValidator()
    {
        RuleFor(request => request.ContractId)
            .NotEmpty().WithMessage("กรุณาระบุสัญญาเช่าที่ต้องการออกบิล");

        RuleFor(request => request.Notes)
            .MaximumLength(1000).WithMessage("หมายเหตุต้องยาวไม่เกิน 1000 ตัวอักษร");
    }
}
