using DormFlow.Application.Features.Dormitories;
using DormFlow.Contracts.Dormitories;
using FluentAssertions;
using FluentValidation.Results;
using Xunit;

namespace DormFlow.UnitTests.Application.Dormitories;

/// <summary>
/// ตรวจกฎการตรวจสอบข้อมูลหอพัก โดยเฉพาะขอบเขตวันที่ออกใบแจ้งหนี้และวันครบกำหนดชำระ
/// ซึ่งเป็นค่าที่ระบบใบแจ้งหนี้ในเฟสถัดไปจะพึ่งพาโดยตรง
/// </summary>
public class DormitoryValidatorsTests
{
    private readonly CreateDormitoryRequestValidator _validator = new();

    [Theory]
    [InlineData(1)]
    [InlineData(28)]
    [InlineData(15)]
    public void BillingDay_ในช่วงที่ยอมรับ_ต้องผ่าน(int billingDay)
    {
        CreateDormitoryRequest request = CreateValidRequest();
        request.BillingDay = billingDay;

        ValidationResult result = _validator.Validate(request);

        result.Errors.Should().NotContain(error => error.PropertyName == nameof(CreateDormitoryRequest.BillingDay));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(29)]
    [InlineData(-1)]
    public void BillingDay_นอกช่วงที่ยอมรับ_ต้องไม่ผ่าน(int billingDay)
    {
        CreateDormitoryRequest request = CreateValidRequest();
        request.BillingDay = billingDay;

        ValidationResult result = _validator.Validate(request);

        result.Errors.Should().Contain(error => error.PropertyName == nameof(CreateDormitoryRequest.BillingDay));
    }

    [Theory]
    [InlineData(1)]
    [InlineData(31)]
    public void PaymentDueDay_ในช่วงที่ยอมรับ_ต้องผ่าน(int paymentDueDay)
    {
        CreateDormitoryRequest request = CreateValidRequest();
        request.PaymentDueDay = paymentDueDay;

        ValidationResult result = _validator.Validate(request);

        result.Errors.Should().NotContain(error => error.PropertyName == nameof(CreateDormitoryRequest.PaymentDueDay));
    }

    [Fact]
    public void PaymentDueDay_เกิน31_ต้องไม่ผ่าน()
    {
        CreateDormitoryRequest request = CreateValidRequest();
        request.PaymentDueDay = 32;

        ValidationResult result = _validator.Validate(request);

        result.Errors.Should().Contain(error => error.PropertyName == nameof(CreateDormitoryRequest.PaymentDueDay));
    }

    [Theory]
    [InlineData("Fixed")]
    [InlineData("Percentage")]
    [InlineData("fixed")]
    public void LateFeeType_ค่าที่ถูกต้อง_ต้องผ่าน(string lateFeeType)
    {
        CreateDormitoryRequest request = CreateValidRequest();
        request.LateFeeType = lateFeeType;

        ValidationResult result = _validator.Validate(request);

        result.Errors.Should().NotContain(error => error.PropertyName == nameof(CreateDormitoryRequest.LateFeeType));
    }

    [Fact]
    public void LateFeeType_ค่าที่ไม่มีอยู่จริง_ต้องไม่ผ่าน()
    {
        CreateDormitoryRequest request = CreateValidRequest();
        request.LateFeeType = "Compound";

        ValidationResult result = _validator.Validate(request);

        result.Errors.Should().Contain(error => error.PropertyName == nameof(CreateDormitoryRequest.LateFeeType));
    }

    [Fact]
    public void Code_ว่างเปล่า_ต้องไม่ผ่าน()
    {
        CreateDormitoryRequest request = CreateValidRequest();
        request.Code = string.Empty;

        ValidationResult result = _validator.Validate(request);

        result.Errors.Should().Contain(error => error.PropertyName == nameof(CreateDormitoryRequest.Code));
    }

    [Fact]
    public void Email_รูปแบบไม่ถูกต้อง_ต้องไม่ผ่าน()
    {
        CreateDormitoryRequest request = CreateValidRequest();
        request.Email = "not-an-email";

        ValidationResult result = _validator.Validate(request);

        result.Errors.Should().Contain(error => error.PropertyName == nameof(CreateDormitoryRequest.Email));
    }

    [Fact]
    public void ข้อมูลครบถ้วนถูกต้อง_ต้องผ่านทั้งหมด()
    {
        ValidationResult result = _validator.Validate(CreateValidRequest());

        result.IsValid.Should().BeTrue();
    }

    private static CreateDormitoryRequest CreateValidRequest() => new()
    {
        Code = "D01",
        Name = "หอพักทดสอบ",
        BillingDay = 1,
        PaymentDueDay = 5,
        LateFeeType = "Fixed",
        LateFeeAmount = 0,
    };
}
