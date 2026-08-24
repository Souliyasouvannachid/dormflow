using System.Net;
using System.Net.Http.Json;
using DormFlow.Application.Common.Abstractions;
using DormFlow.Contracts.Buildings;
using DormFlow.Contracts.Common;
using DormFlow.Contracts.Dormitories;
using DormFlow.Contracts.Floors;
using DormFlow.Contracts.Organizations;
using DormFlow.Contracts.RoomTypes;
using DormFlow.Contracts.Rooms;
using DormFlow.Domain.Enums;
using DormFlow.IntegrationTests.Infrastructure;
using FluentAssertions;
using Xunit;

namespace DormFlow.IntegrationTests.Endpoints;

/// <summary>
/// ทดสอบทั้งสายของลำดับชั้นทรัพย์สินผ่าน HTTP จริง
/// Organization -> Dormitory -> Building -> Floor -> RoomType -> Room
/// เพราะการสร้างห้องพักหนึ่งห้องต้องพึ่งพาข้อมูลที่ถูกต้องของทุกชั้นก่อนหน้า
/// จึงครอบคลุมทั้ง 6 โมดูลในการเดินเรื่องเดียวกัน
/// </summary>
[Collection(ApiTestFixtures.Name)]
public class PropertyHierarchyEndpointTests(DormFlowApiFactory factory)
{
    private readonly DormFlowApiFactory _factory = factory;

    [Fact]
    public async Task SuperAdmin_ต้องสร้างและอ่านข้อมูลได้ตลอดทั้งสายจนถึงห้องพัก()
    {
        using HttpClient client = await TestUserFactory.CreateAuthenticatedClientAsync(
            _factory,
            DormFlowRoles.SuperAdmin);

        OrganizationResponse organization = await CreateOrganizationAsync(client);
        DormitoryResponse dormitory = await CreateDormitoryAsync(client, organization.Id);
        BuildingResponse building = await CreateBuildingAsync(client, dormitory.Id);
        FloorResponse floor = await CreateFloorAsync(client, building.Id);
        RoomTypeResponse roomType = await CreateRoomTypeAsync(client, dormitory.Id);
        RoomResponse room = await CreateRoomAsync(client, building.Id, floor.Id, roomType.Id);

        room.OrganizationId.Should().Be(organization.Id);
        room.DormitoryId.Should().Be(dormitory.Id);
        room.BuildingId.Should().Be(building.Id);
        room.FloorId.Should().Be(floor.Id);
        room.RoomTypeId.Should().Be(roomType.Id);
        room.Status.Should().Be(nameof(RoomStatus.Available));

        using HttpResponseMessage getResponse = await client.GetAsync(
            new Uri($"/api/v1/rooms/{room.Id}", UriKind.Relative),
            TestContext.Current.CancellationToken);
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        using HttpResponseMessage availableResponse = await client.GetAsync(
            new Uri($"/api/v1/rooms/available?dormitoryId={dormitory.Id}", UriKind.Relative),
            TestContext.Current.CancellationToken);
        ApiResponse<PagedResult<RoomResponse>>? availableBody = await availableResponse.Content
            .ReadFromJsonAsync<ApiResponse<PagedResult<RoomResponse>>>(TestContext.Current.CancellationToken);
        availableBody!.Data!.Items.Should().ContainSingle(item => item.Id == room.Id);

        using HttpResponseMessage summaryResponse = await client.GetAsync(
            new Uri($"/api/v1/rooms/summary?dormitoryId={dormitory.Id}", UriKind.Relative),
            TestContext.Current.CancellationToken);
        ApiResponse<RoomSummaryResponse>? summaryBody = await summaryResponse.Content
            .ReadFromJsonAsync<ApiResponse<RoomSummaryResponse>>(TestContext.Current.CancellationToken);
        summaryBody!.Data!.TotalRooms.Should().Be(1);
        summaryBody.Data.Available.Should().Be(1);

        using HttpResponseMessage statusResponse = await client.PatchAsJsonAsync(
            new Uri($"/api/v1/rooms/{room.Id}/status", UriKind.Relative),
            new ChangeStatusRequest<RoomStatus> { Status = RoomStatus.Maintenance },
            TestContext.Current.CancellationToken);
        statusResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        using HttpResponseMessage deleteResponse = await client.DeleteAsync(
            new Uri($"/api/v1/rooms/{room.Id}", UriKind.Relative),
            TestContext.Current.CancellationToken);
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        using HttpResponseMessage getAfterDeleteResponse = await client.GetAsync(
            new Uri($"/api/v1/rooms/{room.Id}", UriKind.Relative),
            TestContext.Current.CancellationToken);
        getAfterDeleteResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task สร้างห้องด้วยชั้นที่ไม่ได้อยู่ในอาคารเดียวกัน_ต้องได้_400()
    {
        using HttpClient client = await TestUserFactory.CreateAuthenticatedClientAsync(
            _factory,
            DormFlowRoles.SuperAdmin);

        OrganizationResponse organization = await CreateOrganizationAsync(client);
        DormitoryResponse dormitory = await CreateDormitoryAsync(client, organization.Id);
        BuildingResponse buildingOne = await CreateBuildingAsync(client, dormitory.Id);
        BuildingResponse buildingTwo = await CreateBuildingAsync(client, dormitory.Id);
        FloorResponse floorOfBuildingTwo = await CreateFloorAsync(client, buildingTwo.Id);
        RoomTypeResponse roomType = await CreateRoomTypeAsync(client, dormitory.Id);

        using HttpResponseMessage response = await client.PostAsJsonAsync(
            new Uri("/api/v1/rooms", UriKind.Relative),
            new CreateRoomRequest
            {
                BuildingId = buildingOne.Id,
                FloorId = floorOfBuildingTwo.Id,
                RoomTypeId = roomType.Id,
                RoomNumber = "101",
                MonthlyRent = 3000,
                DepositAmount = 3000,
                MaximumOccupants = 1,
            },
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task สร้างหอพักด้วยรหัสซ้ำในองค์กรเดียวกัน_ต้องได้_409()
    {
        using HttpClient client = await TestUserFactory.CreateAuthenticatedClientAsync(
            _factory,
            DormFlowRoles.SuperAdmin);

        OrganizationResponse organization = await CreateOrganizationAsync(client);
        DormitoryResponse dormitory = await CreateDormitoryAsync(client, organization.Id);

        using HttpResponseMessage response = await client.PostAsJsonAsync(
            new Uri("/api/v1/dormitories", UriKind.Relative),
            new CreateDormitoryRequest
            {
                OrganizationId = organization.Id,
                Code = dormitory.Code,
                Name = "หอพักซ้ำรหัส",
                BillingDay = 1,
                PaymentDueDay = 5,
                LateFeeType = "Fixed",
            },
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task Owner_ต้องเข้าถึงข้อมูลหอพักขององค์กรอื่นไม่ได้()
    {
        using HttpClient superAdminClient = await TestUserFactory.CreateAuthenticatedClientAsync(
            _factory,
            DormFlowRoles.SuperAdmin);

        OrganizationResponse organizationA = await CreateOrganizationAsync(superAdminClient);
        OrganizationResponse organizationB = await CreateOrganizationAsync(superAdminClient);
        DormitoryResponse dormitoryOfOrganizationA = await CreateDormitoryAsync(superAdminClient, organizationA.Id);

        using HttpClient ownerOfOrganizationBClient = await TestUserFactory.CreateAuthenticatedClientAsync(
            _factory,
            DormFlowRoles.Owner,
            organizationB.Id);

        using HttpResponseMessage response = await ownerOfOrganizationBClient.GetAsync(
            new Uri($"/api/v1/dormitories/{dormitoryOfOrganizationA.Id}", UriKind.Relative),
            TestContext.Current.CancellationToken);

        // ตอบ 404 แทน 403 เพื่อไม่ให้ Owner ขององค์กร B เดาได้ว่ามีหอพักนี้อยู่จริงในองค์กร A
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Owner_ต้องสร้างหอพักได้เฉพาะในองค์กรของตนเองเท่านั้น()
    {
        using HttpClient superAdminClient = await TestUserFactory.CreateAuthenticatedClientAsync(
            _factory,
            DormFlowRoles.SuperAdmin);

        OrganizationResponse ownOrganization = await CreateOrganizationAsync(superAdminClient);
        OrganizationResponse otherOrganization = await CreateOrganizationAsync(superAdminClient);

        using HttpClient ownerClient = await TestUserFactory.CreateAuthenticatedClientAsync(
            _factory,
            DormFlowRoles.Owner,
            ownOrganization.Id);

        // ระบุ organizationId ขององค์กรอื่นมาในคำขอ แต่ Owner ต้องถูกบังคับให้สร้างในองค์กรของตนเองเสมอ
        using HttpResponseMessage response = await ownerClient.PostAsJsonAsync(
            new Uri("/api/v1/dormitories", UriKind.Relative),
            new CreateDormitoryRequest
            {
                OrganizationId = otherOrganization.Id,
                Code = $"D{Guid.NewGuid():N}",
                Name = "หอพักของ Owner",
                BillingDay = 1,
                PaymentDueDay = 5,
                LateFeeType = "Fixed",
            },
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        ApiResponse<DormitoryResponse>? body = await response.Content
            .ReadFromJsonAsync<ApiResponse<DormitoryResponse>>(TestContext.Current.CancellationToken);

        body!.Data!.OrganizationId.Should().Be(ownOrganization.Id);
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

    private static async Task<BuildingResponse> CreateBuildingAsync(HttpClient client, Guid dormitoryId)
    {
        string code = $"B{Guid.NewGuid():N}";

        using HttpResponseMessage response = await client.PostAsJsonAsync(
            new Uri("/api/v1/buildings", UriKind.Relative),
            new CreateBuildingRequest
            {
                DormitoryId = dormitoryId,
                Code = code,
                Name = $"อาคารทดสอบ {code}",
                TotalFloors = 3,
            },
            TestContext.Current.CancellationToken);

        response.EnsureSuccessStatusCode();

        ApiResponse<BuildingResponse>? body = await response.Content
            .ReadFromJsonAsync<ApiResponse<BuildingResponse>>(TestContext.Current.CancellationToken);

        return body!.Data!;
    }

    private static async Task<FloorResponse> CreateFloorAsync(HttpClient client, Guid buildingId)
    {
        using HttpResponseMessage response = await client.PostAsJsonAsync(
            new Uri("/api/v1/floors", UriKind.Relative),
            new CreateFloorRequest { BuildingId = buildingId, FloorNumber = 1, Name = "ชั้น 1" },
            TestContext.Current.CancellationToken);

        response.EnsureSuccessStatusCode();

        ApiResponse<FloorResponse>? body = await response.Content
            .ReadFromJsonAsync<ApiResponse<FloorResponse>>(TestContext.Current.CancellationToken);

        return body!.Data!;
    }

    private static async Task<RoomTypeResponse> CreateRoomTypeAsync(HttpClient client, Guid dormitoryId)
    {
        string code = $"RT{Guid.NewGuid():N}";

        using HttpResponseMessage response = await client.PostAsJsonAsync(
            new Uri("/api/v1/room-types", UriKind.Relative),
            new CreateRoomTypeRequest
            {
                DormitoryId = dormitoryId,
                Code = code,
                Name = $"ประเภทห้องทดสอบ {code}",
                MonthlyRent = 3500,
                DepositAmount = 3500,
                MaximumOccupants = 2,
            },
            TestContext.Current.CancellationToken);

        response.EnsureSuccessStatusCode();

        ApiResponse<RoomTypeResponse>? body = await response.Content
            .ReadFromJsonAsync<ApiResponse<RoomTypeResponse>>(TestContext.Current.CancellationToken);

        return body!.Data!;
    }

    private static async Task<RoomResponse> CreateRoomAsync(
        HttpClient client,
        Guid buildingId,
        Guid floorId,
        Guid roomTypeId)
    {
        using HttpResponseMessage response = await client.PostAsJsonAsync(
            new Uri("/api/v1/rooms", UriKind.Relative),
            new CreateRoomRequest
            {
                BuildingId = buildingId,
                FloorId = floorId,
                RoomTypeId = roomTypeId,
                RoomNumber = "101",
                MonthlyRent = 3500,
                DepositAmount = 3500,
                MaximumOccupants = 2,
            },
            TestContext.Current.CancellationToken);

        response.EnsureSuccessStatusCode();

        ApiResponse<RoomResponse>? body = await response.Content
            .ReadFromJsonAsync<ApiResponse<RoomResponse>>(TestContext.Current.CancellationToken);

        return body!.Data!;
    }
}
