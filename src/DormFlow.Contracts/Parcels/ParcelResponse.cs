namespace DormFlow.Contracts.Parcels;

/// <summary>ข้อมูลพัสดุหนึ่งรายการ</summary>
public class ParcelResponse
{
    /// <summary>รหัสอ้างอิงของพัสดุ</summary>
    public Guid Id { get; init; }

    /// <summary>องค์กรที่เกี่ยวข้อง</summary>
    public Guid OrganizationId { get; init; }

    /// <summary>หอพักที่เกี่ยวข้อง</summary>
    public Guid DormitoryId { get; init; }

    /// <summary>ห้องที่พัสดุนี้ส่งถึง</summary>
    public Guid RoomId { get; init; }

    /// <summary>ผู้เช่าที่พัสดุนี้ส่งถึง</summary>
    public Guid? TenantId { get; init; }

    /// <summary>บริษัทขนส่ง</summary>
    public string? CourierName { get; init; }

    /// <summary>เลขพัสดุ</summary>
    public string? TrackingNumber { get; init; }

    /// <summary>คำอธิบายพัสดุเพิ่มเติม</summary>
    public string? Description { get; init; }

    /// <summary>เวลาที่รับพัสดุไว้ที่หอพัก</summary>
    public DateTimeOffset ReceivedAt { get; init; }

    /// <summary>สถานะของพัสดุ</summary>
    public string Status { get; init; } = string.Empty;

    /// <summary>เวลาที่ผู้เช่ามารับพัสดุ</summary>
    public DateTimeOffset? PickedUpAt { get; init; }

    /// <summary>ชื่อผู้มารับพัสดุจริง</summary>
    public string? PickedUpByName { get; init; }

    /// <summary>หมายเหตุเพิ่มเติม</summary>
    public string? Notes { get; init; }

    /// <summary>เวลาที่สร้างข้อมูล</summary>
    public DateTimeOffset CreatedAt { get; init; }
}
