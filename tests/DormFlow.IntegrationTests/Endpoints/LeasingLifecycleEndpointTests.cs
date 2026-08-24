using System.Net;
using System.Net.Http.Json;
using DormFlow.Application.Common.Abstractions;
using DormFlow.Contracts.Buildings;
using DormFlow.Contracts.Common;
using DormFlow.Contracts.Dormitories;
using DormFlow.Contracts.Floors;
using DormFlow.Contracts.Leases;
using DormFlow.Contracts.Organizations;
using DormFlow.Contracts.Reservations;
using DormFlow.Contracts.RoomTypes;
using DormFlow.Contracts.Rooms;
using DormFlow.Contracts.Tenants;
using DormFlow.Domain.Enums;
using DormFlow.IntegrationTests.Infrastructure;
using FluentAssertions;
using Xunit;

namespace DormFlow.IntegrationTests.Endpoints;

/// <summary>
/// ทดสอบวงจรชีวิตของการเช่าห้องพักทั้งสายผ่าน HTTP จริง
/// Tenant -> Reservation -> Contract (สร้าง/เปิดใช้งาน/สิ้นสุด)
/// เพราะ Contract สามารถสร้างได้สองทาง (ผ่านการจอง หรือทำสัญญาตรง) จึงครอบคลุมทั้งสองเส้นทาง
/// </summary>
[Collection(ApiTestFixtures.Name)]
public class LeasingLifecycleEndpointTests(DormFlowApiFactory factory)
{
    private readonly DormFlowApiFactory _factory = factory;

    [Fact]
    public async Task จองห้องแล้วแปลงเป็นสัญญา_เปิดใช้งาน_แล้วสิ้นสุดสัญญา_ต้องทำงานครบวงจร()
    {
        using HttpClient client = await TestUserFactory.CreateAuthenticatedClientAsync(
            _factory,
            DormFlowRoles.SuperAdmin);

        (OrganizationResponse organization, DormitoryResponse dormitory, RoomResponse room) =
            await CreatePropertyHierarchyAsync(client);
        TenantResponse tenant = await CreateTenantAsync(client, organization.Id);

        // จองห้อง -> ห้องต้องเปลี่ยนเป็น Reserved ทันที
        ReservationResponse reservation = await CreateReservationAsync(client, room.Id, tenant.Id);
        (await GetRoomAsync(client, room.Id)).Status.Should().Be("Reserved");

        // แปลงการจองเป็นสัญญา -> ห้องยังคง Reserved อยู่ (ยังไม่เช็คอิน) สัญญาอยู่ในสถานะ Draft
        ContractResponse contract = await CreateContractFromReservationAsync(client, room.Id, tenant.Id, reservation.Id);
        contract.Status.Should().Be("Draft");
        contract.ReservationId.Should().Be(reservation.Id);
        (await GetRoomAsync(client, room.Id)).Status.Should().Be("Reserved");

        // การจองเดิมต้องถูกทำเครื่องหมายว่าถูกแปลงเป็นสัญญาแล้ว
        using HttpResponseMessage reservationAfterResponse = await client.GetAsync(
            new Uri($"/api/v1/reservations/{reservation.Id}", UriKind.Relative),
            TestContext.Current.CancellationToken);
        ApiResponse<ReservationResponse>? reservationAfterBody = await reservationAfterResponse.Content
            .ReadFromJsonAsync<ApiResponse<ReservationResponse>>(TestContext.Current.CancellationToken);
        reservationAfterBody!.Data!.Status.Should().Be("ConvertedToContract");

        // เปิดใช้งานสัญญา (เช็คอิน) -> ห้องต้องเปลี่ยนเป็น Occupied
        using HttpResponseMessage activateResponse = await client.PostAsync(
            new Uri($"/api/v1/contracts/{contract.Id}/activate", UriKind.Relative),
            content: null,
            TestContext.Current.CancellationToken);
        activateResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        ApiResponse<ContractResponse>? activatedBody = await activateResponse.Content
            .ReadFromJsonAsync<ApiResponse<ContractResponse>>(TestContext.Current.CancellationToken);
        activatedBody!.Data!.Status.Should().Be("Active");
        activatedBody.Data.SignedAt.Should().NotBeNull();
        (await GetRoomAsync(client, room.Id)).Status.Should().Be("Occupied");

        // สิ้นสุดสัญญา (เช็คเอาท์) -> ห้องต้องเปลี่ยนเป็น Cleaning
        using HttpResponseMessage terminateResponse = await client.PostAsJsonAsync(
            new Uri($"/api/v1/contracts/{contract.Id}/terminate", UriKind.Relative),
            new TerminateContractRequest { Reason = "ครบกำหนดสัญญา" },
            TestContext.Current.CancellationToken);
        terminateResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        ApiResponse<ContractResponse>? terminatedBody = await terminateResponse.Content
            .ReadFromJsonAsync<ApiResponse<ContractResponse>>(TestContext.Current.CancellationToken);
        terminatedBody!.Data!.Status.Should().Be("Terminated");
        terminatedBody.Data.TerminatedAt.Should().NotBeNull();
        terminatedBody.Data.TerminationReason.Should().Be("ครบกำหนดสัญญา");
        (await GetRoomAsync(client, room.Id)).Status.Should().Be("Cleaning");
    }

    [Fact]
    public async Task จองห้องที่ไม่ว่างซ้ำ_ต้องได้_409()
    {
        using HttpClient client = await TestUserFactory.CreateAuthenticatedClientAsync(
            _factory,
            DormFlowRoles.SuperAdmin);

        (OrganizationResponse organization, _, RoomResponse room) = await CreatePropertyHierarchyAsync(client);
        TenantResponse tenantOne = await CreateTenantAsync(client, organization.Id);
        TenantResponse tenantTwo = await CreateTenantAsync(client, organization.Id);

        await CreateReservationAsync(client, room.Id, tenantOne.Id);

        using HttpResponseMessage secondReservationResponse = await client.PostAsJsonAsync(
            new Uri("/api/v1/reservations", UriKind.Relative),
            new CreateReservationRequest
            {
                RoomId = room.Id,
                TenantId = tenantTwo.Id,
                MoveInDate = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(7),
                HoldDays = 3,
                ReservationFee = 500,
            },
            TestContext.Current.CancellationToken);

        secondReservationResponse.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task ทำสัญญาตรงโดยไม่ผ่านการจอง_ต้องสร้างสำเร็จและกันห้องไว้()
    {
        using HttpClient client = await TestUserFactory.CreateAuthenticatedClientAsync(
            _factory,
            DormFlowRoles.SuperAdmin);

        (OrganizationResponse organization, _, RoomResponse room) = await CreatePropertyHierarchyAsync(client);
        TenantResponse tenant = await CreateTenantAsync(client, organization.Id);

        using HttpResponseMessage response = await client.PostAsJsonAsync(
            new Uri("/api/v1/contracts", UriKind.Relative),
            new CreateContractRequest
            {
                RoomId = room.Id,
                TenantId = tenant.Id,
                StartDate = DateOnly.FromDateTime(DateTime.UtcNow),
                EndDate = DateOnly.FromDateTime(DateTime.UtcNow).AddYears(1),
            },
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        ApiResponse<ContractResponse>? body = await response.Content
            .ReadFromJsonAsync<ApiResponse<ContractResponse>>(TestContext.Current.CancellationToken);

        body!.Data!.ReservationId.Should().BeNull();
        body.Data.MonthlyRent.Should().Be(room.MonthlyRent);
        (await GetRoomAsync(client, room.Id)).Status.Should().Be("Reserved");
    }

    [Fact]
    public async Task ผู้เช่าที่ถูกขึ้นบัญชีดำ_ต้องจองห้องไม่ได้()
    {
        using HttpClient client = await TestUserFactory.CreateAuthenticatedClientAsync(
            _factory,
            DormFlowRoles.SuperAdmin);

        (OrganizationResponse organization, _, RoomResponse room) = await CreatePropertyHierarchyAsync(client);
        TenantResponse tenant = await CreateTenantAsync(client, organization.Id);

        using HttpResponseMessage blacklistResponse = await client.PatchAsJsonAsync(
            new Uri($"/api/v1/tenants/{tenant.Id}/status", UriKind.Relative),
            new ChangeStatusRequest<TenantStatus> { Status = TenantStatus.Blacklisted },
            TestContext.Current.CancellationToken);
        blacklistResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        using HttpResponseMessage reservationResponse = await client.PostAsJsonAsync(
            new Uri("/api/v1/reservations", UriKind.Relative),
            new CreateReservationRequest
            {
                RoomId = room.Id,
                TenantId = tenant.Id,
                MoveInDate = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(3),
                HoldDays = 3,
                ReservationFee = 500,
            },
            TestContext.Current.CancellationToken);

        reservationResponse.StatusCode.Should().Be(HttpStatusCode.Conflict);
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

    private static async Task<TenantResponse> CreateTenantAsync(HttpClient client, Guid organizationId)
    {
        using HttpResponseMessage response = await client.PostAsJsonAsync(
            new Uri("/api/v1/tenants", UriKind.Relative),
            new CreateTenantRequest
            {
                OrganizationId = organizationId,
                FirstName = "สมชาย",
                LastName = $"ทดสอบ-{Guid.NewGuid():N}",
                Phone = "0800000000",
            },
            TestContext.Current.CancellationToken);

        response.EnsureSuccessStatusCode();

        return (await response.Content
            .ReadFromJsonAsync<ApiResponse<TenantResponse>>(TestContext.Current.CancellationToken))!.Data!;
    }

    private static async Task<ReservationResponse> CreateReservationAsync(HttpClient client, Guid roomId, Guid tenantId)
    {
        using HttpResponseMessage response = await client.PostAsJsonAsync(
            new Uri("/api/v1/reservations", UriKind.Relative),
            new CreateReservationRequest
            {
                RoomId = roomId,
                TenantId = tenantId,
                MoveInDate = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(7),
                HoldDays = 3,
                ReservationFee = 500,
            },
            TestContext.Current.CancellationToken);

        response.EnsureSuccessStatusCode();

        return (await response.Content
            .ReadFromJsonAsync<ApiResponse<ReservationResponse>>(TestContext.Current.CancellationToken))!.Data!;
    }

    private static async Task<ContractResponse> CreateContractFromReservationAsync(
        HttpClient client,
        Guid roomId,
        Guid tenantId,
        Guid reservationId)
    {
        using HttpResponseMessage response = await client.PostAsJsonAsync(
            new Uri("/api/v1/contracts", UriKind.Relative),
            new CreateContractRequest
            {
                RoomId = roomId,
                TenantId = tenantId,
                ReservationId = reservationId,
                StartDate = DateOnly.FromDateTime(DateTime.UtcNow),
                EndDate = DateOnly.FromDateTime(DateTime.UtcNow).AddYears(1),
            },
            TestContext.Current.CancellationToken);

        response.EnsureSuccessStatusCode();

        return (await response.Content
            .ReadFromJsonAsync<ApiResponse<ContractResponse>>(TestContext.Current.CancellationToken))!.Data!;
    }

    private static async Task<RoomResponse> GetRoomAsync(HttpClient client, Guid roomId)
    {
        using HttpResponseMessage response = await client.GetAsync(
            new Uri($"/api/v1/rooms/{roomId}", UriKind.Relative),
            TestContext.Current.CancellationToken);

        return (await response.Content
            .ReadFromJsonAsync<ApiResponse<RoomResponse>>(TestContext.Current.CancellationToken))!.Data!;
    }
}
