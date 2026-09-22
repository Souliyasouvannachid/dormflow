using DormFlow.Contracts.Organizations;
using FluentValidation;

namespace DormFlow.Application.Features.Organizations;

/// <summary>ທົບທວນຄືນການຮ້ອງຂໍເພື່ອສ້າງອົງກອນໃຝ່</summary>
public sealed class CreateOrganizationRequestValidator : AbstractValidator<CreateOrganizationRequest>
{
    /// <summary>ສ້າງຕົວກວດສອບ</summary>
    public CreateOrganizationRequestValidator()
    {
        RuleFor(request => request.Code)
            .NotEmpty().WithMessage("ກະລຸນາລະບຸລະຫັດອົງກອນ")
            .MaximumLength(50).WithMessage("ລະຫັດອົງກອນຕ້ອງມີຄວາມຍາວບໍ່ເກີນ 50 ໂຕອັກສອນ");

        RuleFor(request => request.Name)
            .NotEmpty().WithMessage("ກະລຸນາລະບຸຊື່ອົງກອນ")
            .MaximumLength(200).WithMessage("ຊື່ອົງກອນຕ້ອງມີຄວາມຍາວບໍ່ເກີນ 200 ໂຕອັກສອນ");

        RuleFor(request => request.Email)
            .EmailAddress().WithMessage("ຮູບແບບອີເມວບໍ່ຖືກຕ້ອງ")
            .When(request => !string.IsNullOrWhiteSpace(request.Email));

        RuleFor(request => request.TaxId)
            .MaximumLength(20).WithMessage("ເລກປະຈຳຕົວຜູ້ເສຍພາສີຕ້ອງມີຄວາມຍາວບໍ່ເກີນ 20 ໂຕອັກສອນ");
    }
}

/// <summary>ທົບທວນຄືນການຮ້ອງຂໍແກ້ໄຂຂໍ້ມູນອົງກອນ</summary>
public sealed class UpdateOrganizationRequestValidator : AbstractValidator<UpdateOrganizationRequest>
{
    /// <summary>ສ້າງຕົວກວດສອບ</summary>
    public UpdateOrganizationRequestValidator()
    {
        RuleFor(request => request.Code)
            .NotEmpty().WithMessage("ກະລຸນາລະບຸລະຫັດອົງກອນ")
            .MaximumLength(50).WithMessage("ລະຫັດອົງກອນຕ້ອງມີຄວາມຍາວບໍ່ເກີນ 50 ໂຕອັກສອນ");

        RuleFor(request => request.Name)
            .NotEmpty().WithMessage("ກະລຸນາລະບຸຊື່ອົງກອນ")
            .MaximumLength(200).WithMessage("ຊື່ອົງກອນຕ້ອງມີຄວາມຍາວບໍ່ເກີນ 200 ໂຕອັກສອນ");

        RuleFor(request => request.Email)
            .EmailAddress().WithMessage("ຮູບແບບອີເມວບໍ່ຖືກຕ້ອງ")
            .When(request => !string.IsNullOrWhiteSpace(request.Email));

        RuleFor(request => request.TaxId)
            .MaximumLength(20).WithMessage("ເລກປະຈຳຕົວຜູ້ເສຍພາສີຕ້ອງມີຄວາມຍາວບໍ່ເກີນ 20 ໂຕອັກສອນ");
    }
}
