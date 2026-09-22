using DormFlow.Contracts.Notifications;
using FluentValidation;

namespace DormFlow.Application.Features.Notifications;

/// <summary>ກວດສອບການຮ້ອງຂໍເພື່ອສົ່ງການແຈ້ງເຕືອນ</summary>
public sealed class SendNotificationRequestValidator : AbstractValidator<SendNotificationRequest>
{
    /// <summary>ສ້າງຕົວກວດສອບ</summary>
    public SendNotificationRequestValidator()
    {
        RuleFor(request => request.RecipientUserId)
            .NotEmpty().WithMessage("ກະລຸນາລະບຸຜູ້ຮັບການແຈ້ງເຕືອນ");

        RuleFor(request => request.Title)
            .NotEmpty().WithMessage("ກະລຸນາລະບຸຫົວຂໍ້ການແຈ້ງເຕືອນ")
            .MaximumLength(200).WithMessage("ຫົວຂໍ້ການແຈ້ງເຕືອນຕ້ອງມີຄວາມຍາວບໍ່ເກີນ 200 ໂຕອັກສອນ");

        RuleFor(request => request.Message)
            .NotEmpty().WithMessage("ກະລຸນາລະບຸເນື້ອຫາການແຈ້ງເຕືອນ")
            .MaximumLength(2000).WithMessage("ເນື້ອຫາການແຈ້ງເຕືອນຕ້ອງມີຄວາມຍາວບໍ່ເກີນ 2000 ໂຕອັກສອນ");
    }
}
