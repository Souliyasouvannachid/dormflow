using DormFlow.Contracts.Common;

namespace DormFlow.Contracts.Invoices;

/// <summary>พารามิเตอร์การค้นหารายการใบแจ้งหนี้</summary>
public class InvoiceListRequest : PagedQueryRequest
{
    /// <summary>กรองเฉพาะใบแจ้งหนี้ของหอพักที่ระบุ</summary>
    public Guid? DormitoryId { get; set; }

    /// <summary>กรองเฉพาะใบแจ้งหนี้ของสัญญาเช่าที่ระบุ</summary>
    public Guid? ContractId { get; set; }

    /// <summary>กรองเฉพาะใบแจ้งหนี้ที่มีสถานะตรงกับที่ระบุ</summary>
    public string? Status { get; set; }

    /// <summary>กรองเฉพาะใบแจ้งหนี้ของเดือนที่ระบุ</summary>
    public DateOnly? PeriodMonth { get; set; }
}
