using DormFlow.Contracts.Parking;
using DormFlow.Domain.Enums;
using FluentValidation;

namespace DormFlow.Application.Features.Parking;

/// <summary>ກວດເບິ່ງການຮ້ອງຂໍການລົງທະບຽນບ່ອນຈອດລົດ</summary>
public sealed class RegisterParkingRequestValidator : AbstractValidator<RegisterParkingRequest>
{
    /// <summary>ສ້າງຕົວກວດສອບ</summary>
    public RegisterParkingRequestValidator()
    {
        RuleFor(request => request.RoomId)
            .NotEmpty().WithMessage("ກະລຸນາລະບຸຫ້ອງຂອງຜູ້ເຊົ່າເຈົ້າຂອງລົດ");

        RuleFor(request => request.TenantId)
            .NotEmpty().WithMessage("ກະລຸນາລະບຸຜູ້ເຊົ່າເຈົ້າຂອງລົດ");

        RuleFor(request => request.LicensePlate)
            .NotEmpty().WithMessage("ກະລຸນາລະບຸປ້າຍທະບຽນລົດ")
            .MaximumLength(20).WithMessage("ປ້າຍທະບຽນລົດຕ້ອງມີຄວາມຍາວບໍ່ເກີນ 20 ໂຕອັກສອນ");

        RuleFor(request => request.VehicleType)
            .Must(value => Enum.TryParse<VehicleType>(value, ignoreCase: true, out _))
            .WithMessage("ປະເພດພາຫະນະບໍ່ຖືກຕ້ອງ ຕ້ອງເປັນ Car, Motorcycle ຫຼື Other");

        RuleFor(request => request.Brand)
            .MaximumLength(100).WithMessage("ຍີ່ຫໍ້/ຮຸ່ນລົດຕ້ອງມີຄວາມຍາວບໍ່ເກີນ 100 ໂຕອັກສອນ");

        RuleFor(request => request.Color)
            .MaximumLength(50).WithMessage("ສີລົດຕ້ອງມີຄວາມຍາວບໍ່ເກີນ 50 ໂຕອັກສອນ");

        RuleFor(request => request.ParkingSpotLabel)
            .MaximumLength(50).WithMessage("ປ້າຍຊ່ອງຈອດລົດຕ້ອງມີຄວາມຍາວບໍ່ເກີນ 50 ໂຕອັກສອນ");

        RuleFor(request => request.Notes)
            .MaximumLength(1000).WithMessage("ໝາຍເຫດຕ້ອງມີຄວາມຍາວບໍ່ເກີນ 1000 ໂຕອັກສອນ");
    }
}
