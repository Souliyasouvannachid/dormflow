using System.Net;
using System.Net.Http.Json;
using DormFlow.Application.Common.Abstractions;
using DormFlow.Contracts.Common;
using DormFlow.Contracts.Dormitories;
using DormFlow.Contracts.Organizations;
using DormFlow.Contracts.Users;
using DormFlow.IntegrationTests.Infrastructure;
using FluentAssertions;
using Xunit;

namespace DormFlow.IntegrationTests.Endpoints;

/// <summary>ทดสอบการบริหารจัดการบัญชีผู้ใช้งานผ่าน HTTP จริง</summary>
[Collection(ApiTestFixtures.Name)]
public class UsersEndpointTests(DormFlowApiFactory factory)
{
    private readonly DormFlowApiFactory _factory = factory;

    [Fact]
    public async Task SuperAdmin_ต้องสร้างผู้ใช้_กำหนดบทบาท_และมอบหมายหอพักได้()
    {
        using HttpClient client = await TestUserFactory.CreateAuthenticatedClientAsync(
            _factory,
            DormFlowRoles.SuperAdmin);

        OrganizationResponse organization = await CreateOrganizationAsync(client);
        DormitoryResponse dormitory = await CreateDormitoryAsync(client, organization.Id);

        string email = $"user-{Guid.CreateVersion7():N}@dormflow.local";

        using HttpResponseMessage createResponse = await client.PostAsJsonAsync(
            new Uri("/api/v1/users", UriKind.Relative),
            new CreateUserRequest
            {
                OrganizationId = organization.Id,
                Email = email,
                Password = "Test-Passw0rd1",
                FirstName = "สมชาย",
                LastName = "ทดสอบ",
            },
            TestContext.Current.CancellationToken);

        createResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        ApiResponse<UserResponse>? createdBody = await createResponse.Content
            .ReadFromJsonAsync<ApiResponse<UserResponse>>(TestContext.Current.CancellationToken);
        UserResponse user = createdBody!.Data!;

        user.Email.Should().Be(email);
        user.OrganizationId.Should().Be(organization.Id);

        using HttpResponseMessage duplicateResponse = await client.PostAsJsonAsync(
            new Uri("/api/v1/users", UriKind.Relative),
            new CreateUserRequest
            {
                OrganizationId = organization.Id,
                Email = email,
                Password = "Test-Passw0rd1",
                FirstName = "สมหญิง",
                LastName = "ทดสอบ",
            },
            TestContext.Current.CancellationToken);
        duplicateResponse.StatusCode.Should().Be(HttpStatusCode.Conflict);

        using HttpResponseMessage assignRolesResponse = await client.PostAsJsonAsync(
            new Uri($"/api/v1/users/{user.Id}/roles", UriKind.Relative),
            new AssignUserRolesRequest { Roles = [DormFlowRoles.DormManager] },
            TestContext.Current.CancellationToken);
        assignRolesResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        ApiResponse<UserResponse>? afterRoleAssignmentBody = await assignRolesResponse.Content
            .ReadFromJsonAsync<ApiResponse<UserResponse>>(TestContext.Current.CancellationToken);
        afterRoleAssignmentBody!.Data!.Roles.Should().ContainSingle().Which.Should().Be(DormFlowRoles.DormManager);

        using HttpResponseMessage assignDormitoryResponse = await client.PostAsJsonAsync(
            new Uri($"/api/v1/users/{user.Id}/dormitories", UriKind.Relative),
            new AssignUserDormitoryRequest { DormitoryId = dormitory.Id, IsPrimary = true },
            TestContext.Current.CancellationToken);
        assignDormitoryResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        ApiResponse<UserResponse>? afterDormitoryAssignmentBody = await assignDormitoryResponse.Content
            .ReadFromJsonAsync<ApiResponse<UserResponse>>(TestContext.Current.CancellationToken);
        afterDormitoryAssignmentBody!.Data!.Dormitories.Should().ContainSingle(
            assignment => assignment.DormitoryId == dormitory.Id && assignment.IsPrimary);

        using HttpResponseMessage removeDormitoryResponse = await client.DeleteAsync(
            new Uri($"/api/v1/users/{user.Id}/dormitories/{dormitory.Id}", UriKind.Relative),
            TestContext.Current.CancellationToken);
        removeDormitoryResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        using HttpResponseMessage getAfterRemoveResponse = await client.GetAsync(
            new Uri($"/api/v1/users/{user.Id}", UriKind.Relative),
            TestContext.Current.CancellationToken);
        ApiResponse<UserResponse>? finalBody = await getAfterRemoveResponse.Content
            .ReadFromJsonAsync<ApiResponse<UserResponse>>(TestContext.Current.CancellationToken);
        finalBody!.Data!.Dormitories.Should().BeEmpty();
    }

    [Fact]
    public async Task ผู้ใช้ที่ไม่มีสิทธิ์_users_manage_ต้องได้_403()
    {
        using HttpClient client = await TestUserFactory.CreateAuthenticatedClientAsync(
            _factory,
            DormFlowRoles.Staff);

        using HttpResponseMessage response = await client.GetAsync(
            new Uri("/api/v1/users", UriKind.Relative),
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    private static async Task<OrganizationResponse> CreateOrganizationAsync(HttpClient client)
    {
        string code = $"ORG{Guid.NewGuid():N}";

        using HttpResponseMessage response = await client.PostAsJsonAsync(
            new Uri("/api/v1/organizations", UriKind.Relative),
            new CreateOrganizationRequest { Code = code, Name = $"องค์กรทดสอบ {code}" },
            TestContext.Current.CancellationToken);

        response.EnsureSuccessStatusCode();

        ApiResponse<OrganizationResponse>? body = await response.Content
            .ReadFromJsonAsync<ApiResponse<OrganizationResponse>>(TestContext.Current.CancellationToken);

        return body!.Data!;
    }

    private static async Task<DormitoryResponse> CreateDormitoryAsync(HttpClient client, Guid organizationId)
    {
        string code = $"DORM{Guid.NewGuid():N}";

        using HttpResponseMessage response = await client.PostAsJsonAsync(
            new Uri("/api/v1/dormitories", UriKind.Relative),
            new CreateDormitoryRequest
            {
                OrganizationId = organizationId,
                Code = code,
                Name = $"หอพักทดสอบ {code}",
                BillingDay = 1,
                PaymentDueDay = 5,
                LateFeeType = "Fixed",
            },
            TestContext.Current.CancellationToken);

        response.EnsureSuccessStatusCode();

        ApiResponse<DormitoryResponse>? body = await response.Content
            .ReadFromJsonAsync<ApiResponse<DormitoryResponse>>(TestContext.Current.CancellationToken);

        return body!.Data!;
    }
}
