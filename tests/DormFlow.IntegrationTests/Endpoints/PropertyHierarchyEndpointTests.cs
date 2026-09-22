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
/// ທົດສອບລະບົບຕ່ອງໂສ້ທັງໝົດຂອງການຈັດລໍາດັບຊັບສິນຜ່ານ HTTP ຄວາມຈິງ
/// Organization -> Dormitory -> Building -> Floor -> RoomType -> Room
/// ເນື່ອງຈາກວ່າການສ້າງຫ້ອງຜຶ່ງແມ່ນຂຶ້ນກັບຂໍ້ມູນທີ່ຖືກຕ້ອງຂອງຊັ້ນທີ່ຜ່ານມາທັງຝົດ
/// ດັ່ງນັ້ນກວມເອົາທັງສອງ 6 ໂມດູນໃນເລື່ອງດຽວກັນ
/// </summary>
[Collection(ApiTestFixtures.Name)]
public class PropertyHierarchyEndpointTests(DormFlowApiFactory factory)
{
    private readonly DormFlowApiFactory _factory = factory;

    [Fact]
    public async Task SuperAdmin_ຕ້ອງສາມາດສ້າງແລະອ່ານຂໍ້ມູນໄດ້ຕະຫຼອດແຖວທັງໝົດໄປຫາຫ້ອງຮັບແຂກ()
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
    public async Task ສ້າງຫ້ອງທີ່ມີຊັ້ນທີ່ບໍ່ຢູ່ໃນອາຄານດຽວກັນ_ຕ້ອງໄດ້ຮັບ_400()
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
    public async Task ສ້າງຫໍພັກທີ່ມີIDຊໍ້າກັນຢູ່ໃນອົງກອນດຽວກັນ_ຕ້ອງໄດ້ຮັບ_409()
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
                Name = "ລະຫັດຊ້ຳຂອງຫໍພັກ",
                BillingDay = 1,
                PaymentDueDay = 5,
                LateFeeType = "Fixed",
            },
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task Owner_ຕ້ອງບໍ່ສາມາດເຂົ້າເຖິງຂໍ້ມູນຫໍພັກຈາກອົງກອນອື່ນ()
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

        // ຄໍາຕອບ 404 ແທນ 403 ໃນຄໍາສັ່ງທີ່ຈະບໍ່ Owner ຂອງອົງກອນ B ມັນສາມາດສົມມຸດໄດ້ວ່າຫໍພັກນີ້ຕົວຈິງແລ້ວມີຢູ່ໃນອົງກອນ A
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Owner_ຫໍພັກສາມາດສ້າງໄດ້ພາຍໃນອົງກອນຂອງຕົນເອງເທົ່ານັ້ນ()
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

        // ລະບຸ organizationId ຂອງອົງກອນອື່ນໆໃນການຮ້ອງຂໍ, ແຕ່ Owner ຕ້ອງຖືກບັງຄັບໃຫ້ສ້າງຢູ່ໃນອົງກອນຂອງຕົນເອງສະເໝີ
        using HttpResponseMessage response = await ownerClient.PostAsJsonAsync(
            new Uri("/api/v1/dormitories", UriKind.Relative),
            new CreateDormitoryRequest
            {
                OrganizationId = otherOrganization.Id,
                Code = $"D{Guid.NewGuid():N}",
                Name = "ຫໍພັກຂອງ Owner",
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
            new CreateOrganizationRequest { Code = code, Name = $"ອົງກອນການທົດສອບ {code}" },
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
                Name = $"ຫໍພັກການທົດສອບ {code}",
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
                Name = $"ຕຶກທົດສອບ {code}",
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
            new CreateFloorRequest { BuildingId = buildingId, FloorNumber = 1, Name = "ຊັ້ນ 1" },
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
                Name = $"ປະເພດຫ້ອງທົດລອງ {code}",
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
