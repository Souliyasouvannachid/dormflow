using DormFlow.Application.Common.Authorization;
using FluentAssertions;
using Xunit;

namespace DormFlow.UnitTests.Application.Authorization;

/// <summary>ກວດເບິ່ງຄວາມສົມບູນຂອງບັນຊີລາຍຊື່ລະຫັດການອະນຸຍາດ</summary>
public class PermissionCodesTests
{
    [Fact]
    public void All_ຈະຕ້ອງບໍ່ມີລະຫັດໃບອະນຸຍາດດຽວກັນ()
    {
        PermissionCodes.All.Select(definition => definition.Code)
            .Should().OnlyHaveUniqueItems();
    }

    [Fact]
    public void ສິດທັງໝົດ_ຕ້ອງມີໂມດູນຄວບຄຸມ()
    {
        PermissionCodes.All.Should().OnlyContain(definition => !string.IsNullOrWhiteSpace(definition.Module));
    }

    [Fact]
    public void AllCodes_ຕ້ອງເທົ່າກັບ_All()
    {
        PermissionCodes.AllCodes.Should().HaveCount(PermissionCodes.All.Count);
    }

    [Fact]
    public void Exists_ເມື່ອລະຫັດທີ່ຖືກຕ້ອງມີຢູ່_ຕ້ອງກັບຄືນຄວາມຈິງ()
    {
        PermissionCodes.Exists(PermissionCodes.Rooms.Create).Should().BeTrue();
    }

    [Fact]
    public void Exists_ເມື່ອລະຫັດການອະນຸຍາດບໍ່ມີ_ຕ້ອງກັບຄືນfalse()
    {
        PermissionCodes.Exists("rooms.does-not-exist").Should().BeFalse();
    }
}
