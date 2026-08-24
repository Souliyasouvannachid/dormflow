using DormFlow.Contracts.Common;

namespace DormFlow.Contracts.Reservations;

/// <summary>พารามิเตอร์การค้นหารายการจอง</summary>
public class ReservationListRequest : PagedQueryRequest
{
    /// <summary>กรองเฉพาะการจองของหอพักที่ระบุ</summary>
    public Guid? DormitoryId { get; set; }

    /// <summary>กรองเฉพาะการจองของห้องที่ระบุ</summary>
    public Guid? RoomId { get; set; }

    /// <summary>กรองเฉพาะการจองของผู้เช่าที่ระบุ</summary>
    public Guid? TenantId { get; set; }

    /// <summary>กรองเฉพาะการจองที่มีสถานะตรงกับที่ระบุ</summary>
    public string? Status { get; set; }
}
