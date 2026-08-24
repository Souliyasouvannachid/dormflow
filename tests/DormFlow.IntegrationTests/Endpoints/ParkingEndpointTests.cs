using System.Net;
using System.Net.Http.Json;
using DormFlow.Application.Common.Abstractions;
using DormFlow.Contracts.Buildings;
using DormFlow.Contracts.Common;
using DormFlow.Contracts.Dormitories;
using DormFlow.Contracts.Floors;
using DormFlow.Contracts.Organizations;
using DormFlow.Contracts.Parking;
using DormFlow.Contracts.RoomTypes;
using DormFlow.Contracts.Rooms;
using DormFlow.Contracts.Tenants;
using DormFlow.IntegrationTests.Infrastructure;
using FluentAssertions;
using Xunit;

namespace DormFlow.IntegrationTests.Endpoints;

/// <summary>ทดสอบการจดทะเบียนและสิ้นสุดที่จอดรถผ่าน HTTP จริง</summary>
[Collection(ApiTestFixtures.Name)]
public class ParkingEndpointTests(DormFlowApiFactory factory)
{
    private readonly DormFlowApiFactory _factory = factory;

    [Fact]
    public async Task จดทะเบียนที่จอดรถใหม่_ต้องได้สถานะ_Active()
    {
        using HttpClient client = await TestUserFactory.CreateAuthenticatedClientAsync(
            _factory,
            DormFlowRoles.SuperAdmin);

        (RoomResponse room, TenantResponse tenant) = await CreateRoomWithTenantAsync(client);

        using HttpResponseMessage response = await client.PostAsJsonAsync(
            new Uri("/api/v1/parking", UriKind.Relative),
            new RegisterParkingRequest
            {
                RoomId = room.Id,
                TenantId = tenant.Id,
                LicensePlate = "กข-1234",
                VehicleType = "Car",
                Brand = "Toyota",
                Color = "ขาว",
            },
            TestContext.Current.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        ParkingRegistrationResponse registration = (await response.Content
            .ReadFromJsonAsync<ApiResponse<ParkingRegistrationResponse>>(TestContext.Current.CancellationToken))!.Data!;

        registration.Status.Should().Be("Active");
        registration.LicensePlate.Should().Be("กข-1234");
    }

    [Fact]
    public async Task สิ้นสุดการจดทะเบียนที่จอดรถ_ต้องได้สถานะ_Ended()
    {
        using HttpClient client = await TestUserFactory.CreateAuthenticatedClientAsync(
            _factory,
            DormFlowRoles.SuperAdmin);

        (RoomResponse room, TenantResponse tenant) = await CreateRoomWithTenantAsync(client);
        ParkingRegistrationResponse registration = await RegisterParkingAsync(client, room.Id, tenant.Id, "กข-5678");

        using HttpResponseMessage response = await client.PostAsync(
            new Uri($"/api/v1/parking/{registration.Id}/release", UriKind.Relative),
            content: null,
            TestContext.Current.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        ParkingRegistrationResponse released = (await response.Content
            .ReadFromJsonAsync<ApiResponse<ParkingRegistrationResponse>>(TestContext.Current.CancellationToken))!.Data!;

        released.Status.Should().Be("Ended");
        released.EndedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task จดทะเบียนทะเบียนรถที่มีการจดทะเบียนยังไม่สิ้นสุดอยู่แล้วซ้ำ_ต้องได้_409()
    {
        using HttpClient client = await TestUserFactory.CreateAuthenticatedClientAsync(
            _factory,
            DormFlowRoles.SuperAdmin);

        (RoomResponse room, TenantResponse tenant) = await CreateRoomWithTenantAsync(client);
        await RegisterParkingAsync(client, room.Id, tenant.Id, "กข-9999");

        using HttpResponseMessage response = await client.PostAsJsonAsync(
            new Uri("/api/v1/parking", UriKind.Relative),
            new RegisterParkingRequest
            {
                RoomId = room.Id,
                TenantId = tenant.Id,
                LicensePlate = "กข-9999",
                VehicleType = "Motorcycle",
            },
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task จดทะเบียนทะเบียนรถซ้ำได้หลังจากรายการเดิมสิ้นสุดแล้ว_ต้องสำเร็จ()
    {
        using HttpClient client = await TestUserFactory.CreateAuthenticatedClientAsync(
            _factory,
            DormFlowRoles.SuperAdmin);

        (RoomResponse room, TenantResponse tenant) = await CreateRoomWithTenantAsync(client);
        ParkingRegistrationResponse first = await RegisterParkingAsync(client, room.Id, tenant.Id, "กข-1111");

        await client.PostAsync(
            new Uri($"/api/v1/parking/{first.Id}/release", UriKind.Relative),
            content: null,
            TestContext.Current.CancellationToken);

        using HttpResponseMessage response = await client.PostAsJsonAsync(
            new Uri("/api/v1/parking", UriKind.Relative),
            new RegisterParkingRequest
            {
                RoomId = room.Id,
                TenantId = tenant.Id,
                LicensePlate = "กข-1111",
                VehicleType = "Car",
            },
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    private static async Task<ParkingRegistrationResponse> RegisterParkingAsync(
        HttpClient client, Guid roomId, Guid tenantId, string licensePlate)
    {
        using HttpResponseMessage response = await client.PostAsJsonAsync(
            new Uri("/api/v1/parking", UriKind.Relative),
            new RegisterParkingRequest
            {
                RoomId = roomId,
                TenantId = tenantId,
                LicensePlate = licensePlate,
                VehicleType = "Car",
            },
            TestContext.Current.CancellationToken);

        response.EnsureSuccessStatusCode();

        return (await response.Content
            .ReadFromJsonAsync<ApiResponse<ParkingRegistrationResponse>>(TestContext.Current.CancellationToken))!.Data!;
    }

    private static async Task<(RoomResponse Room, TenantResponse Tenant)> CreateRoomWithTenantAsync(HttpClient client)
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

        using HttpResponseMessage tenantResponse = await client.PostAsJsonAsync(
            new Uri("/api/v1/tenants", UriKind.Relative),
            new CreateTenantRequest
            {
                OrganizationId = organization.Id,
                FirstName = "สมชาย",
                LastName = $"ทดสอบ-{Guid.NewGuid():N}",
                Phone = "0800000000",
            },
            TestContext.Current.CancellationToken);
        TenantResponse tenant = (await tenantResponse.Content
            .ReadFromJsonAsync<ApiResponse<TenantResponse>>(TestContext.Current.CancellationToken))!.Data!;

        return (room, tenant);
    }
}
