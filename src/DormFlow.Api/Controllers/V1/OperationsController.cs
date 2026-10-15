using System.Text;
using Asp.Versioning;
using DormFlow.Api.Authorization;
using DormFlow.Application.Common.Authorization;
using DormFlow.Application.Features.Operations;
using DormFlow.Contracts.Common;
using DormFlow.Contracts.Operations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DormFlow.Api.Controllers.V1;

[ApiController, ApiVersion("1.0"), Authorize]
[Route("api/v{version:apiVersion}")]
public sealed class OperationsController(IOperationsService operations, IAnalyticsService analytics) : ControllerBase
{
    [HttpPost("reservations/{id:guid}/confirm"), RequirePermission(PermissionCodes.Reservations.Create)]
    public async Task<IActionResult> ConfirmReservation(Guid id, CancellationToken ct) { await operations.ConfirmReservationAsync(id, ct); return Ok(ApiResponse.Ok()); }
    [HttpPost("contracts/{id:guid}/renew"), RequirePermission(PermissionCodes.Contracts.Create)]
    public async Task<IActionResult> Renew(Guid id, RenewLeaseRequest request, CancellationToken ct) { await operations.RenewAsync(id, request, ct); return Ok(ApiResponse.Ok()); }
    [HttpGet("contracts/{id:guid}/renewals"), RequirePermission(PermissionCodes.Contracts.View)]
    public async Task<IActionResult> Renewals(Guid id, CancellationToken ct) => Ok(ApiResponse.Ok(await operations.GetRenewalsAsync(id, ct)));
    [HttpGet("tenants/{id:guid}/history"), RequirePermission(PermissionCodes.Contracts.View)]
    public async Task<IActionResult> History(Guid id, CancellationToken ct) => Ok(ApiResponse.Ok(await operations.GetTenantHistoryAsync(id, ct)));
    [HttpGet("billing-periods"), RequirePermission(PermissionCodes.Invoices.View)]
    public async Task<IActionResult> Periods(CancellationToken ct) => Ok(ApiResponse.Ok(await operations.GetPeriodsAsync(ct)));
    [HttpPost("billing-periods"), RequirePermission(PermissionCodes.Invoices.Create)]
    public async Task<IActionResult> Open(OpenBillingPeriodRequest request, CancellationToken ct) => Ok(ApiResponse.Ok(await operations.OpenPeriodAsync(request, ct)));
    [HttpPost("billing-periods/{id:guid}/generate"), RequirePermission(PermissionCodes.Invoices.Create)]
    public async Task<IActionResult> Generate(Guid id, CancellationToken ct) => Ok(ApiResponse.Ok(new { Generated = await operations.GeneratePeriodAsync(id, ct) }));
    [HttpPost("billing-periods/{id:guid}/close"), RequirePermission(PermissionCodes.Invoices.Issue)]
    public async Task<IActionResult> Close(Guid id, CancellationToken ct) { await operations.ClosePeriodAsync(id, ct); return Ok(ApiResponse.Ok()); }
    [HttpGet("invoices/{id:guid}/charges"), RequirePermission(PermissionCodes.Invoices.View)]
    public async Task<IActionResult> Charges(Guid id, CancellationToken ct) => Ok(ApiResponse.Ok(await operations.GetChargesAsync(id, ct)));
    [HttpPost("invoices/{id:guid}/charges"), RequirePermission(PermissionCodes.Invoices.Create)]
    public async Task<IActionResult> Charge(Guid id, InvoiceChargeRequest request, CancellationToken ct) { await operations.AddChargeAsync(id, request, ct); return Ok(ApiResponse.Ok()); }
    [HttpPost("payments/allocate"), RequirePermission(PermissionCodes.Payments.Create), RequirePermission(PermissionCodes.Payments.Approve)]
    public async Task<IActionResult> Allocate(AllocatePaymentRequest request, CancellationToken ct) => Ok(ApiResponse.Ok(await operations.AllocateAsync(request, ct)));
    [HttpPost("parcels/{id:guid}/return"), RequirePermission(PermissionCodes.Parcels.Pickup)]
    public async Task<IActionResult> Return(Guid id, CancellationToken ct) { await operations.ReturnParcelAsync(id, ct); return Ok(ApiResponse.Ok()); }
    [HttpPost("announcements/{id:guid}/cancel"), RequirePermission(PermissionCodes.Announcements.Publish)]
    public async Task<IActionResult> Cancel(Guid id, CancellationToken ct) { await operations.CancelAnnouncementAsync(id, ct); return Ok(ApiResponse.Ok()); }
    [HttpGet("maintenance-requests/{id:guid}/timeline"), RequirePermission(PermissionCodes.Maintenance.View)]
    public async Task<IActionResult> Timeline(Guid id, CancellationToken ct) => Ok(ApiResponse.Ok(await operations.GetTimelineAsync(id, ct)));
    [HttpPost("maintenance-requests/{id:guid}/timeline"), RequirePermission(PermissionCodes.Maintenance.UpdateStatus)]
    public async Task<IActionResult> Note(Guid id, TimelineNoteRequest request, CancellationToken ct) { await operations.AddTimelineAsync(id, request, ct); return Ok(ApiResponse.Ok()); }
    [HttpPost("maintenance-requests/{id:guid}/costs"), RequirePermission(PermissionCodes.Expenses.Create)]
    public async Task<IActionResult> Cost(Guid id, MaintenanceCostRequest request, CancellationToken ct) { await operations.RecordCostAsync(id, request, ct); return Ok(ApiResponse.Ok()); }
    [HttpGet("notifications/unread-count")]
    public async Task<IActionResult> Unread(CancellationToken ct) => Ok(ApiResponse.Ok(await operations.GetUnreadAsync(ct)));
    [HttpPost("notifications/read-all")]
    public async Task<IActionResult> ReadAll(CancellationToken ct) => Ok(ApiResponse.Ok(new { Updated = await operations.ReadAllAsync(ct) }));
    [HttpGet("analytics"), RequirePermission(PermissionCodes.Reports.View)]
    public async Task<IActionResult> Analytics([FromQuery] AnalyticsRequest request, CancellationToken ct) => Ok(ApiResponse.Ok(await analytics.GetAsync(request, ct)));
    [HttpGet("reports/detail/{report}"), RequirePermission(PermissionCodes.Reports.View)]
    public async Task<IActionResult> Report(string report, [FromQuery] AnalyticsRequest request, CancellationToken ct) => Ok(ApiResponse.Ok(await analytics.GetReportAsync(report, request, ct)));
    [HttpGet("reports/detail/{report}/export"), RequirePermission(PermissionCodes.Reports.Export)]
    public async Task<IActionResult> Export(string report, [FromQuery] AnalyticsRequest request, CancellationToken ct)
    {
        var table = await analytics.GetReportAsync(report, request, ct);
        static string Escape(string cell) { if (cell.StartsWith('=') || cell.StartsWith('+') || cell.StartsWith('-') || cell.StartsWith('@')) cell = "'" + cell; return "\"" + cell.Replace("\"", "\"\"", StringComparison.Ordinal) + "\""; }
        var lines = new[] { table.Columns }.Concat(table.Rows).Select(row => string.Join(',', row.Select(Escape)));
        return File(Encoding.UTF8.GetPreamble().Concat(Encoding.UTF8.GetBytes(string.Join("\r\n", lines))).ToArray(), "text/csv; charset=utf-8", $"{report}.csv");
    }
}
