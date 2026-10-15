using DormFlow.Contracts.Operations;
namespace DormFlow.Application.Features.Operations;

public interface IOperationsService
{
    Task ConfirmReservationAsync(Guid id, CancellationToken ct);
    Task RenewAsync(Guid id, RenewLeaseRequest request, CancellationToken ct);
    Task<IReadOnlyList<RenewalResponse>> GetRenewalsAsync(Guid id, CancellationToken ct);
    Task<IReadOnlyList<HistoryResponse>> GetTenantHistoryAsync(Guid id, CancellationToken ct);
    Task<IReadOnlyList<BillingPeriodResponse>> GetPeriodsAsync(CancellationToken ct);
    Task<BillingPeriodResponse> OpenPeriodAsync(OpenBillingPeriodRequest request, CancellationToken ct);
    Task<int> GeneratePeriodAsync(Guid id, CancellationToken ct);
    Task ClosePeriodAsync(Guid id, CancellationToken ct);
    Task AddChargeAsync(Guid id, InvoiceChargeRequest request, CancellationToken ct);
    Task<IReadOnlyList<ChargeResponse>> GetChargesAsync(Guid id, CancellationToken ct);
    Task<PaymentAllocationResult> AllocateAsync(AllocatePaymentRequest request, CancellationToken ct);
    Task ReturnParcelAsync(Guid id, CancellationToken ct);
    Task CancelAnnouncementAsync(Guid id, CancellationToken ct);
    Task<IReadOnlyList<TimelineResponse>> GetTimelineAsync(Guid id, CancellationToken ct);
    Task AddTimelineAsync(Guid id, TimelineNoteRequest request, CancellationToken ct);
    Task RecordCostAsync(Guid id, MaintenanceCostRequest request, CancellationToken ct);
    Task<NotificationCountResponse> GetUnreadAsync(CancellationToken ct);
    Task<int> ReadAllAsync(CancellationToken ct);
}

public interface IAnalyticsService
{
    Task<AnalyticsResponse> GetAsync(AnalyticsRequest request, CancellationToken ct);
    Task<ReportTable> GetReportAsync(string report, AnalyticsRequest request, CancellationToken ct);
}
