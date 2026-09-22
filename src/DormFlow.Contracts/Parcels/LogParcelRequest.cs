namespace DormFlow.Contracts.Parcels;

/// <summary>ການຮ້ອງຂໍເພື່ອບັນທຶກການ parcel ທີ່ມາເຖິງ</summary>
public class LogParcelRequest
{
    /// <summary>ຫ້ອງຊຸດນີ້ແມ່ນສົ່ງໃຫ້</summary>
    public Guid RoomId { get; set; }

    /// <summary>ຜູ້ເຊົ່າທີ່ parcel ນີ້ຖືກຈັດສົ່ງແມ່ນ null ໃນເວລາທີ່ຍັງບໍ່ໄດ້ຮັບການຢືນຢັນວ່າໃຜເປັນຂອງຫ້ອງການ</summary>
    public Guid? TenantId { get; set; }

    /// <summary>ບໍລິສັດຂົນສົ່ງ</summary>
    public string? CourierName { get; set; }

    /// <summary>ໝາຍເລກເຄື່ອງໝາຍ</summary>
    public string? TrackingNumber { get; set; }

    /// <summary>ລາຍລະອຽດການພັດທະນາເພີ່ມເຕີມ</summary>
    public string? Description { get; set; }

    /// <summary>ບັນທຶກເພີ່ມເຕີມ</summary>
    public string? Notes { get; set; }
}
