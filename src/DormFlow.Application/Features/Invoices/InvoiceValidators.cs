using DormFlow.Contracts.Invoices;
using FluentValidation;

namespace DormFlow.Application.Features.Invoices;

/// <summary>ກວດເບິ່ງຄຳຮ້ອງຂໍການສ້າງໃບແຈ້ງໜີ້</summary>
public sealed class CreateInvoiceRequestValidator : AbstractValidator<CreateInvoiceRequest>
{
    /// <summary>ສ້າງຕົວກວດສອບ</summary>
    public CreateInvoiceRequestValidator()
    {
        RuleFor(request => request.ContractId)
            .NotEmpty().WithMessage("ກະລຸນາລະບຸສັນຍາເຊົ່າທີ່ຕ້ອງການອອກໃບແຈ້ງໜີ້");

        RuleFor(request => request.Notes)
            .MaximumLength(1000).WithMessage("ໝາຍເຫດຕ້ອງມີຄວາມຍາວບໍ່ເກີນ 1000 ໂຕອັກສອນ");
    }
}
