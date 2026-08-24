using System.Net;
using System.Net.Http.Json;
using DormFlow.Application.Common.Abstractions;
using DormFlow.Contracts.Announcements;
using DormFlow.Contracts.Common;
using DormFlow.Contracts.Dormitories;
using DormFlow.Contracts.Organizations;
using DormFlow.IntegrationTests.Infrastructure;
using FluentAssertions;
using Xunit;

namespace DormFlow.IntegrationTests.Endpoints;

/// <summary>ทดสอบวงจรชีวิตของประกาศผ่าน HTTP จริง</summary>
[Collection(ApiTestFixtures.Name)]
public class AnnouncementsEndpointTests(DormFlowApiFactory factory)
{
    private readonly DormFlowApiFactory _factory = factory;

    [Fact]
    public async Task สร้างประกาศใหม่_ต้องได้สถานะ_Draft()
    {
        using HttpClient client = await TestUserFactory.CreateAuthenticatedClientAsync(
            _factory,
            DormFlowRoles.SuperAdmin);

        DormitoryResponse dormitory = await CreateDormitoryAsync(client);

        using HttpResponseMessage response = await client.PostAsJsonAsync(
            new Uri("/api/v1/announcements", UriKind.Relative),
            new CreateAnnouncementRequest
            {
                DormitoryId = dormitory.Id,
                Title = "ปิดปรับปรุงลิฟต์",
                Content = "ลิฟต์อาคาร A จะปิดปรับปรุงวันที่ 1-3 กันยายนนี้",
            },
            TestContext.Current.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        AnnouncementResponse announcement = (await response.Content
            .ReadFromJsonAsync<ApiResponse<AnnouncementResponse>>(TestContext.Current.CancellationToken))!.Data!;

        announcement.Status.Should().Be("Draft");
        announcement.PublishedAt.Should().BeNull();
    }

    [Fact]
    public async Task เผยแพร่ประกาศที่เป็นร่าง_ต้องได้สถานะ_Published()
    {
        using HttpClient client = await TestUserFactory.CreateAuthenticatedClientAsync(
            _factory,
            DormFlowRoles.SuperAdmin);

        DormitoryResponse dormitory = await CreateDormitoryAsync(client);
        AnnouncementResponse draft = await CreateAnnouncementAsync(client, dormitory.Id, "ประกาศทดสอบ 1");

        using HttpResponseMessage response = await client.PostAsync(
            new Uri($"/api/v1/announcements/{draft.Id}/publish", UriKind.Relative),
            content: null,
            TestContext.Current.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        AnnouncementResponse published = (await response.Content
            .ReadFromJsonAsync<ApiResponse<AnnouncementResponse>>(TestContext.Current.CancellationToken))!.Data!;

        published.Status.Should().Be("Published");
        published.PublishedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task เผยแพร่ประกาศที่เผยแพร่ไปแล้วซ้ำ_ต้องได้_409()
    {
        using HttpClient client = await TestUserFactory.CreateAuthenticatedClientAsync(
            _factory,
            DormFlowRoles.SuperAdmin);

        DormitoryResponse dormitory = await CreateDormitoryAsync(client);
        AnnouncementResponse draft = await CreateAnnouncementAsync(client, dormitory.Id, "ประกาศทดสอบ 2");

        await client.PostAsync(
            new Uri($"/api/v1/announcements/{draft.Id}/publish", UriKind.Relative),
            content: null,
            TestContext.Current.CancellationToken);

        using HttpResponseMessage response = await client.PostAsync(
            new Uri($"/api/v1/announcements/{draft.Id}/publish", UriKind.Relative),
            content: null,
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task เก็บประกาศที่เผยแพร่แล้วเข้าคลัง_ต้องได้สถานะ_Archived()
    {
        using HttpClient client = await TestUserFactory.CreateAuthenticatedClientAsync(
            _factory,
            DormFlowRoles.SuperAdmin);

        DormitoryResponse dormitory = await CreateDormitoryAsync(client);
        AnnouncementResponse draft = await CreateAnnouncementAsync(client, dormitory.Id, "ประกาศทดสอบ 3");

        await client.PostAsync(
            new Uri($"/api/v1/announcements/{draft.Id}/publish", UriKind.Relative),
            content: null,
            TestContext.Current.CancellationToken);

        using HttpResponseMessage response = await client.PostAsync(
            new Uri($"/api/v1/announcements/{draft.Id}/archive", UriKind.Relative),
            content: null,
            TestContext.Current.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        AnnouncementResponse archived = (await response.Content
            .ReadFromJsonAsync<ApiResponse<AnnouncementResponse>>(TestContext.Current.CancellationToken))!.Data!;

        archived.Status.Should().Be("Archived");
    }

    [Fact]
    public async Task เก็บประกาศที่ยังเป็นร่างเข้าคลัง_ต้องได้_409()
    {
        using HttpClient client = await TestUserFactory.CreateAuthenticatedClientAsync(
            _factory,
            DormFlowRoles.SuperAdmin);

        DormitoryResponse dormitory = await CreateDormitoryAsync(client);
        AnnouncementResponse draft = await CreateAnnouncementAsync(client, dormitory.Id, "ประกาศทดสอบ 4");

        using HttpResponseMessage response = await client.PostAsync(
            new Uri($"/api/v1/announcements/{draft.Id}/archive", UriKind.Relative),
            content: null,
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    private static async Task<AnnouncementResponse> CreateAnnouncementAsync(
        HttpClient client, Guid dormitoryId, string title)
    {
        using HttpResponseMessage response = await client.PostAsJsonAsync(
            new Uri("/api/v1/announcements", UriKind.Relative),
            new CreateAnnouncementRequest
            {
                DormitoryId = dormitoryId,
                Title = title,
                Content = $"เนื้อหาของ {title}",
            },
            TestContext.Current.CancellationToken);

        response.EnsureSuccessStatusCode();

        return (await response.Content
            .ReadFromJsonAsync<ApiResponse<AnnouncementResponse>>(TestContext.Current.CancellationToken))!.Data!;
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
