using DormFlow.Domain.Events;

namespace DormFlow.Domain.Common;

/// <summary>
/// ຫ້ອງຮຽນພື້ນຖານຂອງທຸກໆ Entity ໃນລະບົບ DormFlow
/// ຂໍ້ມູນລວມ Audit, Soft Delete ແລະ Concurrency Token ຢູ່ໃນສະຖານທີ່ດຽວ
/// </summary>
/// <remarks>
/// Primary Key ໃຊ້ UUID v7 ເພື່ອໃຫ້ສາມາດຈັດຕາມທີ່ໃຊ້ເວລາຫຼຸດຜ່ອນການ fragmentation ຂອງ B-tree index ສຸດ PostgreSQL
/// </remarks>
public abstract class BaseEntity : IEntity, IAuditableEntity, ISoftDeletable
{
    private readonly List<IDomainEvent> _domainEvents = [];

    /// <inheritdoc />
    public Guid Id { get; protected set; } = Guid.CreateVersion7();

    /// <inheritdoc />
    public DateTimeOffset CreatedAt { get; set; }

    /// <inheritdoc />
    public Guid? CreatedBy { get; set; }

    /// <inheritdoc />
    public DateTimeOffset? UpdatedAt { get; set; }

    /// <inheritdoc />
    public Guid? UpdatedBy { get; set; }

    /// <inheritdoc />
    public bool IsDeleted { get; set; }

    /// <inheritdoc />
    public DateTimeOffset? DeletedAt { get; set; }

    /// <inheritdoc />
    public Guid? DeletedBy { get; set; }

    /// <summary>
    /// Concurrency Token ສໍາລັບ Optimistic Concurrency
    /// EF Core ມູນຄ່ານີ້ຈະເພີ່ມຂຶ້ນທຸກຄັ້ງທີ່ມັນຖືກບັນທຶກແລະຖິ້ມ DbUpdateConcurrencyException ເມື່ອຄ່າບໍ່ກົງກັນ
    /// </summary>
    public uint Version { get; set; }

    /// <summary>ລາຍການ Domain Event ທີ່ຍັງບໍ່ທັນໄດ້ເຜີຍແຜ່</summary>
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    /// <summary>ເພີ່ມ Domain Event ຄິວ ແລະລໍຖ້າເຜີຍແຜ່</summary>
    /// <param name="domainEvent">ເຫດການທີ່ຈະເຜີຍແຜ່</param>
    public void RaiseDomainEvent(IDomainEvent domainEvent)
    {
        ArgumentNullException.ThrowIfNull(domainEvent);
        _domainEvents.Add(domainEvent);
    }

    /// <summary>ລ້າງຄິວ Domain Event ເອີ້ນວ່າຫຼັງຈາກການພິມເຜີຍແຜ່ສົບຜົນສໍາເລັດ</summary>
    public void ClearDomainEvents() => _domainEvents.Clear();
}
