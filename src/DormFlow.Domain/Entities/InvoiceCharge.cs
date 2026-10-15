using DormFlow.Domain.Common;
namespace DormFlow.Domain.Entities;

public class InvoiceCharge : TenantEntity
{
    public Guid InvoiceId { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
}
