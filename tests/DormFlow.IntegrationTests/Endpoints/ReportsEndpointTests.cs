using System.Net;
using System.Net.Http.Json;
using DormFlow.Application.Common.Abstractions;
using DormFlow.Contracts.Buildings;
using DormFlow.Contracts.Common;
using DormFlow.Contracts.Dormitories;
using DormFlow.Contracts.Floors;
using DormFlow.Contracts.Invoices;
using DormFlow.Contracts.Leases;
using DormFlow.Contracts.Organizations;
using DormFlow.Contracts.Payments;
using DormFlow.Contracts.Reports;
using DormFlow.Contracts.RoomTypes;
using DormFlow.Contracts.Rooms;
using DormFlow.Contracts.Tenants;
using DormFlow.IntegrationTests.Infrastructure;
using FluentAssertions;
using Xunit;

namespace DormFlow.IntegrationTests.Endpoints;

/// <summary>ทดสอบรายงานสรุปรายได้และรายงานใบแจ้งหนี้ค้างชำระผ่าน HTTP จริง</summary>
[Collection(ApiTestFixtures.Name)]
public class ReportsEndpointTests(DormFlowApiFactory factory)
{
    private readonly DormFlowApiFactory _factory = factory;

    [Fact]
    public async Task รายงานสรุปรายได้_ต้องรวมยอดออกบิลและยอดเก็บเงินได้ถูกต้อง()
    {
        using HttpClient client = await TestUserFactory.CreateAuthenticatedClientAsync(
            _factory,
            DormFlowRoles.SuperAdmin);

        (_, ContractResponse contract) = await CreateActiveContractAsync(client);
        DateOnly period = new(2026, 3, 1);
        InvoiceResponse invoice = await CreateAndIssueInvoiceAsync(client, contract.Id, period);

        using HttpResponseMessage paymentResponse = await client.PostAsJsonAsync(
            new Uri("/api/v1/payments", UriKind.Relative),
            new CreatePaymentRequest { InvoiceId = invoice.Id, Amount = invoice.TotalAmount, Method = "Cash" },
            TestContext.Current.CancellationToken);
        PaymentResponse payment = (await paymentResponse.Content
            .ReadFromJsonAsync<ApiResponse<PaymentResponse>>(TestContext.Current.CancellationToken))!.Data!;

        await client.PostAsync(
            new Uri($"/api/v1/payments/{payment.Id}/confirm", UriKind.Relative),
            content: null,
            TestContext.Current.CancellationToken);

        // กรองด้วย dormitoryId ของสัญญานี้โดยเฉพาะ เพราะเรียกในฐานะ SuperAdmin ซึ่งไม่ถูกจำกัดด้วยองค์กร
        // ถ้าไม่กรอง รายงานจะรวมข้อมูลของ Test อื่นที่สร้างใบแจ้งหนี้ในเดือนเดียวกันไว้ในฐานข้อมูลที่ใช้ร่วมกันด้วย
        using HttpResponseMessage response = await client.GetAsync(
            new Uri(
                $"/api/v1/reports/revenue?periodFrom=2026-03-01&periodTo=2026-03-31&dormitoryId={contract.DormitoryId}",
                UriKind.Relative),
            TestContext.Current.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        RevenueReportResponse report = (await response.Content
            .ReadFromJsonAsync<ApiResponse<RevenueReportResponse>>(TestContext.Current.CancellationToken))!.Data!;

        report.InvoiceCount.Should().Be(1);
        report.PaidInvoiceCount.Should().Be(1);
        report.TotalInvoiced.Should().Be(invoice.TotalAmount);
        report.TotalCollected.Should().Be(invoice.TotalAmount);
        report.TotalOutstanding.Should().Be(0);
    }

    [Fact]
    public async Task รายงานใบแจ้งหนี้ค้างชำระ_ต้องแสดงยอดคงเหลือและจำนวนวันเกินกำหนดถูกต้อง()
    {
        using HttpClient client = await TestUserFactory.CreateAuthenticatedClientAsync(
            _factory,
            DormFlowRoles.SuperAdmin);

        (_, ContractResponse contract) = await CreateActiveContractAsync(client);

        // ตั้ง IssueDate ย้อนหลัง 60 วัน เพื่อให้ DueDate (คำนวณจาก IssueDate + PaymentDueDay) เลยกำหนดไปแล้วแน่นอน
        DateOnly issueDate = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(-60);
        using HttpResponseMessage createResponse = await client.PostAsJsonAsync(
            new Uri("/api/v1/invoices", UriKind.Relative),
            new CreateInvoiceRequest { ContractId = contract.Id, PeriodMonth = new DateOnly(2026, 1, 1), IssueDate = issueDate },
            TestContext.Current.CancellationToken);
        createResponse.EnsureSuccessStatusCode();
        InvoiceResponse createdInvoice = (await createResponse.Content
            .ReadFromJsonAsync<ApiResponse<InvoiceResponse>>(TestContext.Current.CancellationToken))!.Data!;

        using HttpResponseMessage issueResponse = await client.PostAsync(
            new Uri($"/api/v1/invoices/{createdInvoice.Id}/issue", UriKind.Relative),
            content: null,
            TestContext.Current.CancellationToken);
        issueResponse.EnsureSuccessStatusCode();
        InvoiceResponse invoice = (await issueResponse.Content
            .ReadFromJsonAsync<ApiResponse<InvoiceResponse>>(TestContext.Current.CancellationToken))!.Data!;

        using HttpResponseMessage response = await client.GetAsync(
            new Uri("/api/v1/reports/outstanding-invoices?pageSize=200", UriKind.Relative),
            TestContext.Current.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        PagedResult<OutstandingInvoiceItemResponse> report = (await response.Content
            .ReadFromJsonAsync<ApiResponse<PagedResult<OutstandingInvoiceItemResponse>>>(TestContext.Current.CancellationToken))!.Data!;

        OutstandingInvoiceItemResponse item = report.Items.Should().ContainSingle(row => row.InvoiceId == invoice.Id).Subject;
        item.RemainingBalance.Should().Be(invoice.TotalAmount);
        item.DaysOverdue.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task ส่งออกรายงานสรุปรายได้เป็น_CSV_ต้องได้ไฟล์ที่มีข้อมูลถูกต้อง()
    {
        using HttpClient client = await TestUserFactory.CreateAuthenticatedClientAsync(
            _factory,
            DormFlowRoles.SuperAdmin);

        (_, ContractResponse contract) = await CreateActiveContractAsync(client);
        await CreateAndIssueInvoiceAsync(client, contract.Id, new DateOnly(2026, 4, 1));

        // กรองด้วย dormitoryId ของสัญญานี้โดยเฉพาะ ด้วยเหตุผลเดียวกับ Test สรุปรายได้ด้านบน
        using HttpResponseMessage response = await client.GetAsync(
            new Uri(
                $"/api/v1/reports/revenue/export?periodFrom=2026-04-01&periodTo=2026-04-30&dormitoryId={contract.DormitoryId}",
                UriKind.Relative),
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Content.Headers.ContentType?.MediaType.Should().Be("text/csv");

        string csv = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        csv.Should().StartWith("PeriodFrom,PeriodTo,InvoiceCount");
        csv.Should().Contain("2026-04-01,2026-04-30,1");
    }

    [Fact]
    public async Task ส่งออกรายงานใบแจ้งหนี้ค้างชำระเป็น_CSV_ต้องได้ไฟล์ที่มีข้อมูลถูกต้อง()
    {
        using HttpClient client = await TestUserFactory.CreateAuthenticatedClientAsync(
            _factory,
            DormFlowRoles.SuperAdmin);

        (_, ContractResponse contract) = await CreateActiveContractAsync(client);
        InvoiceResponse invoice = await CreateAndIssueInvoiceAsync(client, contract.Id, new DateOnly(2026, 2, 1));

        using HttpResponseMessage response = await client.GetAsync(
            new Uri("/api/v1/reports/outstanding-invoices/export", UriKind.Relative),
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Content.Headers.ContentType?.MediaType.Should().Be("text/csv");

        string csv = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        csv.Should().StartWith("InvoiceNumber,ContractId,DueDate");
        csv.Should().Contain(invoice.InvoiceNumber);
    }

    private static async Task<InvoiceResponse> CreateAndIssueInvoiceAsync(HttpClient client, Guid contractId, DateOnly period)
    {
        using HttpResponseMessage createResponse = await client.PostAsJsonAsync(
            new Uri("/api/v1/invoices", UriKind.Relative),
            new CreateInvoiceRequest { ContractId = contractId, PeriodMonth = period },
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

    private static async Task<(RoomResponse Room, ContractResponse Contract)> CreateActiveContractAsync(HttpClient client)
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
}
