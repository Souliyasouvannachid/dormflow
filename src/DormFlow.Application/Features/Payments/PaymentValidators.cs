using DormFlow.Contracts.Payments;
using DormFlow.Domain.Enums;
using FluentValidation;

namespace DormFlow.Application.Features.Payments;

/// <summary>ທົບທວນຄືນການຮ້ອງຂໍການບັນທຶກການຈ່າຍເງິນ</summary>
public sealed class CreatePaymentRequestValidator : AbstractValidator<CreatePaymentRequest>
{
    /// <summary>ສ້າງຕົວກວດສອບ</summary>
    public CreatePaymentRequestValidator()
    {
        RuleFor(request => request.InvoiceId)
            .NotEmpty().WithMessage("ກະລຸນາລະບຸໃບແຈ້ງໜີ້ທີ່ຊຳລະ");

        RuleFor(request => request.Amount)
            .GreaterThan(0).WithMessage("ຈຳນວນເງິນທີ່ຊຳລະຕ້ອງຫຼາຍກວ່າສູນ");

        RuleFor(request => request.Method)
            .Must(value => Enum.TryParse<PaymentMethod>(value, ignoreCase: true, out _))
            .WithMessage("ຊ່ອງທາງການຊຳລະເງິນບໍ່ຖືກຕ້ອງ ຕ້ອງເປັນ Cash, BankTransfer, QrCode ຫຼື Other");

        RuleFor(request => request.ReferenceNumber)
            .MaximumLength(100).WithMessage("ເລກອ້າງອີງຕ້ອງມີຄວາມຍາວບໍ່ເກີນ 100 ໂຕອັກສອນ");

        RuleFor(request => request.Notes)
            .MaximumLength(1000).WithMessage("ໝາຍເຫດຕ້ອງມີຄວາມຍາວບໍ່ເກີນ 1000 ໂຕອັກສອນ");
    }
}

/// <summary>ທົບທວນຄືນຄໍາຮ້ອງຂໍການຄືນເງິນຂອງທ່ານ</summary>
public sealed class RefundPaymentRequestValidator : AbstractValidator<RefundPaymentRequest>
{
    /// <summary>ສ້າງຕົວກວດສອບ</summary>
    public RefundPaymentRequestValidator()
    {
        RuleFor(request => request.Reason)
            .MaximumLength(500).WithMessage("ສາເຫດການຄືນເງິນຕ້ອງມີຄວາມຍາວບໍ່ເກີນ 500 ໂຕອັກສອນ");
    }
}
