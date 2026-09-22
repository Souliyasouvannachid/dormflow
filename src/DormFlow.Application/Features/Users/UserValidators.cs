using DormFlow.Application.Common.Abstractions;
using DormFlow.Contracts.Users;
using FluentValidation;

namespace DormFlow.Application.Features.Users;

/// <summary>ກວດເບິ່ງການຮ້ອງຂໍການສ້າງບັນຊີຜູ້ໃຊ້ໃຝ່</summary>
public sealed class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
{
    /// <summary>ສ້າງຕົວກວດສອບ</summary>
    public CreateUserRequestValidator()
    {
        RuleFor(request => request.Email)
            .NotEmpty().WithMessage("ກະລຸນາລະບຸອີເມວ")
            .MaximumLength(256).WithMessage("ອີເມວຕ້ອງມີຄວາມຍາວບໍ່ເກີນ 256 ໂຕອັກສອນ")
            .EmailAddress().WithMessage("ຮູບແບບອີເມວບໍ່ຖືກຕ້ອງ");

        RuleFor(request => request.Password).SetValidator(new PasswordRuleValidator());

        RuleFor(request => request.FirstName)
            .NotEmpty().WithMessage("ກະລຸນາລະບຸຊື່")
            .MaximumLength(100).WithMessage("ຊື່ຕ້ອງມີຄວາມຍາວບໍ່ເກີນ 100 ໂຕອັກສອນ");

        RuleFor(request => request.LastName)
            .NotEmpty().WithMessage("ກະລຸນາລະບຸນາມສະກຸນ")
            .MaximumLength(100).WithMessage("ນາມສະກຸນຕ້ອງມີຄວາມຍາວບໍ່ເກີນ 100 ໂຕອັກສອນ");
    }
}

/// <summary>ກວດສອບການຮ້ອງຂໍແກ້ໄຂຂໍ້ມູນໂປຣໄຟລ໌ຜູ້ໃຊ້</summary>
public sealed class UpdateUserRequestValidator : AbstractValidator<UpdateUserRequest>
{
    /// <summary>ສ້າງຕົວກວດສອບ</summary>
    public UpdateUserRequestValidator()
    {
        RuleFor(request => request.FirstName)
            .NotEmpty().WithMessage("ກະລຸນາລະບຸຊື່")
            .MaximumLength(100).WithMessage("ຊື່ຕ້ອງມີຄວາມຍາວບໍ່ເກີນ 100 ໂຕອັກສອນ");

        RuleFor(request => request.LastName)
            .NotEmpty().WithMessage("ກະລຸນາລະບຸນາມສະກຸນ")
            .MaximumLength(100).WithMessage("ນາມສະກຸນຕ້ອງມີຄວາມຍາວບໍ່ເກີນ 100 ໂຕອັກສອນ");
    }
}

/// <summary>ທົບທວນຄືນຄໍາຮ້ອງຂໍການມອບຝາຍບົດບາດຂອງຜູ້ໃຊ້</summary>
public sealed class AssignUserRolesRequestValidator : AbstractValidator<AssignUserRolesRequest>
{
    /// <summary>ສ້າງຕົວກວດສອບ</summary>
    public AssignUserRolesRequestValidator()
    {
        RuleFor(request => request.Roles)
            .NotEmpty().WithMessage("ກະລຸນາລະບຸບົດບາດຢ່າງໜ້ອຍໜຶ່ງລາຍການ");

        RuleForEach(request => request.Roles)
            .Must(DormFlowRoles.All.Contains)
            .WithMessage("ພົບຊື່ບົດບາດທີ່ບໍ່ມີຢູ່ໃນລະບົບ");
    }
}

/// <summary>ທົບທວນຄືນຄໍາຮ້ອງຂໍການມອບຝາຍຫໍພັກສໍາລັບຜູ້ໃຊ້</summary>
public sealed class AssignUserDormitoryRequestValidator : AbstractValidator<AssignUserDormitoryRequest>
{
    /// <summary>ສ້າງຕົວກວດສອບ</summary>
    public AssignUserDormitoryRequestValidator()
    {
        RuleFor(request => request.DormitoryId)
            .NotEmpty().WithMessage("ກະລຸນາລະບຸຫໍພັກທີ່ຕ້ອງການມອບໝາຍ");
    }
}

/// <summary>
/// ກົດລະບຽບລະຫັດຜ່ານທີ່ໃຊ້ໃນການສ້າງບັນຊີໂດຍຜູ້ບໍລິຫານ
/// </summary>
/// <remarks>
/// ແຍກອອກຈາກ PasswordRules ຂອງໂມດູນ Authentication ໃນຄໍາສັ່ງທີ່ຈະບໍ່ Features/Users ໂມດູນຕ້ອງໄດ້ຮັບການອ້າງອີງ Authentication
/// ແຕ່ການນໍາໃຊ້ແທ້ທີ່ກໍານົດໄວ້ຂອງມາດຕະຖານ
/// </remarks>
internal sealed class PasswordRuleValidator : AbstractValidator<string>
{
    public PasswordRuleValidator()
    {
        RuleFor(password => password)
            .NotEmpty().WithMessage("ກະລຸນາລະບຸລະຫັດຜ່ານ")
            .MinimumLength(8).WithMessage("ລະຫັດຜ່ານຕ້ອງມີຄວາມຍາວຢ່າງໜ້ອຍ 8 ໂຕອັກສອນ")
            .MaximumLength(128).WithMessage("ລະຫັດຜ່ານຕ້ອງມີຄວາມຍາວບໍ່ເກີນ 128 ໂຕອັກສອນ")
            .Matches("[A-Z]").WithMessage("ລະຫັດຜ່ານຕ້ອງມີໂຕອັກສອນພິມໃຫຍ່ຢ່າງໜ້ອຍໜຶ່ງໂຕ")
            .Matches("[a-z]").WithMessage("ລະຫັດຜ່ານຕ້ອງມີໂຕອັກສອນພິມນ້ອຍຢ່າງໜ້ອຍໜຶ່ງໂຕ")
            .Matches("[0-9]").WithMessage("ລະຫັດຜ່ານຕ້ອງມີໂຕເລກຢ່າງໜ້ອຍໜຶ່ງໂຕ");
    }
}
