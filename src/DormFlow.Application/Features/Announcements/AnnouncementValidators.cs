using DormFlow.Contracts.Announcements;
using FluentValidation;

namespace DormFlow.Application.Features.Announcements;

/// <summary>ທົບທວນຄືນການຮ້ອງຂໍເພື່ອສ້າງການປະກາດໃຝ່</summary>
public sealed class CreateAnnouncementRequestValidator : AbstractValidator<CreateAnnouncementRequest>
{
    /// <summary>ສ້າງຕົວກວດສອບ</summary>
    public CreateAnnouncementRequestValidator()
    {
        RuleFor(request => request.DormitoryId)
            .NotEmpty().WithMessage("ກະລຸນາລະບຸຫໍພັກທີ່ປະກາດນີ້ກ່ຽວຂ້ອງ");

        RuleFor(request => request.Title)
            .NotEmpty().WithMessage("ກະລຸນາລະບຸຫົວຂໍ້ປະກາດ")
            .MaximumLength(200).WithMessage("ຫົວຂໍ້ປະກາດຕ້ອງມີຄວາມຍາວບໍ່ເກີນ 200 ໂຕອັກສອນ");

        RuleFor(request => request.Content)
            .NotEmpty().WithMessage("ກະລຸນາລະບຸເນື້ອຫາປະກາດ")
            .MaximumLength(5000).WithMessage("ເນື້ອຫາປະກາດຕ້ອງມີຄວາມຍາວບໍ່ເກີນ 5000 ໂຕອັກສອນ");

        RuleFor(request => request.Notes)
            .MaximumLength(1000).WithMessage("ໝາຍເຫດຕ້ອງມີຄວາມຍາວບໍ່ເກີນ 1000 ໂຕອັກສອນ");
    }
}
