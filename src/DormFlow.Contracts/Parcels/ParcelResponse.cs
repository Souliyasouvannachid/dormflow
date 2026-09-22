namespace DormFlow.Contracts.Parcels;

/// <summary>ຂໍ້ມູນຜຶ່ງ parcel</summary>
public class ParcelResponse
{
    /// <summary>ລະຫັດອ້າງອິງ parcel</summary>
    public Guid Id { get; init; }

    /// <summary>ອົງກອນທີ່ກ່ຽວຂ້ອງ</summary>
    public Guid OrganizationId { get; init; }

    /// <summary>ຫໍພັກທີ່ກ່ຽວຂ້ອງ</summary>
    public Guid DormitoryId { get; init; }

    /// <summary>ຫ້ອງຊຸດນີ້ແມ່ນສົ່ງໃຫ້</summary>
    public Guid RoomId { get; init; }

    /// <summary>ຜູ້ເຊົ່າທີ່ parcel ນີ້ຖືກແກ້ໄຂ</summary>
    public Guid? TenantId { get; init; }

    /// <summary>ບໍລິສັດຂົນສົ່ງ</summary>
    public string? CourierName { get; init; }

    /// <summary>ໝາຍເລກເຄື່ອງໝາຍ</summary>
    public string? TrackingNumber { get; init; }

    /// <summary>ລາຍລະອຽດການພັດທະນາເພີ່ມເຕີມ</summary>
    public string? Description { get; init; }

    /// <summary>ເວລາໄປຮັບເຄື່ອງຢູ່ຫໍພັກ</summary>
    public DateTimeOffset ReceivedAt { get; init; }

    /// <summary>ສະຖານະການຫຸ້ມຫໍ່</summary>
    public string Status { get; init; } = string.Empty;

    /// <summary>ເວລາຜູ້ເຊົ່າມາຮັບຊຸດ</summary>
    public DateTimeOffset? PickedUpAt { get; init; }

    /// <summary>ຊື່ແທ້ຂອງຜູ້ທີ່ໄດ້ຮັບຊຸດ</summary>
    public string? PickedUpByName { get; init; }

    /// <summary>ບັນທຶກເພີ່ມເຕີມ</summary>
    public string? Notes { get; init; }

    /// <summary>ເວລາສ້າງຂໍ້ມູນ</summary>
    public DateTimeOffset CreatedAt { get; init; }
}
