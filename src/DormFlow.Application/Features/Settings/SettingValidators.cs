using DormFlow.Contracts.Settings;
using FluentValidation;

namespace DormFlow.Application.Features.Settings;

/// <summary>ກວດສອບການຮ້ອງຂໍເພື່ອສ້າງ ຫຼືແກ້ໄຂການຕັ້ງຄ່າ</summary>
public sealed class UpsertSettingRequestValidator : AbstractValidator<UpsertSettingRequest>
{
    /// <summary>ສ້າງຕົວກວດສອບ</summary>
    public UpsertSettingRequestValidator()
    {
        RuleFor(request => request.Value)
            .NotEmpty().WithMessage("ກະລຸນາລະບຸຄ່າຂອງການຕັ້ງຄ່າ")
            .MaximumLength(2000).WithMessage("ຄ່າຂອງການຕັ້ງຄ່າຕ້ອງມີຄວາມຍາວບໍ່ເກີນ 2000 ໂຕອັກສອນ");
    }
}
