namespace DormFlow.Contracts.Reservations;

/// <summary>ข้อมูลการจองห้องพักหนึ่งรายการ</summary>
public class ReservationResponse
{
    /// <summary>รหัสอ้างอิงของการจอง</summary>
    public Guid Id { get; init; }

    /// <summary>องค์กรที่เกี่ยวข้อง</summary>
    public Guid OrganizationId { get; init; }

    /// <summary>หอพักที่เกี่ยวข้อง</summary>
    public Guid DormitoryId { get; init; }

    /// <summary>ห้องที่ถูกจอง</summary>
    public Guid RoomId { get; init; }

    /// <summary>ผู้เช่าที่จอง</summary>
    public Guid TenantId { get; init; }

    /// <summary>วันที่ทำการจอง</summary>
    public DateTimeOffset ReservationDate { get; init; }

    /// <summary>วันที่คาดว่าจะเข้าพัก</summary>
    public DateOnly MoveInDate { get; init; }

    /// <summary>เวลาที่การกันห้องจะหมดอายุ</summary>
    public DateTimeOffset ExpiresAt { get; init; }

    /// <summary>ค่าจองห้อง</summary>
    public decimal ReservationFee { get; init; }

    /// <summary>สถานะของการจอง</summary>
    public string Status { get; init; } = string.Empty;

    /// <summary>หมายเหตุเพิ่มเติม</summary>
    public string? Notes { get; init; }

    /// <summary>เวลาที่สร้างข้อมูล</summary>
    public DateTimeOffset CreatedAt { get; init; }
}
