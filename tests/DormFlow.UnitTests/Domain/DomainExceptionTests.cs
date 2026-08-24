using DormFlow.Domain.Exceptions;
using FluentAssertions;
using Xunit;

namespace DormFlow.UnitTests.Domain;

/// <summary>ตรวจว่าข้อผิดพลาดเชิงธุรกิจพกข้อความและรหัสกฎไปด้วยเสมอ</summary>
public class DomainExceptionTests
{
    [Fact]
    public void BusinessRuleViolationException_ต้องเก็บรหัสกฎที่ระบุ()
    {
        BusinessRuleViolationException exception = new(
            "rooms.duplicate_room_number",
            "เลขห้องนี้ถูกใช้ในอาคารเดียวกันแล้ว");

        exception.RuleCode.Should().Be("rooms.duplicate_room_number");
        exception.Message.Should().Be("เลขห้องนี้ถูกใช้ในอาคารเดียวกันแล้ว");
        exception.Should().BeAssignableTo<DomainException>();
    }

    [Fact]
    public void BusinessRuleViolationException_ต้องไม่รับรหัสกฎที่ว่าง()
    {
        Action act = () => _ = new BusinessRuleViolationException("   ", "ข้อความ");

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void DomainException_ที่ไม่ระบุข้อความ_ต้องมีข้อความเริ่มต้น()
    {
        DomainException exception = new();

        exception.Message.Should().NotBeNullOrWhiteSpace();
    }
}
