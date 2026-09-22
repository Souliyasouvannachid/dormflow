using DormFlow.Application.Features.Authentication.Validators;
using DormFlow.Contracts.Authentication;
using FluentAssertions;
using FluentValidation.Results;
using FluentValidation.TestHelper;
using Xunit;

namespace DormFlow.UnitTests.Application.Authentication;

/// <summary>ກວດເບິ່ງກົດລະບຽບການກວດສອບຂໍ້ມູນຂອງ Endpoint ການລະບຸຕົວຕົນ</summary>
public class AuthenticationValidatorsTests
{
    private readonly LoginRequestValidator _loginValidator = new();
    private readonly ChangePasswordRequestValidator _changePasswordValidator = new();
    private readonly ResetPasswordRequestValidator _resetPasswordValidator = new();

    [Fact]
    public void Login_ອີເມວມີໃຫ້_ບໍ່ຕ້ອງຜ່ານ()
    {
        TestValidationResult<LoginRequest> result = _loginValidator.TestValidate(
            new LoginRequest { Email = string.Empty, Password = "P@ssw0rd1" });

        result.ShouldHaveValidationErrorFor(request => request.Email);
    }

    [Fact]
    public void Login_ຮູບແບບອີເມວຜິດ_ບໍ່ຕ້ອງຜ່ານ()
    {
        TestValidationResult<LoginRequest> result = _loginValidator.TestValidate(
            new LoginRequest { Email = "not-an-email", Password = "P@ssw0rd1" });

        result.ShouldHaveValidationErrorFor(request => request.Email);
    }

    [Fact]
    public void Login_ຂໍ້ມູນແມ່ນຖືກຕ້ອງ_ຕ້ອງຜ່ານ()
    {
        TestValidationResult<LoginRequest> result = _loginValidator.TestValidate(
            new LoginRequest { Email = "user@dormflow.local", Password = "anything" });

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("short1A")]
    [InlineData("alllowercase1")]
    [InlineData("ALLUPPERCASE1")]
    [InlineData("NoDigitsHere")]
    public void ChangePassword_ລະຫັດຜ່ານໃໝ່ບໍ່ຜ່ານນະໂຍບາຍ_ບໍ່ຕ້ອງຜ່ານ(string newPassword)
    {
        TestValidationResult<ChangePasswordRequest> result = _changePasswordValidator.TestValidate(
            new ChangePasswordRequest
            {
                CurrentPassword = "OldPassw0rd1",
                NewPassword = newPassword,
                ConfirmPassword = newPassword,
            });

        result.ShouldHaveValidationErrorFor(request => request.NewPassword);
    }

    [Fact]
    public void ChangePassword_ລະຫັດຜ່ານໃໝ່ແມ່ນຄືກັນກັບລະຫັດຜ່ານປັດຈຸບັນ_ບໍ່ຕ້ອງຜ່ານ()
    {
        TestValidationResult<ChangePasswordRequest> result = _changePasswordValidator.TestValidate(
            new ChangePasswordRequest
            {
                CurrentPassword = "SamePassw0rd1",
                NewPassword = "SamePassw0rd1",
                ConfirmPassword = "SamePassw0rd1",
            });

        result.ShouldHaveValidationErrorFor(request => request.NewPassword);
    }

    [Fact]
    public void ChangePassword_ຢືນຢັນລະຫັດຜ່ານບໍ່ກົງກັນ_ບໍ່ຕ້ອງຜ່ານ()
    {
        TestValidationResult<ChangePasswordRequest> result = _changePasswordValidator.TestValidate(
            new ChangePasswordRequest
            {
                CurrentPassword = "OldPassw0rd1",
                NewPassword = "NewPassw0rd1",
                ConfirmPassword = "Different1",
            });

        result.ShouldHaveValidationErrorFor(request => request.ConfirmPassword);
    }

    [Fact]
    public void ChangePassword_ຂໍ້ມູນແມ່ນຖືກຕ້ອງແລະຄົບຖ້ວນ_ຕ້ອງຜ່ານ()
    {
        TestValidationResult<ChangePasswordRequest> result = _changePasswordValidator.TestValidate(
            new ChangePasswordRequest
            {
                CurrentPassword = "OldPassw0rd1",
                NewPassword = "NewPassw0rd1",
                ConfirmPassword = "NewPassw0rd1",
            });

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void ResetPassword_ໂທເຄັນຫວ່າງເປົ່າ_ບໍ່ຕ້ອງຜ່ານ()
    {
        TestValidationResult<ResetPasswordRequest> result = _resetPasswordValidator.TestValidate(
            new ResetPasswordRequest
            {
                Email = "user@dormflow.local",
                Token = string.Empty,
                NewPassword = "NewPassw0rd1",
                ConfirmPassword = "NewPassw0rd1",
            });

        result.ShouldHaveValidationErrorFor(request => request.Token);
    }

    [Fact]
    public void ValidationFailure_ຂໍ້ຄວາມ_ຕ້ອງເປັນພາສາລາວທີ່ລະບຸ()
    {
        ValidationResult result = _loginValidator.Validate(new LoginRequest { Email = string.Empty, Password = "x" });

        result.Errors.Should().Contain(failure => failure.ErrorMessage == "ກະລຸນາລະບຸອີເມວ");
    }
}
