using DormFlow.Contracts.Deposits;
using FluentValidation;

namespace DormFlow.Application.Features.Deposits;

/// <summary>ກວດສອບການຮ້ອງຂໍບັນທຶກການຮັບເງິນຝາກ</summary>
public sealed class CollectDepositRequestValidator : AbstractValidator<CollectDepositRequest>
{
    /// <summary>ສ້າງຕົວກວດສອບ</summary>
    public CollectDepositRequestValidator()
    {
        RuleFor(request => request.ContractId)
            .NotEmpty().WithMessage("ກະລຸນາລະບຸສັນຍາເຊົ່າທີ່ຮັບເງິນມັດຈຳ");

        RuleFor(request => request.Amount)
            .GreaterThanOrEqualTo(0).WithMessage("ຈຳນວນເງິນມັດຈຳຕ້ອງບໍ່ຕິດລົບ")
            .When(request => request.Amount.HasValue);

        RuleFor(request => request.Notes)
            .MaximumLength(1000).WithMessage("ໝາຍເຫດຕ້ອງມີຄວາມຍາວບໍ່ເກີນ 1000 ໂຕອັກສອນ");
    }
}

/// <summary>ທົບທວນຄືນຄໍາຮ້ອງຂໍການຄືນເງິນເງິນຝາກຂອງທ່ານ</summary>
public sealed class RefundDepositRequestValidator : AbstractValidator<RefundDepositRequest>
{
    /// <summary>ສ້າງຕົວກວດສອບ</summary>
    public RefundDepositRequestValidator()
    {
        RuleFor(request => request.RefundAmount)
            .GreaterThanOrEqualTo(0).WithMessage("ຈຳນວນເງິນທີ່ຄືນຕ້ອງບໍ່ຕິດລົບ");

        RuleFor(request => request.DeductionReason)
            .MaximumLength(500).WithMessage("ເຫດຜົນຂອງສ່ວນທີ່ຫັກໄວ້ຕ້ອງມີຄວາມຍາວບໍ່ເກີນ 500 ໂຕອັກສອນ");
    }
}
