using System.Reflection;
using DormFlow.Application.Common.Abstractions;
using DormFlow.Domain.Common;
using FluentAssertions;
using NetArchTest.Rules;
using TestResult = NetArchTest.Rules.TestResult;
using Xunit;

namespace DormFlow.ArchitectureTests;

/// <summary>ตรวจข้อตกลงการตั้งชื่อที่ทีมใช้ร่วมกัน</summary>
public class NamingConventionTests
{
    private static readonly Assembly DomainAssembly = typeof(BaseEntity).Assembly;
    private static readonly Assembly ApplicationAssembly = typeof(ICurrentUser).Assembly;

    [Fact]
    public void Exception_ทุกตัวใน_Domain_ต้องลงท้ายด้วยคำว่า_Exception()
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
    public void Exception_ทุกตัวใน_Application_ต้องลงท้ายด้วยคำว่า_Exception()
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
    public void Interface_ทุกตัวใน_Application_ต้องขึ้นต้นด้วยตัว_I()
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
    public void Entity_ทุกตัวที่สืบทอด_BaseEntity_ต้องอยู่ใน_Domain()
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
