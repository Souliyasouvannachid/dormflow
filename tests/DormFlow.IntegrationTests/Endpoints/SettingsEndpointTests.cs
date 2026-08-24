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

/// <summary>ทดสอบการจัดการค่าตั้งขององค์กรผ่าน HTTP จริง</summary>
[Collection(ApiTestFixtures.Name)]
public class SettingsEndpointTests(DormFlowApiFactory factory)
{
    private readonly DormFlowApiFactory _factory = factory;

    [Fact]
    public async Task สร้างค่าตั้งใหม่ด้วยคีย์ที่ยังไม่มี_ต้องสำเร็จ()
    {
        using HttpClient client = await CreateOwnerClientAsync();

        using HttpResponseMessage response = await client.PutAsJsonAsync(
            new Uri("/api/v1/settings/invoice.footer-text", UriKind.Relative),
            new UpsertSettingRequest { Value = "ขอบคุณที่ใช้บริการ" },
            TestContext.Current.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        SettingResponse setting = (await response.Content
            .ReadFromJsonAsync<ApiResponse<SettingResponse>>(TestContext.Current.CancellationToken))!.Data!;

        setting.Key.Should().Be("invoice.footer-text");
        setting.Value.Should().Be("ขอบคุณที่ใช้บริการ");
    }

    [Fact]
    public async Task แก้ไขค่าตั้งที่มีอยู่แล้ว_ต้องได้ค่าใหม่_ไม่สร้างรายการซ้ำ()
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
    public async Task ลบค่าตั้ง_แล้วอ่านต่อ_ต้องได้_404()
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
    public async Task สร้างค่าตั้งซ้ำคีย์เดิมได้หลังจากลบไปแล้ว()
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
    public async Task ผู้ใช้ขององค์กรหนึ่งเห็นค่าตั้งขององค์กรตนเองเท่านั้น()
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
            new CreateOrganizationRequest { Code = orgCode, Name = $"องค์กรทดสอบ {orgCode}" },
            TestContext.Current.CancellationToken);
        OrganizationResponse organization = (await orgResponse.Content
            .ReadFromJsonAsync<ApiResponse<OrganizationResponse>>(TestContext.Current.CancellationToken))!.Data!;

        return await TestUserFactory.CreateAuthenticatedClientAsync(_factory, DormFlowRoles.Owner, organization.Id);
    }
}
