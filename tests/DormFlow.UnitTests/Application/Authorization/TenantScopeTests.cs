using DormFlow.Application.Common.Abstractions;
using DormFlow.Application.Common.Authorization;
using DormFlow.Application.Common.Exceptions;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace DormFlow.UnitTests.Application.Authorization;

/// <summary>
/// ກວດເບິ່ງເຫດຜົນສູນກາງທີ່ຄວບຄຸມຂອບເຂດຂໍ້ມູນໂດຍອົງກອນແລະຫໍພັກ
/// ມັນເປັນຈຸດດຽວທີ່ທຸກໂມດູນ (ຫໍພັກ, ອາຄານ, ຊັ້ນ, ປະເພດຫ້ອງ, ຫ້ອງ, ຜູ້ໃຊ້) ອີງໃສ່ການປົກປ້ອງຂໍ້ມູນໃນທົ່ວອົງກອນ
/// </summary>
public class TenantScopeTests
{
    private static readonly Guid OrganizationId = Guid.CreateVersion7();
    private static readonly Guid OtherOrganizationId = Guid.CreateVersion7();
    private static readonly Guid DormitoryId = Guid.CreateVersion7();
    private static readonly Guid OtherDormitoryId = Guid.CreateVersion7();

    [Fact]
    public void ResolveOrganizationIdForCreate_SuperAdmin_ຄ່າທີ່ລະບຸໄວ້ໃນຄໍາຮ້ອງຂໍຕ້ອງຖືກນໍາໃຊ້()
    {
        ICurrentUser currentUser = CreateUser(isSuperAdmin: true);

        Guid result = TenantScope.ResolveOrganizationIdForCreate(currentUser, OrganizationId);

        result.Should().Be(OrganizationId);
    }

    [Fact]
    public void ResolveOrganizationIdForCreate_SuperAdmin_ອົງກອນບໍ່ໄດ້ລະບຸ_ຕ້ອງຖິ້ມຄວາມຜິດພາດໃນການກວດສອບ()
    {
        ICurrentUser currentUser = CreateUser(isSuperAdmin: true);

        Action act = () => TenantScope.ResolveOrganizationIdForCreate(currentUser, null);

        act.Should().Throw<ValidationFailedException>();
    }

    [Fact]
    public void ResolveOrganizationIdForCreate_ຜູ້ໃຊ້ທົ່ວໄປ_ອົງກອນຂອງຕົນເອງຕ້ອງໄດ້ຮັບການບັງຄັບໃຊ້ສະເຝີ()
    {
        ICurrentUser currentUser = CreateUser(organizationId: OrganizationId);

        // ເຖິງແມ່ນວ່າອົງກອນອື່ນຖືກສົ່ງໃນຄໍາຮ້ອງຂໍຜົນໄດ້ຮັບຈະຕ້ອງເປັນຂອງອົງກອນຂອງຜູ້ໃຊ້ເອງເທົ່ານັ້ນ Mass Assignment ໃນທົ່ວອົງກອນ
        Guid result = TenantScope.ResolveOrganizationIdForCreate(currentUser, OtherOrganizationId);

        result.Should().Be(OrganizationId);
    }

    [Fact]
    public void ResolveOrganizationIdForCreate_ຜູ້ໃຊ້ທີ່ບໍ່ກ່ຽວຂ້ອງກັບອົງກອນໃດໆ_ຕ້ອງໄດ້ຮັບການປະຕິເສດ()
    {
        ICurrentUser currentUser = CreateUser(organizationId: null);

        Action act = () => TenantScope.ResolveOrganizationIdForCreate(currentUser, OrganizationId);

        act.Should().Throw<ForbiddenAccessException>();
    }

    [Fact]
    public void EnsureOrganizationAccess_SuperAdmin_ຕ້ອງສາມາດເຂົ້າເຖິງທຸກອົງກອນ()
    {
        ICurrentUser currentUser = CreateUser(isSuperAdmin: true);

        Action act = () => TenantScope.EnsureOrganizationAccess(currentUser, OtherOrganizationId, "not found");

        act.Should().NotThrow();
    }

    [Fact]
    public void EnsureOrganizationAccess_ອົງກອນພົບກັບຜູ້ໃຊ້_ຕ້ອງຜ່ານ()
    {
        ICurrentUser currentUser = CreateUser(organizationId: OrganizationId);

        Action act = () => TenantScope.EnsureOrganizationAccess(currentUser, OrganizationId, "not found");

        act.Should().NotThrow();
    }

    [Fact]
    public void EnsureOrganizationAccess_ອົງກອນແມ່ນແຕກຕ່າງຈາກຜູ້ໃຊ້_ຕ້ອງຖິ້ມ_NotFoundException()
    {
        ICurrentUser currentUser = CreateUser(organizationId: OrganizationId);

        Action act = () => TenantScope.EnsureOrganizationAccess(currentUser, OtherOrganizationId, "ບໍ່ພົບຂໍ້ມູນ");

        // ຄໍາຕອບແມ່ນວ່າບໍ່ພົບຂໍ້ມູນໃດໆ (404) ແທນທີ່ຈະບໍ່ມີສິດ (403) ເພື່ອບໍ່ໃຫ້ຄາດເດົາວ່າຂໍ້ມູນນັ້ນມີຢູ່ໃນອົງກອນອື່ນ
        act.Should().Throw<NotFoundException>().WithMessage("ບໍ່ພົບຂໍ້ມູນ");
    }

    [Fact]
    public void EnsureDormitoryAccess_ອົງກອນໂດຍກົງແລະຫໍພັກທີ່ສາມາດເຂົ້າເຖິງໄດ້_ຕ້ອງຜ່ານ()
    {
        ICurrentUser currentUser = CreateUser(organizationId: OrganizationId, canAccessDormitory: true);

        Action act = () => TenantScope.EnsureDormitoryAccess(currentUser, OrganizationId, DormitoryId, "not found");

        act.Should().NotThrow();
    }

    [Fact]
    public void EnsureDormitoryAccess_ອົງກອນໂດຍກົງແຕ່ບໍ່ສາມາດເຂົ້າເຖິງຫໍພັກໄດ້_ຕ້ອງຖິ້ມຄວາມຜິດພາດ()
    {
        ICurrentUser currentUser = CreateUser(organizationId: OrganizationId, canAccessDormitory: false);

        Action act = () => TenantScope.EnsureDormitoryAccess(currentUser, OrganizationId, DormitoryId, "not found");

        act.Should().Throw<NotFoundException>();
    }

    [Fact]
    public void EnsureDormitoryAccess_ການເຂົ້າເຖິງຫໍພັກແມ່ນມີຢູ່ຈາກອົງກອນຕ່າງໆ_ຕ້ອງຖິ້ມຄວາມຜິດພາດ()
    {
        // ກວມເອົາກໍລະນີທີ່ CanAccessDormitory ຟື້ນຟູ true (ເຊັ່ນ: ລາຍການ Claim ມັນອອກມາຜິດ)
        // ແຕ່ຫໍພັກບໍ່ໄດ້ລວມຢູ່ໃນອົງກອນກວດກາ. ຍັງຕ້ອງຖືກປະຕິເສດ
        ICurrentUser currentUser = CreateUser(organizationId: OtherOrganizationId, canAccessDormitory: true);

        Action act = () => TenantScope.EnsureDormitoryAccess(currentUser, OrganizationId, DormitoryId, "not found");

        act.Should().Throw<NotFoundException>();
    }

    [Fact]
    public void GetDormitoryRestrictionForListing_SuperAdmin_ຕ້ອງບໍ່ໄດ້ຖືກຈໍາກັດ()
    {
        ICurrentUser currentUser = CreateUser(isSuperAdmin: true);

        TenantScope.GetDormitoryRestrictionForListing(currentUser).Should().BeNull();
    }

    [Fact]
    public void GetDormitoryRestrictionForListing_Owner_ຕ້ອງບໍ່ໄດ້ຖືກຈໍາກັດ()
    {
        ICurrentUser currentUser = CreateUser(roles: [DormFlowRoles.Owner]);

        TenantScope.GetDormitoryRestrictionForListing(currentUser).Should().BeNull();
    }

    [Fact]
    public void GetDormitoryRestrictionForListing_DormManager_ຕ້ອງຖືກຈຳກັດໃຫ້ຢູ່ໃນຫໍພັກທີ່ຖືກມອບໝາຍ()
    {
        ICurrentUser currentUser = CreateUser(
            roles: [DormFlowRoles.DormManager],
            dormitoryIds: [DormitoryId]);

        IReadOnlyCollection<Guid>? restriction = TenantScope.GetDormitoryRestrictionForListing(currentUser);

        restriction.Should().BeEquivalentTo([DormitoryId]);
    }

    [Fact]
    public void GetDormitoryRestrictionForListing_ຜູ້ໃຊ້ທີ່ຍັງບໍ່ທັນໄດ້ຮັບການມອບຝາຍຫໍພັກໃດໆ_ທ່ານຕ້ອງໄດ້ຮັບບັນຊີລາຍຊື່ເປົ່າ()
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
