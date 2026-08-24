using System.Net;
using System.Net.Http.Json;
using DormFlow.Application.Common.Abstractions;
using DormFlow.Contracts.Buildings;
using DormFlow.Contracts.Common;
using DormFlow.Contracts.Dormitories;
using DormFlow.Contracts.Floors;
using DormFlow.Contracts.Parcels;
using DormFlow.Contracts.Organizations;
using DormFlow.Contracts.RoomTypes;
using DormFlow.Contracts.Rooms;
using DormFlow.IntegrationTests.Infrastructure;
using FluentAssertions;
using Xunit;

namespace DormFlow.IntegrationTests.Endpoints;

/// <summary>ทดสอบการบันทึกและรับพัสดุผ่าน HTTP จริง</summary>
[Collection(ApiTestFixtures.Name)]
public class ParcelsEndpointTests(DormFlowApiFactory factory)
{
    private readonly DormFlowApiFactory _factory = factory;

    [Fact]
    public async Task บันทึกพัสดุที่มาถึงใหม่_ต้องได้สถานะ_Received()
    {
        using HttpClient client = await TestUserFactory.CreateAuthenticatedClientAsync(
            _factory,
            DormFlowRoles.SuperAdmin);

        RoomResponse room = await CreateRoomAsync(client);

        using HttpResponseMessage response = await client.PostAsJsonAsync(
            new Uri("/api/v1/parcels", UriKind.Relative),
            new LogParcelRequest
            {
                RoomId = room.Id,
                CourierName = "Kerry Express",
                TrackingNumber = "TH1234567890",
                Description = "กล่องพัสดุขนาดกลาง",
            },
            TestContext.Current.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        ParcelResponse parcel = (await response.Content
            .ReadFromJsonAsync<ApiResponse<ParcelResponse>>(TestContext.Current.CancellationToken))!.Data!;

        parcel.Status.Should().Be("Received");
        parcel.CourierName.Should().Be("Kerry Express");
        parcel.PickedUpAt.Should().BeNull();
    }

    [Fact]
    public async Task รับพัสดุแล้ว_ต้องได้สถานะ_PickedUp()
    {
        using HttpClient client = await TestUserFactory.CreateAuthenticatedClientAsync(
            _factory,
            DormFlowRoles.SuperAdmin);

        RoomResponse room = await CreateRoomAsync(client);
        ParcelResponse parcel = await LogParcelAsync(client, room.Id);

        using HttpResponseMessage response = await client.PostAsJsonAsync(
            new Uri($"/api/v1/parcels/{parcel.Id}/pickup", UriKind.Relative),
            new PickupParcelRequest { PickedUpByName = "สมชาย ใจดี" },
            TestContext.Current.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        ParcelResponse pickedUp = (await response.Content
            .ReadFromJsonAsync<ApiResponse<ParcelResponse>>(TestContext.Current.CancellationToken))!.Data!;

        pickedUp.Status.Should().Be("PickedUp");
        pickedUp.PickedUpAt.Should().NotBeNull();
        pickedUp.PickedUpByName.Should().Be("สมชาย ใจดี");
    }

    [Fact]
    public async Task รับพัสดุที่ถูกรับไปแล้วซ้ำ_ต้องได้_409()
    {
        using HttpClient client = await TestUserFactory.CreateAuthenticatedClientAsync(
            _factory,
            DormFlowRoles.SuperAdmin);

        RoomResponse room = await CreateRoomAsync(client);
        ParcelResponse parcel = await LogParcelAsync(client, room.Id);

        await client.PostAsJsonAsync(
            new Uri($"/api/v1/parcels/{parcel.Id}/pickup", UriKind.Relative),
            new PickupParcelRequest(),
            TestContext.Current.CancellationToken);

        using HttpResponseMessage secondResponse = await client.PostAsJsonAsync(
            new Uri($"/api/v1/parcels/{parcel.Id}/pickup", UriKind.Relative),
            new PickupParcelRequest(),
            TestContext.Current.CancellationToken);

        secondResponse.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    private static async Task<ParcelResponse> LogParcelAsync(HttpClient client, Guid roomId)
    {
        using HttpResponseMessage response = await client.PostAsJsonAsync(
            new Uri("/api/v1/parcels", UriKind.Relative),
            new LogParcelRequest { RoomId = roomId, Description = "ทดสอบ" },
            TestContext.Current.CancellationToken);

        response.EnsureSuccessStatusCode();

        return (await response.Content
            .ReadFromJsonAsync<ApiResponse<ParcelResponse>>(TestContext.Current.CancellationToken))!.Data!;
    }

    private static async Task<RoomResponse> CreateRoomAsync(HttpClient client)
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

        return (await roomResponse.Content
            .ReadFromJsonAsync<ApiResponse<RoomResponse>>(TestContext.Current.CancellationToken))!.Data!;
    }
}
