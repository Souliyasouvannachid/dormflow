using DormFlow.Contracts.Authentication;
using FluentValidation;

namespace DormFlow.Application.Features.Authentication.Validators;

/// <summary>ຄວາມຕ້ອງການລະຫັດຜ່ານທີ່ແບ່ງປັນທົ່ວທຸກຈຸດລະຫັດຜ່ານຂອງຜູ້ໃຊ້</summary>
public static class PasswordRules
{
    /// <summary>ຄວາມຍາວລະຫັດຜ່ານຕໍາ່ສຸດ</summary>
    public const int MinimumLength = 8;

    /// <summary>ຄວາມຍາວລະຫັດຜ່ານສູງສຸດ</summary>
    public const int MaximumLength = 128;

    /// <summary>
    /// ໃສ່ກົດລະບຽບລະຫັດຜ່ານສໍາລັບຊ່ອງຂໍ້ມູນທີ່ກໍານົດໄວ້
    /// </summary>
    /// <typeparam name="T">ປະເພດຂອງ request ທີ່ກໍາລັງຖືກກວດສອບ</typeparam>
    /// <param name="rule">ກົດລະບຽບຊ່ອງໃສ່ລະຫັດຜ່ານ</param>
    /// <remarks>
    /// ຊຸດຂອງກົດລະບຽບນີ້ຕ້ອງສອດຄ່ອງກັບຄ່າທີ່ກໍານົດໄວ້ໃນ ASP.NET Core Identity
    /// ມັນມີຈຸດປະສົງເພື່ອໃຫ້ຜູ້ໃຊ້ມີຂໍ້ຄວາມພາສາລາວທີ່ຊັດເຈນກ່ອນທີ່ຄໍາຮ້ອງຂໍຈະມາຮອດຊັ້ນ Identity
    /// </remarks>
    public static IRuleBuilderOptions<T, string> ApplyPasswordPolicy<T>(this IRuleBuilder<T, string> rule)
    {
        ArgumentNullException.ThrowIfNull(rule);

        return rule
            .NotEmpty().WithMessage("ກະລຸນາລະບຸລະຫັດຜ່ານ")
            .MinimumLength(MinimumLength).WithMessage($"ລະຫັດຜ່ານຕ້ອງມີຄວາມຍາວຢ່າງໜ້ອຍ {MinimumLength} ໂຕອັກສອນ")
            .MaximumLength(MaximumLength).WithMessage($"ລະຫັດຜ່ານຕ້ອງມີຄວາມຍາວບໍ່ເກີນ {MaximumLength} ໂຕອັກສອນ")
            .Matches("[A-Z]").WithMessage("ລະຫັດຜ່ານຕ້ອງມີໂຕອັກສອນພິມໃຫຍ່ຢ່າງໜ້ອຍໜຶ່ງໂຕ")
            .Matches("[a-z]").WithMessage("ລະຫັດຜ່ານຕ້ອງມີໂຕອັກສອນພິມນ້ອຍຢ່າງໜ້ອຍໜຶ່ງໂຕ")
            .Matches("[0-9]").WithMessage("ລະຫັດຜ່ານຕ້ອງມີໂຕເລກຢ່າງໜ້ອຍໜຶ່ງໂຕ");
    }
}

/// <summary>ຢືນຢັນຂໍ້ມູນການເຂົ້າສູ່ລະບົບ</summary>
public sealed class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    /// <summary>ສ້າງຕົວກວດສອບ</summary>
    public LoginRequestValidator()
    {
        RuleFor(request => request.Email)
            .NotEmpty().WithMessage("ກະລຸນາລະບຸອີເມວ")
            .MaximumLength(256).WithMessage("ອີເມວຕ້ອງມີຄວາມຍາວບໍ່ເກີນ 256 ໂຕອັກສອນ")
            .EmailAddress().WithMessage("ຮູບແບບອີເມວບໍ່ຖືກຕ້ອງ");

        // ຄວາມຊັບຊ້ອນຂອງລະຫັດຜ່ານແມ່ນບໍ່ຈໍາເປັນໃນເວລາທີ່ເຂົ້າສູ່ລະບົບ
        // ນີ້ແມ່ນຍ້ອນວ່າບັນຊີເກົ່າອາດຈະໄດ້ຕັ້ງລະຫັດຜ່ານກ່ອນທີ່ນະໂຍບາຍປະຈຸບັນຈະມີຜົນ
        RuleFor(request => request.Password)
            .NotEmpty().WithMessage("ກະລຸນາລະບຸລະຫັດຜ່ານ")
            .MaximumLength(PasswordRules.MaximumLength)
            .WithMessage($"ລະຫັດຜ່ານຕ້ອງມີຄວາມຍາວບໍ່ເກີນ {PasswordRules.MaximumLength} ໂຕອັກສອນ");
    }
}

/// <summary>ກວດເບິ່ງຄຳຮ້ອງຂໍການຕໍ່ອາຍຸໂທເຄັນຂອງທ່ານ</summary>
public sealed class RefreshTokenRequestValidator : AbstractValidator<RefreshTokenRequest>
{
    /// <summary>ສ້າງຕົວກວດສອບ</summary>
    public RefreshTokenRequestValidator()
    {
        RuleFor(request => request.RefreshToken)
            .NotEmpty().WithMessage("ກະລຸນາລະບຸ Refresh Token")
            .MaximumLength(512).WithMessage("Refresh Token ບໍ່ຖືກຕ້ອງ");
    }
}

/// <summary>ກວດເບິ່ງການຮ້ອງຂໍອອກຈາກລະບົບ</summary>
public sealed class LogoutRequestValidator : AbstractValidator<LogoutRequest>
{
    /// <summary>ສ້າງຕົວກວດສອບ</summary>
    public LogoutRequestValidator()
    {
        RuleFor(request => request.RefreshToken)
            .NotEmpty().WithMessage("ກະລຸນາລະບຸ Refresh Token")
            .MaximumLength(512).WithMessage("Refresh Token ບໍ່ຖືກຕ້ອງ");
    }
}

/// <summary>ກວດເບິ່ງຄໍາຮ້ອງຂໍການເຊື່ອມຕໍ່ຄືນລະຫັດຜ່ານ</summary>
public sealed class ForgotPasswordRequestValidator : AbstractValidator<ForgotPasswordRequest>
{
    /// <summary>ສ້າງຕົວກວດສອບ</summary>
    public ForgotPasswordRequestValidator()
    {
        RuleFor(request => request.Email)
            .NotEmpty().WithMessage("ກະລຸນາລະບຸອີເມວ")
            .MaximumLength(256).WithMessage("ອີເມວຕ້ອງມີຄວາມຍາວບໍ່ເກີນ 256 ໂຕອັກສອນ")
            .EmailAddress().WithMessage("ຮູບແບບອີເມວບໍ່ຖືກຕ້ອງ");
    }
}

/// <summary>ຢືນຢັນການຮ້ອງຂໍການຕັ້ງລະຫັດຜ່ານໃຝ່</summary>
public sealed class ResetPasswordRequestValidator : AbstractValidator<ResetPasswordRequest>
{
    /// <summary>ສ້າງຕົວກວດສອບ</summary>
    public ResetPasswordRequestValidator()
    {
        RuleFor(request => request.Email)
            .NotEmpty().WithMessage("ກະລຸນາລະບຸອີເມວ")
            .EmailAddress().WithMessage("ຮູບແບບອີເມວບໍ່ຖືກຕ້ອງ");

        RuleFor(request => request.Token)
            .NotEmpty().WithMessage("ກະລຸນາລະບຸໂທເຄນສຳລັບຕັ້ງລະຫັດຜ່ານໃໝ່");

        RuleFor(request => request.NewPassword).ApplyPasswordPolicy();

        RuleFor(request => request.ConfirmPassword)
            .Equal(request => request.NewPassword)
            .WithMessage("ລະຫັດຜ່ານໃໝ່ແລະການຢືນຢັນລະຫັດຜ່ານບໍ່ຕົງກັນ");
    }
}

/// <summary>ກວດເບິ່ງການຮ້ອງຂໍການປ່ຽນແປງລະຫັດຜ່ານ</summary>
public sealed class ChangePasswordRequestValidator : AbstractValidator<ChangePasswordRequest>
{
    /// <summary>ສ້າງຕົວກວດສອບ</summary>
    public ChangePasswordRequestValidator()
    {
        RuleFor(request => request.CurrentPassword)
            .NotEmpty().WithMessage("ກະລຸນາລະບຸລະຫັດຜ່ານປັດຈຸບັນ");

        RuleFor(request => request.NewPassword)
            .ApplyPasswordPolicy()
            .NotEqual(request => request.CurrentPassword)
            .WithMessage("ລະຫັດຜ່ານໃໝ່ຕ້ອງບໍ່ຊ້ຳກັບລະຫັດຜ່ານປັດຈຸບັນ");

        RuleFor(request => request.ConfirmPassword)
            .Equal(request => request.NewPassword)
            .WithMessage("ລະຫັດຜ່ານໃໝ່ແລະການຢືນຢັນລະຫັດຜ່ານບໍ່ຕົງກັນ");
    }
}
