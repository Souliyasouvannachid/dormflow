using DormFlow.Contracts.Dormitories;
using DormFlow.Domain.Enums;
using FluentValidation;

namespace DormFlow.Application.Features.Dormitories;

/// <summary>ທົບທວນຄືນການຮ້ອງຂໍສໍາລັບຫໍພັກໃຝ່</summary>
public sealed class CreateDormitoryRequestValidator : AbstractValidator<CreateDormitoryRequest>
{
    /// <summary>ສ້າງຕົວກວດສອບ</summary>
    public CreateDormitoryRequestValidator()
    {
        RuleFor(request => request.Code)
            .NotEmpty().WithMessage("ກະລຸນາລະບຸລະຫັດຫໍພັກ")
            .MaximumLength(50).WithMessage("ລະຫັດຫໍພັກຕ້ອງມີຄວາມຍາວບໍ່ເກີນ 50 ໂຕອັກສອນ");

        RuleFor(request => request.Name)
            .NotEmpty().WithMessage("ກະລຸນາລະບຸຊື່ຫໍພັກ")
            .MaximumLength(200).WithMessage("ຊື່ຫໍພັກຕ້ອງມີຄວາມຍາວບໍ່ເກີນ 200 ໂຕອັກສອນ");

        RuleFor(request => request.Email)
            .EmailAddress().WithMessage("ຮູບແບບອີເມວບໍ່ຖືກຕ້ອງ")
            .When(request => !string.IsNullOrWhiteSpace(request.Email));

        RuleFor(request => request.BillingDay)
            .InclusiveBetween(1, 28).WithMessage("ວັນທີອອກໃບແຈ້ງໜີ້ຕ້ອງຢູ່ລະຫວ່າງ 1 ຫາ 28");

        RuleFor(request => request.PaymentDueDay)
            .InclusiveBetween(1, 31).WithMessage("ວັນທີຄົບກຳນົດຊຳລະຕ້ອງຢູ່ລະຫວ່າງ 1 ຫາ 31");

        RuleFor(request => request.LateFeeType)
            .Must(value => Enum.TryParse<LateFeeType>(value, ignoreCase: true, out _))
            .WithMessage("ຮູບແບບຄ່າປັບຊຳລະລ່າຊ້າບໍ່ຖືກຕ້ອງ ຕ້ອງເປັນ Fixed ຫຼື Percentage");

        RuleFor(request => request.LateFeeAmount)
            .GreaterThanOrEqualTo(0).WithMessage("ຄ່າປັບຊຳລະລ່າຊ້າຕ້ອງບໍ່ຕິດລົບ");

        RuleFor(request => request.WaterRatePerUnit)
            .GreaterThanOrEqualTo(0).WithMessage("ອັດຕາຄ່ານ້ຳຕໍ່ໜ່ວຍຕ້ອງບໍ່ຕິດລົບ");

        RuleFor(request => request.ElectricityRatePerUnit)
            .GreaterThanOrEqualTo(0).WithMessage("ອັດຕາຄ່າໄຟຕໍ່ໜ່ວຍຕ້ອງບໍ່ຕິດລົບ");
    }
}

/// <summary>ກວດສອບການຮ້ອງຂໍການແກ້ໄຂຂໍ້ມູນຫໍພັກ</summary>
public sealed class UpdateDormitoryRequestValidator : AbstractValidator<UpdateDormitoryRequest>
{
    /// <summary>ສ້າງຕົວກວດສອບ</summary>
    public UpdateDormitoryRequestValidator()
    {
        RuleFor(request => request.Code)
            .NotEmpty().WithMessage("ກະລຸນາລະບຸລະຫັດຫໍພັກ")
            .MaximumLength(50).WithMessage("ລະຫັດຫໍພັກຕ້ອງມີຄວາມຍາວບໍ່ເກີນ 50 ໂຕອັກສອນ");

        RuleFor(request => request.Name)
            .NotEmpty().WithMessage("ກະລຸນາລະບຸຊື່ຫໍພັກ")
            .MaximumLength(200).WithMessage("ຊື່ຫໍພັກຕ້ອງມີຄວາມຍາວບໍ່ເກີນ 200 ໂຕອັກສອນ");

        RuleFor(request => request.Email)
            .EmailAddress().WithMessage("ຮູບແບບອີເມວບໍ່ຖືກຕ້ອງ")
            .When(request => !string.IsNullOrWhiteSpace(request.Email));

        RuleFor(request => request.BillingDay)
            .InclusiveBetween(1, 28).WithMessage("ວັນທີອອກໃບແຈ້ງໜີ້ຕ້ອງຢູ່ລະຫວ່າງ 1 ຫາ 28");

        RuleFor(request => request.PaymentDueDay)
            .InclusiveBetween(1, 31).WithMessage("ວັນທີຄົບກຳນົດຊຳລະຕ້ອງຢູ່ລະຫວ່າງ 1 ຫາ 31");

        RuleFor(request => request.LateFeeType)
            .Must(value => Enum.TryParse<LateFeeType>(value, ignoreCase: true, out _))
            .WithMessage("ຮູບແບບຄ່າປັບຊຳລະລ່າຊ້າບໍ່ຖືກຕ້ອງ ຕ້ອງເປັນ Fixed ຫຼື Percentage");

        RuleFor(request => request.LateFeeAmount)
            .GreaterThanOrEqualTo(0).WithMessage("ຄ່າປັບຊຳລະລ່າຊ້າຕ້ອງບໍ່ຕິດລົບ");

        RuleFor(request => request.WaterRatePerUnit)
            .GreaterThanOrEqualTo(0).WithMessage("ອັດຕາຄ່ານ້ຳຕໍ່ໜ່ວຍຕ້ອງບໍ່ຕິດລົບ");

        RuleFor(request => request.ElectricityRatePerUnit)
            .GreaterThanOrEqualTo(0).WithMessage("ອັດຕາຄ່າໄຟຕໍ່ໜ່ວຍຕ້ອງບໍ່ຕິດລົບ");
    }
}
