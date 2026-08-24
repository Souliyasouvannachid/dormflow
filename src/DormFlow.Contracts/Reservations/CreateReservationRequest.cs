namespace DormFlow.Contracts.Reservations;

/// <summary>คำขอจองห้องพัก</summary>
public class CreateReservationRequest
{
    /// <summary>ห้องที่ต้องการจอง ต้องอยู่ในสถานะว่างเท่านั้น</summary>
    public Guid RoomId { get; set; }

    /// <summary>ผู้เช่าที่จอง</summary>
    public Guid TenantId { get; set; }

    /// <summary>วันที่คาดว่าจะเข้าพัก</summary>
    public DateOnly MoveInDate { get; set; }

    /// <summary>จำนวนวันที่กันห้องไว้ก่อนหมดอายุ นับจากเวลาที่จอง</summary>
    public int HoldDays { get; set; } = 3;

    /// <summary>ค่าจองห้อง</summary>
    public decimal ReservationFee { get; set; }

    /// <summary>หมายเหตุเพิ่มเติม</summary>
    public string? Notes { get; set; }
}
