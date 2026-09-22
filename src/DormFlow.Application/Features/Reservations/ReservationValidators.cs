using DormFlow.Application.Common.Abstractions;
using DormFlow.Contracts.Reservations;
using FluentValidation;

namespace DormFlow.Application.Features.Reservations;

/// <summary>ກວດເບິ່ງຄໍາຮ້ອງຂໍການຈອງຂອງທ່ານ</summary>
public sealed class CreateReservationRequestValidator : AbstractValidator<CreateReservationRequest>
{
    /// <summary>ສ້າງຕົວກວດສອບ</summary>
    public CreateReservationRequestValidator(IDateTimeProvider dateTimeProvider)
    {
        ArgumentNullException.ThrowIfNull(dateTimeProvider);

        RuleFor(request => request.RoomId)
            .NotEmpty().WithMessage("ກະລຸນາລະບຸຫ້ອງທີ່ຕ້ອງການຈອງ");

        RuleFor(request => request.TenantId)
            .NotEmpty().WithMessage("ກະລຸນາລະບຸຜູ້ເຊົ່າທີ່ຈອງ");

        RuleFor(request => request.MoveInDate)
            .GreaterThanOrEqualTo(_ => DateOnly.FromDateTime(dateTimeProvider.UtcNow.UtcDateTime))
            .WithMessage("ວັນທີຄາດວ່າຈະເຂົ້າພັກຕ້ອງບໍ່ແມ່ນວັນທີທີ່ຜ່ານມາແລ້ວ");

        RuleFor(request => request.HoldDays)
            .InclusiveBetween(1, 30).WithMessage("ຈຳນວນວັນທີ່ກັນຫ້ອງຕ້ອງຢູ່ລະຫວ່າງ 1 ຫາ 30 ວັນ");

        RuleFor(request => request.ReservationFee)
            .GreaterThanOrEqualTo(0).WithMessage("ຄ່າຈອງຫ້ອງຕ້ອງບໍ່ຕິດລົບ");

        RuleFor(request => request.Notes)
            .MaximumLength(1000).WithMessage("ໝາຍເຫດຕ້ອງມີຄວາມຍາວບໍ່ເກີນ 1000 ໂຕອັກສອນ");
    }
}
