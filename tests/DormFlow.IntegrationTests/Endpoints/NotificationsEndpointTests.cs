using System.Net;
using System.Net.Http.Json;
using DormFlow.Application.Common.Abstractions;
using DormFlow.Contracts.Authentication;
using DormFlow.Contracts.Common;
using DormFlow.Contracts.Notifications;
using DormFlow.Contracts.Organizations;
using DormFlow.IntegrationTests.Infrastructure;
using FluentAssertions;
using Xunit;

namespace DormFlow.IntegrationTests.Endpoints;

/// <summary>ทดสอบวงจรชีวิตของการแจ้งเตือนผ่าน HTTP จริง</summary>
[Collection(ApiTestFixtures.Name)]
public class NotificationsEndpointTests(DormFlowApiFactory factory)
{
    private readonly DormFlowApiFactory _factory = factory;

    [Fact]
    public async Task ส่งการแจ้งเตือนถึงผู้ใช้_ต้องปรากฏในรายการแจ้งเตือนของผู้รับ()
    {
        using HttpClient senderClient = await TestUserFactory.CreateAuthenticatedClientAsync(
            _factory,
            DormFlowRoles.SuperAdmin);

        Guid organizationId = await CreateOrganizationAsync(senderClient);
        (HttpClient recipientClient, Guid recipientUserId) = await CreateRecipientAsync(organizationId);

        using HttpResponseMessage response = await senderClient.PostAsJsonAsync(
            new Uri("/api/v1/notifications", UriKind.Relative),
            new SendNotificationRequest
            {
                RecipientUserId = recipientUserId,
                Title = "แจ้งเตือนทดสอบ",
                Message = "นี่คือเนื้อหาการแจ้งเตือนทดสอบ",
            },
            TestContext.Current.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        using HttpResponseMessage listResponse = await recipientClient.GetAsync(
            new Uri("/api/v1/notifications", UriKind.Relative),
            TestContext.Current.CancellationToken);
        listResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        PagedResult<NotificationResponse> result = (await listResponse.Content
            .ReadFromJsonAsync<ApiResponse<PagedResult<NotificationResponse>>>(TestContext.Current.CancellationToken))!.Data!;

        result.Items.Should().ContainSingle();
        result.Items[0].Title.Should().Be("แจ้งเตือนทดสอบ");
        result.Items[0].IsRead.Should().BeFalse();
    }

    [Fact]
    public async Task ทำเครื่องหมายอ่านการแจ้งเตือนของตนเอง_ต้องได้_IsRead_true()
    {
        using HttpClient senderClient = await TestUserFactory.CreateAuthenticatedClientAsync(
            _factory,
            DormFlowRoles.SuperAdmin);

        Guid organizationId = await CreateOrganizationAsync(senderClient);
        (HttpClient recipientClient, Guid recipientUserId) = await CreateRecipientAsync(organizationId);

        NotificationResponse sent = await SendNotificationAsync(senderClient, recipientUserId);

        using HttpResponseMessage response = await recipientClient.PostAsync(
            new Uri($"/api/v1/notifications/{sent.Id}/read", UriKind.Relative),
            content: null,
            TestContext.Current.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        NotificationResponse read = (await response.Content
            .ReadFromJsonAsync<ApiResponse<NotificationResponse>>(TestContext.Current.CancellationToken))!.Data!;

        read.IsRead.Should().BeTrue();
        read.ReadAt.Should().NotBeNull();
    }

    [Fact]
    public async Task ทำเครื่องหมายอ่านการแจ้งเตือนของผู้อื่น_ต้องได้_404()
    {
        using HttpClient senderClient = await TestUserFactory.CreateAuthenticatedClientAsync(
            _factory,
            DormFlowRoles.SuperAdmin);

        Guid organizationId = await CreateOrganizationAsync(senderClient);
        (_, Guid recipientUserId) = await CreateRecipientAsync(organizationId);
        (HttpClient otherClient, _) = await CreateRecipientAsync(organizationId);

        NotificationResponse sent = await SendNotificationAsync(senderClient, recipientUserId);

        using HttpResponseMessage response = await otherClient.PostAsync(
            new Uri($"/api/v1/notifications/{sent.Id}/read", UriKind.Relative),
            content: null,
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ส่งการแจ้งเตือนถึงผู้ใช้ต่างองค์กร_ต้องได้_404()
    {
        using HttpClient superAdminClient = await TestUserFactory.CreateAuthenticatedClientAsync(
            _factory,
            DormFlowRoles.SuperAdmin);

        Guid ownOrganizationId = await CreateOrganizationAsync(superAdminClient);
        Guid otherOrganizationId = await CreateOrganizationAsync(superAdminClient);

        using HttpClient senderClient = await TestUserFactory.CreateAuthenticatedClientAsync(
            _factory,
            DormFlowRoles.Owner,
            ownOrganizationId);

        (_, Guid recipientInOtherOrgId) = await CreateRecipientAsync(otherOrganizationId);

        using HttpResponseMessage response = await senderClient.PostAsJsonAsync(
            new Uri("/api/v1/notifications", UriKind.Relative),
            new SendNotificationRequest
            {
                RecipientUserId = recipientInOtherOrgId,
                Title = "ทดสอบข้ามองค์กร",
                Message = "ต้องไม่สำเร็จ",
            },
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    private static async Task<NotificationResponse> SendNotificationAsync(
        HttpClient senderClient, Guid recipientUserId)
    {
        using HttpResponseMessage response = await senderClient.PostAsJsonAsync(
            new Uri("/api/v1/notifications", UriKind.Relative),
            new SendNotificationRequest
            {
                RecipientUserId = recipientUserId,
                Title = "แจ้งเตือนทดสอบ",
                Message = "เนื้อหาทดสอบ",
            },
            TestContext.Current.CancellationToken);

        response.EnsureSuccessStatusCode();

        return (await response.Content
            .ReadFromJsonAsync<ApiResponse<NotificationResponse>>(TestContext.Current.CancellationToken))!.Data!;
    }

    private async Task<(HttpClient Client, Guid UserId)> CreateRecipientAsync(Guid organizationId)
    {
        HttpClient client = await TestUserFactory.CreateAuthenticatedClientAsync(
            _factory,
            DormFlowRoles.Staff,
            organizationId);

        using HttpResponseMessage meResponse = await client.GetAsync(
            new Uri("/api/v1/auth/me", UriKind.Relative),
            TestContext.Current.CancellationToken);
        CurrentUserResponse me = (await meResponse.Content
            .ReadFromJsonAsync<ApiResponse<CurrentUserResponse>>(TestContext.Current.CancellationToken))!.Data!;

        return (client, me.Id);
    }

    private static async Task<Guid> CreateOrganizationAsync(HttpClient superAdminClient)
    {
        string orgCode = $"ORG{Guid.NewGuid():N}";
        using HttpResponseMessage orgResponse = await superAdminClient.PostAsJsonAsync(
            new Uri("/api/v1/organizations", UriKind.Relative),
            new CreateOrganizationRequest { Code = orgCode, Name = $"องค์กรทดสอบ {orgCode}" },
            TestContext.Current.CancellationToken);
        OrganizationResponse organization = (await orgResponse.Content
            .ReadFromJsonAsync<ApiResponse<OrganizationResponse>>(TestContext.Current.CancellationToken))!.Data!;

        return organization.Id;
    }
}
