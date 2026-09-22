using DormFlow.Contracts.RoomTypes;
using FluentValidation;

namespace DormFlow.Application.Features.RoomTypes;

/// <summary>ທົບທວນຄືນການຮ້ອງຂໍເພື່ອສ້າງປະເພດຫ້ອງໃຝ່</summary>
public sealed class CreateRoomTypeRequestValidator : AbstractValidator<CreateRoomTypeRequest>
{
    /// <summary>ສ້າງຕົວກວດສອບ</summary>
    public CreateRoomTypeRequestValidator()
    {
        RuleFor(request => request.DormitoryId)
            .NotEmpty().WithMessage("ກະລຸນາລະບຸຫໍພັກທີ່ປະເພດຫ້ອງນີ້ສັງກັດ");

        RuleFor(request => request.Code)
            .NotEmpty().WithMessage("ກະລຸນາລະບຸລະຫັດປະເພດຫ້ອງ")
            .MaximumLength(50).WithMessage("ລະຫັດປະເພດຫ້ອງຕ້ອງມີຄວາມຍາວບໍ່ເກີນ 50 ໂຕອັກສອນ");

        RuleFor(request => request.Name)
            .NotEmpty().WithMessage("ກະລຸນາລະບຸຊື່ປະເພດຫ້ອງ")
            .MaximumLength(200).WithMessage("ຊື່ປະເພດຫ້ອງຕ້ອງມີຄວາມຍາວບໍ່ເກີນ 200 ໂຕອັກສອນ");

        RuleFor(request => request.MonthlyRent)
            .GreaterThanOrEqualTo(0).WithMessage("ຄ່າເຊົ່າລາຍເດືອນຕ້ອງບໍ່ຕິດລົບ");

        RuleFor(request => request.DailyRent)
            .GreaterThanOrEqualTo(0).WithMessage("ຄ່າເຊົ່າລາຍວັນຕ້ອງບໍ່ຕິດລົບ")
            .When(request => request.DailyRent.HasValue);

        RuleFor(request => request.DepositAmount)
            .GreaterThanOrEqualTo(0).WithMessage("ຄ່າມັດຈຳຕ້ອງບໍ່ຕິດລົບ");

        RuleFor(request => request.MaximumOccupants)
            .GreaterThan(0).WithMessage("ຈຳນວນຜູ້ເຂົ້າພັກສູງສຸດຕ້ອງຫຼາຍກວ່າ 0");
    }
}

/// <summary>ກວດເບິ່ງຄຳຮ້ອງຂໍແກ້ໄຂຂໍ້ມູນປະເພດຫ້ອງ</summary>
public sealed class UpdateRoomTypeRequestValidator : AbstractValidator<UpdateRoomTypeRequest>
{
    /// <summary>ສ້າງຕົວກວດສອບ</summary>
    public UpdateRoomTypeRequestValidator()
    {
        RuleFor(request => request.Code)
            .NotEmpty().WithMessage("ກະລຸນາລະບຸລະຫັດປະເພດຫ້ອງ")
            .MaximumLength(50).WithMessage("ລະຫັດປະເພດຫ້ອງຕ້ອງມີຄວາມຍາວບໍ່ເກີນ 50 ໂຕອັກສອນ");

        RuleFor(request => request.Name)
            .NotEmpty().WithMessage("ກະລຸນາລະບຸຊື່ປະເພດຫ້ອງ")
            .MaximumLength(200).WithMessage("ຊື່ປະເພດຫ້ອງຕ້ອງມີຄວາມຍາວບໍ່ເກີນ 200 ໂຕອັກສອນ");

        RuleFor(request => request.MonthlyRent)
            .GreaterThanOrEqualTo(0).WithMessage("ຄ່າເຊົ່າລາຍເດືອນຕ້ອງບໍ່ຕິດລົບ");

        RuleFor(request => request.DailyRent)
            .GreaterThanOrEqualTo(0).WithMessage("ຄ່າເຊົ່າລາຍວັນຕ້ອງບໍ່ຕິດລົບ")
            .When(request => request.DailyRent.HasValue);

        RuleFor(request => request.DepositAmount)
            .GreaterThanOrEqualTo(0).WithMessage("ຄ່າມັດຈຳຕ້ອງບໍ່ຕິດລົບ");

        RuleFor(request => request.MaximumOccupants)
            .GreaterThan(0).WithMessage("ຈຳນວນຜູ້ເຂົ້າພັກສູງສຸດຕ້ອງຫຼາຍກວ່າ 0");
    }
}
