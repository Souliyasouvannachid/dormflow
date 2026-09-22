using System.Net;
using System.Net.Http.Json;
using DormFlow.Application.Common.Abstractions;
using DormFlow.Contracts.Common;
using DormFlow.Contracts.Dormitories;
using DormFlow.Contracts.Expenses;
using DormFlow.Contracts.Organizations;
using DormFlow.IntegrationTests.Infrastructure;
using FluentAssertions;
using Xunit;

namespace DormFlow.IntegrationTests.Endpoints;

/// <summary>ການທົດສອບການບັນທຶກແລະຄ່າໃຊ້ຈ່າຍໃນການຄົ້ນຫາໂດຍຜ່ານ HTTP ຄວາມຈິງ</summary>
[Collection(ApiTestFixtures.Name)]
public class ExpensesEndpointTests(DormFlowApiFactory factory)
{
    private readonly DormFlowApiFactory _factory = factory;

    [Fact]
    public async Task ບັນທຶກຄ່າໃຊ້ຈ່າຍໃຝ່_ຕ້ອງປະສົບຜົນສໍາເລັດ()
    {
        using HttpClient client = await TestUserFactory.CreateAuthenticatedClientAsync(
            _factory,
            DormFlowRoles.SuperAdmin);

        DormitoryResponse dormitory = await CreateDormitoryAsync(client);

        using HttpResponseMessage response = await client.PostAsJsonAsync(
            new Uri("/api/v1/expenses", UriKind.Relative),
            new CreateExpenseRequest
            {
                DormitoryId = dormitory.Id,
                Category = "Utilities",
                Amount = 4500,
                ExpenseDate = new DateOnly(2026, 3, 5),
                Description = "ໃບເກັບເງິນຄ່າໄຟຟ້າສູນກາງສໍາລັບເດືອນມີນາ",
                PaidTo = "ການໄຟຟ້າແຂວງ",
            },
            TestContext.Current.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        ExpenseResponse expense = (await response.Content
            .ReadFromJsonAsync<ApiResponse<ExpenseResponse>>(TestContext.Current.CancellationToken))!.Data!;

        expense.Category.Should().Be("Utilities");
        expense.Amount.Should().Be(4500);
        expense.PaidTo.Should().Be("ການໄຟຟ້າແຂວງ");
    }

    [Fact]
    public async Task ກັ່ນຕອງລາຍການລາຍຈ່າຍຕາມໝວດໝູ່ແລະຊ່ວງວັນທີ_ຕ້ອງໄດ້ຮັບຜົນທີ່ຖືກຕ້ອງ()
    {
        using HttpClient client = await TestUserFactory.CreateAuthenticatedClientAsync(
            _factory,
            DormFlowRoles.SuperAdmin);

        DormitoryResponse dormitory = await CreateDormitoryAsync(client);

        await CreateExpenseAsync(client, dormitory.Id, "Utilities", new DateOnly(2026, 5, 1), 1000);
        await CreateExpenseAsync(client, dormitory.Id, "Maintenance", new DateOnly(2026, 5, 10), 2000);
        await CreateExpenseAsync(client, dormitory.Id, "Utilities", new DateOnly(2026, 6, 1), 1500);

        using HttpResponseMessage response = await client.GetAsync(
            new Uri(
                $"/api/v1/expenses?dormitoryId={dormitory.Id}&category=Utilities&expenseDateFrom=2026-05-01&expenseDateTo=2026-05-31",
                UriKind.Relative),
            TestContext.Current.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        PagedResult<ExpenseResponse> result = (await response.Content
            .ReadFromJsonAsync<ApiResponse<PagedResult<ExpenseResponse>>>(TestContext.Current.CancellationToken))!.Data!;

        result.Items.Should().ContainSingle();
        result.Items[0].Amount.Should().Be(1000);
        result.Items[0].Category.Should().Be("Utilities");
    }

    [Fact]
    public async Task ບັນທຶກຄ່າໃຊ້ຈ່າຍທີ່ມີປະເພດທີ່ບໍ່ຖືກຕ້ອງ_ຕ້ອງໄດ້ຮັບ_400()
    {
        using HttpClient client = await TestUserFactory.CreateAuthenticatedClientAsync(
            _factory,
            DormFlowRoles.SuperAdmin);

        DormitoryResponse dormitory = await CreateDormitoryAsync(client);

        using HttpResponseMessage response = await client.PostAsJsonAsync(
            new Uri("/api/v1/expenses", UriKind.Relative),
            new CreateExpenseRequest
            {
                DormitoryId = dormitory.Id,
                Category = "NotARealCategory",
                Amount = 100,
                ExpenseDate = DateOnly.FromDateTime(DateTime.UtcNow),
                Description = "ການທົດສອບ",
            },
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    private static async Task CreateExpenseAsync(
        HttpClient client, Guid dormitoryId, string category, DateOnly expenseDate, decimal amount)
    {
        using HttpResponseMessage response = await client.PostAsJsonAsync(
            new Uri("/api/v1/expenses", UriKind.Relative),
            new CreateExpenseRequest
            {
                DormitoryId = dormitoryId,
                Category = category,
                Amount = amount,
                ExpenseDate = expenseDate,
                Description = $"ການທົດສອບ {category}",
            },
            TestContext.Current.CancellationToken);

        response.EnsureSuccessStatusCode();
    }

    private static async Task<DormitoryResponse> CreateDormitoryAsync(HttpClient client)
    {
        string orgCode = $"ORG{Guid.NewGuid():N}";
        using HttpResponseMessage orgResponse = await client.PostAsJsonAsync(
            new Uri("/api/v1/organizations", UriKind.Relative),
            new CreateOrganizationRequest { Code = orgCode, Name = $"ອົງກອນການທົດສອບ {orgCode}" },
            TestContext.Current.CancellationToken);
        OrganizationResponse organization = (await orgResponse.Content
            .ReadFromJsonAsync<ApiResponse<OrganizationResponse>>(TestContext.Current.CancellationToken))!.Data!;

        string dormCode = $"DORM{Guid.NewGuid():N}";
        using HttpResponseMessage dormResponse = await client.PostAsJsonAsync(
            new Uri("/api/v1/dormitories", UriKind.Relative),
            new CreateDormitoryRequest
            {
                OrganizationId = organization.Id,
                Code = dormCode,
                Name = $"ຫໍພັກການທົດສອບ {dormCode}",
                BillingDay = 1,
                PaymentDueDay = 5,
                LateFeeType = "Fixed",
            },
            TestContext.Current.CancellationToken);

        return (await dormResponse.Content
            .ReadFromJsonAsync<ApiResponse<DormitoryResponse>>(TestContext.Current.CancellationToken))!.Data!;
    }
}
