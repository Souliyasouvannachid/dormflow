using DormFlow.Domain.Entities;
using DormFlow.Domain.Enums;
using FluentAssertions;
using Xunit;

namespace DormFlow.UnitTests.Domain;

/// <summary>ตรวจตรรกะสถานะขององค์กร</summary>
public class OrganizationTests
{
    [Theory]
    [InlineData(OrganizationStatus.Active, true)]
    [InlineData(OrganizationStatus.Inactive, false)]
    [InlineData(OrganizationStatus.Suspended, false)]
    public void CanUsersSignIn_ต้องเป็นจริงเฉพาะสถานะ_Active(OrganizationStatus status, bool expected)
    {
        Organization organization = new()
        {
            Code = "ORG-1",
            Name = "หอพักทดสอบ",
            Status = status,
        };

        organization.CanUsersSignIn.Should().Be(expected);
    }

    [Fact]
    public void Organization_ที่สร้างใหม่_ต้องมีสถานะ_Active_เป็นค่าเริ่มต้น()
    {
        Organization organization = new() { Code = "ORG-2", Name = "หอพักทดสอบ" };

        organization.Status.Should().Be(OrganizationStatus.Active);
    }
}
