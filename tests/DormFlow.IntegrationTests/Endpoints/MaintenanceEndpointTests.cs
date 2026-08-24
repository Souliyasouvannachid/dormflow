using System.Net;
using System.Net.Http.Json;
using DormFlow.Application.Common.Abstractions;
using DormFlow.Contracts.Buildings;
using DormFlow.Contracts.Common;
using DormFlow.Contracts.Dormitories;
using DormFlow.Contracts.Floors;
using DormFlow.Contracts.Maintenance;
using DormFlow.Contracts.Organizations;
using DormFlow.Contracts.RoomTypes;
using DormFlow.Contracts.Rooms;
using DormFlow.Contracts.Users;
using DormFlow.Domain.Enums;
using DormFlow.IntegrationTests.Infrastructure;
using FluentAssertions;
using Xunit;

namespace DormFlow.IntegrationTests.Endpoints;

/// <summary>
/// ทดสอบวงจรชีวิตของงานแจ้งซ่อมบำรุงผ่าน HTTP จริง
/// Report -> Assign (ห้องเปลี่ยนเป็น Maintenance) -> InProgress -> Complete (ห้องกลับเป็น Available)
/// </summary>
[Collection(ApiTestFixtures.Name)]
public class MaintenanceEndpointTests(DormFlowApiFactory factory)
{
    private readonly DormFlowApiFactory _factory = factory;

    [Fact]
    public async Task แจ้งซ่อม_มอบหมายช่าง_ดำเนินการ_แล้วปิดงาน_ต้องทำงานครบวงจร()
    {
        using HttpClient client = await TestUserFactory.CreateAuthenticatedClientAsync(
            _factory,
            DormFlowRoles.SuperAdmin);

        (OrganizationResponse organization, DormitoryResponse dormitory, RoomResponse room) =
            await CreatePropertyHierarchyAsync(client);
        UserResponse technician = await CreateTechnicianAsync(client, organization.Id, dormitory.Id);

        MaintenanceRequestResponse request = await ReportIssueAsync(client, room.Id);
        request.Status.Should().Be("Reported");
        (await GetRoomAsync(client, room.Id)).Status.Should().Be("Available");

        using HttpResponseMessage assignResponse = await client.PostAsJsonAsync(
            new Uri($"/api/v1/maintenance-requests/{request.Id}/assign", UriKind.Relative),
            new AssignMaintenanceRequestRequest { TechnicianId = technician.Id },
            TestContext.Current.CancellationToken);
        assignResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        MaintenanceRequestResponse assigned = (await assignResponse.Content
            .ReadFromJsonAsync<ApiResponse<MaintenanceRequestResponse>>(TestContext.Current.CancellationToken))!.Data!;
        assigned.Status.Should().Be("Assigned");
        assigned.AssignedTechnicianId.Should().Be(technician.Id);
        assigned.AssignedAt.Should().NotBeNull();
        (await GetRoomAsync(client, room.Id)).Status.Should().Be("Maintenance");

        MaintenanceRequestResponse inProgress = await ChangeStatusAsync(client, request.Id, "InProgress");
        inProgress.Status.Should().Be("InProgress");
        (await GetRoomAsync(client, room.Id)).Status.Should().Be("Maintenance");

        MaintenanceRequestResponse completed = await ChangeStatusAsync(client, request.Id, "Completed");
        completed.Status.Should().Be("Completed");
        completed.CompletedAt.Should().NotBeNull();
        (await GetRoomAsync(client, room.Id)).Status.Should().Be("Available");
    }

    [Fact]
    public async Task มอบหมายผู้ใช้ที่ไม่มีบทบาทช่างซ่อมบำรุง_ต้องได้_400()
    {
        using HttpClient client = await TestUserFactory.CreateAuthenticatedClientAsync(
            _factory,
            DormFlowRoles.SuperAdmin);

        (_, _, RoomResponse room) = await CreatePropertyHierarchyAsync(client);
        MaintenanceRequestResponse request = await ReportIssueAsync(client, room.Id);

        string nonTechnicianEmail = await TestUserFactory.CreateAsync(_factory, DormFlowRoles.Staff);
        using HttpResponseMessage listResponse = await client.GetAsync(
            new Uri($"/api/v1/users?search={Uri.EscapeDataString(nonTechnicianEmail)}", UriKind.Relative),
            TestContext.Current.CancellationToken);
        PagedResult<UserResponse> users = (await listResponse.Content
            .ReadFromJsonAsync<ApiResponse<PagedResult<UserResponse>>>(TestContext.Current.CancellationToken))!.Data!;
        Guid nonTechnicianId = users.Items.Single(u => u.Email == nonTechnicianEmail).Id;

        using HttpResponseMessage assignResponse = await client.PostAsJsonAsync(
            new Uri($"/api/v1/maintenance-requests/{request.Id}/assign", UriKind.Relative),
            new AssignMaintenanceRequestRequest { TechnicianId = nonTechnicianId },
            TestContext.Current.CancellationToken);

        assignResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task มอบหมายช่างที่ไม่ได้ดูแลหอพักนี้_ต้องได้_400()
    {
        using HttpClient client = await TestUserFactory.CreateAuthenticatedClientAsync(
            _factory,
            DormFlowRoles.SuperAdmin);

        (OrganizationResponse organization, _, RoomResponse room) = await CreatePropertyHierarchyAsync(client);
        MaintenanceRequestResponse request = await ReportIssueAsync(client, room.Id);

        // ช่างสังกัดองค์กรเดียวกัน แต่ไม่เคยได้รับมอบหมายให้ดูแลหอพักใดเลย
        string technicianEmail = await TestUserFactory.CreateAsync(_factory, DormFlowRoles.Technician, organization.Id);
        using HttpResponseMessage listResponse = await client.GetAsync(
            new Uri($"/api/v1/users?search={Uri.EscapeDataString(technicianEmail)}", UriKind.Relative),
            TestContext.Current.CancellationToken);
        PagedResult<UserResponse> users = (await listResponse.Content
            .ReadFromJsonAsync<ApiResponse<PagedResult<UserResponse>>>(TestContext.Current.CancellationToken))!.Data!;
        Guid technicianId = users.Items.Single(u => u.Email == technicianEmail).Id;

        using HttpResponseMessage assignResponse = await client.PostAsJsonAsync(
            new Uri($"/api/v1/maintenance-requests/{request.Id}/assign", UriKind.Relative),
            new AssignMaintenanceRequestRequest { TechnicianId = technicianId },
            TestContext.Current.CancellationToken);

        assignResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task เปลี่ยนสถานะเป็น_Assigned_ผ่าน_endpoint_ทั่วไปโดยตรง_ต้องถูกปฏิเสธ()
    {
        using HttpClient client = await TestUserFactory.CreateAuthenticatedClientAsync(
            _factory,
            DormFlowRoles.SuperAdmin);

        (_, _, RoomResponse room) = await CreatePropertyHierarchyAsync(client);
        MaintenanceRequestResponse request = await ReportIssueAsync(client, room.Id);

        using HttpResponseMessage response = await client.PatchAsJsonAsync(
            new Uri($"/api/v1/maintenance-requests/{request.Id}/status", UriKind.Relative),
            new ChangeStatusRequest<MaintenanceStatus> { Status = MaintenanceStatus.Assigned },
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task เปลี่ยนสถานะข้ามขั้นตอนที่ไม่อนุญาต_ต้องได้_409()
    {
        using HttpClient client = await TestUserFactory.CreateAuthenticatedClientAsync(
            _factory,
            DormFlowRoles.SuperAdmin);

        (_, _, RoomResponse room) = await CreatePropertyHierarchyAsync(client);
        MaintenanceRequestResponse request = await ReportIssueAsync(client, room.Id);

        using HttpResponseMessage response = await client.PatchAsJsonAsync(
            new Uri($"/api/v1/maintenance-requests/{request.Id}/status", UriKind.Relative),
            new ChangeStatusRequest<MaintenanceStatus> { Status = MaintenanceStatus.Completed },
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task ห้องมีงานซ่อมสองรายการ_ปิดรายการแรกห้องต้องยังซ่อมอยู่_ปิดรายการที่สองห้องจึงว่าง()
    {
        using HttpClient client = await TestUserFactory.CreateAuthenticatedClientAsync(
            _factory,
            DormFlowRoles.SuperAdmin);

        (OrganizationResponse organization, DormitoryResponse dormitory, RoomResponse room) =
            await CreatePropertyHierarchyAsync(client);
        UserResponse technician = await CreateTechnicianAsync(client, organization.Id, dormitory.Id);

        MaintenanceRequestResponse first = await ReportIssueAsync(client, room.Id);
        MaintenanceRequestResponse second = await ReportIssueAsync(client, room.Id);

        await AssignAsync(client, first.Id, technician.Id);
        await AssignAsync(client, second.Id, technician.Id);
        (await GetRoomAsync(client, room.Id)).Status.Should().Be("Maintenance");

        await ChangeStatusAsync(client, first.Id, "InProgress");
        await ChangeStatusAsync(client, first.Id, "Completed");
        (await GetRoomAsync(client, room.Id)).Status.Should().Be("Maintenance");

        await ChangeStatusAsync(client, second.Id, "InProgress");
        await ChangeStatusAsync(client, second.Id, "Completed");
        (await GetRoomAsync(client, room.Id)).Status.Should().Be("Available");
    }

    [Fact]
    public async Task มอบหมายซ้ำระหว่างสถานะ_Assigned_ต้องเปลี่ยนช่างได้โดยสถานะไม่เปลี่ยน()
    {
        using HttpClient client = await TestUserFactory.CreateAuthenticatedClientAsync(
            _factory,
            DormFlowRoles.SuperAdmin);

        (OrganizationResponse organization, DormitoryResponse dormitory, RoomResponse room) =
            await CreatePropertyHierarchyAsync(client);
        UserResponse technicianA = await CreateTechnicianAsync(client, organization.Id, dormitory.Id);
        UserResponse technicianB = await CreateTechnicianAsync(client, organization.Id, dormitory.Id);

        MaintenanceRequestResponse request = await ReportIssueAsync(client, room.Id);
        MaintenanceRequestResponse assignedToA = await AssignAsync(client, request.Id, technicianA.Id);
        assignedToA.Status.Should().Be("Assigned");

        MaintenanceRequestResponse assignedToB = await AssignAsync(client, request.Id, technicianB.Id);
        assignedToB.Status.Should().Be("Assigned");
        assignedToB.AssignedTechnicianId.Should().Be(technicianB.Id);
    }

    [Fact]
    public async Task งานซ่อมของห้องที่มีผู้เช่าอยู่_ไม่ต้องเปลี่ยนสถานะห้อง()
    {
        using HttpClient client = await TestUserFactory.CreateAuthenticatedClientAsync(
            _factory,
            DormFlowRoles.SuperAdmin);

        (OrganizationResponse organization, DormitoryResponse dormitory, RoomResponse room) =
            await CreatePropertyHierarchyAsync(client);
        UserResponse technician = await CreateTechnicianAsync(client, organization.Id, dormitory.Id);

        using HttpResponseMessage occupyResponse = await client.PatchAsJsonAsync(
            new Uri($"/api/v1/rooms/{room.Id}/status", UriKind.Relative),
            new ChangeStatusRequest<RoomStatus> { Status = RoomStatus.Occupied },
            TestContext.Current.CancellationToken);
        occupyResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        MaintenanceRequestResponse request = await ReportIssueAsync(client, room.Id);
        await AssignAsync(client, request.Id, technician.Id);
        (await GetRoomAsync(client, room.Id)).Status.Should().Be("Occupied");

        await ChangeStatusAsync(client, request.Id, "InProgress");
        await ChangeStatusAsync(client, request.Id, "Completed");
        (await GetRoomAsync(client, room.Id)).Status.Should().Be("Occupied");
    }

    private static async Task<MaintenanceRequestResponse> ReportIssueAsync(HttpClient client, Guid roomId)
    {
        using HttpResponseMessage response = await client.PostAsJsonAsync(
            new Uri("/api/v1/maintenance-requests", UriKind.Relative),
            new CreateMaintenanceRequestRequest
            {
                RoomId = roomId,
                Title = "ก๊อกน้ำรั่ว",
                Description = "น้ำหยดตลอดเวลา",
                Priority = "High",
            },
            TestContext.Current.CancellationToken);

        response.EnsureSuccessStatusCode();

        return (await response.Content
            .ReadFromJsonAsync<ApiResponse<MaintenanceRequestResponse>>(TestContext.Current.CancellationToken))!.Data!;
    }

    private static async Task<MaintenanceRequestResponse> AssignAsync(HttpClient client, Guid requestId, Guid technicianId)
    {
        using HttpResponseMessage response = await client.PostAsJsonAsync(
            new Uri($"/api/v1/maintenance-requests/{requestId}/assign", UriKind.Relative),
            new AssignMaintenanceRequestRequest { TechnicianId = technicianId },
            TestContext.Current.CancellationToken);

        response.EnsureSuccessStatusCode();

        return (await response.Content
            .ReadFromJsonAsync<ApiResponse<MaintenanceRequestResponse>>(TestContext.Current.CancellationToken))!.Data!;
    }

    private static async Task<MaintenanceRequestResponse> ChangeStatusAsync(HttpClient client, Guid requestId, string status)
    {
        MaintenanceStatus target = Enum.Parse<MaintenanceStatus>(status, ignoreCase: true);

        using HttpResponseMessage response = await client.PatchAsJsonAsync(
            new Uri($"/api/v1/maintenance-requests/{requestId}/status", UriKind.Relative),
            new ChangeStatusRequest<MaintenanceStatus> { Status = target },
            TestContext.Current.CancellationToken);

        response.EnsureSuccessStatusCode();

        return (await response.Content
            .ReadFromJsonAsync<ApiResponse<MaintenanceRequestResponse>>(TestContext.Current.CancellationToken))!.Data!;
    }

    private static async Task<RoomResponse> GetRoomAsync(HttpClient client, Guid roomId)
    {
        using HttpResponseMessage response = await client.GetAsync(
            new Uri($"/api/v1/rooms/{roomId}", UriKind.Relative),
            TestContext.Current.CancellationToken);

        return (await response.Content
            .ReadFromJsonAsync<ApiResponse<RoomResponse>>(TestContext.Current.CancellationToken))!.Data!;
    }

    private static async Task<UserResponse> CreateTechnicianAsync(HttpClient client, Guid organizationId, Guid dormitoryId)
    {
        string email = $"tech-{Guid.NewGuid():N}@dormflow.local";

        using HttpResponseMessage createResponse = await client.PostAsJsonAsync(
            new Uri("/api/v1/users", UriKind.Relative),
            new CreateUserRequest
            {
                OrganizationId = organizationId,
                Email = email,
                Password = "Test-Passw0rd1",
                FirstName = "ช่าง",
                LastName = $"ทดสอบ-{Guid.NewGuid():N}",
            },
            TestContext.Current.CancellationToken);
        createResponse.EnsureSuccessStatusCode();

        UserResponse user = (await createResponse.Content
            .ReadFromJsonAsync<ApiResponse<UserResponse>>(TestContext.Current.CancellationToken))!.Data!;

        using HttpResponseMessage roleResponse = await client.PostAsJsonAsync(
            new Uri($"/api/v1/users/{user.Id}/roles", UriKind.Relative),
            new AssignUserRolesRequest { Roles = [DormFlowRoles.Technician] },
            TestContext.Current.CancellationToken);
        roleResponse.EnsureSuccessStatusCode();

        using HttpResponseMessage dormitoryResponse = await client.PostAsJsonAsync(
            new Uri($"/api/v1/users/{user.Id}/dormitories", UriKind.Relative),
            new AssignUserDormitoryRequest { DormitoryId = dormitoryId, IsPrimary = true },
            TestContext.Current.CancellationToken);
        dormitoryResponse.EnsureSuccessStatusCode();

        return user;
    }

    private static async Task<(OrganizationResponse Organization, DormitoryResponse Dormitory, RoomResponse Room)>
        CreatePropertyHierarchyAsync(HttpClient client)
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
        DormitoryResponse dormitory = (await dormResponse.Content
            .ReadFromJsonAsync<ApiResponse<DormitoryResponse>>(TestContext.Current.CancellationToken))!.Data!;

        string buildingCode = $"B{Guid.NewGuid():N}";
        using HttpResponseMessage buildingResponse = await client.PostAsJsonAsync(
            new Uri("/api/v1/buildings", UriKind.Relative),
            new CreateBuildingRequest { DormitoryId = dormitory.Id, Code = buildingCode, Name = $"อาคาร {buildingCode}", TotalFloors = 3 },
            TestContext.Current.CancellationToken);
        BuildingResponse building = (await buildingResponse.Content
            .ReadFromJsonAsync<ApiResponse<BuildingResponse>>(TestContext.Current.CancellationToken))!.Data!;

        using HttpResponseMessage floorResponse = await client.PostAsJsonAsync(
            new Uri("/api/v1/floors", UriKind.Relative),
            new CreateFloorRequest { BuildingId = building.Id, FloorNumber = 1, Name = "ชั้น 1" },
            TestContext.Current.CancellationToken);
        FloorResponse floor = (await floorResponse.Content
            .ReadFromJsonAsync<ApiResponse<FloorResponse>>(TestContext.Current.CancellationToken))!.Data!;

        string roomTypeCode = $"RT{Guid.NewGuid():N}";
        using HttpResponseMessage roomTypeResponse = await client.PostAsJsonAsync(
            new Uri("/api/v1/room-types", UriKind.Relative),
            new CreateRoomTypeRequest
            {
                DormitoryId = dormitory.Id,
                Code = roomTypeCode,
                Name = $"ประเภทห้อง {roomTypeCode}",
                MonthlyRent = 3500,
                DepositAmount = 3500,
                MaximumOccupants = 2,
            },
            TestContext.Current.CancellationToken);
        RoomTypeResponse roomType = (await roomTypeResponse.Content
            .ReadFromJsonAsync<ApiResponse<RoomTypeResponse>>(TestContext.Current.CancellationToken))!.Data!;

        using HttpResponseMessage roomResponse = await client.PostAsJsonAsync(
            new Uri("/api/v1/rooms", UriKind.Relative),
            new CreateRoomRequest
            {
                BuildingId = building.Id,
                FloorId = floor.Id,
                RoomTypeId = roomType.Id,
                RoomNumber = "101",
                MonthlyRent = 3500,
                DepositAmount = 3500,
                MaximumOccupants = 2,
            },
            TestContext.Current.CancellationToken);
        RoomResponse room = (await roomResponse.Content
            .ReadFromJsonAsync<ApiResponse<RoomResponse>>(TestContext.Current.CancellationToken))!.Data!;

        return (organization, dormitory, room);
    }
}
