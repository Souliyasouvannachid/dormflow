using DormFlow.Domain.Exceptions;
using FluentAssertions;
using Xunit;

namespace DormFlow.UnitTests.Domain;

/// <summary>ໃຫ້ແນ່ໃຈວ່າຄວາມຜິດພາດທາງທຸລະກິດມີຂໍ້ຄວາມແລະກົດລະບຽບ ID ສະເຝີ</summary>
public class DomainExceptionTests
{
    [Fact]
    public void BusinessRuleViolationException_ຕ້ອງເກັບຮັກສາIDກົດລະບຽບທີ່ລະບຸໄວ້()
    {
        BusinessRuleViolationException exception = new(
            "rooms.duplicate_room_number",
            "ໝາຍເລກຫ້ອງນີ້ແມ່ນໃຊ້ໃນອາຄານດຽວກັນແລ້ວ");

        exception.RuleCode.Should().Be("rooms.duplicate_room_number");
        exception.Message.Should().Be("ໝາຍເລກຫ້ອງນີ້ແມ່ນໃຊ້ໃນອາຄານດຽວກັນແລ້ວ");
        exception.Should().BeAssignableTo<DomainException>();
    }

    [Fact]
    public void BusinessRuleViolationException_IDກົດລະບຽບຫວ່າງເປົ່າຕ້ອງບໍ່ໄດ້ຮັບການຍອມຮັບ()
    {
        Action act = () => _ = new BusinessRuleViolationException("   ", "ຂໍ້ຄວາມ");

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void DomainException_ເຊິ່ງບໍ່ໄດ້ລະບຸຂໍ້ຄວາມ_ຕ້ອງມີຂໍ້ຄວາມເບື້ອງຕົ້ນ()
    {
        DomainException exception = new();

        exception.Message.Should().NotBeNullOrWhiteSpace();
    }
}
