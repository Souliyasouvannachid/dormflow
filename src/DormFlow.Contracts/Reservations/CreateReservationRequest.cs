namespace DormFlow.Contracts.Reservations;

/// <summary>ຄຳຮ້ອງຂໍຈອງ</summary>
public class CreateReservationRequest
{
    /// <summary>ຫ້ອງທີ່ເຈົ້າຕ້ອງການຈອງຕ້ອງຢູ່ໃນສະຖານະບໍ່ເຮັດວຽກເທົ່ານັ້ນ</summary>
    public Guid RoomId { get; set; }

    /// <summary>ຜູ້ເຊົ່າທີ່ຈອງແລ້ວ</summary>
    public Guid TenantId { get; set; }

    /// <summary>ວັນທີທີ່ຄາດວ່າຈະຢູ່</summary>
    public DateOnly MoveInDate { get; set; }

    /// <summary>ຈຳນວນມື້ທີ່ຫ້ອງຖືກຈອງໄວ້ກ່ອນທີ່ມັນຈະໝົດອາຍຸ. ຈາກເວລາຂອງການຈອງ</summary>
    public int HoldDays { get; set; } = 3;

    /// <summary>ຄ່າຈອງຫ້ອງ</summary>
    public decimal ReservationFee { get; set; }

    /// <summary>ບັນທຶກເພີ່ມເຕີມ</summary>
    public string? Notes { get; set; }
}
