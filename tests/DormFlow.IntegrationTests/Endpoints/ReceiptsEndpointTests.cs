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
using DormFlow.Contracts.Receipts;
using DormFlow.Contracts.RoomTypes;
using DormFlow.Contracts.Rooms;
using DormFlow.Contracts.Tenants;
using DormFlow.IntegrationTests.Infrastructure;
using FluentAssertions;
using Xunit;

namespace DormFlow.IntegrationTests.Endpoints;

/// <summary>ຜ່ານການທົດສອບສໍາລັບການອອກໃບຮັບເງິນ HTTP ຄວາມຈິງ</summary>
[Collection(ApiTestFixtures.Name)]
public class ReceiptsEndpointTests(DormFlowApiFactory factory)
{
    private readonly DormFlowApiFactory _factory = factory;

    [Fact]
    public async Task ອອກໃບຮັບເງິນສໍາລັບການຈ່າຍເງິນທີ່ຢືນຢັນ_ຕ້ອງປະສົບຜົນສໍາເລັດ()
    {
        using HttpClient client = await TestUserFactory.CreateAuthenticatedClientAsync(
            _factory,
            DormFlowRoles.SuperAdmin);

        PaymentResponse payment = await CreateConfirmedPaymentAsync(client);

        using HttpResponseMessage response = await client.PostAsJsonAsync(
            new Uri("/api/v1/receipts", UriKind.Relative),
            new IssueReceiptRequest { PaymentId = payment.Id },
            TestContext.Current.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        ReceiptResponse receipt = (await response.Content
            .ReadFromJsonAsync<ApiResponse<ReceiptResponse>>(TestContext.Current.CancellationToken))!.Data!;

        receipt.PaymentId.Should().Be(payment.Id);
        receipt.Amount.Should().Be(payment.Amount);
        receipt.ReceiptNumber.Should().StartWith("RCP-");
    }

    [Fact]
    public async Task ອອກໃບຮັບເງິນຊໍ້າກັນສໍາລັບລາຍການການຈ່າຍເງິນດຽວກັນ_ຕ້ອງໄດ້ຮັບ_409()
    {
        using HttpClient client = await TestUserFactory.CreateAuthenticatedClientAsync(
            _factory,
            DormFlowRoles.SuperAdmin);

        PaymentResponse payment = await CreateConfirmedPaymentAsync(client);

        await client.PostAsJsonAsync(
            new Uri("/api/v1/receipts", UriKind.Relative),
            new IssueReceiptRequest { PaymentId = payment.Id },
            TestContext.Current.CancellationToken);

        using HttpResponseMessage secondResponse = await client.PostAsJsonAsync(
            new Uri("/api/v1/receipts", UriKind.Relative),
            new IssueReceiptRequest { PaymentId = payment.Id },
            TestContext.Current.CancellationToken);

        secondResponse.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task ອອກໃບຮັບເງິນສໍາລັບການຈ່າຍເງິນທີ່ບໍ່ໄດ້ຢືນຢັນ_ຕ້ອງໄດ້ຮັບ_409()
    {
        using HttpClient client = await TestUserFactory.CreateAuthenticatedClientAsync(
            _factory,
            DormFlowRoles.SuperAdmin);

        (_, InvoiceResponse invoice) = await CreateIssuedInvoiceAsync(client);

        using HttpResponseMessage paymentResponse = await client.PostAsJsonAsync(
            new Uri("/api/v1/payments", UriKind.Relative),
            new CreatePaymentRequest { InvoiceId = invoice.Id, Amount = invoice.TotalAmount, Method = "Cash" },
            TestContext.Current.CancellationToken);
        PaymentResponse pendingPayment = (await paymentResponse.Content
            .ReadFromJsonAsync<ApiResponse<PaymentResponse>>(TestContext.Current.CancellationToken))!.Data!;

        using HttpResponseMessage response = await client.PostAsJsonAsync(
            new Uri("/api/v1/receipts", UriKind.Relative),
            new IssueReceiptRequest { PaymentId = pendingPayment.Id },
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task ອອກໃບຮັບເງິນສໍາລັບການຈ່າຍເງິນຄືນ_ຕ້ອງໄດ້ຮັບ_409()
    {
        using HttpClient client = await TestUserFactory.CreateAuthenticatedClientAsync(
            _factory,
            DormFlowRoles.SuperAdmin);

        PaymentResponse payment = await CreateConfirmedPaymentAsync(client);

        await client.PostAsJsonAsync(
            new Uri($"/api/v1/payments/{payment.Id}/refund", UriKind.Relative),
            new RefundPaymentRequest { Reason = "ຈ່າຍເງິນຜິດ" },
            TestContext.Current.CancellationToken);

        using HttpResponseMessage response = await client.PostAsJsonAsync(
            new Uri("/api/v1/receipts", UriKind.Relative),
            new IssueReceiptRequest { PaymentId = payment.Id },
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    private static async Task<PaymentResponse> CreateConfirmedPaymentAsync(HttpClient client)
    {
        (_, InvoiceResponse invoice) = await CreateIssuedInvoiceAsync(client);

        using HttpResponseMessage paymentResponse = await client.PostAsJsonAsync(
            new Uri("/api/v1/payments", UriKind.Relative),
            new CreatePaymentRequest { InvoiceId = invoice.Id, Amount = invoice.TotalAmount, Method = "Cash" },
            TestContext.Current.CancellationToken);
        PaymentResponse payment = (await paymentResponse.Content
            .ReadFromJsonAsync<ApiResponse<PaymentResponse>>(TestContext.Current.CancellationToken))!.Data!;

        using HttpResponseMessage confirmResponse = await client.PostAsync(
            new Uri($"/api/v1/payments/{payment.Id}/confirm", UriKind.Relative),
            content: null,
            TestContext.Current.CancellationToken);
        confirmResponse.EnsureSuccessStatusCode();

        return (await confirmResponse.Content
            .ReadFromJsonAsync<ApiResponse<PaymentResponse>>(TestContext.Current.CancellationToken))!.Data!;
    }

    private static async Task<(ContractResponse Contract, InvoiceResponse Invoice)> CreateIssuedInvoiceAsync(HttpClient client)
    {
        ContractResponse contract = await CreateActiveContractAsync(client);

        using HttpResponseMessage createResponse = await client.PostAsJsonAsync(
            new Uri("/api/v1/invoices", UriKind.Relative),
            new CreateInvoiceRequest { ContractId = contract.Id, PeriodMonth = new DateOnly(2026, 1, 1) },
            TestContext.Current.CancellationToken);
        createResponse.EnsureSuccessStatusCode();
        InvoiceResponse invoice = (await createResponse.Content
            .ReadFromJsonAsync<ApiResponse<InvoiceResponse>>(TestContext.Current.CancellationToken))!.Data!;

        using HttpResponseMessage issueResponse = await client.PostAsync(
            new Uri($"/api/v1/invoices/{invoice.Id}/issue", UriKind.Relative),
            content: null,
            TestContext.Current.CancellationToken);
        issueResponse.EnsureSuccessStatusCode();

        InvoiceResponse issuedInvoice = (await issueResponse.Content
            .ReadFromJsonAsync<ApiResponse<InvoiceResponse>>(TestContext.Current.CancellationToken))!.Data!;

        return (contract, issuedInvoice);
    }

    private static async Task<ContractResponse> CreateActiveContractAsync(HttpClient client)
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
        RoomResponse room = (await roomResponse.Content
            .ReadFromJsonAsync<ApiResponse<RoomResponse>>(TestContext.Current.CancellationToken))!.Data!;

        using HttpResponseMessage tenantResponse = await client.PostAsJsonAsync(
            new Uri("/api/v1/tenants", UriKind.Relative),
            new CreateTenantRequest
            {
                OrganizationId = organization.Id,
                FirstName = "ສົມໃຈ",
                LastName = $"ການທົດສອບ-{Guid.NewGuid():N}",
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

        return (await activateResponse.Content
            .ReadFromJsonAsync<ApiResponse<ContractResponse>>(TestContext.Current.CancellationToken))!.Data!;
    }
}
