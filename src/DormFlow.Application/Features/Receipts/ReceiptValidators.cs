using DormFlow.Contracts.Receipts;
using FluentValidation;

namespace DormFlow.Application.Features.Receipts;

/// <summary>ກວດເບິ່ງການຮ້ອງຂໍການຮັບ</summary>
public sealed class IssueReceiptRequestValidator : AbstractValidator<IssueReceiptRequest>
{
    /// <summary>ສ້າງຕົວກວດສອບ</summary>
    public IssueReceiptRequestValidator()
    {
        RuleFor(request => request.PaymentId)
            .NotEmpty().WithMessage("ກະລຸນາລະບຸລາຍການຊຳລະເງິນທີ່ຕ້ອງການອອກໃບຮັບເງິນ");

        RuleFor(request => request.Notes)
            .MaximumLength(1000).WithMessage("ໝາຍເຫດຕ້ອງມີຄວາມຍາວບໍ່ເກີນ 1000 ໂຕອັກສອນ");
    }
}
