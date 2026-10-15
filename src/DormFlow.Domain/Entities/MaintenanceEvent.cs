using DormFlow.Domain.Common;
namespace DormFlow.Domain.Entities;

public class MaintenanceEvent : TenantEntity
{
    public Guid MaintenanceRequestId { get; set; }
    public string Status { get; set; } = string.Empty;
    public string Note { get; set; } = string.Empty;
}
