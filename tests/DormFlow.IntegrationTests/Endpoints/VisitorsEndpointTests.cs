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
using DormFlow.Contracts.Visitors;
using DormFlow.IntegrationTests.Infrastructure;
using FluentAssertions;
using Xunit;

namespace DormFlow.IntegrationTests.Endpoints;

/// <summary>ການທົດສອບເຂົ້າ-ທາງອອກຂອງນັກທ່ອງທ່ຽວທີ່ຜ່ານ HTTP ຄວາມຈິງ</summary>
[Collection(ApiTestFixtures.Name)]
public class VisitorsEndpointTests(DormFlowApiFactory factory)
{
    private readonly DormFlowApiFactory _factory = factory;

    [Fact]
    public async Task ບັນທຶກການເຂົ້າມາຂອງນັກທ່ອງທ່ຽວໃຝ່_ຕ້ອງໄດ້ຮັບການສະຖານະ_CheckedIn()
    {
        using HttpClient client = await TestUserFactory.CreateAuthenticatedClientAsync(
            _factory,
            DormFlowRoles.SuperAdmin);

        RoomResponse room = await CreateRoomAsync(client);

        using HttpResponseMessage response = await client.PostAsJsonAsync(
            new Uri("/api/v1/visitors", UriKind.Relative),
            new CheckInVisitorRequest
            {
                RoomId = room.Id,
                VisitorName = "Somying ມາຢ້ຽມຢາມ",
                VisitorPhone = "0891234567",
                Purpose = "ຢ້ຽມຢາມຝູ່ເພື່ອນ",
            },
            TestContext.Current.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        VisitorResponse visitor = (await response.Content
            .ReadFromJsonAsync<ApiResponse<VisitorResponse>>(TestContext.Current.CancellationToken))!.Data!;

        visitor.Status.Should().Be("CheckedIn");
        visitor.VisitorName.Should().Be("Somying ມາຢ້ຽມຢາມ");
        visitor.CheckOutAt.Should().BeNull();
    }

    [Fact]
    public async Task ບັນທຶກຜູ້ເຂົ້າຊົມ_ຕ້ອງໄດ້ຮັບການສະຖານະ_CheckedOut()
    {
        using HttpClient client = await TestUserFactory.CreateAuthenticatedClientAsync(
            _factory,
            DormFlowRoles.SuperAdmin);

        RoomResponse room = await CreateRoomAsync(client);
        VisitorResponse visitor = await CheckInVisitorAsync(client, room.Id);

        using HttpResponseMessage response = await client.PostAsync(
            new Uri($"/api/v1/visitors/{visitor.Id}/checkout", UriKind.Relative),
            content: null,
            TestContext.Current.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        VisitorResponse checkedOut = (await response.Content
            .ReadFromJsonAsync<ApiResponse<VisitorResponse>>(TestContext.Current.CancellationToken))!.Data!;

        checkedOut.Status.Should().Be("CheckedOut");
        checkedOut.CheckOutAt.Should().NotBeNull();
    }

    [Fact]
    public async Task ບັນທຶກການອອກຂອງຜູ້ເຂົ້າຊົມຊ້ຳຜູ້ທີ່ໄດ້ອອກໄປແລ້ວ_ຕ້ອງໄດ້ຮັບ_409()
    {
        using HttpClient client = await TestUserFactory.CreateAuthenticatedClientAsync(
            _factory,
            DormFlowRoles.SuperAdmin);

        RoomResponse room = await CreateRoomAsync(client);
        VisitorResponse visitor = await CheckInVisitorAsync(client, room.Id);

        await client.PostAsync(
            new Uri($"/api/v1/visitors/{visitor.Id}/checkout", UriKind.Relative),
            content: null,
            TestContext.Current.CancellationToken);

        using HttpResponseMessage secondResponse = await client.PostAsync(
            new Uri($"/api/v1/visitors/{visitor.Id}/checkout", UriKind.Relative),
            content: null,
            TestContext.Current.CancellationToken);

        secondResponse.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    private static async Task<VisitorResponse> CheckInVisitorAsync(HttpClient client, Guid roomId)
    {
        using HttpResponseMessage response = await client.PostAsJsonAsync(
            new Uri("/api/v1/visitors", UriKind.Relative),
            new CheckInVisitorRequest { RoomId = roomId, VisitorName = "ການທົດສອບ" },
            TestContext.Current.CancellationToken);

        response.EnsureSuccessStatusCode();

        return (await response.Content
            .ReadFromJsonAsync<ApiResponse<VisitorResponse>>(TestContext.Current.CancellationToken))!.Data!;
    }

    private static async Task<RoomResponse> CreateRoomAsync(HttpClient client)
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
        DormitoryResponse dormitory = (await dormResponse.Content
            .ReadFromJsonAsync<ApiResponse<DormitoryResponse>>(TestContext.Current.CancellationToken))!.Data!;

        string buildingCode = $"B{Guid.NewGuid():N}";
        using HttpResponseMessage buildingResponse = await client.PostAsJsonAsync(
            new Uri("/api/v1/buildings", UriKind.Relative),
            new CreateBuildingRequest { DormitoryId = dormitory.Id, Code = buildingCode, Name = $"ອາຄານ {buildingCode}", TotalFloors = 3 },
            TestContext.Current.CancellationToken);
        BuildingResponse building = (await buildingResponse.Content
            .ReadFromJsonAsync<ApiResponse<BuildingResponse>>(TestContext.Current.CancellationToken))!.Data!;

        using HttpResponseMessage floorResponse = await client.PostAsJsonAsync(
            new Uri("/api/v1/floors", UriKind.Relative),
            new CreateFloorRequest { BuildingId = building.Id, FloorNumber = 1, Name = "ຊັ້ນ 1" },
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
                Name = $"ປະເພດຫ້ອງ {roomTypeCode}",
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
