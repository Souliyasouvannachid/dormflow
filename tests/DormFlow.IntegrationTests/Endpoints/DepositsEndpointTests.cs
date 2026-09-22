using System.Net;
using System.Net.Http.Json;
using DormFlow.Application.Common.Abstractions;
using DormFlow.Contracts.Buildings;
using DormFlow.Contracts.Common;
using DormFlow.Contracts.Deposits;
using DormFlow.Contracts.Dormitories;
using DormFlow.Contracts.Floors;
using DormFlow.Contracts.Leases;
using DormFlow.Contracts.Organizations;
using DormFlow.Contracts.RoomTypes;
using DormFlow.Contracts.Rooms;
using DormFlow.Contracts.Tenants;
using DormFlow.IntegrationTests.Infrastructure;
using FluentAssertions;
using Xunit;

namespace DormFlow.IntegrationTests.Endpoints;

/// <summary>
/// ທົດສອບວົງຈອນຊີວິດຂອງເງິນຝາກຜ່ານ HTTP ຄວາມຈິງ
/// Collect (ຕ້ອງມີສັນຍາທີ່ຍັງບໍ່ທັນສໍາເລັດ) -> Terminate ສັນຍາ -> Refund (ເຕັມ/ບາງ/ປະຖິ້ມທັງຝົດ)
/// </summary>
[Collection(ApiTestFixtures.Name)]
public class DepositsEndpointTests(DormFlowApiFactory factory)
{
    private readonly DormFlowApiFactory _factory = factory;

    [Fact]
    public async Task ໄດ້ຮັບເງິນຝາກແລະສົ່ງຄືນມັນຢ່າງເຕັມທີ່ຫຼັງຈາກສັນຍາສິ້ນສຸດລົງ_ຕ້ອງເຮັດວຽກຢູ່ໃນວົງເຕັມ()
    {
        using HttpClient client = await TestUserFactory.CreateAuthenticatedClientAsync(
            _factory,
            DormFlowRoles.SuperAdmin);

        ContractResponse contract = await CreateActiveContractAsync(client);

        DepositResponse deposit = await CollectDepositAsync(client, contract.Id);
        deposit.Status.Should().Be("Held");
        deposit.Amount.Should().Be(contract.DepositAmount);

        await TerminateContractAsync(client, contract.Id);

        using HttpResponseMessage refundResponse = await client.PostAsJsonAsync(
            new Uri($"/api/v1/deposits/{deposit.Id}/refund", UriKind.Relative),
            new RefundDepositRequest { RefundAmount = deposit.Amount },
            TestContext.Current.CancellationToken);
        refundResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        DepositResponse refunded = (await refundResponse.Content
            .ReadFromJsonAsync<ApiResponse<DepositResponse>>(TestContext.Current.CancellationToken))!.Data!;
        refunded.Status.Should().Be("Refunded");
        refunded.RefundedAmount.Should().Be(deposit.Amount);
        refunded.RefundedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task ການຄືນເງິນເງິນຝາກບາງສ່ວນທີ່ມີເຫດຜົນ_ຕ້ອງໄດ້ຮັບການຟື້ນຟູບາງສະຖານະ()
    {
        using HttpClient client = await TestUserFactory.CreateAuthenticatedClientAsync(
            _factory,
            DormFlowRoles.SuperAdmin);

        ContractResponse contract = await CreateActiveContractAsync(client);
        DepositResponse deposit = await CollectDepositAsync(client, contract.Id);
        await TerminateContractAsync(client, contract.Id);

        decimal partialAmount = deposit.Amount / 2;
        using HttpResponseMessage refundResponse = await client.PostAsJsonAsync(
            new Uri($"/api/v1/deposits/{deposit.Id}/refund", UriKind.Relative),
            new RefundDepositRequest { RefundAmount = partialAmount, DeductionReason = "ຄ່າທຳຄວາມສະອາດຫ້ອງ" },
            TestContext.Current.CancellationToken);
        refundResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        DepositResponse refunded = (await refundResponse.Content
            .ReadFromJsonAsync<ApiResponse<DepositResponse>>(TestContext.Current.CancellationToken))!.Data!;
        refunded.Status.Should().Be("PartiallyRefunded");
        refunded.RefundedAmount.Should().Be(partialAmount);
        refunded.DeductionReason.Should().Be("ຄ່າທຳຄວາມສະອາດຫ້ອງ");
    }

    [Fact]
    public async Task ຍົກເລີກເງິນຝາກທັງໝົດ_ຕ້ອງໄດ້ຮັບສະຖານະການປະຕິບັດ()
    {
        using HttpClient client = await TestUserFactory.CreateAuthenticatedClientAsync(
            _factory,
            DormFlowRoles.SuperAdmin);

        ContractResponse contract = await CreateActiveContractAsync(client);
        DepositResponse deposit = await CollectDepositAsync(client, contract.Id);
        await TerminateContractAsync(client, contract.Id);

        using HttpResponseMessage refundResponse = await client.PostAsJsonAsync(
            new Uri($"/api/v1/deposits/{deposit.Id}/refund", UriKind.Relative),
            new RefundDepositRequest { RefundAmount = 0, DeductionReason = "ຄວາມເສຍຫາຍເກີນຂອບເຂດຈໍາກັດເງິນຝາກ" },
            TestContext.Current.CancellationToken);
        refundResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        DepositResponse refunded = (await refundResponse.Content
            .ReadFromJsonAsync<ApiResponse<DepositResponse>>(TestContext.Current.CancellationToken))!.Data!;
        refunded.Status.Should().Be("Forfeited");
        refunded.RefundedAmount.Should().Be(0);
    }

    [Fact]
    public async Task ໄດ້ຮັບເງິນຝາກທີ່ຊ້ໍາກັນສໍາລັບສັນຍາດຽວກັນ_ຕ້ອງໄດ້ຮັບ_409()
    {
        using HttpClient client = await TestUserFactory.CreateAuthenticatedClientAsync(
            _factory,
            DormFlowRoles.SuperAdmin);

        ContractResponse contract = await CreateActiveContractAsync(client);
        await CollectDepositAsync(client, contract.Id);

        using HttpResponseMessage secondResponse = await client.PostAsJsonAsync(
            new Uri("/api/v1/deposits", UriKind.Relative),
            new CollectDepositRequest { ContractId = contract.Id },
            TestContext.Current.CancellationToken);

        secondResponse.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task ຈ່າຍຄືນເງິນຝາກກ່ອນສັນຍາສິ້ນສຸດລົງ_ຕ້ອງໄດ້ຮັບ_409()
    {
        using HttpClient client = await TestUserFactory.CreateAuthenticatedClientAsync(
            _factory,
            DormFlowRoles.SuperAdmin);

        ContractResponse contract = await CreateActiveContractAsync(client);
        DepositResponse deposit = await CollectDepositAsync(client, contract.Id);

        using HttpResponseMessage refundResponse = await client.PostAsJsonAsync(
            new Uri($"/api/v1/deposits/{deposit.Id}/refund", UriKind.Relative),
            new RefundDepositRequest { RefundAmount = deposit.Amount },
            TestContext.Current.CancellationToken);

        refundResponse.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task ການຄືນເງິນເກີນຈໍານວນທີ່ໄດ້ຮັບ_ຕ້ອງໄດ້ຮັບ_400()
    {
        using HttpClient client = await TestUserFactory.CreateAuthenticatedClientAsync(
            _factory,
            DormFlowRoles.SuperAdmin);

        ContractResponse contract = await CreateActiveContractAsync(client);
        DepositResponse deposit = await CollectDepositAsync(client, contract.Id);
        await TerminateContractAsync(client, contract.Id);

        using HttpResponseMessage refundResponse = await client.PostAsJsonAsync(
            new Uri($"/api/v1/deposits/{deposit.Id}/refund", UriKind.Relative),
            new RefundDepositRequest { RefundAmount = deposit.Amount + 1000 },
            TestContext.Current.CancellationToken);

        refundResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task ການຄືນເງິນບາງສ່ວນໂດຍບໍ່ໄດ້ລະບຸເຫດຜົນສໍາລັບສ່ວນທີ່ແຕກຫັກ_ຕ້ອງໄດ້ຮັບ_400()
    {
        using HttpClient client = await TestUserFactory.CreateAuthenticatedClientAsync(
            _factory,
            DormFlowRoles.SuperAdmin);

        ContractResponse contract = await CreateActiveContractAsync(client);
        DepositResponse deposit = await CollectDepositAsync(client, contract.Id);
        await TerminateContractAsync(client, contract.Id);

        using HttpResponseMessage refundResponse = await client.PostAsJsonAsync(
            new Uri($"/api/v1/deposits/{deposit.Id}/refund", UriKind.Relative),
            new RefundDepositRequest { RefundAmount = deposit.Amount / 2 },
            TestContext.Current.CancellationToken);

        refundResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task ຈ່າຍຄືນເງິນທີ່ຝາກໄວ້ແລ້ວ_ຕ້ອງໄດ້ຮັບ_409()
    {
        using HttpClient client = await TestUserFactory.CreateAuthenticatedClientAsync(
            _factory,
            DormFlowRoles.SuperAdmin);

        ContractResponse contract = await CreateActiveContractAsync(client);
        DepositResponse deposit = await CollectDepositAsync(client, contract.Id);
        await TerminateContractAsync(client, contract.Id);

        await client.PostAsJsonAsync(
            new Uri($"/api/v1/deposits/{deposit.Id}/refund", UriKind.Relative),
            new RefundDepositRequest { RefundAmount = deposit.Amount },
            TestContext.Current.CancellationToken);

        using HttpResponseMessage secondRefundResponse = await client.PostAsJsonAsync(
            new Uri($"/api/v1/deposits/{deposit.Id}/refund", UriKind.Relative),
            new RefundDepositRequest { RefundAmount = 0 },
            TestContext.Current.CancellationToken);

        secondRefundResponse.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    private static async Task<DepositResponse> CollectDepositAsync(HttpClient client, Guid contractId)
    {
        using HttpResponseMessage response = await client.PostAsJsonAsync(
            new Uri("/api/v1/deposits", UriKind.Relative),
            new CollectDepositRequest { ContractId = contractId },
            TestContext.Current.CancellationToken);

        response.EnsureSuccessStatusCode();

        return (await response.Content
            .ReadFromJsonAsync<ApiResponse<DepositResponse>>(TestContext.Current.CancellationToken))!.Data!;
    }

    private static async Task TerminateContractAsync(HttpClient client, Guid contractId)
    {
        using HttpResponseMessage response = await client.PostAsJsonAsync(
            new Uri($"/api/v1/contracts/{contractId}/terminate", UriKind.Relative),
            new TerminateContractRequest { Reason = "ສັນຍາຄົບກຳນົດ" },
            TestContext.Current.CancellationToken);

        response.EnsureSuccessStatusCode();
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
