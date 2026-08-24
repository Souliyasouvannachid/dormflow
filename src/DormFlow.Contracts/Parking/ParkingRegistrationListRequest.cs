using DormFlow.Contracts.Common;

namespace DormFlow.Contracts.Parking;

/// <summary>พารามิเตอร์การค้นหารายการจดทะเบียนที่จอดรถ</summary>
public class ParkingRegistrationListRequest : PagedQueryRequest
{
    /// <summary>กรองเฉพาะรายการของหอพักที่ระบุ</summary>
    public Guid? DormitoryId { get; set; }

    /// <summary>กรองเฉพาะรายการของผู้เช่าที่ระบุ</summary>
    public Guid? TenantId { get; set; }

    /// <summary>กรองเฉพาะรายการที่มีสถานะตรงกับที่ระบุ</summary>
    public string? Status { get; set; }
}
