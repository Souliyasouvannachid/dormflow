using DormFlow.Application.Common.Abstractions;
using DormFlow.Contracts.Tenants;
using FluentValidation;

namespace DormFlow.Application.Features.Tenants;

/// <summary>ທົບທວນຄືນການຮ້ອງຂໍເພື່ອສ້າງຂໍ້ມູນຜູ້ເຊົ່າໃຝ່</summary>
public sealed class CreateTenantRequestValidator : AbstractValidator<CreateTenantRequest>
{
    /// <summary>ສ້າງຕົວກວດສອບ</summary>
    public CreateTenantRequestValidator(IDateTimeProvider dateTimeProvider)
    {
        ArgumentNullException.ThrowIfNull(dateTimeProvider);

        RuleFor(request => request.FirstName)
            .NotEmpty().WithMessage("ກະລຸນາລະບຸຊື່")
            .MaximumLength(100).WithMessage("ຊື່ຕ້ອງມີຄວາມຍາວບໍ່ເກີນ 100 ໂຕອັກສອນ");

        RuleFor(request => request.LastName)
            .NotEmpty().WithMessage("ກະລຸນາລະບຸນາມສະກຸນ")
            .MaximumLength(100).WithMessage("ນາມສະກຸນຕ້ອງມີຄວາມຍາວບໍ່ເກີນ 100 ໂຕອັກສອນ");

        RuleFor(request => request.Email)
            .EmailAddress().WithMessage("ຮູບແບບອີເມວບໍ່ຖືກຕ້ອງ")
            .When(request => !string.IsNullOrWhiteSpace(request.Email));

        RuleFor(request => request.NationalId)
            .MaximumLength(20).WithMessage("ເລກບັດປະຈຳຕົວຕ້ອງມີຄວາມຍາວບໍ່ເກີນ 20 ໂຕອັກສອນ");

        RuleFor(request => request.DateOfBirth!.Value)
            .LessThan(_ => DateOnly.FromDateTime(dateTimeProvider.UtcNow.UtcDateTime))
            .WithMessage("ວັນເດືອນປີເກີດຕ້ອງບໍ່ແມ່ນວັນທີໃນອະນາຄົດ")
            .When(request => request.DateOfBirth.HasValue);
    }
}

/// <summary>ທົບທວນຄືນການຮ້ອງຂໍແກ້ໄຂຂໍ້ມູນຜູ້ເຊົ່າ</summary>
public sealed class UpdateTenantRequestValidator : AbstractValidator<UpdateTenantRequest>
{
    /// <summary>ສ້າງຕົວກວດສອບ</summary>
    public UpdateTenantRequestValidator(IDateTimeProvider dateTimeProvider)
    {
        ArgumentNullException.ThrowIfNull(dateTimeProvider);

        RuleFor(request => request.FirstName)
            .NotEmpty().WithMessage("ກະລຸນາລະບຸຊື່")
            .MaximumLength(100).WithMessage("ຊື່ຕ້ອງມີຄວາມຍາວບໍ່ເກີນ 100 ໂຕອັກສອນ");

        RuleFor(request => request.LastName)
            .NotEmpty().WithMessage("ກະລຸນາລະບຸນາມສະກຸນ")
            .MaximumLength(100).WithMessage("ນາມສະກຸນຕ້ອງມີຄວາມຍາວບໍ່ເກີນ 100 ໂຕອັກສອນ");

        RuleFor(request => request.Email)
            .EmailAddress().WithMessage("ຮູບແບບອີເມວບໍ່ຖືກຕ້ອງ")
            .When(request => !string.IsNullOrWhiteSpace(request.Email));

        RuleFor(request => request.NationalId)
            .MaximumLength(20).WithMessage("ເລກບັດປະຈຳຕົວຕ້ອງມີຄວາມຍາວບໍ່ເກີນ 20 ໂຕອັກສອນ");

        RuleFor(request => request.DateOfBirth!.Value)
            .LessThan(_ => DateOnly.FromDateTime(dateTimeProvider.UtcNow.UtcDateTime))
            .WithMessage("ວັນເດືອນປີເກີດຕ້ອງບໍ່ແມ່ນວັນທີໃນອະນາຄົດ")
            .When(request => request.DateOfBirth.HasValue);
    }
}
