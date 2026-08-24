namespace DormFlow.Contracts.Parking;

/// <summary>ข้อมูลการจดทะเบียนที่จอดรถหนึ่งรายการ</summary>
public class ParkingRegistrationResponse
{
    /// <summary>รหัสอ้างอิงของรายการ</summary>
    public Guid Id { get; init; }

    /// <summary>องค์กรที่เกี่ยวข้อง</summary>
    public Guid OrganizationId { get; init; }

    /// <summary>หอพักที่เกี่ยวข้อง</summary>
    public Guid DormitoryId { get; init; }

    /// <summary>ห้องของผู้เช่าเจ้าของรถ</summary>
    public Guid RoomId { get; init; }

    /// <summary>ผู้เช่าเจ้าของรถ</summary>
    public Guid TenantId { get; init; }

    /// <summary>ทะเบียนรถ</summary>
    public string LicensePlate { get; init; } = string.Empty;

    /// <summary>ประเภทของยานพาหนะ</summary>
    public string VehicleType { get; init; } = string.Empty;

    /// <summary>ยี่ห้อ/รุ่นรถ</summary>
    public string? Brand { get; init; }

    /// <summary>สีรถ</summary>
    public string? Color { get; init; }

    /// <summary>ป้ายช่องจอดรถ</summary>
    public string? ParkingSpotLabel { get; init; }

    /// <summary>สถานะของการจดทะเบียน</summary>
    public string Status { get; init; } = string.Empty;

    /// <summary>เวลาที่จดทะเบียน</summary>
    public DateTimeOffset RegisteredAt { get; init; }

    /// <summary>เวลาที่สิ้นสุดการจดทะเบียน</summary>
    public DateTimeOffset? EndedAt { get; init; }

    /// <summary>หมายเหตุเพิ่มเติม</summary>
    public string? Notes { get; init; }

    /// <summary>เวลาที่สร้างข้อมูล</summary>
    public DateTimeOffset CreatedAt { get; init; }
}
