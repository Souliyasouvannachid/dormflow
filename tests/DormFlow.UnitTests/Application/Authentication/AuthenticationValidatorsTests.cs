using DormFlow.Application.Features.Authentication.Validators;
using DormFlow.Contracts.Authentication;
using FluentAssertions;
using FluentValidation.Results;
using FluentValidation.TestHelper;
using Xunit;

namespace DormFlow.UnitTests.Application.Authentication;

/// <summary>ตรวจกฎการตรวจสอบข้อมูลของ Endpoint การยืนยันตัวตน</summary>
public class AuthenticationValidatorsTests
{
    private readonly LoginRequestValidator _loginValidator = new();
    private readonly ChangePasswordRequestValidator _changePasswordValidator = new();
    private readonly ResetPasswordRequestValidator _resetPasswordValidator = new();

    [Fact]
    public void Login_อีเมลว่าง_ต้องไม่ผ่าน()
    {
        TestValidationResult<LoginRequest> result = _loginValidator.TestValidate(
            new LoginRequest { Email = string.Empty, Password = "P@ssw0rd1" });

        result.ShouldHaveValidationErrorFor(request => request.Email);
    }

    [Fact]
    public void Login_รูปแบบอีเมลผิด_ต้องไม่ผ่าน()
    {
        TestValidationResult<LoginRequest> result = _loginValidator.TestValidate(
            new LoginRequest { Email = "not-an-email", Password = "P@ssw0rd1" });

        result.ShouldHaveValidationErrorFor(request => request.Email);
    }

    [Fact]
    public void Login_ข้อมูลถูกต้อง_ต้องผ่าน()
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
    public void ChangePassword_รหัสผ่านใหม่ไม่ผ่านนโยบาย_ต้องไม่ผ่าน(string newPassword)
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
    public void ChangePassword_รหัสผ่านใหม่ซ้ำกับรหัสผ่านปัจจุบัน_ต้องไม่ผ่าน()
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
    public void ChangePassword_ยืนยันรหัสผ่านไม่ตรงกัน_ต้องไม่ผ่าน()
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
    public void ChangePassword_ข้อมูลถูกต้องครบถ้วน_ต้องผ่าน()
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
    public void ResetPassword_โทเคนว่าง_ต้องไม่ผ่าน()
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
    public void ValidationFailure_ข้อความ_ต้องเป็นภาษาไทยที่กำหนดไว้()
    {
        ValidationResult result = _loginValidator.Validate(new LoginRequest { Email = string.Empty, Password = "x" });

        result.Errors.Should().Contain(failure => failure.ErrorMessage == "กรุณาระบุอีเมล");
    }
}
