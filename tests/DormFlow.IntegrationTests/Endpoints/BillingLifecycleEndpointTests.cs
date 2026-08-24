using System.Net;
using System.Net.Http.Json;
using DormFlow.Application.Common.Abstractions;
using DormFlow.Contracts.Buildings;
using DormFlow.Contracts.Common;
using DormFlow.Contracts.Dormitories;
using DormFlow.Contracts.Floors;
using DormFlow.Contracts.Invoices;
using DormFlow.Contracts.Leases;
using DormFlow.Contracts.MeterReadings;
using DormFlow.Contracts.Organizations;
using DormFlow.Contracts.Payments;
using DormFlow.Contracts.RoomTypes;
using DormFlow.Contracts.Rooms;
using DormFlow.Contracts.Tenants;
using DormFlow.IntegrationTests.Infrastructure;
using FluentAssertions;
using Xunit;

namespace DormFlow.IntegrationTests.Endpoints;

/// <summary>
/// ทดสอบวงจรชีวิตการออกบิลและรับชำระเงินผ่าน HTTP จริง
/// MeterReading -> Invoice (สร้าง/ออกให้ผู้เช่า) -> Payment (บันทึก/ยืนยัน/คืนเงิน)
/// </summary>
[Collection(ApiTestFixtures.Name)]
public class BillingLifecycleEndpointTests(DormFlowApiFactory factory)
{
    private readonly DormFlowApiFactory _factory = factory;

    [Fact]
    public async Task ออกบิลจากค่ามิเตอร์_ชำระครบ_ต้องเปลี่ยนสถานะเป็นชำระแล้ว()
    {
        using HttpClient client = await TestUserFactory.CreateAuthenticatedClientAsync(
            _factory,
            DormFlowRoles.SuperAdmin);

        (RoomResponse room, ContractResponse contract) = await CreateActiveContractAsync(client, waterRate: 10, electricityRate: 8);

        DateOnly period = new(2026, 4, 1);
        await CreateMeterReadingAsync(client, room.Id, "Water", period, 50);
        await CreateMeterReadingAsync(client, room.Id, "Electricity", period, 100);

        using HttpResponseMessage createResponse = await client.PostAsJsonAsync(
            new Uri("/api/v1/invoices", UriKind.Relative),
            new CreateInvoiceRequest { ContractId = contract.Id, PeriodMonth = period },
            TestContext.Current.CancellationToken);
        createResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        InvoiceResponse invoice = (await createResponse.Content
            .ReadFromJsonAsync<ApiResponse<InvoiceResponse>>(TestContext.Current.CancellationToken))!.Data!;

        invoice.Status.Should().Be("Draft");
        invoice.RentAmount.Should().Be(contract.MonthlyRent);
        invoice.WaterAmount.Should().Be(500);
        invoice.ElectricityAmount.Should().Be(800);
        invoice.TotalAmount.Should().Be(contract.MonthlyRent + 500 + 800);

        using HttpResponseMessage issueResponse = await client.PostAsync(
            new Uri($"/api/v1/invoices/{invoice.Id}/issue", UriKind.Relative),
            content: null,
            TestContext.Current.CancellationToken);
        issueResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        InvoiceResponse issuedInvoice = (await issueResponse.Content
            .ReadFromJsonAsync<ApiResponse<InvoiceResponse>>(TestContext.Current.CancellationToken))!.Data!;
        issuedInvoice.Status.Should().Be("Issued");

        using HttpResponseMessage paymentResponse = await client.PostAsJsonAsync(
            new Uri("/api/v1/payments", UriKind.Relative),
            new CreatePaymentRequest
            {
                InvoiceId = invoice.Id,
                Amount = issuedInvoice.TotalAmount,
                Method = "BankTransfer",
                ReferenceNumber = "REF-0001",
            },
            TestContext.Current.CancellationToken);
        paymentResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        PaymentResponse payment = (await paymentResponse.Content
            .ReadFromJsonAsync<ApiResponse<PaymentResponse>>(TestContext.Current.CancellationToken))!.Data!;
        payment.Status.Should().Be("Pending");

        // ก่อนยืนยัน ยอดใบแจ้งหนี้ต้องยังไม่เปลี่ยน
        (await GetInvoiceAsync(client, invoice.Id)).Status.Should().Be("Issued");

        using HttpResponseMessage confirmResponse = await client.PostAsync(
            new Uri($"/api/v1/payments/{payment.Id}/confirm", UriKind.Relative),
            content: null,
            TestContext.Current.CancellationToken);
        confirmResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        PaymentResponse confirmedPayment = (await confirmResponse.Content
            .ReadFromJsonAsync<ApiResponse<PaymentResponse>>(TestContext.Current.CancellationToken))!.Data!;
        confirmedPayment.Status.Should().Be("Confirmed");
        confirmedPayment.ConfirmedAt.Should().NotBeNull();

        InvoiceResponse paidInvoice = await GetInvoiceAsync(client, invoice.Id);
        paidInvoice.Status.Should().Be("Paid");
        paidInvoice.PaidAmount.Should().Be(issuedInvoice.TotalAmount);
    }

    [Fact]
    public async Task ชำระบางส่วนแล้วคืนเงิน_ยอดใบแจ้งหนี้ต้องกลับเป็นเดิม()
    {
        using HttpClient client = await TestUserFactory.CreateAuthenticatedClientAsync(
            _factory,
            DormFlowRoles.SuperAdmin);

        (RoomResponse room, ContractResponse contract) = await CreateActiveContractAsync(client, waterRate: 0, electricityRate: 0);

        DateOnly period = new(2026, 5, 1);
        InvoiceResponse invoice = await CreateAndIssueInvoiceAsync(client, contract.Id, period);

        decimal halfAmount = invoice.TotalAmount / 2;

        using HttpResponseMessage paymentResponse = await client.PostAsJsonAsync(
            new Uri("/api/v1/payments", UriKind.Relative),
            new CreatePaymentRequest { InvoiceId = invoice.Id, Amount = halfAmount, Method = "Cash" },
            TestContext.Current.CancellationToken);
        PaymentResponse payment = (await paymentResponse.Content
            .ReadFromJsonAsync<ApiResponse<PaymentResponse>>(TestContext.Current.CancellationToken))!.Data!;

        await client.PostAsync(
            new Uri($"/api/v1/payments/{payment.Id}/confirm", UriKind.Relative),
            content: null,
            TestContext.Current.CancellationToken);

        (await GetInvoiceAsync(client, invoice.Id)).Status.Should().Be("PartiallyPaid");

        using HttpResponseMessage refundResponse = await client.PostAsJsonAsync(
            new Uri($"/api/v1/payments/{payment.Id}/refund", UriKind.Relative),
            new RefundPaymentRequest { Reason = "ชำระผิดยอด" },
            TestContext.Current.CancellationToken);
        refundResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        PaymentResponse refundedPayment = (await refundResponse.Content
            .ReadFromJsonAsync<ApiResponse<PaymentResponse>>(TestContext.Current.CancellationToken))!.Data!;
        refundedPayment.Status.Should().Be("Refunded");
        refundedPayment.RefundReason.Should().Be("ชำระผิดยอด");

        InvoiceResponse invoiceAfterRefund = await GetInvoiceAsync(client, invoice.Id);
        invoiceAfterRefund.Status.Should().Be("Issued");
        invoiceAfterRefund.PaidAmount.Should().Be(0);

        _ = room;
    }

    [Fact]
    public async Task ออกบิลซ้ำเดือนเดียวกันของสัญญาเดิม_ต้องได้_409()
    {
        using HttpClient client = await TestUserFactory.CreateAuthenticatedClientAsync(
            _factory,
            DormFlowRoles.SuperAdmin);

        (_, ContractResponse contract) = await CreateActiveContractAsync(client, waterRate: 0, electricityRate: 0);
        DateOnly period = new(2026, 6, 1);

        await CreateAndIssueInvoiceAsync(client, contract.Id, period);

        using HttpResponseMessage duplicateResponse = await client.PostAsJsonAsync(
            new Uri("/api/v1/invoices", UriKind.Relative),
            new CreateInvoiceRequest { ContractId = contract.Id, PeriodMonth = period },
            TestContext.Current.CancellationToken);

        duplicateResponse.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task ยกเลิกใบแจ้งหนี้ที่มีการชำระเงินแล้ว_ต้องได้_409()
    {
        using HttpClient client = await TestUserFactory.CreateAuthenticatedClientAsync(
            _factory,
            DormFlowRoles.SuperAdmin);

        (_, ContractResponse contract) = await CreateActiveContractAsync(client, waterRate: 0, electricityRate: 0);
        InvoiceResponse invoice = await CreateAndIssueInvoiceAsync(client, contract.Id, new DateOnly(2026, 7, 1));

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

        using HttpResponseMessage cancelResponse = await client.PostAsync(
            new Uri($"/api/v1/invoices/{invoice.Id}/cancel", UriKind.Relative),
            content: null,
            TestContext.Current.CancellationToken);

        cancelResponse.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task ยืนยันยอดชำระเกินยอดคงเหลือ_ต้องได้_409()
    {
        using HttpClient client = await TestUserFactory.CreateAuthenticatedClientAsync(
            _factory,
            DormFlowRoles.SuperAdmin);

        (_, ContractResponse contract) = await CreateActiveContractAsync(client, waterRate: 0, electricityRate: 0);
        InvoiceResponse invoice = await CreateAndIssueInvoiceAsync(client, contract.Id, new DateOnly(2026, 8, 1));

        using HttpResponseMessage paymentResponse = await client.PostAsJsonAsync(
            new Uri("/api/v1/payments", UriKind.Relative),
            new CreatePaymentRequest { InvoiceId = invoice.Id, Amount = invoice.TotalAmount + 1000, Method = "Cash" },
            TestContext.Current.CancellationToken);
        PaymentResponse payment = (await paymentResponse.Content
            .ReadFromJsonAsync<ApiResponse<PaymentResponse>>(TestContext.Current.CancellationToken))!.Data!;

        using HttpResponseMessage confirmResponse = await client.PostAsync(
            new Uri($"/api/v1/payments/{payment.Id}/confirm", UriKind.Relative),
            content: null,
            TestContext.Current.CancellationToken);

        confirmResponse.StatusCode.Should().Be(HttpStatusCode.Conflict);
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

    private static async Task<InvoiceResponse> GetInvoiceAsync(HttpClient client, Guid invoiceId)
    {
        using HttpResponseMessage response = await client.GetAsync(
            new Uri($"/api/v1/invoices/{invoiceId}", UriKind.Relative),
            TestContext.Current.CancellationToken);

        return (await response.Content
            .ReadFromJsonAsync<ApiResponse<InvoiceResponse>>(TestContext.Current.CancellationToken))!.Data!;
    }

    private static async Task CreateMeterReadingAsync(
        HttpClient client, Guid roomId, string meterType, DateOnly periodMonth, decimal currentReading)
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
    }

    private static async Task<(RoomResponse Room, ContractResponse Contract)> CreateActiveContractAsync(
        HttpClient client, decimal waterRate, decimal electricityRate)
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
                WaterRatePerUnit = waterRate,
                ElectricityRatePerUnit = electricityRate,
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
