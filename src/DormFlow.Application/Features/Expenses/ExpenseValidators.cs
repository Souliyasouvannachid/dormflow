using DormFlow.Contracts.Expenses;
using DormFlow.Domain.Enums;
using FluentValidation;

namespace DormFlow.Application.Features.Expenses;

/// <summary>ທົບທວນຄືນຄໍາຮ້ອງຂໍບັນທຶກຄ່າໃຊ້ຈ່າຍ</summary>
public sealed class CreateExpenseRequestValidator : AbstractValidator<CreateExpenseRequest>
{
    /// <summary>ສ້າງຕົວກວດສອບ</summary>
    public CreateExpenseRequestValidator()
    {
        RuleFor(request => request.DormitoryId)
            .NotEmpty().WithMessage("ກະລຸນາລະບຸຫໍພັກທີ່ກ່ຽວຂ້ອງກັບຄ່າໃຊ້ຈ່າຍນີ້");

        RuleFor(request => request.Category)
            .Must(value => Enum.TryParse<ExpenseCategory>(value, ignoreCase: true, out _))
            .WithMessage("ໝວດໝູ່ຄ່າໃຊ້ຈ່າຍບໍ່ຖືກຕ້ອງ");

        RuleFor(request => request.Amount)
            .GreaterThan(0).WithMessage("ຈຳນວນເງິນຕ້ອງຫຼາຍກວ່າສູນ");

        RuleFor(request => request.Description)
            .NotEmpty().WithMessage("ກະລຸນາລະບຸຄຳອະທິບາຍລາຍການ")
            .MaximumLength(500).WithMessage("ຄຳອະທິບາຍຕ້ອງມີຄວາມຍາວບໍ່ເກີນ 500 ໂຕອັກສອນ");

        RuleFor(request => request.PaidTo)
            .MaximumLength(200).WithMessage("ຊື່ຜູ້ຮັບເງິນຕ້ອງມີຄວາມຍາວບໍ່ເກີນ 200 ໂຕອັກສອນ");

        RuleFor(request => request.Notes)
            .MaximumLength(1000).WithMessage("ໝາຍເຫດຕ້ອງມີຄວາມຍາວບໍ່ເກີນ 1000 ໂຕອັກສອນ");
    }
}
