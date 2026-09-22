using DormFlow.Contracts.Buildings;
using FluentValidation;

namespace DormFlow.Application.Features.Buildings;

/// <summary>ທົບທວນຄືນການຮ້ອງຂໍສໍາລັບອາຄານໃຝ່</summary>
public sealed class CreateBuildingRequestValidator : AbstractValidator<CreateBuildingRequest>
{
    /// <summary>ສ້າງຕົວກວດສອບ</summary>
    public CreateBuildingRequestValidator()
    {
        RuleFor(request => request.DormitoryId)
            .NotEmpty().WithMessage("ກະລຸນາລະບຸຫໍພັກທີ່ອາຄານນີ້ສັງກັດ");

        RuleFor(request => request.Code)
            .NotEmpty().WithMessage("ກະລຸນາລະບຸລະຫັດອາຄານ")
            .MaximumLength(50).WithMessage("ລະຫັດອາຄານຕ້ອງມີຄວາມຍາວບໍ່ເກີນ 50 ໂຕອັກສອນ");

        RuleFor(request => request.Name)
            .NotEmpty().WithMessage("ກະລຸນາລະບຸຊື່ອາຄານ")
            .MaximumLength(200).WithMessage("ຊື່ອາຄານຕ້ອງມີຄວາມຍາວບໍ່ເກີນ 200 ໂຕອັກສອນ");

        RuleFor(request => request.TotalFloors)
            .GreaterThan(0).WithMessage("ຈຳນວນຊັ້ນຕ້ອງຫຼາຍກວ່າ 0");
    }
}

/// <summary>ກວດສອບການຮ້ອງຂໍການແກ້ໄຂຂໍ້ມູນອາຄານ</summary>
public sealed class UpdateBuildingRequestValidator : AbstractValidator<UpdateBuildingRequest>
{
    /// <summary>ສ້າງຕົວກວດສອບ</summary>
    public UpdateBuildingRequestValidator()
    {
        RuleFor(request => request.Code)
            .NotEmpty().WithMessage("ກະລຸນາລະບຸລະຫັດອາຄານ")
            .MaximumLength(50).WithMessage("ລະຫັດອາຄານຕ້ອງມີຄວາມຍາວບໍ່ເກີນ 50 ໂຕອັກສອນ");

        RuleFor(request => request.Name)
            .NotEmpty().WithMessage("ກະລຸນາລະບຸຊື່ອາຄານ")
            .MaximumLength(200).WithMessage("ຊື່ອາຄານຕ້ອງມີຄວາມຍາວບໍ່ເກີນ 200 ໂຕອັກສອນ");

        RuleFor(request => request.TotalFloors)
            .GreaterThan(0).WithMessage("ຈຳນວນຊັ້ນຕ້ອງຫຼາຍກວ່າ 0");
    }
}
