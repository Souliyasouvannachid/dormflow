using System.Reflection;
using DormFlow.Application.Common.Abstractions;
using DormFlow.Domain.Common;
using FluentAssertions;
using NetArchTest.Rules;
using TestResult = NetArchTest.Rules.TestResult;
using Xunit;

namespace DormFlow.ArchitectureTests;

/// <summary>ກວດເບິ່ງສົນທິສັນຍາການຕັ້ງຊື່ທີ່ແບ່ງປັນໂດຍທີມງານ</summary>
public class NamingConventionTests
{
    private static readonly Assembly DomainAssembly = typeof(BaseEntity).Assembly;
    private static readonly Assembly ApplicationAssembly = typeof(ICurrentUser).Assembly;

    [Fact]
    public void Exception_ທຸກໆລາຍການໃນ_Domain_ຕ້ອງສິ້ນສຸດດ້ວຍຄໍາ_Exception()
    {
        TestResult result = Types.InAssembly(DomainAssembly)
            .That()
            .Inherit(typeof(Exception))
            .Should()
            .HaveNameEndingWith("Exception")
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Exception_ທຸກໆລາຍການໃນ_Application_ຕ້ອງສິ້ນສຸດດ້ວຍຄໍາ_Exception()
    {
        TestResult result = Types.InAssembly(ApplicationAssembly)
            .That()
            .Inherit(typeof(Exception))
            .Should()
            .HaveNameEndingWith("Exception")
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Interface_ທຸກໆລາຍການໃນ_Application_ຕ້ອງເລີ່ມຕົ້ນດ້ວຍອັນດຽວ_I()
    {
        TestResult result = Types.InAssembly(ApplicationAssembly)
            .That()
            .AreInterfaces()
            .And()
            .ArePublic()
            .Should()
            .HaveNameStartingWith("I")
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Entity_ທັງຝົດທີ່ຖືກສືບທອດ_BaseEntity_ຕ້ອງຢູ່ໃນ_Domain()
    {
        TestResult result = Types.InAssembly(DomainAssembly)
            .That()
            .Inherit(typeof(BaseEntity))
            .Should()
            .ResideInNamespaceStartingWith("DormFlow.Domain")
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }
}
