using System.Net;
using System.Net.Http.Json;
using DormFlow.Application.Common.Abstractions;
using DormFlow.Contracts.Buildings;
using DormFlow.Contracts.Common;
using DormFlow.Contracts.Dormitories;
using DormFlow.Contracts.Floors;
using DormFlow.Contracts.MeterReadings;
using DormFlow.Contracts.Organizations;
using DormFlow.Contracts.RoomTypes;
using DormFlow.Contracts.Rooms;
using DormFlow.IntegrationTests.Infrastructure;
using FluentAssertions;
using Xunit;

namespace DormFlow.IntegrationTests.Endpoints;

/// <summary>ทดสอบการจดค่ามิเตอร์ผ่าน HTTP จริง</summary>
[Collection(ApiTestFixtures.Name)]
public class MeterReadingsEndpointTests(DormFlowApiFactory factory)
{
    private readonly DormFlowApiFactory _factory = factory;

    [Fact]
    public async Task จดค่ามิเตอร์ครั้งแรก_ค่าครั้งก่อนหน้าต้องเป็นศูนย์()
    {
        using HttpClient client = await TestUserFactory.CreateAuthenticatedClientAsync(
            _factory,
            DormFlowRoles.SuperAdmin);

        RoomResponse room = await CreateRoomAsync(client);

        using HttpResponseMessage response = await client.PostAsJsonAsync(
            new Uri("/api/v1/meter-readings", UriKind.Relative),
            new CreateMeterReadingRequest
            {
                RoomId = room.Id,
                MeterType = "Water",
                PeriodMonth = new DateOnly(2026, 1, 15),
                ReadingDate = new DateOnly(2026, 1, 31),
                CurrentReading = 120,
            },
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        MeterReadingResponse reading = (await response.Content
            .ReadFromJsonAsync<ApiResponse<MeterReadingResponse>>(TestContext.Current.CancellationToken))!.Data!;

        reading.PreviousReading.Should().Be(0);
        reading.CurrentReading.Should().Be(120);
        reading.UnitsUsed.Should().Be(120);
        reading.PeriodMonth.Should().Be(new DateOnly(2026, 1, 1));
    }

    [Fact]
    public async Task จดค่ามิเตอร์เดือนถัดไป_ต้องดึงค่าครั้งก่อนหน้ามาต่ออัตโนมัติ()
    {
        using HttpClient client = await TestUserFactory.CreateAuthenticatedClientAsync(
            _factory,
            DormFlowRoles.SuperAdmin);

        RoomResponse room = await CreateRoomAsync(client);

        await CreateMeterReadingAsync(client, room.Id, "Electricity", new DateOnly(2026, 1, 1), 200);
        MeterReadingResponse second = await CreateMeterReadingAsync(
            client, room.Id, "Electricity", new DateOnly(2026, 2, 1), 350);

        second.PreviousReading.Should().Be(200);
        second.CurrentReading.Should().Be(350);
        second.UnitsUsed.Should().Be(150);
    }

    [Fact]
    public async Task จดค่ามิเตอร์ประเภทเดียวกันซ้ำในเดือนเดียวกัน_ต้องได้_409()
    {
        using HttpClient client = await TestUserFactory.CreateAuthenticatedClientAsync(
            _factory,
            DormFlowRoles.SuperAdmin);

        RoomResponse room = await CreateRoomAsync(client);

        await CreateMeterReadingAsync(client, room.Id, "Water", new DateOnly(2026, 3, 1), 50);

        using HttpResponseMessage duplicateResponse = await client.PostAsJsonAsync(
            new Uri("/api/v1/meter-readings", UriKind.Relative),
            new CreateMeterReadingRequest
            {
                RoomId = room.Id,
                MeterType = "Water",
                PeriodMonth = new DateOnly(2026, 3, 20),
                ReadingDate = new DateOnly(2026, 3, 25),
                CurrentReading = 60,
            },
            TestContext.Current.CancellationToken);

        duplicateResponse.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task จดค่ามิเตอร์ต่ำกว่าค่าครั้งก่อนหน้า_ต้องได้_400()
    {
        using HttpClient client = await TestUserFactory.CreateAuthenticatedClientAsync(
            _factory,
            DormFlowRoles.SuperAdmin);

        RoomResponse room = await CreateRoomAsync(client);

        await CreateMeterReadingAsync(client, room.Id, "Water", new DateOnly(2026, 1, 1), 500);

        using HttpResponseMessage response = await client.PostAsJsonAsync(
            new Uri("/api/v1/meter-readings", UriKind.Relative),
            new CreateMeterReadingRequest
            {
                RoomId = room.Id,
                MeterType = "Water",
                PeriodMonth = new DateOnly(2026, 2, 1),
                ReadingDate = new DateOnly(2026, 2, 28),
                CurrentReading = 400,
            },
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
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

    private static async Task<MeterReadingResponse> CreateMeterReadingAsync(
        HttpClient client,
        Guid roomId,
        string meterType,
        DateOnly periodMonth,
        decimal currentReading)
    {
        using HttpResponseMessage response = await client.PostAsJsonAsync(
            new Uri("/api/v1/meter-readings", UriKind.Relative),
            new CreateMeterReadingRequest
            {
                RoomId = roomId,
                MeterType = meterType,
                PeriodMonth = periodMonth,
                ReadingDate = periodMonth.AddDays(20),
                CurrentReading = currentReading,
            },
            TestContext.Current.CancellationToken);

        response.EnsureSuccessStatusCode();

        return (await response.Content
            .ReadFromJsonAsync<ApiResponse<MeterReadingResponse>>(TestContext.Current.CancellationToken))!.Data!;
    }
}
