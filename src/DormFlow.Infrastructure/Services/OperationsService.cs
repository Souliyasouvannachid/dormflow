using DormFlow.Application.Common.Abstractions;
using DormFlow.Application.Common.Authorization;
using DormFlow.Application.Common.Exceptions;
using DormFlow.Application.Features.Invoices;
using DormFlow.Application.Features.Operations;
using DormFlow.Contracts.Invoices;
using DormFlow.Contracts.Operations;
using DormFlow.Domain.Common;
using DormFlow.Domain.Entities;
using DormFlow.Domain.Enums;
using DormFlow.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DormFlow.Infrastructure.Services;

public sealed class OperationsService(DormFlowDbContext db, ICurrentUser user, IDateTimeProvider clock, IInvoiceService invoices) : IOperationsService
{
    private void Check(TenantEntity entity) => TenantScope.EnsureDormitoryAccess(user, entity.OrganizationId, entity.DormitoryId.GetValueOrDefault(), "Record not found.");
    private async Task<T> FindAsync<T>(Guid id, CancellationToken ct) where T : TenantEntity
    {
        var entity = await db.Set<T>().FindAsync([id], ct) ?? throw new NotFoundException("Record not found.");
        if (entity.IsDeleted) throw new NotFoundException("Record not found.");
        Check(entity);
        return entity;
    }
    private IQueryable<T> Scope<T>() where T : TenantEntity
    {
        var org = TenantScope.GetOrganizationRestrictionForListing(user);
        var dorms = TenantScope.GetDormitoryRestrictionForListing(user);
        return db.Set<T>().Where(e => (user.IsSuperAdmin || e.OrganizationId == org) && (dorms == null || (e.DormitoryId.HasValue && dorms.Contains(e.DormitoryId.Value))));
    }
    public async Task ConfirmReservationAsync(Guid id, CancellationToken ct)
    {
        var reservation = await FindAsync<Reservation>(id, ct);
        if (reservation.Status != ReservationStatus.Pending || reservation.ExpiresAt <= clock.UtcNow) throw new ConflictException("Reservation is not pending or has expired.");
        reservation.ConfirmedAt ??= clock.UtcNow;
        await db.SaveChangesAsync(ct);
    }
    public async Task RenewAsync(Guid id, RenewLeaseRequest request, CancellationToken ct)
    {
        var contract = await FindAsync<Contract>(id, ct);
        if (contract.Status != ContractStatus.Active) throw new ConflictException("Only active contracts can be renewed.");
        if (request.EndDate <= contract.EndDate || request.MonthlyRent < 0 || request.WaterRatePerUnit < 0 || request.ElectricityRatePerUnit < 0) throw new ValidationFailedException("Specify a later end date and non-negative rates.");
        db.ContractRenewals.Add(new ContractRenewal { OrganizationId = contract.OrganizationId, DormitoryId = contract.DormitoryId, ContractId = id, PreviousEndDate = contract.EndDate, NewEndDate = request.EndDate, PreviousRent = contract.MonthlyRent, NewRent = request.MonthlyRent ?? contract.MonthlyRent });
        contract.EndDate = request.EndDate;
        contract.MonthlyRent = request.MonthlyRent ?? contract.MonthlyRent;
        contract.WaterRatePerUnit = request.WaterRatePerUnit ?? contract.WaterRatePerUnit;
        contract.ElectricityRatePerUnit = request.ElectricityRatePerUnit ?? contract.ElectricityRatePerUnit;
        await db.SaveChangesAsync(ct);
    }
    public async Task<IReadOnlyList<RenewalResponse>> GetRenewalsAsync(Guid id, CancellationToken ct)
    {
        await FindAsync<Contract>(id, ct);
        return await db.ContractRenewals.Where(x => x.ContractId == id).OrderByDescending(x => x.CreatedAt).Select(x => new RenewalResponse(x.Id, x.PreviousEndDate, x.NewEndDate, x.PreviousRent, x.NewRent, x.CreatedAt)).ToListAsync(ct);
    }
    public async Task<IReadOnlyList<HistoryResponse>> GetTenantHistoryAsync(Guid id, CancellationToken ct)
    {
        var tenant = await db.Tenants.FindAsync([id], ct) ?? throw new NotFoundException("Tenant not found.");
        TenantScope.EnsureOrganizationAccess(user, tenant.OrganizationId, "Tenant not found.");
        return await Scope<Contract>().Where(x => x.TenantId == id || x.CoTenantIds.Contains(id)).OrderByDescending(x => x.StartDate).Select(x => new HistoryResponse(x.Id, x.RoomId, x.ContractNumber, x.Status.ToString(), x.StartDate, x.EndDate, x.SignedAt, x.TerminatedAt)).ToListAsync(ct);
    }
    public async Task<IReadOnlyList<BillingPeriodResponse>> GetPeriodsAsync(CancellationToken ct) => await Scope<BillingPeriod>().OrderByDescending(x => x.Month).Select(x => new BillingPeriodResponse(x.Id, x.DormitoryId, x.Month, x.IsClosed, x.ClosedAt)).ToListAsync(ct);
    public async Task<BillingPeriodResponse> OpenPeriodAsync(OpenBillingPeriodRequest request, CancellationToken ct)
    {
        var dorm = await db.Dormitories.FindAsync([request.DormitoryId], ct) ?? throw new NotFoundException("Dormitory not found.");
        TenantScope.EnsureDormitoryAccess(user, dorm.OrganizationId, dorm.Id, "Dormitory not found.");
        var month = new DateOnly(request.Month.Year, request.Month.Month, 1);
        if (await db.BillingPeriods.AnyAsync(x => x.DormitoryId == dorm.Id && x.Month == month, ct)) throw new ConflictException("Billing period already exists.");
        var period = new BillingPeriod { OrganizationId = dorm.OrganizationId, DormitoryId = dorm.Id, Month = month };
        db.BillingPeriods.Add(period);
        await db.SaveChangesAsync(ct);
        return new(period.Id, period.DormitoryId, period.Month, period.IsClosed, period.ClosedAt);
    }
    public async Task<int> GeneratePeriodAsync(Guid id, CancellationToken ct)
    {
        // The period row serializes generation and close; all invoices commit together.
        return await db.Database.CreateExecutionStrategy().ExecuteAsync(async () =>
        {
            await using var tx = await db.Database.BeginTransactionAsync(ct);
            await db.Database.ExecuteSqlInterpolatedAsync($"SELECT 1 FROM billing_periods WHERE id = {id} FOR UPDATE", ct);
            var period = await FindAsync<BillingPeriod>(id, ct);
            await db.Entry(period).ReloadAsync(ct);
            if (period.IsClosed) throw new ConflictException("Billing period is closed.");
            var contracts = await Scope<Contract>().Where(c => c.DormitoryId == period.DormitoryId && c.Status == ContractStatus.Active && c.StartDate < period.Month.AddMonths(1) && c.EndDate >= period.Month).ToListAsync(ct);
            int count = 0;
            foreach (var contract in contracts)
            {
                if (await db.Invoices.AnyAsync(i => i.ContractId == contract.Id && i.PeriodMonth == period.Month, ct)) continue;
                await invoices.CreateAsync(new CreateInvoiceRequest { ContractId = contract.Id, PeriodMonth = period.Month }, ct);
                count++;
            }
            await tx.CommitAsync(ct);
            return count;
        });
    }
    public async Task ClosePeriodAsync(Guid id, CancellationToken ct)
    {
        await db.Database.CreateExecutionStrategy().ExecuteAsync(async () =>
        {
            await using var tx = await db.Database.BeginTransactionAsync(ct);
            await db.Database.ExecuteSqlInterpolatedAsync($"SELECT 1 FROM billing_periods WHERE id = {id} FOR UPDATE", ct);
            var period = await FindAsync<BillingPeriod>(id, ct);
            await db.Entry(period).ReloadAsync(ct);
            period.IsClosed = true;
            period.ClosedAt = clock.UtcNow;
            await db.SaveChangesAsync(ct);
            await tx.CommitAsync(ct);
        });
    }
    public async Task AddChargeAsync(Guid id, InvoiceChargeRequest request, CancellationToken ct)
    {
        var invoice = await FindAsync<Invoice>(id, ct);
        if (invoice.Status != InvoiceStatus.Draft) throw new ConflictException("Only draft invoices can be changed.");
        if (request.Amount <= 0 || string.IsNullOrWhiteSpace(request.Description) || request.Description.Length > 256) throw new ValidationFailedException("Enter a description and positive amount.");
        db.InvoiceCharges.Add(new InvoiceCharge { OrganizationId = invoice.OrganizationId, DormitoryId = invoice.DormitoryId, InvoiceId = id, Description = request.Description.Trim(), Amount = request.Amount });
        invoice.TotalAmount += request.Amount;
        await db.SaveChangesAsync(ct);
    }
    public async Task<IReadOnlyList<ChargeResponse>> GetChargesAsync(Guid id, CancellationToken ct)
    {
        await FindAsync<Invoice>(id, ct);
        return await db.InvoiceCharges.Where(x => x.InvoiceId == id).Select(x => new ChargeResponse(x.Id, x.Description, x.Amount)).ToListAsync(ct);
    }
    public async Task<PaymentAllocationResult> AllocateAsync(AllocatePaymentRequest request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.IdempotencyKey) || request.IdempotencyKey.Length > 80 || request.Allocations.Count == 0 || request.Allocations.Count > 100 || request.Amount <= 0 || request.Allocations.Any(x => x.Amount <= 0) || request.Allocations.Sum(x => x.Amount) != request.Amount || request.Allocations.Select(x => x.InvoiceId).Distinct().Count() != request.Allocations.Count || !Enum.TryParse<PaymentMethod>(request.Method, true, out var method) || !Enum.IsDefined(method))
            throw new ValidationFailedException("Invalid payment allocations, method, or total.");
        return await db.Database.CreateExecutionStrategy().ExecuteAsync(async () =>
        {
            await using var tx = await db.Database.BeginTransactionAsync(ct);
            string key = $"ALLOC:{user.UserId}:{request.IdempotencyKey}";
            await db.Database.ExecuteSqlInterpolatedAsync($"SELECT pg_advisory_xact_lock(hashtext({key}))", ct);
            var existing = await Scope<Payment>().Where(p => p.Notes == key).OrderBy(p => p.InvoiceId).ToListAsync(ct);
            if (existing.Count > 0)
            {
                if (existing.Count != request.Allocations.Count || existing.Any(p => !request.Allocations.Any(a => a.InvoiceId == p.InvoiceId && a.Amount == p.Amount)) || existing.Any(p => p.Method != method || p.ReferenceNumber != request.ReferenceNumber)) throw new ConflictException("Idempotency key has already been used for another payment.");
                return new PaymentAllocationResult(request.IdempotencyKey, existing.Select(p => p.Id).ToArray());
            }
            var paymentIds = new List<Guid>();
            Guid? organizationId = null;
            Guid? tenantId = null;
            foreach (var allocation in request.Allocations.OrderBy(a => a.InvoiceId))
            {
                await db.Database.ExecuteSqlInterpolatedAsync($"SELECT 1 FROM invoices WHERE id = {allocation.InvoiceId} FOR UPDATE", ct);
                var invoice = await FindAsync<Invoice>(allocation.InvoiceId, ct);
                await db.Entry(invoice).ReloadAsync(ct);
                if (invoice.Status is InvoiceStatus.Draft or InvoiceStatus.Cancelled || allocation.Amount > invoice.TotalAmount - invoice.PaidAmount) throw new ConflictException("Payment exceeds the outstanding balance or invoice is not payable.");
                var contract = await FindAsync<Contract>(invoice.ContractId, ct);
                organizationId ??= invoice.OrganizationId;
                tenantId ??= contract.TenantId;
                if (organizationId != invoice.OrganizationId || tenantId != contract.TenantId) throw new ValidationFailedException("Allocations must belong to one tenant and organization.");
                var payment = new Payment { OrganizationId = invoice.OrganizationId, DormitoryId = invoice.DormitoryId, InvoiceId = invoice.Id, Amount = allocation.Amount, Method = method, PaymentDate = request.PaymentDate, ReferenceNumber = request.ReferenceNumber, Status = PaymentStatus.Confirmed, ConfirmedAt = clock.UtcNow, Notes = key };
                db.Payments.Add(payment);
                invoice.PaidAmount += allocation.Amount;
                invoice.Status = invoice.PaidAmount == invoice.TotalAmount ? InvoiceStatus.Paid : InvoiceStatus.PartiallyPaid;
                paymentIds.Add(payment.Id);
            }
            await db.SaveChangesAsync(ct);
            await tx.CommitAsync(ct);
            return new PaymentAllocationResult(request.IdempotencyKey, paymentIds);
        });
    }
    public async Task ReturnParcelAsync(Guid id, CancellationToken ct)
    {
        var parcel = await FindAsync<Parcel>(id, ct);
        if (parcel.Status != ParcelStatus.Received) throw new ConflictException("Only parcels awaiting pickup can be returned.");
        parcel.Status = ParcelStatus.Returned;
        await db.SaveChangesAsync(ct);
    }
    public async Task CancelAnnouncementAsync(Guid id, CancellationToken ct)
    {
        var announcement = await FindAsync<Announcement>(id, ct);
        if (announcement.Status is AnnouncementStatus.Archived or AnnouncementStatus.Cancelled) throw new ConflictException("Announcement is already closed.");
        announcement.Status = AnnouncementStatus.Cancelled;
        await db.SaveChangesAsync(ct);
    }
    public async Task<IReadOnlyList<TimelineResponse>> GetTimelineAsync(Guid id, CancellationToken ct)
    {
        await FindAsync<MaintenanceRequest>(id, ct);
        return await db.MaintenanceEvents.Where(x => x.MaintenanceRequestId == id).OrderBy(x => x.CreatedAt).Select(x => new TimelineResponse(x.Id, x.Status, x.Note, x.CreatedBy, x.CreatedAt)).ToListAsync(ct);
    }
    public async Task AddTimelineAsync(Guid id, TimelineNoteRequest request, CancellationToken ct)
    {
        var job = await FindAsync<MaintenanceRequest>(id, ct);
        if (string.IsNullOrWhiteSpace(request.Note) || request.Note.Length > 2000) throw new ValidationFailedException("Enter a note of at most 2000 characters.");
        if (user.Roles.Contains(DormFlowRoles.Technician) && job.AssignedTechnicianId != user.UserId) throw new ForbiddenAccessException("This job is not assigned to you.");
        db.MaintenanceEvents.Add(new MaintenanceEvent { OrganizationId = job.OrganizationId, DormitoryId = job.DormitoryId, MaintenanceRequestId = id, Status = job.Status.ToString(), Note = request.Note.Trim() });
        await db.SaveChangesAsync(ct);
    }
    public async Task RecordCostAsync(Guid id, MaintenanceCostRequest request, CancellationToken ct)
    {
        var job = await FindAsync<MaintenanceRequest>(id, ct);
        if (request.Amount <= 0 || string.IsNullOrWhiteSpace(request.Description) || request.Description.Length > 256) throw new ValidationFailedException("Enter a description and positive cost.");
        db.Expenses.Add(new Expense { OrganizationId = job.OrganizationId, DormitoryId = job.DormitoryId, MaintenanceRequestId = id, Category = ExpenseCategory.Maintenance, Amount = request.Amount, ExpenseDate = DateOnly.FromDateTime(clock.UtcNow.UtcDateTime), Description = request.Description });
        await db.SaveChangesAsync(ct);
    }
    public async Task<NotificationCountResponse> GetUnreadAsync(CancellationToken ct) => new(await db.Notifications.CountAsync(x => x.RecipientUserId == user.UserId && !x.IsRead, ct));
    public async Task<int> ReadAllAsync(CancellationToken ct)
    {
        var now = clock.UtcNow;
        return await db.Notifications.Where(x => x.RecipientUserId == user.UserId && !x.IsRead).ExecuteUpdateAsync(x => x.SetProperty(n => n.IsRead, true).SetProperty(n => n.ReadAt, now), ct);
    }
}
