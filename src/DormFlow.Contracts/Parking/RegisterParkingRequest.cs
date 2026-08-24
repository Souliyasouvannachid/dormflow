namespace DormFlow.Contracts.Parking;

/// <summary>คำขอจดทะเบียนที่จอดรถใหม่</summary>
public class RegisterParkingRequest
{
    /// <summary>ห้องของผู้เช่าเจ้าของรถ</summary>
    public Guid RoomId { get; set; }

    /// <summary>ผู้เช่าเจ้าของรถ</summary>
    public Guid TenantId { get; set; }

    /// <summary>ทะเบียนรถ</summary>
    public string LicensePlate { get; set; } = string.Empty;

    /// <summary>ประเภทของยานพาหนะ ("Car", "Motorcycle", "Other")</summary>
    public string VehicleType { get; set; } = string.Empty;

    /// <summary>ยี่ห้อ/รุ่นรถ</summary>
    public string? Brand { get; set; }

    /// <summary>สีรถ</summary>
    public string? Color { get; set; }

    /// <summary>ป้ายช่องจอดรถ</summary>
    public string? ParkingSpotLabel { get; set; }

    /// <summary>หมายเหตุเพิ่มเติม</summary>
    public string? Notes { get; set; }
}
