namespace DormFlow.Domain.Events;

/// <summary>
/// ຊັ້ນພື້ນຖານຂອງ Domain Event ຕັ້ງ <see cref="EventId"/> ແລະ <see cref="OccurredAt"/> ອັດຕະໂນມັດ
/// </summary>
public abstract record DomainEvent : IDomainEvent
{
    /// <inheritdoc />
    public Guid EventId { get; init; } = Guid.CreateVersion7();

    /// <inheritdoc />
    public DateTimeOffset OccurredAt { get; init; } = DateTimeOffset.UtcNow;
}
