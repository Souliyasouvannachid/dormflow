using System.Net;
using System.Net.Http.Json;
using DormFlow.Application.Common.Abstractions;
using DormFlow.Contracts.Common;
using DormFlow.Contracts.Organizations;
using DormFlow.Contracts.Settings;
using DormFlow.IntegrationTests.Infrastructure;
using FluentAssertions;
using Xunit;

namespace DormFlow.IntegrationTests.Endpoints;

/// <summary>ການທົດສອບການຄຸ້ມຄອງການຕັ້ງຄ່າອົງກອນໂດຍຜ່ານ HTTP ຄວາມຈິງ</summary>
[Collection(ApiTestFixtures.Name)]
public class SettingsEndpointTests(DormFlowApiFactory factory)
{
    private readonly DormFlowApiFactory _factory = factory;

    [Fact]
    public async Task ສ້າງການຕັ້ງຄ່າໃໝ່ດ້ວຍກະແຈທີ່ຍັງບໍ່ທັນມີຢູ່_ຕ້ອງປະສົບຜົນສໍາເລັດ()
    {
        using HttpClient client = await CreateOwnerClientAsync();

        using HttpResponseMessage response = await client.PutAsJsonAsync(
            new Uri("/api/v1/settings/invoice.footer-text", UriKind.Relative),
            new UpsertSettingRequest { Value = "ຂອບໃຈທີ່ໃຊ້ບໍລິການ" },
            TestContext.Current.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        SettingResponse setting = (await response.Content
            .ReadFromJsonAsync<ApiResponse<SettingResponse>>(TestContext.Current.CancellationToken))!.Data!;

        setting.Key.Should().Be("invoice.footer-text");
        setting.Value.Should().Be("ຂອບໃຈທີ່ໃຊ້ບໍລິການ");
    }

    [Fact]
    public async Task ແກ້ໄຂການຕັ້ງຄ່າທີ່ມີຢູ່_ຕ້ອງໄດ້ຮັບມູນຄ່າໃຝ່_ຢ່າສ້າງລາຍການຊໍ້າກັນ()
    {
        using HttpClient client = await CreateOwnerClientAsync();

        await UpsertSettingAsync(client, "currency.code", "THB");

        using HttpResponseMessage response = await client.PutAsJsonAsync(
            new Uri("/api/v1/settings/currency.code", UriKind.Relative),
            new UpsertSettingRequest { Value = "USD" },
            TestContext.Current.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        SettingResponse setting = (await response.Content
            .ReadFromJsonAsync<ApiResponse<SettingResponse>>(TestContext.Current.CancellationToken))!.Data!;
        setting.Value.Should().Be("USD");

        using HttpResponseMessage listResponse = await client.GetAsync(
            new Uri("/api/v1/settings?search=currency.code", UriKind.Relative),
            TestContext.Current.CancellationToken);
        PagedResult<SettingResponse> list = (await listResponse.Content
            .ReadFromJsonAsync<ApiResponse<PagedResult<SettingResponse>>>(TestContext.Current.CancellationToken))!.Data!;

        list.Items.Should().ContainSingle();
    }

    [Fact]
    public async Task ລຶບການຕັ້ງຄ່າ_ແລ້ວອ່ານຕໍ່_ຕ້ອງໄດ້ຮັບ_404()
    {
        using HttpClient client = await CreateOwnerClientAsync();

        await UpsertSettingAsync(client, "feature.beta-enabled", "false");

        using HttpResponseMessage deleteResponse = await client.DeleteAsync(
            new Uri("/api/v1/settings/feature.beta-enabled", UriKind.Relative),
            TestContext.Current.CancellationToken);
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        using HttpResponseMessage getResponse = await client.GetAsync(
            new Uri("/api/v1/settings/feature.beta-enabled", UriKind.Relative),
            TestContext.Current.CancellationToken);
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ທ່ານສາມາດສ້າງການຕັ້ງຄ່າຊ້ໍາກັນສໍາລັບລະຫັດດຽວກັນຫຼັງຈາກລຶບມັນ()
    {
        using HttpClient client = await CreateOwnerClientAsync();

        await UpsertSettingAsync(client, "reusable.key", "first");

        using HttpResponseMessage deleteResponse = await client.DeleteAsync(
            new Uri("/api/v1/settings/reusable.key", UriKind.Relative),
            TestContext.Current.CancellationToken);
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        using HttpResponseMessage response = await client.PutAsJsonAsync(
            new Uri("/api/v1/settings/reusable.key", UriKind.Relative),
            new UpsertSettingRequest { Value = "second" },
            TestContext.Current.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        SettingResponse setting = (await response.Content
            .ReadFromJsonAsync<ApiResponse<SettingResponse>>(TestContext.Current.CancellationToken))!.Data!;
        setting.Value.Should().Be("second");
    }

    [Fact]
    public async Task ຜູ້ໃຊ້ຂອງອົງກອນພຽງແຕ່ເບິ່ງການຕັ້ງຄ່າຂອງອົງກອນຂອງຕົນເອງ()
    {
        using HttpClient ownerAClient = await CreateOwnerClientAsync();
        using HttpClient ownerBClient = await CreateOwnerClientAsync();

        await UpsertSettingAsync(ownerAClient, "shared.key-name", "org-a-value");

        using HttpResponseMessage response = await ownerBClient.GetAsync(
            new Uri("/api/v1/settings/shared.key-name", UriKind.Relative),
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    private static async Task UpsertSettingAsync(HttpClient client, string key, string value)
    {
        using HttpResponseMessage response = await client.PutAsJsonAsync(
            new Uri($"/api/v1/settings/{key}", UriKind.Relative),
            new UpsertSettingRequest { Value = value },
            TestContext.Current.CancellationToken);

        response.EnsureSuccessStatusCode();
    }

    private async Task<HttpClient> CreateOwnerClientAsync()
    {
        using HttpClient superAdminClient = await TestUserFactory.CreateAuthenticatedClientAsync(
            _factory,
            DormFlowRoles.SuperAdmin);

        string orgCode = $"ORG{Guid.NewGuid():N}";
        using HttpResponseMessage orgResponse = await superAdminClient.PostAsJsonAsync(
            new Uri("/api/v1/organizations", UriKind.Relative),
            new CreateOrganizationRequest { Code = orgCode, Name = $"ອົງກອນການທົດສອບ {orgCode}" },
            TestContext.Current.CancellationToken);
        OrganizationResponse organization = (await orgResponse.Content
            .ReadFromJsonAsync<ApiResponse<OrganizationResponse>>(TestContext.Current.CancellationToken))!.Data!;

        return await TestUserFactory.CreateAuthenticatedClientAsync(_factory, DormFlowRoles.Owner, organization.Id);
    }
}
