using DormFlow.Domain.Entities;
using DormFlow.Domain.Enums;
using FluentAssertions;
using Xunit;

namespace DormFlow.UnitTests.Domain;

/// <summary>ກວດເບິ່ງເຫດຜົນຂອງລັດຂອງອົງກອນ</summary>
public class OrganizationTests
{
    [Theory]
    [InlineData(OrganizationStatus.Active, true)]
    [InlineData(OrganizationStatus.Inactive, false)]
    [InlineData(OrganizationStatus.Suspended, false)]
    public void CanUsersSignIn_ຕ້ອງເປັນຄວາມຈິງພຽງແຕ່ຢູ່ໃນລັດ_Active(OrganizationStatus status, bool expected)
    {
        Organization organization = new()
        {
            Code = "ORG-1",
            Name = "ຫໍພັກການທົດສອບ",
            Status = status,
        };

        organization.CanUsersSignIn.Should().Be(expected);
    }

    [Fact]
    public void Organization_ສ້າງໃໝ່_ຕ້ອງມີສະຖານະ_Active_ເປັນຄ່າເລີ່ມຕົ້ນ()
    {
        Organization organization = new() { Code = "ORG-2", Name = "ຫໍພັກການທົດສອບ" };

        organization.Status.Should().Be(OrganizationStatus.Active);
    }
}
