using DormFlow.Application.Features.Dormitories;
using DormFlow.Contracts.Dormitories;
using FluentAssertions;
using FluentValidation.Results;
using Xunit;

namespace DormFlow.UnitTests.Application.Dormitories;

/// <summary>
/// ກວດສອບກົດລະບຽບການກວດສອບຂໍ້ມູນຫໍພັກ. ໂດຍສະເພາະແມ່ນຂອບເຂດຂອງວັນທີໃບເກັບເງິນແລະວັນທີການຊໍາລະຊໍາລະ
/// ນີ້ແມ່ນມູນຄ່າທີ່ໄລຍະຕໍ່ໄປຂອງລະບົບໃບເກັບເງິນຈະອີງໃສ່ໂດຍກົງ
/// </summary>
public class DormitoryValidatorsTests
{
    private readonly CreateDormitoryRequestValidator _validator = new();

    [Theory]
    [InlineData(1)]
    [InlineData(28)]
    [InlineData(15)]
    public void BillingDay_ໃນລະຫວ່າງການຍອມຮັບ_ຕ້ອງຜ່ານ(int billingDay)
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
    public void BillingDay_ຢູ່ນອກຂອບເຂດທີ່ຍອມຮັບ_ບໍ່ຕ້ອງຜ່ານ(int billingDay)
    {
        CreateDormitoryRequest request = CreateValidRequest();
        request.BillingDay = billingDay;

        ValidationResult result = _validator.Validate(request);

        result.Errors.Should().Contain(error => error.PropertyName == nameof(CreateDormitoryRequest.BillingDay));
    }

    [Theory]
    [InlineData(1)]
    [InlineData(31)]
    public void PaymentDueDay_ໃນລະຫວ່າງການຍອມຮັບ_ຕ້ອງຜ່ານ(int paymentDueDay)
    {
        CreateDormitoryRequest request = CreateValidRequest();
        request.PaymentDueDay = paymentDueDay;

        ValidationResult result = _validator.Validate(request);

        result.Errors.Should().NotContain(error => error.PropertyName == nameof(CreateDormitoryRequest.PaymentDueDay));
    }

    [Fact]
    public void PaymentDueDay_ເກີນ31_ບໍ່ຕ້ອງຜ່ານ()
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
    public void LateFeeType_ຄ່າທີ່ຖືກຕ້ອງ_ຕ້ອງຜ່ານ(string lateFeeType)
    {
        CreateDormitoryRequest request = CreateValidRequest();
        request.LateFeeType = lateFeeType;

        ValidationResult result = _validator.Validate(request);

        result.Errors.Should().NotContain(error => error.PropertyName == nameof(CreateDormitoryRequest.LateFeeType));
    }

    [Fact]
    public void LateFeeType_ຄ່າທີ່ບໍ່ມີຕົວຈິງແລ້ວ_ບໍ່ຕ້ອງຜ່ານ()
    {
        CreateDormitoryRequest request = CreateValidRequest();
        request.LateFeeType = "Compound";

        ValidationResult result = _validator.Validate(request);

        result.Errors.Should().Contain(error => error.PropertyName == nameof(CreateDormitoryRequest.LateFeeType));
    }

    [Fact]
    public void Code_ຫວ່າງເປົ່າ_ບໍ່ຕ້ອງຜ່ານ()
    {
        CreateDormitoryRequest request = CreateValidRequest();
        request.Code = string.Empty;

        ValidationResult result = _validator.Validate(request);

        result.Errors.Should().Contain(error => error.PropertyName == nameof(CreateDormitoryRequest.Code));
    }

    [Fact]
    public void Email_ຮູບແບບບໍ່ຖືກຕ້ອງ_ບໍ່ຕ້ອງຜ່ານ()
    {
        CreateDormitoryRequest request = CreateValidRequest();
        request.Email = "not-an-email";

        ValidationResult result = _validator.Validate(request);

        result.Errors.Should().Contain(error => error.PropertyName == nameof(CreateDormitoryRequest.Email));
    }

    [Fact]
    public void ຂໍ້ມູນຄົບຖ້ວນແລະຖືກຕ້ອງ_ຕ້ອງຜ່ານທັງຝົດ()
    {
        ValidationResult result = _validator.Validate(CreateValidRequest());

        result.IsValid.Should().BeTrue();
    }

    private static CreateDormitoryRequest CreateValidRequest() => new()
    {
        Code = "D01",
        Name = "ຫໍພັກການທົດສອບ",
        BillingDay = 1,
        PaymentDueDay = 5,
        LateFeeType = "Fixed",
        LateFeeAmount = 0,
    };
}
