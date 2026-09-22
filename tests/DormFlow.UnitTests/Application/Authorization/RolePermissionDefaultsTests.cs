using DormFlow.Application.Common.Abstractions;
using DormFlow.Application.Common.Authorization;
using FluentAssertions;
using Xunit;

namespace DormFlow.UnitTests.Application.Authorization;

/// <summary>ກວດສອບວ່າການອະນຸຍາດເລີ່ມຕົ້ນຂອງແຕ່ລະບົດບາດສອດຄ່ອງກັບລະຫັດການອະນຸຍາດຕົວຈິງ</summary>
public class RolePermissionDefaultsTests
{
    [Fact]
    public void ບົດບາດມາດຕະຖານທັງຝົດ_ຕ້ອງການຄໍານິຍາມການອະນຸຍາດເລີ່ມຕົ້ນ()
    {
        RolePermissionDefaults.ByRole.Keys.Should().BeEquivalentTo(DormFlowRoles.All);
    }

    [Fact]
    public void SuperAdmin_ຕ້ອງໄດ້ຮັບສິດທັງຝົດທີ່ມີຢູ່ໃນລະບົບ()
    {
        RolePermissionDefaults.ForRole(DormFlowRoles.SuperAdmin)
            .Should().BeEquivalentTo(PermissionCodes.AllCodes);
    }

    [Fact]
    public void Tenant_ຕ້ອງບໍ່ມີສິດຜູ້ເບິ່ງແຍງລະບົບເລີຍ()
    {
        RolePermissionDefaults.ForRole(DormFlowRoles.Tenant).Should().BeEmpty();
    }

    [Fact]
    public void ທຸກໆສິດໃນທຸກໆບົດບາດ_ຕ້ອງເປັນລະຫັດການອະນຸຍາດທີ່ມີຢູ່ໃນລະບົບ()
    {
        IEnumerable<string> allAssignedPermissions = RolePermissionDefaults.ByRole.Values.SelectMany(codes => codes);

        allAssignedPermissions.Should().OnlyContain(code => PermissionCodes.Exists(code));
    }

    [Fact]
    public void ForRole_ເມື່ອບໍ່ພົບບົດບາດ_ລາຍການເປົ່າຕ້ອງຖືກສົ່ງຄືນ()
    {
        RolePermissionDefaults.ForRole("NotARealRole").Should().BeEmpty();
    }

    [Fact]
    public void Owner_ຕ້ອງບໍ່ມີການອະນຸຍາດໃຫ້ສ້າງຫຼືລຶບອົງກອນ()
    {
        IReadOnlyList<string> ownerPermissions = RolePermissionDefaults.ForRole(DormFlowRoles.Owner);

        ownerPermissions.Should().NotContain(PermissionCodes.Organizations.Create);
        ownerPermissions.Should().NotContain(PermissionCodes.Organizations.Delete);
    }
}
