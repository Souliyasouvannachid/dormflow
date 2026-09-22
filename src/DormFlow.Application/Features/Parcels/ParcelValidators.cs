using DormFlow.Contracts.Parcels;
using FluentValidation;

namespace DormFlow.Application.Features.Parcels;

/// <summary>ທົບທວນຄືນການຮ້ອງຂໍເພື່ອບັນທຶກ parcels ມາຮອດ</summary>
public sealed class LogParcelRequestValidator : AbstractValidator<LogParcelRequest>
{
    /// <summary>ສ້າງຕົວກວດສອບ</summary>
    public LogParcelRequestValidator()
    {
        RuleFor(request => request.RoomId)
            .NotEmpty().WithMessage("ກະລຸນາລະບຸຫ້ອງທີ່ພັດສະດຸນີ້ສົ່ງເຖິງ");

        RuleFor(request => request.CourierName)
            .MaximumLength(100).WithMessage("ຊື່ບໍລິສັດຂົນສົ່ງຕ້ອງມີຄວາມຍາວບໍ່ເກີນ 100 ໂຕອັກສອນ");

        RuleFor(request => request.TrackingNumber)
            .MaximumLength(100).WithMessage("ເລກພັດສະດຸຕ້ອງມີຄວາມຍາວບໍ່ເກີນ 100 ໂຕອັກສອນ");

        RuleFor(request => request.Description)
            .MaximumLength(500).WithMessage("ຄຳອະທິບາຍຕ້ອງມີຄວາມຍາວບໍ່ເກີນ 500 ໂຕອັກສອນ");

        RuleFor(request => request.Notes)
            .MaximumLength(1000).WithMessage("ໝາຍເຫດຕ້ອງມີຄວາມຍາວບໍ່ເກີນ 1000 ໂຕອັກສອນ");
    }
}

/// <summary>ກວດສອບການຮ້ອງຂໍເພື່ອບັນທຶກການຮັບຂອງ parcel ໄດ້</summary>
public sealed class PickupParcelRequestValidator : AbstractValidator<PickupParcelRequest>
{
    /// <summary>ສ້າງຕົວກວດສອບ</summary>
    public PickupParcelRequestValidator()
    {
        RuleFor(request => request.PickedUpByName)
            .MaximumLength(200).WithMessage("ຊື່ຜູ້ຮັບພັດສະດຸຕ້ອງມີຄວາມຍາວບໍ່ເກີນ 200 ໂຕອັກສອນ");
    }
}
