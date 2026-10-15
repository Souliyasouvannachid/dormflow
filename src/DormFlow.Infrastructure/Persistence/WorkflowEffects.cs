using DormFlow.Application.Common.Exceptions;
using DormFlow.Domain.Common;
using DormFlow.Domain.Entities;
using DormFlow.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace DormFlow.Infrastructure.Persistence;

/// <summary>Persist derived documents and notifications atomically with the source change.</summary>
internal static class WorkflowEffects
{
    internal static async Task ApplyAsync(DormFlowDbContext db, CancellationToken ct)
    {
        db.ChangeTracker.DetectChanges();
        foreach (var entry in db.ChangeTracker.Entries<Announcement>().Where(e => e.State == EntityState.Added || e.State == EntityState.Modified).ToArray())
        {
            if (entry.Entity.Priority is not ("Low" or "Normal" or "High" or "Urgent") || entry.Entity.TargetRoles.Any(r => r is not ("SuperAdmin" or "Owner" or "DormManager" or "Manager" or "Staff" or "Technician" or "Accountant")))
                throw new ValidationFailedException("Invalid announcement priority or recipient roles.");
        }
        foreach (var entry in db.ChangeTracker.Entries<MaintenanceRequest>().Where(e => e.State == EntityState.Added || e.State == EntityState.Modified).ToArray())
        {
            if (string.IsNullOrWhiteSpace(entry.Entity.Category) || entry.Entity.Category.Length > 100 || entry.Entity.ActualCost < 0)
                throw new ValidationFailedException("Invalid maintenance category or cost.");
        }
        foreach (var entry in db.ChangeTracker.Entries<Contract>().Where(e => e.State == EntityState.Added).ToArray())
        {
            var contract = entry.Entity;
            var dorm = await db.Dormitories.FindAsync([contract.DormitoryId], ct);
            contract.WaterRatePerUnit ??= dorm?.WaterRatePerUnit ?? 0;
            contract.ElectricityRatePerUnit ??= dorm?.ElectricityRatePerUnit ?? 0;
        }
        foreach (var entry in db.ChangeTracker.Entries<Invoice>().Where(e => e.State == EntityState.Added).ToArray())
        {
            if (await db.BillingPeriods.AnyAsync(p => p.DormitoryId == entry.Entity.DormitoryId && p.Month == entry.Entity.PeriodMonth && p.IsClosed, ct))
                throw new ConflictException("Billing period is closed.");
        }
        foreach (var entry in db.ChangeTracker.Entries<Expense>().Where(e => e.State == EntityState.Added && e.Entity.MaintenanceRequestId != null).ToArray())
        {
            var expense = entry.Entity;
            var job = await db.MaintenanceRequests.FindAsync([expense.MaintenanceRequestId], ct);
            if (job is null || job.OrganizationId != expense.OrganizationId || job.DormitoryId != expense.DormitoryId)
                throw new NotFoundException("Maintenance request not found.");
            job.ActualCost += expense.Amount;
        }
        foreach (var entry in db.ChangeTracker.Entries<Payment>().Where(e => e.State == EntityState.Added || e.State == EntityState.Modified).ToArray())
        {
            var payment = entry.Entity;
            if (payment.Status != PaymentStatus.Confirmed || (entry.State == EntityState.Modified && entry.OriginalValues.GetValue<PaymentStatus>(nameof(Payment.Status)) == PaymentStatus.Confirmed)) continue;
            if (!db.Receipts.Local.Any(r => r.PaymentId == payment.Id) && !await db.Receipts.AnyAsync(r => r.PaymentId == payment.Id, ct))
                db.Receipts.Add(new Receipt { OrganizationId = payment.OrganizationId, DormitoryId = payment.DormitoryId, PaymentId = payment.Id, Amount = payment.Amount, IssuedAt = payment.ConfirmedAt ?? DateTimeOffset.UtcNow, ReceiptNumber = $"RCP-{payment.Id:N}" });
        }
        foreach (var entry in db.ChangeTracker.Entries<MaintenanceRequest>().Where(e => e.State == EntityState.Added || e.State == EntityState.Modified).ToArray())
        {
            var job = entry.Entity;
            if (job.TenantId is { } tenantId && !await db.Tenants.AnyAsync(t => t.Id == tenantId && t.OrganizationId == job.OrganizationId, ct))
                throw new NotFoundException("Tenant not found.");
            bool changed = entry.State == EntityState.Added || entry.Property(x => x.Status).IsModified || entry.Property(x => x.AssignedTechnicianId).IsModified;
            if (!changed) continue;
            db.MaintenanceEvents.Add(new MaintenanceEvent { OrganizationId = job.OrganizationId, DormitoryId = job.DormitoryId, MaintenanceRequestId = job.Id, Status = job.Status.ToString(), Note = job.AssignedTechnicianId.HasValue ? $"Assigned to {job.AssignedTechnicianId}" : job.Title });
            await NotifyAsync(db, job, "งานซ่อม: " + job.Title, job.Status.ToString(), job.AssignedTechnicianId, [], ct);
        }
        foreach (var entry in db.ChangeTracker.Entries<Parcel>().Where(e => e.State == EntityState.Added).ToArray())
            await NotifyAsync(db, entry.Entity, "พัสดุเข้าใหม่", entry.Entity.TrackingNumber ?? "พัสดุรอรับ", null, [], ct);
        foreach (var entry in db.ChangeTracker.Entries<Announcement>().Where(e => (e.State == EntityState.Added || e.State == EntityState.Modified) && e.Entity.Status == AnnouncementStatus.Published).ToArray())
        {
            if (entry.State == EntityState.Modified && !entry.Property(x => x.Status).IsModified) continue;
            await NotifyAsync(db, entry.Entity, entry.Entity.Title, entry.Entity.Content, null, entry.Entity.TargetRoles, ct);
        }
    }

    private static async Task NotifyAsync(DormFlowDbContext db, TenantEntity source, string title, string message, Guid? assignee, string[] roles, CancellationToken ct)
    {
        var users = db.Users.Where(u => u.OrganizationId == source.OrganizationId && !u.IsDeleted);
        var ownerIds = from ur in db.UserRoles join r in db.Roles on ur.RoleId equals r.Id where r.Name == "Owner" select ur.UserId;
        users = users.Where(u => ownerIds.Contains(u.Id) || db.UserDormitories.Any(d => d.UserId == u.Id && d.DormitoryId == source.DormitoryId));
        if (roles.Length > 0)
            users = users.Where(u => (from ur in db.UserRoles join r in db.Roles on ur.RoleId equals r.Id where ur.UserId == u.Id && roles.Contains(r.Name!) select ur.UserId).Any());
        var recipients = await users.Select(u => u.Id).ToListAsync(ct);
        if (assignee.HasValue && await db.Users.AnyAsync(u => u.Id == assignee && u.OrganizationId == source.OrganizationId && !u.IsDeleted, ct)) recipients.Add(assignee.Value);
        foreach (var id in recipients.Distinct())
            db.Notifications.Add(new Notification { OrganizationId = source.OrganizationId, RecipientUserId = id, Title = title[..Math.Min(title.Length, 200)], Message = message[..Math.Min(message.Length, 2000)] });
    }
}
