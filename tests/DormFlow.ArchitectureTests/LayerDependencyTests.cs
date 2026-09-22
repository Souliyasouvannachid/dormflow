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
/// ການຂື້ນກັບໂດຍກົງລະຫວ່າງ Layer ຕາມຫຼັກການ Clean Architecture
/// </summary>
/// <remarks>
/// ຊຸດຂອງການທົດສອບນີ້ເພີ່ມຄວາມສ່ຽງຂອງການບໍ່ຕັ້ງໃຈ ProjectReference ກາຍເປັນໃນທິດທາງທີ່ບໍ່ຖືກຕ້ອງ build ລົ້ມເຫຼວ
/// ແທນທີ່ຈະຖືກຄົ້ນພົບ review ຫຼືບໍ່ພົບເລີຍ
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
    public void Domain_ບໍ່ຕ້ອງຂຶ້ນກັບ_Layer_ບາງສິ່ງບາງຢ່າງອື່ນ()
    {
        TestResult result = Types.InAssembly(DomainAssembly)
            .ShouldNot()
            .HaveDependencyOnAny(ApplicationNamespace, InfrastructureNamespace, ApiNamespace, ContractsNamespace)
            .GetResult();

        AssertSuccess(result);
    }

    [Fact]
    public void Domain_ຕ້ອງບໍ່ຮູ້_Entity_Framework_ຫຼື_ASP_NET_Core()
    {
        TestResult result = Types.InAssembly(DomainAssembly)
            .ShouldNot()
            .HaveDependencyOnAny("Microsoft.EntityFrameworkCore", "Microsoft.AspNetCore", "Npgsql")
            .GetResult();

        AssertSuccess(result);
    }

    [Fact]
    public void Application_ບໍ່ຕ້ອງຂຶ້ນກັບ_Infrastructure_ຫຼື_Api()
    {
        TestResult result = Types.InAssembly(ApplicationAssembly)
            .ShouldNot()
            .HaveDependencyOnAny(InfrastructureNamespace, ApiNamespace)
            .GetResult();

        AssertSuccess(result);
    }

    [Fact]
    public void Application_ຕ້ອງບໍ່ຮູ້_Entity_Framework_ຫຼື_ASP_NET_Core()
    {
        TestResult result = Types.InAssembly(ApplicationAssembly)
            .ShouldNot()
            .HaveDependencyOnAny("Microsoft.EntityFrameworkCore", "Microsoft.AspNetCore", "Npgsql")
            .GetResult();

        AssertSuccess(result);
    }

    [Fact]
    public void Contracts_ມັນຕ້ອງເປັນສັນຍາບໍລິສຸດແລະບໍ່ຂຶ້ນກັບ_Layer_ອັນໃດ()
    {
        TestResult result = Types.InAssembly(ContractsAssembly)
            .ShouldNot()
            .HaveDependencyOnAny(DomainNamespace, ApplicationNamespace, InfrastructureNamespace, ApiNamespace)
            .GetResult();

        AssertSuccess(result);
    }

    [Fact]
    public void Infrastructure_ບໍ່ຕ້ອງຂຶ້ນກັບ_Api()
    {
        TestResult result = Types.InAssembly(InfrastructureAssembly)
            .ShouldNot()
            .HaveDependencyOn(ApiNamespace)
            .GetResult();

        AssertSuccess(result);
    }

    [Fact]
    public void DbContext_ຕ້ອງຢູ່ໃນ_Infrastructure_ເທົ່ານັ້ນ()
    {
        Assembly[] assembliesThatMustNotHaveDbContext = [DomainAssembly, ApplicationAssembly, ContractsAssembly, ApiAssembly];

        foreach (Assembly assembly in assembliesThatMustNotHaveDbContext)
        {
            IEnumerable<Type> dbContextTypes = Types.InAssembly(assembly)
                .That()
                .Inherit(typeof(Microsoft.EntityFrameworkCore.DbContext))
                .GetTypes();

            dbContextTypes.Should().BeEmpty(
                "ໂຄງການ {0} ບໍ່ຕ້ອງມີ DbContext ເນື່ອງຈາກວ່າການເຂົ້າເຖິງຖານຂໍ້ມູນແມ່ນຜ້າທີ່ຂອງ Infrastructure ເທົ່ານັ້ນ",
                assembly.GetName().Name);
        }
    }

    private static void AssertSuccess(TestResult result, string? context = null)
    {
        string failing = result.FailingTypeNames is null
            ? string.Empty
            : string.Join(", ", result.FailingTypeNames);

        result.IsSuccessful.Should().BeTrue(
            "ປະເພດຕໍ່ໄປນີ້ລະເມີດກົດລະບຽບການເພິ່ງພາອາໄສລະຫວ່າງ Layer{0}: {1}",
            context is null ? string.Empty : $" ({context})",
            failing);
    }
}
