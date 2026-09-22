using DormFlow.Contracts.Floors;
using FluentValidation;

namespace DormFlow.Application.Features.Floors;

/// <summary>ທົບທວນຄືນການຮ້ອງຂໍເພື່ອສ້າງຊັ້ນໃຝ່</summary>
public sealed class CreateFloorRequestValidator : AbstractValidator<CreateFloorRequest>
{
    /// <summary>ສ້າງຕົວກວດສອບ</summary>
    public CreateFloorRequestValidator()
    {
        RuleFor(request => request.BuildingId)
            .NotEmpty().WithMessage("ກະລຸນາລະບຸອາຄານທີ່ຊັ້ນນີ້ສັງກັດ");

        RuleFor(request => request.FloorNumber)
            .InclusiveBetween(-5, 200).WithMessage("ລຳດັບຊັ້ນບໍ່ຖືກຕ້ອງ");

        RuleFor(request => request.Name)
            .MaximumLength(100).WithMessage("ຊື່ຊັ້ນຕ້ອງມີຄວາມຍາວບໍ່ເກີນ 100 ໂຕອັກສອນ");
    }
}

/// <summary>ທົບທວນຄືນການຮ້ອງຂໍການແກ້ໄຂຂໍ້ມູນຊັ້ນ</summary>
public sealed class UpdateFloorRequestValidator : AbstractValidator<UpdateFloorRequest>
{
    /// <summary>ສ້າງຕົວກວດສອບ</summary>
    public UpdateFloorRequestValidator()
    {
        RuleFor(request => request.FloorNumber)
            .InclusiveBetween(-5, 200).WithMessage("ລຳດັບຊັ້ນບໍ່ຖືກຕ້ອງ");

        RuleFor(request => request.Name)
            .MaximumLength(100).WithMessage("ຊື່ຊັ້ນຕ້ອງມີຄວາມຍາວບໍ່ເກີນ 100 ໂຕອັກສອນ");
    }
}
