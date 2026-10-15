using DormFlow.Domain.Common;
namespace DormFlow.Domain.Entities;

public class BillingPeriod : TenantEntity
{
    public DateOnly Month { get; set; }
    public bool IsClosed { get; set; }
    public DateTimeOffset? ClosedAt { get; set; }
}
