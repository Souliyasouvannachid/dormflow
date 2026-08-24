using System.Net;
using System.Net.Http.Json;
using DormFlow.Application.Common.Abstractions;
using DormFlow.Contracts.Common;
using DormFlow.Contracts.Permissions;
using DormFlow.Contracts.Roles;
using DormFlow.IntegrationTests.Infrastructure;
using FluentAssertions;
using Xunit;

namespace DormFlow.IntegrationTests.Endpoints;

/// <summary>ทดสอบการจัดการบทบาทและสิทธิ์ผ่าน HTTP จริง</summary>
[Collection(ApiTestFixtures.Name)]
public class RolesPermissionsEndpointTests(DormFlowApiFactory factory)
{
    private readonly DormFlowApiFactory _factory = factory;

    [Fact]
    public async Task GetPermissions_ต้องได้รายการสิทธิ์ที่ถูกSeedไว้ทั้งหมด()
    {
        using HttpClient client = await TestUserFactory.CreateAuthenticatedClientAsync(
            _factory,
            DormFlowRoles.SuperAdmin);

        using HttpResponseMessage response = await client.GetAsync(
            new Uri("/api/v1/permissions", UriKind.Relative),
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        ApiResponse<IReadOnlyList<PermissionResponse>>? body = await response.Content
            .ReadFromJsonAsync<ApiResponse<IReadOnlyList<PermissionResponse>>>(TestContext.Current.CancellationToken);

        body!.Data.Should().Contain(permission => permission.Code == "rooms.create");
    }

    [Fact]
    public async Task สร้างบทบาทใหม่_กำหนดสิทธิ์_แล้วลบได้()
    {
        using HttpClient client = await TestUserFactory.CreateAuthenticatedClientAsync(
            _factory,
            DormFlowRoles.SuperAdmin);

        string roleName = $"CustomRole-{Guid.CreateVersion7():N}";

        using HttpResponseMessage createResponse = await client.PostAsJsonAsync(
            new Uri("/api/v1/roles", UriKind.Relative),
            new CreateRoleRequest { Name = roleName, Description = "บทบาททดสอบ" },
            TestContext.Current.CancellationToken);
        createResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        ApiResponse<RoleResponse>? createdBody = await createResponse.Content
            .ReadFromJsonAsync<ApiResponse<RoleResponse>>(TestContext.Current.CancellationToken);
        RoleResponse role = createdBody!.Data!;

        role.IsSystemRole.Should().BeFalse();
        role.Permissions.Should().BeEmpty();

        using HttpResponseMessage assignResponse = await client.PostAsJsonAsync(
            new Uri($"/api/v1/roles/{role.Id}/permissions", UriKind.Relative),
            new AssignRolePermissionsRequest { Permissions = ["rooms.view", "rooms.create"] },
            TestContext.Current.CancellationToken);
        assignResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        ApiResponse<RoleResponse>? afterAssignBody = await assignResponse.Content
            .ReadFromJsonAsync<ApiResponse<RoleResponse>>(TestContext.Current.CancellationToken);
        afterAssignBody!.Data!.Permissions.Should().BeEquivalentTo(["rooms.view", "rooms.create"]);

        using HttpResponseMessage deleteResponse = await client.DeleteAsync(
            new Uri($"/api/v1/roles/{role.Id}", UriKind.Relative),
            TestContext.Current.CancellationToken);
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task ลบบทบาทมาตรฐานของระบบ_ต้องได้_409()
    {
        using HttpClient client = await TestUserFactory.CreateAuthenticatedClientAsync(
            _factory,
            DormFlowRoles.SuperAdmin);

        using HttpResponseMessage listResponse = await client.GetAsync(
            new Uri("/api/v1/roles?search=Staff&pageSize=1", UriKind.Relative),
            TestContext.Current.CancellationToken);

        ApiResponse<PagedResult<RoleResponse>>? listBody = await listResponse.Content
            .ReadFromJsonAsync<ApiResponse<PagedResult<RoleResponse>>>(TestContext.Current.CancellationToken);
        RoleResponse staffRole = listBody!.Data!.Items.Single(role => role.Name == DormFlowRoles.Staff);

        staffRole.IsSystemRole.Should().BeTrue();

        using HttpResponseMessage deleteResponse = await client.DeleteAsync(
            new Uri($"/api/v1/roles/{staffRole.Id}", UriKind.Relative),
            TestContext.Current.CancellationToken);

        deleteResponse.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task เปลี่ยนชื่อบทบาทมาตรฐานของระบบ_ต้องได้_409()
    {
        using HttpClient client = await TestUserFactory.CreateAuthenticatedClientAsync(
            _factory,
            DormFlowRoles.SuperAdmin);

        using HttpResponseMessage listResponse = await client.GetAsync(
            new Uri("/api/v1/roles?search=Technician&pageSize=1", UriKind.Relative),
            TestContext.Current.CancellationToken);

        ApiResponse<PagedResult<RoleResponse>>? listBody = await listResponse.Content
            .ReadFromJsonAsync<ApiResponse<PagedResult<RoleResponse>>>(TestContext.Current.CancellationToken);
        RoleResponse technicianRole = listBody!.Data!.Items.Single(role => role.Name == DormFlowRoles.Technician);

        using HttpResponseMessage updateResponse = await client.PutAsJsonAsync(
            new Uri($"/api/v1/roles/{technicianRole.Id}", UriKind.Relative),
            new UpdateRoleRequest { Name = "ช่างเทคนิค", Description = "เปลี่ยนชื่อ" },
            TestContext.Current.CancellationToken);

        updateResponse.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }
}
