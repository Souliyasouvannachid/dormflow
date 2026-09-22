using DormFlow.Contracts.MeterReadings;
using DormFlow.Application.Common.Abstractions;
using DormFlow.Domain.Enums;
using FluentValidation;

namespace DormFlow.Application.Features.MeterReadings;

/// <summary>ກວດສອບການຮ້ອງຂໍເພື່ອບັນທຶກຄ່າແມັດ</summary>
public sealed class CreateMeterReadingRequestValidator : AbstractValidator<CreateMeterReadingRequest>
{
    /// <summary>ສ້າງຕົວກວດສອບ</summary>
    public CreateMeterReadingRequestValidator(IDateTimeProvider dateTimeProvider)
    {
        ArgumentNullException.ThrowIfNull(dateTimeProvider);

        RuleFor(request => request.RoomId)
            .NotEmpty().WithMessage("ກະລຸນາລະບຸຫ້ອງທີ່ຈະຈົດຄ່າມິເຕີ");

        RuleFor(request => request.MeterType)
            .Must(value => Enum.TryParse<MeterType>(value, ignoreCase: true, out _))
            .WithMessage("ປະເພດມິເຕີບໍ່ຖືກຕ້ອງ ຕ້ອງເປັນ Water ຫຼື Electricity");

        RuleFor(request => request.ReadingDate)
            .LessThanOrEqualTo(_ => DateOnly.FromDateTime(dateTimeProvider.UtcNow.UtcDateTime))
            .WithMessage("ວັນທີຈົດມິເຕີຕ້ອງບໍ່ແມ່ນວັນທີໃນອະນາຄົດ");

        RuleFor(request => request.CurrentReading)
            .GreaterThanOrEqualTo(0).WithMessage("ຄ່າມິເຕີຕ້ອງບໍ່ຕິດລົບ");

        RuleFor(request => request.Notes)
            .MaximumLength(1000).WithMessage("ໝາຍເຫດຕ້ອງມີຄວາມຍາວບໍ່ເກີນ 1000 ໂຕອັກສອນ");
    }
}
