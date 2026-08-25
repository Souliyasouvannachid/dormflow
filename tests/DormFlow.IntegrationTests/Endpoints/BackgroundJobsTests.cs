using System.Net.Http.Json;
using DormFlow.Application.Common.Abstractions;
using DormFlow.Application.Features.Invoices;
using DormFlow.Application.Features.Reservations;
using DormFlow.Contracts.Buildings;
using DormFlow.Contracts.Common;
using DormFlow.Contracts.Dormitories;
using DormFlow.Contracts.Floors;
using DormFlow.Contracts.Invoices;
using DormFlow.Contracts.Leases;
using DormFlow.Contracts.Organizations;
using DormFlow.Contracts.Reservations;
using DormFlow.Contracts.RoomTypes;
using DormFlow.Contracts.Rooms;
using DormFlow.Contracts.Tenants;
using DormFlow.Domain.Entities;
using DormFlow.Domain.Enums;
using DormFlow.Infrastructure.Persistence;
using DormFlow.IntegrationTests.Infrastructure;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace DormFlow.IntegrationTests.Endpoints;

/// <summary>
/// ทดสอบตรรกะของ Background Job (หมดอายุการจอง, ทำเครื่องหมายใบแจ้งหนี้เกินกำหนดชำระ+คำนวณค่าปรับ)
/// โดยตรงผ่าน Application Service ที่ Hangfire เรียกใช้จริง เพราะไม่มี HTTP Endpoint สำหรับสั่งงานนี้
/// (เป็นการทำงานของระบบเอง ไม่ใช่คำสั่งที่ผู้ใช้เรียกได้) Hangfire Server เองถูกปิดไว้ในสภาพแวดล้อม Testing
/// </summary>
[Collection(ApiTestFixtures.Name)]
public class BackgroundJobsTests(DormFlowApiFactory factory)
{
    private readonly DormFlowApiFactory _factory = factory;

    [Fact]
    public async Task การจองที่หมดอายุแล้ว_ต้องเปลี่ยนเป็น_Expired_และปล่อยห้องคืน()
    {
        using HttpClient client = await TestUserFactory.CreateAuthenticatedClientAsync(
            _factory,
            DormFlowRoles.SuperAdmin);

        (OrganizationResponse organization, _, RoomResponse room) = await CreatePropertyHierarchyAsync(client);
        TenantResponse tenant = await CreateTenantAsync(client, organization.Id);
        ReservationResponse reservation = await CreateReservationAsync(client, room.Id, tenant.Id);

        await BackdateReservationExpiryAsync(reservation.Id, TimeSpan.FromHours(1));

        int expiredCount = await ExpirePastDueReservationsAsync();
        expiredCount.Should().BeGreaterThanOrEqualTo(1);

        (await GetReservationAsync(client, reservation.Id)).Status.Should().Be("Expired");
        (await GetRoomAsync(client, room.Id)).Status.Should().Be("Available");
    }

    [Fact]
    public async Task การจองที่ยังไม่หมดอายุ_ต้องไม่ถูกแตะต้อง()
    {
        using HttpClient client = await TestUserFactory.CreateAuthenticatedClientAsync(
            _factory,
            DormFlowRoles.SuperAdmin);

        (OrganizationResponse organization, _, RoomResponse room) = await CreatePropertyHierarchyAsync(client);
        TenantResponse tenant = await CreateTenantAsync(client, organization.Id);
        ReservationResponse reservation = await CreateReservationAsync(client, room.Id, tenant.Id);

        await ExpirePastDueReservationsAsync();

        (await GetReservationAsync(client, reservation.Id)).Status.Should().Be("Pending");
        (await GetRoomAsync(client, room.Id)).Status.Should().Be("Reserved");
    }

    [Fact]
    public async Task การจองที่หมดอายุแต่ห้องถูกเปลี่ยนสถานะไปทางอื่นแล้ว_ต้องไม่แตะต้องห้อง()
    {
        using HttpClient client = await TestUserFactory.CreateAuthenticatedClientAsync(
            _factory,
            DormFlowRoles.SuperAdmin);

        (OrganizationResponse organization, _, RoomResponse room) = await CreatePropertyHierarchyAsync(client);
        TenantResponse tenant = await CreateTenantAsync(client, organization.Id);
        ReservationResponse reservation = await CreateReservationAsync(client, room.Id, tenant.Id);

        await BackdateReservationExpiryAsync(reservation.Id, TimeSpan.FromHours(1));

        using HttpResponseMessage changeStatusResponse = await client.PatchAsJsonAsync(
            new Uri($"/api/v1/rooms/{room.Id}/status", UriKind.Relative),
            new ChangeStatusRequest<RoomStatus> { Status = RoomStatus.Maintenance },
            TestContext.Current.CancellationToken);
        changeStatusResponse.EnsureSuccessStatusCode();

        await ExpirePastDueReservationsAsync();

        (await GetReservationAsync(client, reservation.Id)).Status.Should().Be("Expired");
        (await GetRoomAsync(client, room.Id)).Status.Should().Be("Maintenance");
    }

    [Fact]
    public async Task ใบแจ้งหนี้ที่เลยกำหนดชำระ_แบบค่าปรับคงที่_ต้องเปลี่ยนเป็น_Overdue_พร้อมบวกค่าปรับ()
    {
        using HttpClient client = await TestUserFactory.CreateAuthenticatedClientAsync(
            _factory,
            DormFlowRoles.SuperAdmin);

        (_, ContractResponse contract) = await CreateActiveContractAsync(
            client, lateFeeType: "Fixed", lateFeeAmount: 200);

        InvoiceResponse invoice = await CreateAndIssuePastDueInvoiceAsync(client, contract.Id, new DateOnly(2026, 1, 1));

        int overdueCount = await MarkOverdueInvoicesAsync();
        overdueCount.Should().BeGreaterThanOrEqualTo(1);

        InvoiceResponse afterJob = await GetInvoiceAsync(client, invoice.Id);
        afterJob.Status.Should().Be("Overdue");
        afterJob.LateFeeAmount.Should().Be(200);
        afterJob.TotalAmount.Should().Be(invoice.TotalAmount + 200);
    }

    [Fact]
    public async Task ใบแจ้งหนี้ที่เลยกำหนดชำระ_แบบค่าปรับร้อยละ_ต้องคำนวณจากยอดคงเหลือ()
    {
        using HttpClient client = await TestUserFactory.CreateAuthenticatedClientAsync(
            _factory,
            DormFlowRoles.SuperAdmin);

        (_, ContractResponse contract) = await CreateActiveContractAsync(
            client, lateFeeType: "Percentage", lateFeeAmount: 10);

        InvoiceResponse invoice = await CreateAndIssuePastDueInvoiceAsync(client, contract.Id, new DateOnly(2026, 2, 1));

        await MarkOverdueInvoicesAsync();

        InvoiceResponse afterJob = await GetInvoiceAsync(client, invoice.Id);
        decimal expectedFee = Math.Round(invoice.TotalAmount * 10 / 100m, 2);
        afterJob.Status.Should().Be("Overdue");
        afterJob.LateFeeAmount.Should().Be(expectedFee);
        afterJob.TotalAmount.Should().Be(invoice.TotalAmount + expectedFee);
    }

    [Fact]
    public async Task ใบแจ้งหนี้ที่ยังไม่ถึงกำหนดชำระ_ต้องไม่ถูกแตะต้อง()
    {
        using HttpClient client = await TestUserFactory.CreateAuthenticatedClientAsync(
            _factory,
            DormFlowRoles.SuperAdmin);

        (_, ContractResponse contract) = await CreateActiveContractAsync(
            client, lateFeeType: "Fixed", lateFeeAmount: 200);

        using HttpResponseMessage createResponse = await client.PostAsJsonAsync(
            new Uri("/api/v1/invoices", UriKind.Relative),
            new CreateInvoiceRequest { ContractId = contract.Id, PeriodMonth = new DateOnly(2026, 3, 1) },
            TestContext.Current.CancellationToken);
        createResponse.EnsureSuccessStatusCode();
        InvoiceResponse draftInvoice = (await createResponse.Content
            .ReadFromJsonAsync<ApiResponse<InvoiceResponse>>(TestContext.Current.CancellationToken))!.Data!;

        using HttpResponseMessage issueResponse = await client.PostAsync(
            new Uri($"/api/v1/invoices/{draftInvoice.Id}/issue", UriKind.Relative),
            content: null,
            TestContext.Current.CancellationToken);
        issueResponse.EnsureSuccessStatusCode();

        await MarkOverdueInvoicesAsync();

        (await GetInvoiceAsync(client, draftInvoice.Id)).Status.Should().Be("Issued");
    }

    [Fact]
    public async Task รัน_MarkOverdueInvoicesAsync_ซ้ำกับใบแจ้งหนี้ที่เกินกำหนดแล้ว_ต้องไม่บวกค่าปรับซ้ำ()
    {
        using HttpClient client = await TestUserFactory.CreateAuthenticatedClientAsync(
            _factory,
            DormFlowRoles.SuperAdmin);

        (_, ContractResponse contract) = await CreateActiveContractAsync(
            client, lateFeeType: "Fixed", lateFeeAmount: 200);

        InvoiceResponse invoice = await CreateAndIssuePastDueInvoiceAsync(client, contract.Id, new DateOnly(2026, 4, 1));

        await MarkOverdueInvoicesAsync();
        InvoiceResponse afterFirstRun = await GetInvoiceAsync(client, invoice.Id);

        int secondRunCount = await MarkOverdueInvoicesAsync();
        InvoiceResponse afterSecondRun = await GetInvoiceAsync(client, invoice.Id);

        secondRunCount.Should().Be(0);
        afterSecondRun.LateFeeAmount.Should().Be(afterFirstRun.LateFeeAmount);
        afterSecondRun.TotalAmount.Should().Be(afterFirstRun.TotalAmount);
    }

    private async Task<int> ExpirePastDueReservationsAsync()
    {
        using IServiceScope scope = _factory.Services.CreateScope();
        IReservationService reservationService = scope.ServiceProvider.GetRequiredService<IReservationService>();
        return await reservationService.ExpirePastDueReservationsAsync(TestContext.Current.CancellationToken);
    }

    private async Task<int> MarkOverdueInvoicesAsync()
    {
        using IServiceScope scope = _factory.Services.CreateScope();
        IInvoiceService invoiceService = scope.ServiceProvider.GetRequiredService<IInvoiceService>();
        return await invoiceService.MarkOverdueInvoicesAsync(TestContext.Current.CancellationToken);
    }

    /// <summary>ตั้ง ExpiresAt ของการจองให้ผ่านไปแล้ว โดยเข้าถึง DbContext ตรง ๆ เพราะ API ไม่รองรับการระบุค่านี้เอง</summary>
    private async Task BackdateReservationExpiryAsync(Guid reservationId, TimeSpan pastDue)
    {
        using IServiceScope scope = _factory.Services.CreateScope();
        DormFlowDbContext dbContext = scope.ServiceProvider.GetRequiredService<DormFlowDbContext>();

        Reservation reservation = await dbContext.Reservations
            .FirstAsync(r => r.Id == reservationId, TestContext.Current.CancellationToken);
        reservation.ExpiresAt = DateTimeOffset.UtcNow.Subtract(pastDue);

        await dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);
    }

    /// <summary>สร้างและออกใบแจ้งหนี้ที่มีวันครบกำหนดชำระผ่านไปแล้ว ด้วยการระบุ IssueDate ย้อนหลังผ่าน API ปกติ</summary>
    private static async Task<InvoiceResponse> CreateAndIssuePastDueInvoiceAsync(
        HttpClient client, Guid contractId, DateOnly periodMonth)
    {
        DateOnly pastIssueDate = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(-60);

        using HttpResponseMessage createResponse = await client.PostAsJsonAsync(
            new Uri("/api/v1/invoices", UriKind.Relative),
            new CreateInvoiceRequest { ContractId = contractId, PeriodMonth = periodMonth, IssueDate = pastIssueDate },
            TestContext.Current.CancellationToken);
        createResponse.EnsureSuccessStatusCode();

        InvoiceResponse invoice = (await createResponse.Content
            .ReadFromJsonAsync<ApiResponse<InvoiceResponse>>(TestContext.Current.CancellationToken))!.Data!;

        using HttpResponseMessage issueResponse = await client.PostAsync(
            new Uri($"/api/v1/invoices/{invoice.Id}/issue", UriKind.Relative),
            content: null,
            TestContext.Current.CancellationToken);
        issueResponse.EnsureSuccessStatusCode();

        return (await issueResponse.Content
            .ReadFromJsonAsync<ApiResponse<InvoiceResponse>>(TestContext.Current.CancellationToken))!.Data!;
    }

    private static async Task<InvoiceResponse> GetInvoiceAsync(HttpClient client, Guid invoiceId)
    {
        using HttpResponseMessage response = await client.GetAsync(
            new Uri($"/api/v1/invoices/{invoiceId}", UriKind.Relative),
            TestContext.Current.CancellationToken);

        return (await response.Content
            .ReadFromJsonAsync<ApiResponse<InvoiceResponse>>(TestContext.Current.CancellationToken))!.Data!;
    }

    private static async Task<ReservationResponse> GetReservationAsync(HttpClient client, Guid reservationId)
    {
        using HttpResponseMessage response = await client.GetAsync(
            new Uri($"/api/v1/reservations/{reservationId}", UriKind.Relative),
            TestContext.Current.CancellationToken);

        return (await response.Content
            .ReadFromJsonAsync<ApiResponse<ReservationResponse>>(TestContext.Current.CancellationToken))!.Data!;
    }

    private static async Task<RoomResponse> GetRoomAsync(HttpClient client, Guid roomId)
    {
        using HttpResponseMessage response = await client.GetAsync(
            new Uri($"/api/v1/rooms/{roomId}", UriKind.Relative),
            TestContext.Current.CancellationToken);

        return (await response.Content
            .ReadFromJsonAsync<ApiResponse<RoomResponse>>(TestContext.Current.CancellationToken))!.Data!;
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

    private static async Task<(RoomResponse Room, ContractResponse Contract)> CreateActiveContractAsync(
        HttpClient client, string lateFeeType, decimal lateFeeAmount)
    {
        (OrganizationResponse organization, DormitoryResponse _, RoomResponse room) =
            await CreatePropertyHierarchyWithLateFeeAsync(client, lateFeeType, lateFeeAmount);

        TenantResponse tenant = await CreateTenantAsync(client, organization.Id);

        using HttpResponseMessage contractResponse = await client.PostAsJsonAsync(
            new Uri("/api/v1/contracts", UriKind.Relative),
            new CreateContractRequest
            {
                RoomId = room.Id,
                TenantId = tenant.Id,
                StartDate = DateOnly.FromDateTime(DateTime.UtcNow).AddYears(-1),
                EndDate = DateOnly.FromDateTime(DateTime.UtcNow).AddYears(1),
            },
            TestContext.Current.CancellationToken);
        ContractResponse contract = (await contractResponse.Content
            .ReadFromJsonAsync<ApiResponse<ContractResponse>>(TestContext.Current.CancellationToken))!.Data!;

        using HttpResponseMessage activateResponse = await client.PostAsync(
            new Uri($"/api/v1/contracts/{contract.Id}/activate", UriKind.Relative),
            content: null,
            TestContext.Current.CancellationToken);
        activateResponse.EnsureSuccessStatusCode();

        ContractResponse activatedContract = (await activateResponse.Content
            .ReadFromJsonAsync<ApiResponse<ContractResponse>>(TestContext.Current.CancellationToken))!.Data!;

        return (room, activatedContract);
    }

    private static async Task<(OrganizationResponse Organization, DormitoryResponse Dormitory, RoomResponse Room)>
        CreatePropertyHierarchyWithLateFeeAsync(HttpClient client, string lateFeeType, decimal lateFeeAmount)
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
                LateFeeType = lateFeeType,
                LateFeeAmount = lateFeeAmount,
                WaterRatePerUnit = 0,
                ElectricityRatePerUnit = 0,
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
