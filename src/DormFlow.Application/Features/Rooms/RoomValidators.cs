using DormFlow.Contracts.Rooms;
using FluentValidation;

namespace DormFlow.Application.Features.Rooms;

/// <summary>ກວດສອບການຮ້ອງຂໍເພື່ອສ້າງຫ້ອງໃຝ່</summary>
public sealed class CreateRoomRequestValidator : AbstractValidator<CreateRoomRequest>
{
    /// <summary>ສ້າງຕົວກວດສອບ</summary>
    public CreateRoomRequestValidator()
    {
        RuleFor(request => request.BuildingId)
            .NotEmpty().WithMessage("ກະລຸນາລະບຸອາຄານທີ່ຫ້ອງນີ້ສັງກັດ");

        RuleFor(request => request.FloorId)
            .NotEmpty().WithMessage("ກະລຸນາລະບຸຊັ້ນທີ່ຫ້ອງນີ້ສັງກັດ");

        RuleFor(request => request.RoomTypeId)
            .NotEmpty().WithMessage("ກະລຸນາລະບຸປະເພດຂອງຫ້ອງ");

        RuleFor(request => request.RoomNumber)
            .NotEmpty().WithMessage("ກະລຸນາລະບຸເລກຫ້ອງ")
            .MaximumLength(20).WithMessage("ເລກຫ້ອງຕ້ອງມີຄວາມຍາວບໍ່ເກີນ 20 ໂຕອັກສອນ");

        RuleFor(request => request.MonthlyRent)
            .GreaterThanOrEqualTo(0).WithMessage("ຄ່າເຊົ່າລາຍເດືອນຕ້ອງບໍ່ຕິດລົບ");

        RuleFor(request => request.DepositAmount)
            .GreaterThanOrEqualTo(0).WithMessage("ຄ່າມັດຈຳຕ້ອງບໍ່ຕິດລົບ");

        RuleFor(request => request.MaximumOccupants)
            .GreaterThan(0).WithMessage("ຈຳນວນຜູ້ເຂົ້າພັກສູງສຸດຕ້ອງຫຼາຍກວ່າ 0");

        RuleFor(request => request.FloorPlanX)
            .InclusiveBetween(0, 100).When(request => request.FloorPlanX.HasValue)
            .WithMessage("ຕຳແໜ່ງ X ຕ້ອງຢູ່ລະຫວ່າງ 0 ຫາ 100");

        RuleFor(request => request.FloorPlanY)
            .InclusiveBetween(0, 100).When(request => request.FloorPlanY.HasValue)
            .WithMessage("ຕຳແໜ່ງ Y ຕ້ອງຢູ່ລະຫວ່າງ 0 ຫາ 100");

        RuleFor(request => request.FloorPlanWidth)
            .InclusiveBetween(0, 100).When(request => request.FloorPlanWidth.HasValue)
            .WithMessage("ຄວາມກວ້າງຕ້ອງຢູ່ລະຫວ່າງ 0 ຫາ 100");

        RuleFor(request => request.FloorPlanHeight)
            .InclusiveBetween(0, 100).When(request => request.FloorPlanHeight.HasValue)
            .WithMessage("ຄວາມສູງຕ້ອງຢູ່ລະຫວ່າງ 0 ຫາ 100");
    }
}

/// <summary>ກວດສອບການຮ້ອງຂໍແກ້ໄຂຂໍ້ມູນຫ້ອງ</summary>
public sealed class UpdateRoomRequestValidator : AbstractValidator<UpdateRoomRequest>
{
    /// <summary>ສ້າງຕົວກວດສອບ</summary>
    public UpdateRoomRequestValidator()
    {
        RuleFor(request => request.FloorId)
            .NotEmpty().WithMessage("ກະລຸນາລະບຸຊັ້ນທີ່ຫ້ອງນີ້ສັງກັດ");

        RuleFor(request => request.RoomTypeId)
            .NotEmpty().WithMessage("ກະລຸນາລະບຸປະເພດຂອງຫ້ອງ");

        RuleFor(request => request.RoomNumber)
            .NotEmpty().WithMessage("ກະລຸນາລະບຸເລກຫ້ອງ")
            .MaximumLength(20).WithMessage("ເລກຫ້ອງຕ້ອງມີຄວາມຍາວບໍ່ເກີນ 20 ໂຕອັກສອນ");

        RuleFor(request => request.MonthlyRent)
            .GreaterThanOrEqualTo(0).WithMessage("ຄ່າເຊົ່າລາຍເດືອນຕ້ອງບໍ່ຕິດລົບ");

        RuleFor(request => request.DepositAmount)
            .GreaterThanOrEqualTo(0).WithMessage("ຄ່າມັດຈຳຕ້ອງບໍ່ຕິດລົບ");

        RuleFor(request => request.MaximumOccupants)
            .GreaterThan(0).WithMessage("ຈຳນວນຜູ້ເຂົ້າພັກສູງສຸດຕ້ອງຫຼາຍກວ່າ 0");

        RuleFor(request => request.FloorPlanX)
            .InclusiveBetween(0, 100).When(request => request.FloorPlanX.HasValue)
            .WithMessage("ຕຳແໜ່ງ X ຕ້ອງຢູ່ລະຫວ່າງ 0 ຫາ 100");

        RuleFor(request => request.FloorPlanY)
            .InclusiveBetween(0, 100).When(request => request.FloorPlanY.HasValue)
            .WithMessage("ຕຳແໜ່ງ Y ຕ້ອງຢູ່ລະຫວ່າງ 0 ຫາ 100");

        RuleFor(request => request.FloorPlanWidth)
            .InclusiveBetween(0, 100).When(request => request.FloorPlanWidth.HasValue)
            .WithMessage("ຄວາມກວ້າງຕ້ອງຢູ່ລະຫວ່າງ 0 ຫາ 100");

        RuleFor(request => request.FloorPlanHeight)
            .InclusiveBetween(0, 100).When(request => request.FloorPlanHeight.HasValue)
            .WithMessage("ຄວາມສູງຕ້ອງຢູ່ລະຫວ່າງ 0 ຫາ 100");
    }
}
