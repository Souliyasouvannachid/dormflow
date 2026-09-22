using DormFlow.Application.Common.Authorization;
using DormFlow.Contracts.Roles;
using FluentValidation;

namespace DormFlow.Application.Features.Roles;

/// <summary>ທົບທວນຄືນການຮ້ອງຂໍເພື່ອສ້າງບົດບາດໃຝ່</summary>
public sealed class CreateRoleRequestValidator : AbstractValidator<CreateRoleRequest>
{
    /// <summary>ສ້າງຕົວກວດສອບ</summary>
    public CreateRoleRequestValidator()
    {
        RuleFor(request => request.Name)
            .NotEmpty().WithMessage("ກະລຸນາລະບຸຊື່ບົດບາດ")
            .MaximumLength(100).WithMessage("ຊື່ບົດບາດຕ້ອງມີຄວາມຍາວບໍ່ເກີນ 100 ໂຕອັກສອນ");

        RuleFor(request => request.Description)
            .MaximumLength(256).WithMessage("ຄຳອະທິບາຍຕ້ອງມີຄວາມຍາວບໍ່ເກີນ 256 ໂຕອັກສອນ");
    }
}

/// <summary>ທົບທວນຄືນການຮ້ອງຂໍແກ້ໄຂຂໍ້ມູນບົດບາດ</summary>
public sealed class UpdateRoleRequestValidator : AbstractValidator<UpdateRoleRequest>
{
    /// <summary>ສ້າງຕົວກວດສອບ</summary>
    public UpdateRoleRequestValidator()
    {
        RuleFor(request => request.Name)
            .NotEmpty().WithMessage("ກະລຸນາລະບຸຊື່ບົດບາດ")
            .MaximumLength(100).WithMessage("ຊື່ບົດບາດຕ້ອງມີຄວາມຍາວບໍ່ເກີນ 100 ໂຕອັກສອນ");

        RuleFor(request => request.Description)
            .MaximumLength(256).WithMessage("ຄຳອະທິບາຍຕ້ອງມີຄວາມຍາວບໍ່ເກີນ 256 ໂຕອັກສອນ");
    }
}

/// <summary>ກວດເບິ່ງການຮ້ອງຂໍການອະນຸຍາດບົດບາດ</summary>
public sealed class AssignRolePermissionsRequestValidator : AbstractValidator<AssignRolePermissionsRequest>
{
    /// <summary>ສ້າງຕົວກວດສອບ</summary>
    public AssignRolePermissionsRequestValidator()
    {
        RuleForEach(request => request.Permissions)
            .Must(PermissionCodes.Exists)
            .WithMessage("ພົບລະຫັດສິດທີ່ບໍ່ມີຢູ່ໃນລະບົບ");
    }
}
