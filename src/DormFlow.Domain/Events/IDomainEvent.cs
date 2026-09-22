namespace DormFlow.Domain.Events;

/// <summary>
/// ເຫດການທາງທຸລະກິດທີ່ເກີດຂຶ້ນພາຍໃນ Domain
/// ມັນຈະຖືກເຜີຍແຜ່ພຽງແຕ່ຫຼັງຈາກຖານຂໍ້ມູນໄດ້ຖືກບັນທຶກໄວ້ສົບຜົນສໍາເລັດ
/// </summary>
public interface IDomainEvent
{
    /// <summary>ລະຫັດອ້າງອີງເຫດການແມ່ນໃຊ້ສໍາລັບ Idempotency ຂອງຜູ້ຮັບປາຍທາງ</summary>
    Guid EventId { get; }

    /// <summary>ເວລາທີ່ເຫດການເກີດຂຶ້ນ</summary>
    DateTimeOffset OccurredAt { get; }
}
