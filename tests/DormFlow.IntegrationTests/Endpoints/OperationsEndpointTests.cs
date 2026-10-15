using System.Net;
using System.Net.Http.Json;
using DormFlow.Application.Common.Abstractions;
using DormFlow.Contracts.Common;
using DormFlow.Contracts.Invoices;
using DormFlow.Contracts.Maintenance;
using DormFlow.Contracts.Operations;
using DormFlow.Domain.Entities;
using DormFlow.Domain.Enums;
using DormFlow.Infrastructure.Persistence;
using DormFlow.IntegrationTests.Infrastructure;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace DormFlow.IntegrationTests.Endpoints;

[Collection(ApiTestFixtures.Name)]
public class OperationsEndpointTests(DormFlowApiFactory factory)
{
    private static CancellationToken Ct => TestContext.Current.CancellationToken;
    private async Task<(Guid Org, Guid Dorm, Guid Room, Guid Tenant, Guid Contract, Guid Invoice)> SeedAsync()
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<DormFlowDbContext>();
        string code = Guid.NewGuid().ToString("N")[..10];
        var org = new Organization { Code = code, Name = "Operations test" };
        var dorm = new Dormitory { OrganizationId = org.Id, Code = code, Name = "Test dorm", WaterRatePerUnit = 18, ElectricityRatePerUnit = 9 };
        var building = new Building { OrganizationId = org.Id, DormitoryId = dorm.Id, Code = code, Name = "A" };
        var floor = new Floor { OrganizationId = org.Id, DormitoryId = dorm.Id, BuildingId = building.Id, FloorNumber = 1, Name = "1" };
        var type = new RoomType { OrganizationId = org.Id, DormitoryId = dorm.Id, Code = code, Name = "Standard", MaximumOccupants = 3 };
        var room = new Room { OrganizationId = org.Id, DormitoryId = dorm.Id, BuildingId = building.Id, FloorId = floor.Id, RoomTypeId = type.Id, RoomNumber = code, MonthlyRent = 1000, MaximumOccupants = 3, Status = RoomStatus.Occupied };
        var tenant = new Tenant { OrganizationId = org.Id, FirstName = "Test", LastName = code };
        var contract = new Contract { OrganizationId = org.Id, DormitoryId = dorm.Id, RoomId = room.Id, TenantId = tenant.Id, ContractNumber = code, StartDate = new DateOnly(2026, 1, 1), EndDate = new DateOnly(2027, 12, 31), MonthlyRent = 1000, Status = ContractStatus.Active, WaterRatePerUnit = 12, ElectricityRatePerUnit = 6 };
        var invoice = new Invoice { OrganizationId = org.Id, DormitoryId = dorm.Id, ContractId = contract.Id, InvoiceNumber = code, PeriodMonth = new DateOnly(2026, 1, 1), IssueDate = new DateOnly(2026, 1, 1), DueDate = new DateOnly(2026, 1, 5), RentAmount = 1000, TotalAmount = 1000, Status = InvoiceStatus.Issued };
        db.AddRange(org, dorm, building, floor, type, room, tenant, contract, invoice);
        await db.SaveChangesAsync(Ct);
        return (org.Id, dorm.Id, room.Id, tenant.Id, contract.Id, invoice.Id);
    }
    [Fact]
    public async Task Allocation_IsAtomic_Idempotent_AndCreatesReceipt()
    {
        var seed = await SeedAsync();
        using var client = await TestUserFactory.CreateAuthenticatedClientAsync(factory, DormFlowRoles.SuperAdmin);
        var request = new AllocatePaymentRequest(Guid.NewGuid().ToString("N"), 400, "Cash", DateTimeOffset.UtcNow, null, [new(seed.Invoice, 400)]);
        var response = await client.PostAsJsonAsync("/api/v1/payments/allocate", request, Ct);
        response.StatusCode.Should().Be(HttpStatusCode.OK, await response.Content.ReadAsStringAsync(Ct));
        var first = (await response.Content.ReadFromJsonAsync<ApiResponse<PaymentAllocationResult>>(Ct))!.Data!;
        var repeated = await client.PostAsJsonAsync("/api/v1/payments/allocate", request, Ct);
        repeated.StatusCode.Should().Be(HttpStatusCode.OK);
        var second = (await repeated.Content.ReadFromJsonAsync<ApiResponse<PaymentAllocationResult>>(Ct))!.Data!;
        second.PaymentIds.Should().Equal(first.PaymentIds);
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<DormFlowDbContext>();
        (await db.Invoices.SingleAsync(i => i.Id == seed.Invoice, Ct)).PaidAmount.Should().Be(400);
        (await db.Receipts.CountAsync(r => first.PaymentIds.Contains(r.PaymentId), Ct)).Should().Be(1);
        var changed = await client.PostAsJsonAsync("/api/v1/payments/allocate", request with { Amount = 500, Allocations = [new(seed.Invoice, 500)] }, Ct);
        changed.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }
    [Fact]
    public async Task ParallelAllocations_CannotOverpay()
    {
        var seed = await SeedAsync();
        using var client = await TestUserFactory.CreateAuthenticatedClientAsync(factory, DormFlowRoles.SuperAdmin);
        var one = new AllocatePaymentRequest(Guid.NewGuid().ToString("N"), 700, "BankTransfer", DateTimeOffset.UtcNow, "test", [new(seed.Invoice, 700)]);
        var results = await Task.WhenAll(client.PostAsJsonAsync("/api/v1/payments/allocate", one, Ct), client.PostAsJsonAsync("/api/v1/payments/allocate", one with { IdempotencyKey = Guid.NewGuid().ToString("N") }, Ct));
        results.Count(r => r.StatusCode == HttpStatusCode.OK).Should().Be(1);
        results.Count(r => r.StatusCode == HttpStatusCode.Conflict).Should().Be(1);
    }
    [Fact]
    public async Task Allocation_RollsBackWhenAnyInvoiceIsInvalid()
    {
        var seed = await SeedAsync();
        using var client = await TestUserFactory.CreateAuthenticatedClientAsync(factory, DormFlowRoles.SuperAdmin);
        var response = await client.PostAsJsonAsync("/api/v1/payments/allocate", new AllocatePaymentRequest(Guid.NewGuid().ToString("N"), 1200, "Cash", DateTimeOffset.UtcNow, null, [new(seed.Invoice, 200), new(Guid.NewGuid(), 1000)]), Ct);
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        using var scope = factory.Services.CreateScope();
        (await scope.ServiceProvider.GetRequiredService<DormFlowDbContext>().Invoices.SingleAsync(i => i.Id == seed.Invoice, Ct)).PaidAmount.Should().Be(0);
    }
    [Fact]
    public async Task Renewal_PreservesHistory_AndRejectsEarlierEndDate()
    {
        var seed = await SeedAsync();
        using var client = await TestUserFactory.CreateAuthenticatedClientAsync(factory, DormFlowRoles.SuperAdmin);
        var response = await client.PostAsJsonAsync($"/api/v1/contracts/{seed.Contract}/renew", new RenewLeaseRequest(new DateOnly(2028, 12, 31), 1200, 14, 7), Ct);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var history = await client.GetFromJsonAsync<ApiResponse<RenewalResponse[]>>($"/api/v1/contracts/{seed.Contract}/renewals", Ct);
        history!.Data!.Single().PreviousRent.Should().Be(1000);
        var invalid = await client.PostAsJsonAsync($"/api/v1/contracts/{seed.Contract}/renew", new RenewLeaseRequest(new DateOnly(2028, 1, 1), null, null, null), Ct);
        invalid.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
    [Fact]
    public async Task Billing_GeneratesOnce_AndClosedPeriodRejectsGeneration()
    {
        var seed = await SeedAsync();
        using var client = await TestUserFactory.CreateAuthenticatedClientAsync(factory, DormFlowRoles.SuperAdmin);
        var open = await client.PostAsJsonAsync("/api/v1/billing-periods", new OpenBillingPeriodRequest(seed.Dorm, new DateOnly(2026, 6, 1)), Ct);
        open.StatusCode.Should().Be(HttpStatusCode.OK);
        var period = (await open.Content.ReadFromJsonAsync<ApiResponse<BillingPeriodResponse>>(Ct))!.Data!;
        (await client.PostAsync($"/api/v1/billing-periods/{period.Id}/generate", null, Ct)).StatusCode.Should().Be(HttpStatusCode.OK);
        (await client.PostAsync($"/api/v1/billing-periods/{period.Id}/generate", null, Ct)).StatusCode.Should().Be(HttpStatusCode.OK);
        using (var scope = factory.Services.CreateScope())
            (await scope.ServiceProvider.GetRequiredService<DormFlowDbContext>().Invoices.CountAsync(i => i.ContractId == seed.Contract && i.PeriodMonth == period.Month, Ct)).Should().Be(1);
        (await client.PostAsync($"/api/v1/billing-periods/{period.Id}/close", null, Ct)).StatusCode.Should().Be(HttpStatusCode.OK);
        (await client.PostAsync($"/api/v1/billing-periods/{period.Id}/generate", null, Ct)).StatusCode.Should().Be(HttpStatusCode.Conflict);
    }
    [Fact]
    public async Task Invoice_UsesContractRates_AndAddsDraftCharges()
    {
        var seed = await SeedAsync();
        using (var scope = factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<DormFlowDbContext>();
            db.MeterReadings.Add(new MeterReading { OrganizationId = seed.Org, DormitoryId = seed.Dorm, RoomId = seed.Room, MeterType = MeterType.Water, PeriodMonth = new DateOnly(2026, 7, 1), ReadingDate = new DateOnly(2026, 7, 1), CurrentReading = 10, UnitsUsed = 10 });
            await db.SaveChangesAsync(Ct);
        }
        using var client = await TestUserFactory.CreateAuthenticatedClientAsync(factory, DormFlowRoles.SuperAdmin);
        var created = await client.PostAsJsonAsync("/api/v1/invoices", new CreateInvoiceRequest { ContractId = seed.Contract, PeriodMonth = new DateOnly(2026, 7, 1) }, Ct);
        created.StatusCode.Should().Be(HttpStatusCode.OK);
        var invoice = (await created.Content.ReadFromJsonAsync<ApiResponse<InvoiceResponse>>(Ct))!.Data!;
        invoice.WaterAmount.Should().Be(120);
        (await client.PostAsJsonAsync($"/api/v1/invoices/{invoice.Id}/charges", new InvoiceChargeRequest("Cleaning", 150), Ct)).StatusCode.Should().Be(HttpStatusCode.OK);
        var updated = await client.GetFromJsonAsync<ApiResponse<InvoiceResponse>>($"/api/v1/invoices/{invoice.Id}", Ct);
        updated!.Data!.TotalAmount.Should().Be(1270);
    }
    [Fact]
    public async Task Maintenance_RecordsTimelineCost_AndNotifiesOwner()
    {
        var seed = await SeedAsync();
        using var client = await TestUserFactory.CreateAuthenticatedClientAsync(factory, DormFlowRoles.Owner, seed.Org);
        var created = await client.PostAsJsonAsync("/api/v1/maintenance-requests", new CreateMaintenanceRequestRequest { RoomId = seed.Room, TenantId = seed.Tenant, Title = "Water leak", Category = "Water", Priority = "High" }, Ct);
        created.StatusCode.Should().Be(HttpStatusCode.OK, await created.Content.ReadAsStringAsync(Ct));
        var job = (await created.Content.ReadFromJsonAsync<ApiResponse<MaintenanceRequestResponse>>(Ct))!.Data!;
        (await client.PostAsJsonAsync($"/api/v1/maintenance-requests/{job.Id}/costs", new MaintenanceCostRequest(250, "Pipe replacement"), Ct)).StatusCode.Should().Be(HttpStatusCode.OK);
        var timeline = await client.GetFromJsonAsync<ApiResponse<TimelineResponse[]>>($"/api/v1/maintenance-requests/{job.Id}/timeline", Ct);
        timeline!.Data.Should().NotBeEmpty();
        var unread = await client.GetFromJsonAsync<ApiResponse<NotificationCountResponse>>("/api/v1/notifications/unread-count", Ct);
        unread!.Data!.UnreadCount.Should().BeGreaterThan(0);
        (await client.PostAsync("/api/v1/notifications/read-all", null, Ct)).EnsureSuccessStatusCode();
        (await client.GetFromJsonAsync<ApiResponse<NotificationCountResponse>>("/api/v1/notifications/unread-count", Ct))!.Data!.UnreadCount.Should().Be(0);
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<DormFlowDbContext>();
        (await db.MaintenanceRequests.SingleAsync(x => x.Id == job.Id, Ct)).ActualCost.Should().Be(250);
        (await db.Expenses.SingleAsync(x => x.MaintenanceRequestId == job.Id, Ct)).Amount.Should().Be(250);
    }
    [Theory]
    [InlineData("revenue")][InlineData("expenses")][InlineData("cash-flow")][InlineData("outstanding")][InlineData("occupancy")][InlineData("contracts")][InlineData("reservations")][InlineData("maintenance")][InlineData("parcels")][InlineData("move-in-out")][InlineData("room-performance")]
    public async Task Reports_AreAvailable_AndScopedToOrganization(string report)
    {
        var seed = await SeedAsync();
        using var client = await TestUserFactory.CreateAuthenticatedClientAsync(factory, DormFlowRoles.Owner, seed.Org);
        var response = await client.GetAsync($"/api/v1/reports/detail/{report}?from=2026-01-01&to=2027-12-31", Ct);
        response.StatusCode.Should().Be(HttpStatusCode.OK, await response.Content.ReadAsStringAsync(Ct));
        var analytics = await client.GetFromJsonAsync<ApiResponse<AnalyticsResponse>>($"/api/v1/analytics?from=2026-01-01&to=2027-12-31", Ct);
        analytics!.Data!.Outstanding.Should().Be(1000);
    }
    [Fact]
    public async Task ExtensionCommands_DenyCrossOrganizationAccess()
    {
        var one = await SeedAsync(); var two = await SeedAsync();
        using var client = await TestUserFactory.CreateAuthenticatedClientAsync(factory, DormFlowRoles.Owner, one.Org);
        (await client.PostAsJsonAsync($"/api/v1/contracts/{two.Contract}/renew", new RenewLeaseRequest(new DateOnly(2029, 1, 1), null, null, null), Ct)).StatusCode.Should().Be(HttpStatusCode.NotFound);
        (await client.GetAsync($"/api/v1/tenants/{two.Tenant}/history", Ct)).StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
