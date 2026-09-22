using DormFlow.Contracts.Visitors;
using FluentValidation;

namespace DormFlow.Application.Features.Visitors;

/// <summary>ກວດສອບການຮ້ອງຂໍບັນທຶກຂອງແຂກ</summary>
public sealed class CheckInVisitorRequestValidator : AbstractValidator<CheckInVisitorRequest>
{
    /// <summary>ສ້າງຕົວກວດສອບ</summary>
    public CheckInVisitorRequestValidator()
    {
        RuleFor(request => request.RoomId)
            .NotEmpty().WithMessage("ກະລຸນາລະບຸຫ້ອງທີ່ຜູ້ມາຢ້ຽມຢາມມາຢ້ຽມ");

        RuleFor(request => request.VisitorName)
            .NotEmpty().WithMessage("ກະລຸນາລະບຸຊື່ຜູ້ມາຢ້ຽມຢາມ")
            .MaximumLength(200).WithMessage("ຊື່ຜູ້ມາຢ້ຽມຢາມຕ້ອງມີຄວາມຍາວບໍ່ເກີນ 200 ໂຕອັກສອນ");

        RuleFor(request => request.VisitorPhone)
            .MaximumLength(30).WithMessage("ເບີໂທລະສັບຕ້ອງມີຄວາມຍາວບໍ່ເກີນ 30 ໂຕອັກສອນ");

        RuleFor(request => request.IdNumber)
            .MaximumLength(20).WithMessage("ເລກບັດປະຈຳຕົວຕ້ອງມີຄວາມຍາວບໍ່ເກີນ 20 ໂຕອັກສອນ");

        RuleFor(request => request.Purpose)
            .MaximumLength(500).WithMessage("ຈຸດປະສົງຕ້ອງມີຄວາມຍາວບໍ່ເກີນ 500 ໂຕອັກສອນ");

        RuleFor(request => request.Notes)
            .MaximumLength(1000).WithMessage("ໝາຍເຫດຕ້ອງມີຄວາມຍາວບໍ່ເກີນ 1000 ໂຕອັກສອນ");
    }
}
