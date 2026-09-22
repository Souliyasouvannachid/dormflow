using DormFlow.Contracts.Leases;
using FluentValidation;

namespace DormFlow.Application.Features.Contracts;

/// <summary>ທົບທວນຄືນການຮ້ອງຂໍເພື່ອສ້າງສັນຍາເຊົ່າໃຝ່</summary>
public sealed class CreateContractRequestValidator : AbstractValidator<CreateContractRequest>
{
    /// <summary>ສ້າງຕົວກວດສອບ</summary>
    public CreateContractRequestValidator()
    {
        RuleFor(request => request.RoomId)
            .NotEmpty().WithMessage("ກະລຸນາລະບຸຫ້ອງທີ່ຈະເຮັດສັນຍາ");

        RuleFor(request => request.TenantId)
            .NotEmpty().WithMessage("ກະລຸນາລະບຸຜູ້ເຊົ່າຕາມສັນຍາ");

        RuleFor(request => request.EndDate)
            .GreaterThan(request => request.StartDate)
            .WithMessage("ວັນທີສິ້ນສຸດສັນຍາຕ້ອງຢູ່ຫຼັງວັນທີເລີ່ມສັນຍາ");

        RuleFor(request => request.MonthlyRent)
            .GreaterThanOrEqualTo(0).WithMessage("ຄ່າເຊົ່າລາຍເດືອນຕ້ອງບໍ່ຕິດລົບ")
            .When(request => request.MonthlyRent.HasValue);

        RuleFor(request => request.DepositAmount)
            .GreaterThanOrEqualTo(0).WithMessage("ຄ່າມັດຈຳຕ້ອງບໍ່ຕິດລົບ")
            .When(request => request.DepositAmount.HasValue);

        RuleFor(request => request.Notes)
            .MaximumLength(1000).WithMessage("ໝາຍເຫດຕ້ອງມີຄວາມຍາວບໍ່ເກີນ 1000 ໂຕອັກສອນ");
    }
}

/// <summary>ທົບທວນຄືນການຮ້ອງຂໍໃຫ້ສິ້ນສຸດສັນຍາເຊົ່າ</summary>
public sealed class TerminateContractRequestValidator : AbstractValidator<TerminateContractRequest>
{
    /// <summary>ສ້າງຕົວກວດສອບ</summary>
    public TerminateContractRequestValidator()
    {
        RuleFor(request => request.Reason)
            .MaximumLength(500).WithMessage("ສາເຫດຕ້ອງມີຄວາມຍາວບໍ່ເກີນ 500 ໂຕອັກສອນ");
    }
}
