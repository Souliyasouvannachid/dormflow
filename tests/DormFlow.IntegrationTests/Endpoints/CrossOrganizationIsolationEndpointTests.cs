using System.Net.Http.Json;
using DormFlow.Application.Common.Abstractions;
using DormFlow.Contracts.Buildings;
using DormFlow.Contracts.Common;
using DormFlow.Contracts.Dormitories;
using DormFlow.Contracts.Floors;
using DormFlow.Contracts.Invoices;
using DormFlow.Contracts.Leases;
using DormFlow.Contracts.Maintenance;
using DormFlow.Contracts.Organizations;
using DormFlow.Contracts.RoomTypes;
using DormFlow.Contracts.Rooms;
using DormFlow.Contracts.Tenants;
using DormFlow.IntegrationTests.Infrastructure;
using FluentAssertions;
using Xunit;

namespace DormFlow.IntegrationTests.Endpoints;

/// <summary>
/// ทดสอบว่าผู้ใช้บทบาท Owner ที่เรียก List Endpoint โดยไม่ระบุตัวกรองหอพัก ต้องเห็นเฉพาะข้อมูล
/// ขององค์กรตนเองเท่านั้น ไม่เห็นข้อมูลข้ามองค์กร
/// </summary>
/// <remarks>
/// เป็น Regression Test ของช่องโหว่ที่พบระหว่างพัฒนา: <see cref="TenantScope.GetDormitoryRestrictionForListing"/>
/// คืนค่า null ให้ Owner (หมายถึง "เห็นได้ทุกหอพักในองค์กรของตน") แต่ก่อนแก้ไข Repository ของโมดูลที่แยกตามหอพัก
/// (Buildings, Floors, RoomTypes, Rooms, Reservations, Contracts, MeterReadings, Invoices, Payments,
/// MaintenanceRequests) ไม่มีการกรองด้วย OrganizationId เพิ่มเติมเลย ทำให้ Owner เห็นข้อมูลของทุกองค์กรในระบบ
/// แก้ไขด้วย <see cref="TenantScope.GetOrganizationRestrictionForListing"/> ที่ต้องเรียกคู่กันเสมอ
/// </remarks>
[Collection(ApiTestFixtures.Name)]
public class CrossOrganizationIsolationEndpointTests(DormFlowApiFactory factory)
{
    private readonly DormFlowApiFactory _factory = factory;

    [Fact]
    public async Task Owner_เรียกรายการห้องพักโดยไม่ระบุตัวกรอง_ต้องไม่เห็นห้องขององค์กรอื่น()
    {
        using HttpClient adminClient = await TestUserFactory.CreateAuthenticatedClientAsync(
            _factory,
            DormFlowRoles.SuperAdmin);

        (OrganizationResponse orgA, _, RoomResponse roomA) = await CreatePropertyHierarchyAsync(adminClient);
        (_, _, RoomResponse roomB) = await CreatePropertyHierarchyAsync(adminClient);

        using HttpClient ownerAClient = await TestUserFactory.CreateAuthenticatedClientAsync(
            _factory,
            DormFlowRoles.Owner,
            orgA.Id);

        using HttpResponseMessage response = await ownerAClient.GetAsync(
            new Uri("/api/v1/rooms?pageSize=200", UriKind.Relative),
            TestContext.Current.CancellationToken);
        response.EnsureSuccessStatusCode();

        PagedResult<RoomResponse> rooms = (await response.Content
            .ReadFromJsonAsync<ApiResponse<PagedResult<RoomResponse>>>(TestContext.Current.CancellationToken))!.Data!;

        rooms.Items.Should().Contain(room => room.Id == roomA.Id);
        rooms.Items.Should().NotContain(room => room.Id == roomB.Id);
        rooms.Items.Should().OnlyContain(room => room.OrganizationId == orgA.Id);
    }

    [Fact]
    public async Task Owner_เรียกรายการงานแจ้งซ่อมโดยไม่ระบุตัวกรอง_ต้องไม่เห็นงานขององค์กรอื่น()
    {
        using HttpClient adminClient = await TestUserFactory.CreateAuthenticatedClientAsync(
            _factory,
            DormFlowRoles.SuperAdmin);

        (OrganizationResponse orgA, _, RoomResponse roomA) = await CreatePropertyHierarchyAsync(adminClient);
        (_, _, RoomResponse roomB) = await CreatePropertyHierarchyAsync(adminClient);

        Guid requestAId = await ReportIssueAsync(adminClient, roomA.Id);
        Guid requestBId = await ReportIssueAsync(adminClient, roomB.Id);

        using HttpClient ownerAClient = await TestUserFactory.CreateAuthenticatedClientAsync(
            _factory,
            DormFlowRoles.Owner,
            orgA.Id);

        using HttpResponseMessage response = await ownerAClient.GetAsync(
            new Uri("/api/v1/maintenance-requests?pageSize=200", UriKind.Relative),
            TestContext.Current.CancellationToken);
        response.EnsureSuccessStatusCode();

        PagedResult<MaintenanceRequestResponse> requests = (await response.Content
            .ReadFromJsonAsync<ApiResponse<PagedResult<MaintenanceRequestResponse>>>(TestContext.Current.CancellationToken))!.Data!;

        requests.Items.Should().Contain(item => item.Id == requestAId);
        requests.Items.Should().NotContain(item => item.Id == requestBId);
        requests.Items.Should().OnlyContain(item => item.OrganizationId == orgA.Id);
    }

    [Fact]
    public async Task Owner_เรียกรายการใบแจ้งหนี้โดยไม่ระบุตัวกรอง_ต้องไม่เห็นใบแจ้งหนี้ขององค์กรอื่น()
    {
        using HttpClient adminClient = await TestUserFactory.CreateAuthenticatedClientAsync(
            _factory,
            DormFlowRoles.SuperAdmin);

        (OrganizationResponse orgA, InvoiceResponse invoiceA) = await CreateInvoiceAsync(adminClient);
        (_, InvoiceResponse invoiceB) = await CreateInvoiceAsync(adminClient);

        using HttpClient ownerAClient = await TestUserFactory.CreateAuthenticatedClientAsync(
            _factory,
            DormFlowRoles.Owner,
            orgA.Id);

        using HttpResponseMessage response = await ownerAClient.GetAsync(
            new Uri("/api/v1/invoices?pageSize=200", UriKind.Relative),
            TestContext.Current.CancellationToken);
        response.EnsureSuccessStatusCode();

        PagedResult<InvoiceResponse> invoices = (await response.Content
            .ReadFromJsonAsync<ApiResponse<PagedResult<InvoiceResponse>>>(TestContext.Current.CancellationToken))!.Data!;

        invoices.Items.Should().Contain(invoice => invoice.Id == invoiceA.Id);
        invoices.Items.Should().NotContain(invoice => invoice.Id == invoiceB.Id);
        invoices.Items.Should().OnlyContain(invoice => invoice.OrganizationId == orgA.Id);
    }

    private static async Task<Guid> ReportIssueAsync(HttpClient client, Guid roomId)
    {
        using HttpResponseMessage response = await client.PostAsJsonAsync(
            new Uri("/api/v1/maintenance-requests", UriKind.Relative),
            new CreateMaintenanceRequestRequest { RoomId = roomId, Title = "ทดสอบ", Priority = "Low" },
            TestContext.Current.CancellationToken);
        response.EnsureSuccessStatusCode();

        return (await response.Content
            .ReadFromJsonAsync<ApiResponse<MaintenanceRequestResponse>>(TestContext.Current.CancellationToken))!.Data!.Id;
    }

    private static async Task<(OrganizationResponse Organization, InvoiceResponse Invoice)> CreateInvoiceAsync(HttpClient client)
    {
        (OrganizationResponse organization, _, RoomResponse room) = await CreatePropertyHierarchyAsync(client);

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

        using HttpResponseMessage invoiceResponse = await client.PostAsJsonAsync(
            new Uri("/api/v1/invoices", UriKind.Relative),
            new CreateInvoiceRequest { ContractId = contract.Id, PeriodMonth = new DateOnly(2026, 1, 1) },
            TestContext.Current.CancellationToken);
        invoiceResponse.EnsureSuccessStatusCode();

        InvoiceResponse invoice = (await invoiceResponse.Content
            .ReadFromJsonAsync<ApiResponse<InvoiceResponse>>(TestContext.Current.CancellationToken))!.Data!;

        return (organization, invoice);
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
