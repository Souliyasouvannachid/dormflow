using System.Security.Claims;
using DormFlow.Api.Services;
using DormFlow.Application.Common.Abstractions;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace DormFlow.UnitTests.Api;

/// <summary>
/// ตรวจตรรกะสิทธิ์ที่อ่านจาก Claim ของ Access Token
/// นี่คือจุดที่ตัดสินว่าผู้ใช้เห็นข้อมูลขององค์กรและหอพักใดได้บ้าง จึงต้องมีเทสต์คุมทุกเส้นทาง
/// </summary>
public class CurrentUserTests
{
    private static readonly Guid UserId = Guid.CreateVersion7();
    private static readonly Guid OrganizationId = Guid.CreateVersion7();
    private static readonly Guid AssignedDormitoryId = Guid.CreateVersion7();
    private static readonly Guid OtherDormitoryId = Guid.CreateVersion7();

    [Fact]
    public void ผู้ใช้ที่ยังไม่ยืนยันตัวตน_ต้องไม่มีสิทธิ์ใด()
    {
        ICurrentUser currentUser = CreateAnonymous();

        currentUser.IsAuthenticated.Should().BeFalse();
        currentUser.UserId.Should().BeNull();
        currentUser.OrganizationId.Should().BeNull();
        currentUser.IsSuperAdmin.Should().BeFalse();
        currentUser.Permissions.Should().BeEmpty();
        currentUser.DormitoryIds.Should().BeEmpty();
        currentUser.HasPermission("rooms.view").Should().BeFalse();
        currentUser.CanAccessDormitory(AssignedDormitoryId).Should().BeFalse();
    }

    [Fact]
    public void ผู้ใช้ที่ยืนยันตัวตนแล้ว_ต้องอ่าน_UserId_และ_OrganizationId_ได้()
    {
        ICurrentUser currentUser = CreateAuthenticated(DormFlowRoles.Staff);

        currentUser.IsAuthenticated.Should().BeTrue();
        currentUser.UserId.Should().Be(UserId);
        currentUser.OrganizationId.Should().Be(OrganizationId);
    }

    [Fact]
    public void สิทธิ์ที่มีใน_Claim_เท่านั้นที่ต้องผ่าน()
    {
        ICurrentUser currentUser = CreateAuthenticated(
            DormFlowRoles.Staff,
            permissions: ["rooms.view", "rooms.create"]);

        currentUser.HasPermission("rooms.view").Should().BeTrue();
        currentUser.HasPermission("rooms.create").Should().BeTrue();
        currentUser.HasPermission("rooms.update").Should().BeFalse();
        currentUser.HasPermission("invoices.cancel").Should().BeFalse();
    }

    [Fact]
    public void SuperAdmin_ต้องมีทุกสิทธิ์แม้ไม่มี_Permission_Claim()
    {
        ICurrentUser currentUser = CreateAuthenticated(DormFlowRoles.SuperAdmin);

        currentUser.IsSuperAdmin.Should().BeTrue();
        currentUser.HasPermission("settings.manage").Should().BeTrue();
        currentUser.HasPermission("audit-logs.view").Should().BeTrue();
    }

    [Fact]
    public void SuperAdmin_ต้องเข้าถึงได้ทุกหอพัก()
    {
        ICurrentUser currentUser = CreateAuthenticated(DormFlowRoles.SuperAdmin);

        currentUser.CanAccessDormitory(OtherDormitoryId).Should().BeTrue();
    }

    [Fact]
    public void Owner_ต้องเข้าถึงได้ทุกหอพักในองค์กรของตน()
    {
        ICurrentUser currentUser = CreateAuthenticated(DormFlowRoles.Owner);

        currentUser.CanAccessDormitory(OtherDormitoryId).Should().BeTrue();
    }

    [Fact]
    public void เจ้าหน้าที่_ต้องเข้าถึงได้เฉพาะหอพักที่ได้รับมอบหมาย()
    {
        ICurrentUser currentUser = CreateAuthenticated(
            DormFlowRoles.DormManager,
            dormitoryIds: [AssignedDormitoryId]);

        currentUser.DormitoryIds.Should().ContainSingle().Which.Should().Be(AssignedDormitoryId);
        currentUser.CanAccessDormitory(AssignedDormitoryId).Should().BeTrue();
        currentUser.CanAccessDormitory(OtherDormitoryId).Should().BeFalse();
    }

    [Fact]
    public void Claim_หอพักที่ไม่ใช่_UUID_ต้องถูกข้ามไป()
    {
        ClaimsIdentity identity = CreateIdentity(DormFlowRoles.Staff);
        identity.AddClaim(new Claim(DormFlowClaimTypes.DormitoryId, "ไม่ใช่ uuid"));
        identity.AddClaim(new Claim(DormFlowClaimTypes.DormitoryId, AssignedDormitoryId.ToString()));

        ICurrentUser currentUser = CreateCurrentUser(new ClaimsPrincipal(identity));

        currentUser.DormitoryIds.Should().ContainSingle().Which.Should().Be(AssignedDormitoryId);
    }

    [Fact]
    public void HasPermission_ต้องไม่รับรหัสสิทธิ์ที่ว่าง()
    {
        ICurrentUser currentUser = CreateAuthenticated(DormFlowRoles.Staff);

        Action act = () => currentUser.HasPermission("  ");

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void IpAddress_และ_UserAgent_ต้องอ่านจากคำขอปัจจุบัน()
    {
        DefaultHttpContext context = new();
        context.User = new ClaimsPrincipal(CreateIdentity(DormFlowRoles.Staff));
        context.Connection.RemoteIpAddress = System.Net.IPAddress.Parse("203.0.113.10");
        context.Request.Headers.UserAgent = "DormFlow-Test/1.0";

        CurrentUser currentUser = new(new HttpContextAccessor { HttpContext = context });

        currentUser.IpAddress.Should().Be("203.0.113.10");
        currentUser.UserAgent.Should().Be("DormFlow-Test/1.0");
    }

    private static CurrentUser CreateAnonymous()
        => new CurrentUser(new HttpContextAccessor { HttpContext = new DefaultHttpContext() });

    private static CurrentUser CreateAuthenticated(
        string role,
        IEnumerable<string>? permissions = null,
        IEnumerable<Guid>? dormitoryIds = null)
    {
        ClaimsIdentity identity = CreateIdentity(role);

        foreach (string permission in permissions ?? [])
        {
            identity.AddClaim(new Claim(DormFlowClaimTypes.Permission, permission));
        }

        foreach (Guid dormitoryId in dormitoryIds ?? [])
        {
            identity.AddClaim(new Claim(DormFlowClaimTypes.DormitoryId, dormitoryId.ToString()));
        }

        return CreateCurrentUser(new ClaimsPrincipal(identity));
    }

    private static ClaimsIdentity CreateIdentity(string role)
        => new(
            [
                new Claim(DormFlowClaimTypes.UserId, UserId.ToString()),
                new Claim(DormFlowClaimTypes.OrganizationId, OrganizationId.ToString()),
                new Claim(ClaimTypes.Role, role),
            ],
            authenticationType: "TestScheme",
            nameType: ClaimTypes.Name,
            roleType: ClaimTypes.Role);

    private static CurrentUser CreateCurrentUser(ClaimsPrincipal principal)
    {
        DefaultHttpContext context = new() { User = principal };
        return new CurrentUser(new HttpContextAccessor { HttpContext = context });
    }
}
