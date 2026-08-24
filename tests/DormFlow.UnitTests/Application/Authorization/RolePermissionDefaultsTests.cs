using DormFlow.Application.Common.Abstractions;
using DormFlow.Application.Common.Authorization;
using FluentAssertions;
using Xunit;

namespace DormFlow.UnitTests.Application.Authorization;

/// <summary>ตรวจว่าสิทธิ์เริ่มต้นของแต่ละบทบาทสอดคล้องกับรหัสสิทธิ์ที่มีอยู่จริง</summary>
public class RolePermissionDefaultsTests
{
    [Fact]
    public void ทุกบทบาทมาตรฐาน_ต้องมีนิยามสิทธิ์เริ่มต้น()
    {
        RolePermissionDefaults.ByRole.Keys.Should().BeEquivalentTo(DormFlowRoles.All);
    }

    [Fact]
    public void SuperAdmin_ต้องได้รับทุกสิทธิ์ที่มีอยู่ในระบบ()
    {
        RolePermissionDefaults.ForRole(DormFlowRoles.SuperAdmin)
            .Should().BeEquivalentTo(PermissionCodes.AllCodes);
    }

    [Fact]
    public void Tenant_ต้องไม่มีสิทธิ์ฝั่งผู้ดูแลเลย()
    {
        RolePermissionDefaults.ForRole(DormFlowRoles.Tenant).Should().BeEmpty();
    }

    [Fact]
    public void ทุกสิทธิ์ในทุกบทบาท_ต้องเป็นรหัสสิทธิ์ที่มีอยู่จริงในระบบ()
    {
        IEnumerable<string> allAssignedPermissions = RolePermissionDefaults.ByRole.Values.SelectMany(codes => codes);

        allAssignedPermissions.Should().OnlyContain(code => PermissionCodes.Exists(code));
    }

    [Fact]
    public void ForRole_เมื่อไม่พบบทบาท_ต้องคืนรายการว่าง()
    {
        RolePermissionDefaults.ForRole("NotARealRole").Should().BeEmpty();
    }

    [Fact]
    public void Owner_ต้องไม่มีสิทธิ์สร้างหรือลบองค์กร()
    {
        IReadOnlyList<string> ownerPermissions = RolePermissionDefaults.ForRole(DormFlowRoles.Owner);

        ownerPermissions.Should().NotContain(PermissionCodes.Organizations.Create);
        ownerPermissions.Should().NotContain(PermissionCodes.Organizations.Delete);
    }
}
