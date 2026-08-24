using System.Reflection;
using DormFlow.Api.Middleware;
using DormFlow.Application.Common.Abstractions;
using DormFlow.Contracts.Common;
using DormFlow.Domain.Common;
using DormFlow.Infrastructure.Persistence;
using FluentAssertions;
using NetArchTest.Rules;
using TestResult = NetArchTest.Rules.TestResult;
using Xunit;

namespace DormFlow.ArchitectureTests;

/// <summary>
/// บังคับทิศทางการพึ่งพาระหว่าง Layer ตามหลัก Clean Architecture
/// </summary>
/// <remarks>
/// เทสต์ชุดนี้ทำให้การเผลอเพิ่ม ProjectReference ผิดทิศทางกลายเป็น build ที่ล้มเหลว
/// แทนที่จะถูกพบตอน review หรือไม่ถูกพบเลย
/// </remarks>
public class LayerDependencyTests
{
    private static readonly Assembly DomainAssembly = typeof(BaseEntity).Assembly;
    private static readonly Assembly ApplicationAssembly = typeof(ICurrentUser).Assembly;
    private static readonly Assembly ContractsAssembly = typeof(ApiResponse).Assembly;
    private static readonly Assembly InfrastructureAssembly = typeof(DormFlowDbContext).Assembly;
    private static readonly Assembly ApiAssembly = typeof(GlobalExceptionHandlingMiddleware).Assembly;

    private const string DomainNamespace = "DormFlow.Domain";
    private const string ApplicationNamespace = "DormFlow.Application";
    private const string ContractsNamespace = "DormFlow.Contracts";
    private const string InfrastructureNamespace = "DormFlow.Infrastructure";
    private const string ApiNamespace = "DormFlow.Api";

    [Fact]
    public void Domain_ต้องไม่พึ่งพา_Layer_อื่นเลย()
    {
        TestResult result = Types.InAssembly(DomainAssembly)
            .ShouldNot()
            .HaveDependencyOnAny(ApplicationNamespace, InfrastructureNamespace, ApiNamespace, ContractsNamespace)
            .GetResult();

        AssertSuccess(result);
    }

    [Fact]
    public void Domain_ต้องไม่รู้จัก_Entity_Framework_หรือ_ASP_NET_Core()
    {
        TestResult result = Types.InAssembly(DomainAssembly)
            .ShouldNot()
            .HaveDependencyOnAny("Microsoft.EntityFrameworkCore", "Microsoft.AspNetCore", "Npgsql")
            .GetResult();

        AssertSuccess(result);
    }

    [Fact]
    public void Application_ต้องไม่พึ่งพา_Infrastructure_หรือ_Api()
    {
        TestResult result = Types.InAssembly(ApplicationAssembly)
            .ShouldNot()
            .HaveDependencyOnAny(InfrastructureNamespace, ApiNamespace)
            .GetResult();

        AssertSuccess(result);
    }

    [Fact]
    public void Application_ต้องไม่รู้จัก_Entity_Framework_หรือ_ASP_NET_Core()
    {
        TestResult result = Types.InAssembly(ApplicationAssembly)
            .ShouldNot()
            .HaveDependencyOnAny("Microsoft.EntityFrameworkCore", "Microsoft.AspNetCore", "Npgsql")
            .GetResult();

        AssertSuccess(result);
    }

    [Fact]
    public void Contracts_ต้องเป็นสัญญาล้วนและไม่พึ่งพา_Layer_ใด()
    {
        TestResult result = Types.InAssembly(ContractsAssembly)
            .ShouldNot()
            .HaveDependencyOnAny(DomainNamespace, ApplicationNamespace, InfrastructureNamespace, ApiNamespace)
            .GetResult();

        AssertSuccess(result);
    }

    [Fact]
    public void Infrastructure_ต้องไม่พึ่งพา_Api()
    {
        TestResult result = Types.InAssembly(InfrastructureAssembly)
            .ShouldNot()
            .HaveDependencyOn(ApiNamespace)
            .GetResult();

        AssertSuccess(result);
    }

    [Fact]
    public void DbContext_ต้องอยู่ใน_Infrastructure_เท่านั้น()
    {
        Assembly[] assembliesThatMustNotHaveDbContext = [DomainAssembly, ApplicationAssembly, ContractsAssembly, ApiAssembly];

        foreach (Assembly assembly in assembliesThatMustNotHaveDbContext)
        {
            IEnumerable<Type> dbContextTypes = Types.InAssembly(assembly)
                .That()
                .Inherit(typeof(Microsoft.EntityFrameworkCore.DbContext))
                .GetTypes();

            dbContextTypes.Should().BeEmpty(
                "โปรเจกต์ {0} ต้องไม่มี DbContext เพราะการเข้าถึงฐานข้อมูลเป็นหน้าที่ของ Infrastructure เท่านั้น",
                assembly.GetName().Name);
        }
    }

    private static void AssertSuccess(TestResult result, string? context = null)
    {
        string failing = result.FailingTypeNames is null
            ? string.Empty
            : string.Join(", ", result.FailingTypeNames);

        result.IsSuccessful.Should().BeTrue(
            "ชนิดต่อไปนี้ละเมิดกฎการพึ่งพาระหว่าง Layer{0}: {1}",
            context is null ? string.Empty : $" ({context})",
            failing);
    }
}
