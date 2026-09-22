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

/// <summary>ທົດສອບວົງຈອນຊີວິດຂອງການປະກາດຜ່ານ HTTP ຄວາມຈິງ</summary>
[Collection(ApiTestFixtures.Name)]
public class AnnouncementsEndpointTests(DormFlowApiFactory factory)
{
    private readonly DormFlowApiFactory _factory = factory;

    [Fact]
    public async Task ສ້າງການປະກາດໃຝ່_ຕ້ອງໄດ້ຮັບການສະຖານະ_Draft()
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
                Title = "ປິດສໍາລັບການບໍາລຸງຮັກສາລິຟ",
                Content = "ຟ ໃນ ການ ກໍ່ ສ້າງ A ຈະຖືກປິດສໍາລັບການບໍາລຸງຮັກສາ 1-3 ເດືອນກັນຍານີ້",
            },
            TestContext.Current.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        AnnouncementResponse announcement = (await response.Content
            .ReadFromJsonAsync<ApiResponse<AnnouncementResponse>>(TestContext.Current.CancellationToken))!.Data!;

        announcement.Status.Should().Be("Draft");
        announcement.PublishedAt.Should().BeNull();
    }

    [Fact]
    public async Task ເຜີຍແຜ່ຮ່າງປະກາດ_ຕ້ອງໄດ້ຮັບການສະຖານະ_Published()
    {
        using HttpClient client = await TestUserFactory.CreateAuthenticatedClientAsync(
            _factory,
            DormFlowRoles.SuperAdmin);

        DormitoryResponse dormitory = await CreateDormitoryAsync(client);
        AnnouncementResponse draft = await CreateAnnouncementAsync(client, dormitory.Id, "ປະກາດການທົດສອບ 1");

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
    public async Task ເຜີຍແຜ່ການປະກາດທີ່ໄດ້ເຜີຍແຜ່ແລ້ວຄືນໃໝ່_ຕ້ອງໄດ້ຮັບ_409()
    {
        using HttpClient client = await TestUserFactory.CreateAuthenticatedClientAsync(
            _factory,
            DormFlowRoles.SuperAdmin);

        DormitoryResponse dormitory = await CreateDormitoryAsync(client);
        AnnouncementResponse draft = await CreateAnnouncementAsync(client, dormitory.Id, "ປະກາດການທົດສອບ 2");

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
    public async Task ເກັບຮັກສາປະກາດທີ່ເຜີຍແຜ່ຢູ່ໃນບ່ອນເກັບມ້ຽນ_ຕ້ອງໄດ້ຮັບການສະຖານະ_Archived()
    {
        using HttpClient client = await TestUserFactory.CreateAuthenticatedClientAsync(
            _factory,
            DormFlowRoles.SuperAdmin);

        DormitoryResponse dormitory = await CreateDormitoryAsync(client);
        AnnouncementResponse draft = await CreateAnnouncementAsync(client, dormitory.Id, "ປະກາດການທົດສອບ 3");

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
    public async Task ຮັກສາການປະກາດທີ່ຍັງຮ່າງຢູ່ໃນບ່ອນເກັບມ້ຽນ_ຕ້ອງໄດ້ຮັບ_409()
    {
        using HttpClient client = await TestUserFactory.CreateAuthenticatedClientAsync(
            _factory,
            DormFlowRoles.SuperAdmin);

        DormitoryResponse dormitory = await CreateDormitoryAsync(client);
        AnnouncementResponse draft = await CreateAnnouncementAsync(client, dormitory.Id, "ປະກາດການທົດສອບ 4");

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
                Content = $"ເນື້ອໃນຂອງ {title}",
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
