using DormFlow.Application.Common.Authorization;
using FluentAssertions;
using Xunit;

namespace DormFlow.UnitTests.Application.Authorization;

/// <summary>ตรวจความสมบูรณ์ของรายการรหัสสิทธิ์</summary>
public class PermissionCodesTests
{
    [Fact]
    public void All_ต้องไม่มีรหัสสิทธิ์ซ้ำกัน()
    {
        PermissionCodes.All.Select(definition => definition.Code)
            .Should().OnlyHaveUniqueItems();
    }

    [Fact]
    public void ทุกสิทธิ์_ต้องมีโมดูลกำกับ()
    {
        PermissionCodes.All.Should().OnlyContain(definition => !string.IsNullOrWhiteSpace(definition.Module));
    }

    [Fact]
    public void AllCodes_ต้องมีจำนวนเท่ากับ_All()
    {
        PermissionCodes.AllCodes.Should().HaveCount(PermissionCodes.All.Count);
    }

    [Fact]
    public void Exists_เมื่อรหัสสิทธิ์มีอยู่จริง_ต้องคืนค่าจริง()
    {
        PermissionCodes.Exists(PermissionCodes.Rooms.Create).Should().BeTrue();
    }

    [Fact]
    public void Exists_เมื่อรหัสสิทธิ์ไม่มีอยู่จริง_ต้องคืนค่าเท็จ()
    {
        PermissionCodes.Exists("rooms.does-not-exist").Should().BeFalse();
    }
}
