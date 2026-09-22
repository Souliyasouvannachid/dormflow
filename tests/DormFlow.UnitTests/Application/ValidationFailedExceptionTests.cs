using DormFlow.Application.Common.Exceptions;
using FluentAssertions;
using FluentValidation.Results;
using Xunit;

namespace DormFlow.UnitTests.Application;

/// <summary>
/// ກວດເບິ່ງວ່າຄວາມຜິດພາດຈາກ FluentValidation ປ່ຽນເປັນກະແຈ camelCase
/// ກົງກັບຊື່ຊ່ອງຂໍ້ມູນໃນເອກະສານ JSON ທີ່ Client ສົ່ງເຂົ້າມາ
/// </summary>
public class ValidationFailedExceptionTests
{
    [Fact]
    public void ຊື່ພາກສະຜາມ_ຕ້ອງໄດ້ຮັບການປ່ຽນເປັນ_camelCase()
    {
        ValidationFailedException exception = new([new ValidationFailure("RoomNumber", "ກະລຸນາລະບຸໝາຍເລກຫ້ອງ")]);

        exception.Errors.Should().ContainKey("roomNumber");
    }

    [Fact]
    public void ຊື່ຊ່ອງຂໍ້ມູນNested_ຕ້ອງໄດ້ຮັບການຫັນປ່ຽນໃນທຸກລະດັບ()
    {
        ValidationFailedException exception = new([new ValidationFailure("Address.PostalCode", "ລະຫັດໄປສະນີບໍ່ຖືກຕ້ອງ")]);

        exception.Errors.Should().ContainKey("address.postalCode");
    }

    [Fact]
    public void ຄວາມຜິດພາດຫຼາຍຊ່ອງຂໍ້ມູນດຽວກັນ_ຕ້ອງໄດ້ຮັບການນໍາເອົາຮ່ວມກັນ()
    {
        ValidationFailedException exception = new(
        [
            new ValidationFailure("Code", "ກະລຸນາໃສ່ລະຫັດ"),
            new ValidationFailure("Code", "ລະຫັດຈະຕ້ອງບໍ່ຍາວກວ່າ 20 ຈົດໝາຍ"),
        ]);

        exception.Errors["code"].Should().HaveCount(2);
    }

    [Fact]
    public void ສຳເນົາຂໍ້ຄວາມໃນຊ່ອງຂໍ້ມູນດຽວກັນ_ຈະຕ້ອງເຫຼືອພຽງແຕ່ໜຶ່ງລາຍການເທົ່ານັ້ນ()
    {
        ValidationFailedException exception = new(
        [
            new ValidationFailure("Code", "ກະລຸນາໃສ່ລະຫັດ"),
            new ValidationFailure("Code", "ກະລຸນາໃສ່ລະຫັດ"),
        ]);

        exception.Errors["code"].Should().ContainSingle();
    }

    [Fact]
    public void ສ້າງໂດຍບໍ່ໄດ້ລະບຸຂໍ້ຜິດພາດ_ທ່ານຕ້ອງໄດ້ຮັບວັດຈະນານຸກົມເປົ່າ()
    {
        ValidationFailedException exception = new();

        exception.Errors.Should().BeEmpty();
        exception.Message.Should().NotBeNullOrWhiteSpace();
    }
}
