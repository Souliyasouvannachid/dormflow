using DormFlow.Domain.Events;

namespace DormFlow.Domain.Common;

/// <summary>
/// คลาสฐานของทุก Entity ในระบบ DormFlow
/// รวมข้อมูล Audit, Soft Delete และ Concurrency Token ไว้ที่เดียว
/// </summary>
/// <remarks>
/// Primary Key ใช้ UUID v7 เพื่อให้เรียงตามเวลาได้ ลดการแตกกระจายของ B-tree index บน PostgreSQL
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
    /// Concurrency Token สำหรับ Optimistic Concurrency
    /// EF Core จะเพิ่มค่านี้ทุกครั้งที่บันทึก และโยน DbUpdateConcurrencyException เมื่อค่าไม่ตรง
    /// </summary>
    public uint Version { get; set; }

    /// <summary>รายการ Domain Event ที่ยังไม่ถูกเผยแพร่</summary>
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    /// <summary>เพิ่ม Domain Event เข้าคิวรอเผยแพร่</summary>
    /// <param name="domainEvent">เหตุการณ์ที่ต้องการเผยแพร่</param>
    public void RaiseDomainEvent(IDomainEvent domainEvent)
    {
        ArgumentNullException.ThrowIfNull(domainEvent);
        _domainEvents.Add(domainEvent);
    }

    /// <summary>ล้างคิว Domain Event เรียกหลังเผยแพร่สำเร็จแล้ว</summary>
    public void ClearDomainEvents() => _domainEvents.Clear();
}
