using System.Security.Claims;
using DormFlow.Api.Services;
using DormFlow.Application.Common.Abstractions;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace DormFlow.UnitTests.Api;

/// <summary>
/// ກວດເບິ່ງເຫດຜົນການອະນຸຍາດອ່ານຈາກ Claim ຂອງ Access Token
/// ນີ້ແມ່ນບ່ອນທີ່ມັນຖືກກໍານົດວ່າອົງກອນແລະຜູ້ນໍາໃຊ້ຂໍ້ມູນຫໍພັກໃດສາມາດເບິ່ງໄດ້. ດັ່ງນັ້ນ, ຕ້ອງມີການທົດສອບເພື່ອຄວບຄຸມທຸກໆເສັ້ນທາງ
/// </summary>
public class CurrentUserTests
{
    private static readonly Guid UserId = Guid.CreateVersion7();
    private static readonly Guid OrganizationId = Guid.CreateVersion7();
    private static readonly Guid AssignedDormitoryId = Guid.CreateVersion7();
    private static readonly Guid OtherDormitoryId = Guid.CreateVersion7();

    [Fact]
    public void ຜູ້ໃຊ້ທີ່ບໍ່ໄດ້ຢືນຢັນ_ຕ້ອງບໍ່ມີສິດໃດໆ()
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
    public void ຢືນຢັນຜູ້ໃຊ້_ຕ້ອງອ່ານ_UserId_ແລະ_OrganizationId_ແມ່ນແລ້ວ()
    {
        ICurrentUser currentUser = CreateAuthenticated(DormFlowRoles.Staff);

        currentUser.IsAuthenticated.Should().BeTrue();
        currentUser.UserId.Should().Be(UserId);
        currentUser.OrganizationId.Should().Be(OrganizationId);
    }

    [Fact]
    public void ສິດທີ່ມີຢູ່ໃນ_Claim_ນັ້ນແມ່ນທັງຝົດທີ່ເຈົ້າຕ້ອງຜ່ານ()
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
    public void SuperAdmin_ຕ້ອງມີສິດທຸກຢ່າງເຖິງແມ່ນວ່າເຈົ້າບໍ່ມີ_Permission_Claim()
    {
        ICurrentUser currentUser = CreateAuthenticated(DormFlowRoles.SuperAdmin);

        currentUser.IsSuperAdmin.Should().BeTrue();
        currentUser.HasPermission("settings.manage").Should().BeTrue();
        currentUser.HasPermission("audit-logs.view").Should().BeTrue();
    }

    [Fact]
    public void SuperAdmin_ຕ້ອງເຂົ້າໄດ້ທຸກຫໍພັກ()
    {
        ICurrentUser currentUser = CreateAuthenticated(DormFlowRoles.SuperAdmin);

        currentUser.CanAccessDormitory(OtherDormitoryId).Should().BeTrue();
    }

    [Fact]
    public void Owner_ຕ້ອງສາມາດເຂົ້າເຖິງທຸກຫໍພັກໃນອົງກອນຂອງພວກເຂົາ()
    {
        ICurrentUser currentUser = CreateAuthenticated(DormFlowRoles.Owner);

        currentUser.CanAccessDormitory(OtherDormitoryId).Should().BeTrue();
    }

    [Fact]
    public void ທາງການ_ຕ້ອງມີການເຂົ້າເຖິງພຽງແຕ່ຫໍພັກທີ່ໄດ້ຮັບມອບຝາຍ()
    {
        ICurrentUser currentUser = CreateAuthenticated(
            DormFlowRoles.DormManager,
            dormitoryIds: [AssignedDormitoryId]);

        currentUser.DormitoryIds.Should().ContainSingle().Which.Should().Be(AssignedDormitoryId);
        currentUser.CanAccessDormitory(AssignedDormitoryId).Should().BeTrue();
        currentUser.CanAccessDormitory(OtherDormitoryId).Should().BeFalse();
    }

    [Fact]
    public void Claim_ຫໍພັກທີ່ບໍ່ແມ່ນ_UUID_ຕ້ອງຂ້າມ()
    {
        ClaimsIdentity identity = CreateIdentity(DormFlowRoles.Staff);
        identity.AddClaim(new Claim(DormFlowClaimTypes.DormitoryId, "ບໍ່ uuid"));
        identity.AddClaim(new Claim(DormFlowClaimTypes.DormitoryId, AssignedDormitoryId.ToString()));

        ICurrentUser currentUser = CreateCurrentUser(new ClaimsPrincipal(identity));

        currentUser.DormitoryIds.Should().ContainSingle().Which.Should().Be(AssignedDormitoryId);
    }

    [Fact]
    public void HasPermission_ທ່ານຕ້ອງບໍ່ຍອມຮັບລະຫັດການອະນຸຍາດຫວ່າງເປົ່າ()
    {
        ICurrentUser currentUser = CreateAuthenticated(DormFlowRoles.Staff);

        Action act = () => currentUser.HasPermission("  ");

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void IpAddress_ແລະ_UserAgent_ຕ້ອງໄດ້ຮັບການອ່ານຈາກຄໍາຮ້ອງຂໍໃນປະຈຸບັນ()
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
