namespace DormFlow.Domain.Events;

/// <summary>
/// คลาสฐานของ Domain Event กำหนด <see cref="EventId"/> และ <see cref="OccurredAt"/> ให้อัตโนมัติ
/// </summary>
public abstract record DomainEvent : IDomainEvent
{
    /// <inheritdoc />
    public Guid EventId { get; init; } = Guid.CreateVersion7();

    /// <inheritdoc />
    public DateTimeOffset OccurredAt { get; init; } = DateTimeOffset.UtcNow;
}
