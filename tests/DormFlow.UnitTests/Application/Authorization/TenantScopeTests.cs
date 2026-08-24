using DormFlow.Application.Common.Abstractions;
using DormFlow.Application.Common.Authorization;
using DormFlow.Application.Common.Exceptions;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace DormFlow.UnitTests.Application.Authorization;

/// <summary>
/// ตรวจตรรกะกลางที่บังคับขอบเขตข้อมูลตามองค์กรและหอพัก
/// เป็นจุดเดียวที่ทุกโมดูล (หอพัก, อาคาร, ชั้น, ประเภทห้อง, ห้องพัก, ผู้ใช้) พึ่งพาเพื่อกันข้อมูลข้ามองค์กร
/// </summary>
public class TenantScopeTests
{
    private static readonly Guid OrganizationId = Guid.CreateVersion7();
    private static readonly Guid OtherOrganizationId = Guid.CreateVersion7();
    private static readonly Guid DormitoryId = Guid.CreateVersion7();
    private static readonly Guid OtherDormitoryId = Guid.CreateVersion7();

    [Fact]
    public void ResolveOrganizationIdForCreate_SuperAdmin_ต้องใช้ค่าที่ระบุมาในคำขอ()
    {
        ICurrentUser currentUser = CreateUser(isSuperAdmin: true);

        Guid result = TenantScope.ResolveOrganizationIdForCreate(currentUser, OrganizationId);

        result.Should().Be(OrganizationId);
    }

    [Fact]
    public void ResolveOrganizationIdForCreate_SuperAdmin_ที่ไม่ระบุองค์กร_ต้องโยนข้อผิดพลาดการตรวจสอบ()
    {
        ICurrentUser currentUser = CreateUser(isSuperAdmin: true);

        Action act = () => TenantScope.ResolveOrganizationIdForCreate(currentUser, null);

        act.Should().Throw<ValidationFailedException>();
    }

    [Fact]
    public void ResolveOrganizationIdForCreate_ผู้ใช้ทั่วไป_ต้องถูกบังคับใช้องค์กรของตนเองเสมอ()
    {
        ICurrentUser currentUser = CreateUser(organizationId: OrganizationId);

        // แม้ส่งองค์กรอื่นมาในคำขอ ผลลัพธ์ต้องเป็นองค์กรของผู้ใช้เองเท่านั้น ป้องกัน Mass Assignment ข้ามองค์กร
        Guid result = TenantScope.ResolveOrganizationIdForCreate(currentUser, OtherOrganizationId);

        result.Should().Be(OrganizationId);
    }

    [Fact]
    public void ResolveOrganizationIdForCreate_ผู้ใช้ที่ไม่สังกัดองค์กรใด_ต้องถูกปฏิเสธ()
    {
        ICurrentUser currentUser = CreateUser(organizationId: null);

        Action act = () => TenantScope.ResolveOrganizationIdForCreate(currentUser, OrganizationId);

        act.Should().Throw<ForbiddenAccessException>();
    }

    [Fact]
    public void EnsureOrganizationAccess_SuperAdmin_ต้องเข้าถึงได้ทุกองค์กร()
    {
        ICurrentUser currentUser = CreateUser(isSuperAdmin: true);

        Action act = () => TenantScope.EnsureOrganizationAccess(currentUser, OtherOrganizationId, "not found");

        act.Should().NotThrow();
    }

    [Fact]
    public void EnsureOrganizationAccess_องค์กรตรงกับผู้ใช้_ต้องผ่าน()
    {
        ICurrentUser currentUser = CreateUser(organizationId: OrganizationId);

        Action act = () => TenantScope.EnsureOrganizationAccess(currentUser, OrganizationId, "not found");

        act.Should().NotThrow();
    }

    [Fact]
    public void EnsureOrganizationAccess_องค์กรต่างจากผู้ใช้_ต้องโยน_NotFoundException()
    {
        ICurrentUser currentUser = CreateUser(organizationId: OrganizationId);

        Action act = () => TenantScope.EnsureOrganizationAccess(currentUser, OtherOrganizationId, "ไม่พบข้อมูล");

        // ตอบเป็นไม่พบข้อมูล (404) แทนไม่มีสิทธิ์ (403) เพื่อไม่ให้เดาได้ว่ามีข้อมูลอยู่จริงในองค์กรอื่น
        act.Should().Throw<NotFoundException>().WithMessage("ไม่พบข้อมูล");
    }

    [Fact]
    public void EnsureDormitoryAccess_องค์กรตรงและเข้าถึงหอพักได้_ต้องผ่าน()
    {
        ICurrentUser currentUser = CreateUser(organizationId: OrganizationId, canAccessDormitory: true);

        Action act = () => TenantScope.EnsureDormitoryAccess(currentUser, OrganizationId, DormitoryId, "not found");

        act.Should().NotThrow();
    }

    [Fact]
    public void EnsureDormitoryAccess_องค์กรตรงแต่เข้าถึงหอพักไม่ได้_ต้องโยนข้อผิดพลาด()
    {
        ICurrentUser currentUser = CreateUser(organizationId: OrganizationId, canAccessDormitory: false);

        Action act = () => TenantScope.EnsureDormitoryAccess(currentUser, OrganizationId, DormitoryId, "not found");

        act.Should().Throw<NotFoundException>();
    }

    [Fact]
    public void EnsureDormitoryAccess_เข้าถึงหอพักได้แต่ต่างองค์กร_ต้องโยนข้อผิดพลาด()
    {
        // ครอบคลุมกรณีที่ CanAccessDormitory คืนค่า true (เช่น รายการ Claim หลุดมาผิด)
        // แต่หอพักนั้นไม่ได้อยู่ในองค์กรที่ตรวจสอบ ต้องยังคงถูกปฏิเสธอยู่ดี
        ICurrentUser currentUser = CreateUser(organizationId: OtherOrganizationId, canAccessDormitory: true);

        Action act = () => TenantScope.EnsureDormitoryAccess(currentUser, OrganizationId, DormitoryId, "not found");

        act.Should().Throw<NotFoundException>();
    }

    [Fact]
    public void GetDormitoryRestrictionForListing_SuperAdmin_ต้องไม่ถูกจำกัด()
    {
        ICurrentUser currentUser = CreateUser(isSuperAdmin: true);

        TenantScope.GetDormitoryRestrictionForListing(currentUser).Should().BeNull();
    }

    [Fact]
    public void GetDormitoryRestrictionForListing_Owner_ต้องไม่ถูกจำกัด()
    {
        ICurrentUser currentUser = CreateUser(roles: [DormFlowRoles.Owner]);

        TenantScope.GetDormitoryRestrictionForListing(currentUser).Should().BeNull();
    }

    [Fact]
    public void GetDormitoryRestrictionForListing_DormManager_ต้องจำกัดเฉพาะหอพักที่ได้รับมอบหมาย()
    {
        ICurrentUser currentUser = CreateUser(
            roles: [DormFlowRoles.DormManager],
            dormitoryIds: [DormitoryId]);

        IReadOnlyCollection<Guid>? restriction = TenantScope.GetDormitoryRestrictionForListing(currentUser);

        restriction.Should().BeEquivalentTo([DormitoryId]);
    }

    [Fact]
    public void GetDormitoryRestrictionForListing_ผู้ใช้ที่ยังไม่ได้รับมอบหมายหอพักใด_ต้องได้รายการว่าง()
    {
        ICurrentUser currentUser = CreateUser(roles: [DormFlowRoles.Staff], dormitoryIds: []);

        IReadOnlyCollection<Guid>? restriction = TenantScope.GetDormitoryRestrictionForListing(currentUser);

        restriction.Should().NotBeNull();
        restriction.Should().BeEmpty();
    }

    private static ICurrentUser CreateUser(
        bool isSuperAdmin = false,
        Guid? organizationId = null,
        bool canAccessDormitory = false,
        IReadOnlyCollection<string>? roles = null,
        IReadOnlyCollection<Guid>? dormitoryIds = null)
    {
        ICurrentUser currentUser = Substitute.For<ICurrentUser>();

        currentUser.IsSuperAdmin.Returns(isSuperAdmin);
        currentUser.OrganizationId.Returns(organizationId);
        currentUser.Roles.Returns(roles ?? []);
        currentUser.DormitoryIds.Returns(dormitoryIds ?? []);
        currentUser.CanAccessDormitory(Arg.Any<Guid>()).Returns(canAccessDormitory);

        return currentUser;
    }
}
