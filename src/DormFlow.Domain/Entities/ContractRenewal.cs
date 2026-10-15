using DormFlow.Domain.Common;
namespace DormFlow.Domain.Entities;

public class ContractRenewal : TenantEntity
{
    public Guid ContractId { get; set; }
    public DateOnly PreviousEndDate { get; set; }
    public DateOnly NewEndDate { get; set; }
    public decimal PreviousRent { get; set; }
    public decimal NewRent { get; set; }
}
