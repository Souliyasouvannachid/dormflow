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

/// <summary>ทดสอบการบันทึกและค้นหาค่าใช้จ่ายผ่าน HTTP จริง</summary>
[Collection(ApiTestFixtures.Name)]
public class ExpensesEndpointTests(DormFlowApiFactory factory)
{
    private readonly DormFlowApiFactory _factory = factory;

    [Fact]
    public async Task บันทึกค่าใช้จ่ายใหม่_ต้องสำเร็จ()
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
                Description = "ค่าไฟฟ้าส่วนกลางเดือนมีนาคม",
                PaidTo = "การไฟฟ้าส่วนภูมิภาค",
            },
            TestContext.Current.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        ExpenseResponse expense = (await response.Content
            .ReadFromJsonAsync<ApiResponse<ExpenseResponse>>(TestContext.Current.CancellationToken))!.Data!;

        expense.Category.Should().Be("Utilities");
        expense.Amount.Should().Be(4500);
        expense.PaidTo.Should().Be("การไฟฟ้าส่วนภูมิภาค");
    }

    [Fact]
    public async Task กรองรายการค่าใช้จ่ายตามหมวดหมู่และช่วงวันที่_ต้องได้ผลลัพธ์ถูกต้อง()
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
    public async Task บันทึกค่าใช้จ่ายด้วยหมวดหมู่ที่ไม่ถูกต้อง_ต้องได้_400()
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
                Description = "ทดสอบ",
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
                Description = $"ทดสอบ {category}",
            },
            TestContext.Current.CancellationToken);

        response.EnsureSuccessStatusCode();
    }

    private static async Task<DormitoryResponse> CreateDormitoryAsync(HttpClient client)
    {
        string orgCode = $"ORG{Guid.NewGuid():N}";
        using HttpResponseMessage orgResponse = await client.PostAsJsonAsync(
            new Uri("/api/v1/organizations", UriKind.Relative),
            new CreateOrganizationRequest { Code = orgCode, Name = $"องค์กรทดสอบ {orgCode}" },
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
                Name = $"หอพักทดสอบ {dormCode}",
                BillingDay = 1,
                PaymentDueDay = 5,
                LateFeeType = "Fixed",
            },
            TestContext.Current.CancellationToken);

        return (await dormResponse.Content
            .ReadFromJsonAsync<ApiResponse<DormitoryResponse>>(TestContext.Current.CancellationToken))!.Data!;
    }
}
