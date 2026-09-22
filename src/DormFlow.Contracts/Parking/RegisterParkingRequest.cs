namespace DormFlow.Contracts.Parking;

/// <summary>ຮ້ອງຂໍການລົງທະບຽນບ່ອນຈອດລົດໃຝ່</summary>
public class RegisterParkingRequest
{
    /// <summary>ຫ້ອງເຈົ້າຂອງລົດ</summary>
    public Guid RoomId { get; set; }

    /// <summary>ເຈົ້າຂອງລົດເຊົ່າ</summary>
    public Guid TenantId { get; set; }

    /// <summary>ທະບຽນລົດ</summary>
    public string LicensePlate { get; set; } = string.Empty;

    /// <summary>ປະເພດຍານພາຫະນະ ("Car", "Motorcycle", "Other")</summary>
    public string VehicleType { get; set; } = string.Empty;

    /// <summary>ຍີ່ຫໍ້/ຕົວແບບລົດ</summary>
    public string? Brand { get; set; }

    /// <summary>ສີລົດ</summary>
    public string? Color { get; set; }

    /// <summary>ປ້າຍບ່ອນຈອດລົດ</summary>
    public string? ParkingSpotLabel { get; set; }

    /// <summary>ບັນທຶກເພີ່ມເຕີມ</summary>
    public string? Notes { get; set; }
}
