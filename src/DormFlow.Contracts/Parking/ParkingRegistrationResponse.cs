namespace DormFlow.Contracts.Parking;

/// <summary>ຂໍ້ມູນການລົງທະບຽນບ່ອນຈອດລົດ</summary>
public class ParkingRegistrationResponse
{
    /// <summary>ລະຫັດອ້າງອີງລາຍການ</summary>
    public Guid Id { get; init; }

    /// <summary>ອົງກອນທີ່ກ່ຽວຂ້ອງ</summary>
    public Guid OrganizationId { get; init; }

    /// <summary>ຫໍພັກທີ່ກ່ຽວຂ້ອງ</summary>
    public Guid DormitoryId { get; init; }

    /// <summary>ຫ້ອງເຈົ້າຂອງລົດ</summary>
    public Guid RoomId { get; init; }

    /// <summary>ເຈົ້າຂອງລົດເຊົ່າ</summary>
    public Guid TenantId { get; init; }

    /// <summary>ທະບຽນລົດ</summary>
    public string LicensePlate { get; init; } = string.Empty;

    /// <summary>ປະເພດຍານພາຫະນະ</summary>
    public string VehicleType { get; init; } = string.Empty;

    /// <summary>ຍີ່ຫໍ້/ຕົວແບບລົດ</summary>
    public string? Brand { get; init; }

    /// <summary>ສີລົດ</summary>
    public string? Color { get; init; }

    /// <summary>ປ້າຍບ່ອນຈອດລົດ</summary>
    public string? ParkingSpotLabel { get; init; }

    /// <summary>ສະຖານະການລົງທະບຽນ</summary>
    public string Status { get; init; } = string.Empty;

    /// <summary>ເວລາລົງທະບຽນ</summary>
    public DateTimeOffset RegisteredAt { get; init; }

    /// <summary>ເວລາສິ້ນສຸດການລົງທະບຽນ</summary>
    public DateTimeOffset? EndedAt { get; init; }

    /// <summary>ບັນທຶກເພີ່ມເຕີມ</summary>
    public string? Notes { get; init; }

    /// <summary>ເວລາສ້າງຂໍ້ມູນ</summary>
    public DateTimeOffset CreatedAt { get; init; }
}
