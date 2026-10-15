using System.Globalization;
using DormFlow.Application.Common.Abstractions;
using DormFlow.Application.Common.Authorization;
using DormFlow.Application.Common.Exceptions;
using DormFlow.Application.Features.Operations;
using DormFlow.Contracts.Operations;
using DormFlow.Domain.Common;
using DormFlow.Domain.Entities;
using DormFlow.Domain.Enums;
using DormFlow.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DormFlow.Infrastructure.Services;

public sealed class AnalyticsService(DormFlowDbContext db, ICurrentUser user, IDateTimeProvider clock) : IAnalyticsService
{
    private IQueryable<T> Scope<T>(AnalyticsRequest request) where T : TenantEntity
    {
        if (request.From > request.To || request.To.DayNumber - request.From.DayNumber > 3660) throw new ValidationFailedException("Choose a date range of up to ten years.");
        var org = TenantScope.GetOrganizationRestrictionForListing(user);
        var dorms = TenantScope.GetDormitoryRestrictionForListing(user);
        return db.Set<T>().AsNoTracking().Where(x => (user.IsSuperAdmin || x.OrganizationId == org) && (!request.DormitoryId.HasValue || x.DormitoryId == request.DormitoryId) && (dorms == null || (x.DormitoryId.HasValue && dorms.Contains(x.DormitoryId.Value))));
    }
    private IQueryable<Payment> Payments(AnalyticsRequest r)
    {
        var from = new DateTimeOffset(r.From.ToDateTime(TimeOnly.MinValue), TimeSpan.FromHours(7)).ToUniversalTime();
        var to = new DateTimeOffset(r.To.AddDays(1).ToDateTime(TimeOnly.MinValue), TimeSpan.FromHours(7)).ToUniversalTime();
        return Scope<Payment>(r).Where(p => (p.Status == PaymentStatus.Confirmed || p.Status == PaymentStatus.Refunded) && p.PaymentDate >= from && p.PaymentDate < to);
    }
    public async Task<AnalyticsResponse> GetAsync(AnalyticsRequest request, CancellationToken ct)
    {
        var payments = await Payments(request).Select(x => new { x.Amount, x.Method, x.PaymentDate, x.Status, x.RefundedAt }).ToListAsync(ct);
        var expenses = await Scope<Expense>(request).Where(x => x.ExpenseDate >= request.From && x.ExpenseDate <= request.To).Select(x => new { x.Amount, x.Category, x.ExpenseDate }).ToListAsync(ct);
        var today = DateOnly.FromDateTime(clock.UtcNow.ToOffset(TimeSpan.FromHours(7)).DateTime);
        var end = today.AddDays(30);
        decimal outstanding = await Scope<Invoice>(request).Where(i => i.Status != InvoiceStatus.Cancelled && i.Status != InvoiceStatus.Draft).SumAsync(i => i.TotalAmount - i.PaidAmount, ct);
        int outstandingCount = await Scope<Invoice>(request).CountAsync(i => i.Status != InvoiceStatus.Cancelled && i.Status != InvoiceStatus.Draft && i.TotalAmount > i.PaidAmount, ct);
        int expiring = await Scope<Contract>(request).CountAsync(c => c.Status == ContractStatus.Active && c.EndDate >= today && c.EndDate <= end, ct);
        var rooms = await Scope<Room>(request).GroupBy(r => r.Status).Select(g => new { g.Key, Count = g.Count() }).ToListAsync(ct);
        // Refunds are cash outflows on their refund date, not retroactive removal of revenue.
        var refundFrom = new DateTimeOffset(request.From.ToDateTime(TimeOnly.MinValue), TimeSpan.FromHours(7)).ToUniversalTime();
        var refundTo = new DateTimeOffset(request.To.AddDays(1).ToDateTime(TimeOnly.MinValue), TimeSpan.FromHours(7)).ToUniversalTime();
        var refunds = await Scope<Payment>(request).Where(p => p.Status == PaymentStatus.Refunded && p.RefundedAt >= refundFrom && p.RefundedAt < refundTo).Select(p => new { p.Amount, p.RefundedAt }).ToListAsync(ct);
        var revenue = payments.Sum(p => p.Amount);
        var expense = expenses.Sum(e => e.Amount) + refunds.Sum(p => p.Amount);
        List<ReportRow> monthlyRevenue = [], monthlyExpenses = [];
        for (var month = new DateOnly(request.From.Year, request.From.Month, 1); month <= request.To; month = month.AddMonths(1))
        {
            var key = month.ToString("yyyy-MM", CultureInfo.InvariantCulture);
            monthlyRevenue.Add(new(key, payments.Where(p => p.PaymentDate.ToOffset(TimeSpan.FromHours(7)).Year == month.Year && p.PaymentDate.ToOffset(TimeSpan.FromHours(7)).Month == month.Month).Sum(p => p.Amount)));
            monthlyExpenses.Add(new(key, expenses.Where(e => e.ExpenseDate.Year == month.Year && e.ExpenseDate.Month == month.Month).Sum(e => e.Amount) + refunds.Where(p => p.RefundedAt!.Value.ToOffset(TimeSpan.FromHours(7)).Year == month.Year && p.RefundedAt.Value.ToOffset(TimeSpan.FromHours(7)).Month == month.Month).Sum(p => p.Amount)));
        }
        return new(revenue, expense, revenue - expense, outstanding, expiring, monthlyRevenue, monthlyExpenses,
            payments.GroupBy(p => p.Method).Select(g => new ReportRow(g.Key.ToString(), g.Sum(p => p.Amount), g.Count())).ToArray(),
            rooms.Select(r => new ReportRow(r.Key.ToString(), r.Count, r.Count)).ToArray(),
            expenses.GroupBy(e => e.Category).Select(g => new ReportRow(g.Key.ToString(), g.Sum(e => e.Amount), g.Count())).ToArray(), outstandingCount);
    }
    public async Task<ReportTable> GetReportAsync(string report, AnalyticsRequest request, CancellationToken ct)
    {
        static string S(object? value) => Convert.ToString(value, CultureInfo.InvariantCulture) ?? string.Empty;
        switch (report)
        {
            case "revenue":
                var payments = await Payments(request).OrderByDescending(p => p.PaymentDate).Take(5000).ToListAsync(ct);
                return new(["Payment", "Invoice", "Date", "Method", "Amount", "Status"], payments.Select(p => (IReadOnlyList<string>)new[] { S(p.Id), S(p.InvoiceId), S(p.PaymentDate), S(p.Method), S(p.Amount), S(p.Status) }).ToArray());
            case "expenses":
                var expenses = await Scope<Expense>(request).Where(e => e.ExpenseDate >= request.From && e.ExpenseDate <= request.To).OrderByDescending(e => e.ExpenseDate).Take(5000).ToListAsync(ct);
                return new(["Date", "Category", "Description", "Amount", "Maintenance"], expenses.Select(e => (IReadOnlyList<string>)new[] { S(e.ExpenseDate), S(e.Category), e.Description, S(e.Amount), S(e.MaintenanceRequestId) }).ToArray());
            case "cash-flow":
                var summary = await GetAsync(request, ct);
                return new(["Month", "Revenue", "Outflows", "Net"], summary.MonthlyRevenue.Zip(summary.MonthlyExpenses, (r, e) => (IReadOnlyList<string>)new[] { r.Label, S(r.Amount), S(e.Amount), S(r.Amount - e.Amount) }).ToArray());
            case "outstanding":
                var invoices = await Scope<Invoice>(request).Where(i => i.Status != InvoiceStatus.Draft && i.Status != InvoiceStatus.Cancelled && i.TotalAmount > i.PaidAmount).OrderBy(i => i.DueDate).Take(5000).ToListAsync(ct);
                return new(["Invoice", "Due date", "Total", "Paid", "Outstanding"], invoices.Select(i => (IReadOnlyList<string>)new[] { i.InvoiceNumber, S(i.DueDate), S(i.TotalAmount), S(i.PaidAmount), S(i.TotalAmount - i.PaidAmount) }).ToArray());
            case "occupancy":
            case "room-performance":
                var rooms = await Scope<Room>(request).OrderBy(r => r.RoomNumber).Take(5000).ToListAsync(ct);
                var income = await (from p in Payments(request) join i in db.Invoices on p.InvoiceId equals i.Id join c in db.Contracts on i.ContractId equals c.Id group p by c.RoomId into g select new { RoomId = g.Key, Amount = g.Sum(p => p.Amount) }).ToDictionaryAsync(x => x.RoomId, x => x.Amount, ct);
                return new(["Room", "Status", "Monthly rent", "Collected in period"], rooms.Select(r => (IReadOnlyList<string>)new[] { r.RoomNumber, S(r.Status), S(r.MonthlyRent), S(income.GetValueOrDefault(r.Id)) }).ToArray());
            case "contracts":
            case "move-in-out":
                var contracts = await Scope<Contract>(request).Where(c => c.StartDate <= request.To && c.EndDate >= request.From).OrderByDescending(c => c.StartDate).Take(5000).ToListAsync(ct);
                return new(["Contract", "Room", "Tenant", "Start", "End", "Status", "Move in", "Move out"], contracts.Select(c => (IReadOnlyList<string>)new[] { c.ContractNumber, S(c.RoomId), S(c.TenantId), S(c.StartDate), S(c.EndDate), S(c.Status), S(c.SignedAt), S(c.TerminatedAt) }).ToArray());
            case "reservations":
                var reservations = await Scope<Reservation>(request).Where(x => DateOnly.FromDateTime(x.CreatedAt.UtcDateTime) >= request.From && DateOnly.FromDateTime(x.CreatedAt.UtcDateTime) <= request.To).OrderByDescending(x => x.CreatedAt).Take(5000).ToListAsync(ct);
                return new(["Reservation", "Room", "Tenant", "Status", "Created"], reservations.Select(r => (IReadOnlyList<string>)new[] { S(r.Id), S(r.RoomId), S(r.TenantId), S(r.Status), S(r.CreatedAt) }).ToArray());
            case "maintenance":
                var jobs = await Scope<MaintenanceRequest>(request).Where(x => DateOnly.FromDateTime(x.CreatedAt.UtcDateTime) >= request.From && DateOnly.FromDateTime(x.CreatedAt.UtcDateTime) <= request.To).OrderByDescending(x => x.CreatedAt).Take(5000).ToListAsync(ct);
                return new(["Job", "Room", "Category", "Priority", "Status", "Actual cost"], jobs.Select(j => (IReadOnlyList<string>)new[] { j.Title, S(j.RoomId), j.Category, S(j.Priority), S(j.Status), S(j.ActualCost) }).ToArray());
            case "parcels":
                var parcels = await Scope<Parcel>(request).Where(x => DateOnly.FromDateTime(x.CreatedAt.UtcDateTime) >= request.From && DateOnly.FromDateTime(x.CreatedAt.UtcDateTime) <= request.To).OrderByDescending(x => x.CreatedAt).Take(5000).ToListAsync(ct);
                return new(["Tracking", "Room", "Tenant", "Status", "Received"], parcels.Select(p => (IReadOnlyList<string>)new[] { p.TrackingNumber ?? "", S(p.RoomId), S(p.TenantId), S(p.Status), S(p.CreatedAt) }).ToArray());
            default: throw new NotFoundException("Report not found.");
        }
    }
}
