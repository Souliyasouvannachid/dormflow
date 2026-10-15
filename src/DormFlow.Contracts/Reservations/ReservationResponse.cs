namespace DormFlow.Contracts.Reservations;

/// <summary>ຂໍ້ມູນການຈອງຜຶ່ງ</summary>
public class ReservationResponse
{
    public DateTimeOffset? ConfirmedAt { get; init; }
    /// <summary>ລະຫັດອ້າງອີງການຈອງ</summary>
    public Guid Id { get; init; }

    /// <summary>ອົງກອນທີ່ກ່ຽວຂ້ອງ</summary>
    public Guid OrganizationId { get; init; }

    /// <summary>ຫໍພັກທີ່ກ່ຽວຂ້ອງ</summary>
    public Guid DormitoryId { get; init; }

    /// <summary>ຫ້ອງສະຫງວນ</summary>
    public Guid RoomId { get; init; }

    /// <summary>ຜູ້ເຊົ່າທີ່ຈອງແລ້ວ</summary>
    public Guid TenantId { get; init; }

    /// <summary>ວັນທີຈອງ</summary>
    public DateTimeOffset ReservationDate { get; init; }

    /// <summary>ວັນທີທີ່ຄາດວ່າຈະຢູ່</summary>
    public DateOnly MoveInDate { get; init; }

    /// <summary>ໃນເວລາທີ່ຕັນຫ້ອງຈະຝົດອາຍຸ</summary>
    public DateTimeOffset ExpiresAt { get; init; }

    /// <summary>ຄ່າຈອງຫ້ອງ</summary>
    public decimal ReservationFee { get; init; }

    /// <summary>ສະຖານະການຈອງ</summary>
    public string Status { get; init; } = string.Empty;

    /// <summary>ບັນທຶກເພີ່ມເຕີມ</summary>
    public string? Notes { get; init; }

    /// <summary>ເວລາສ້າງຂໍ້ມູນ</summary>
    public DateTimeOffset CreatedAt { get; init; }
}
