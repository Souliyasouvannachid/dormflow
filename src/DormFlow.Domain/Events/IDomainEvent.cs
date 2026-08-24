namespace DormFlow.Domain.Events;

/// <summary>
/// เหตุการณ์เชิงธุรกิจที่เกิดขึ้นภายใน Domain
/// จะถูกเผยแพร่หลังจากบันทึกฐานข้อมูลสำเร็จเท่านั้น
/// </summary>
public interface IDomainEvent
{
    /// <summary>รหัสอ้างอิงของเหตุการณ์ ใช้สำหรับ Idempotency ของผู้รับปลายทาง</summary>
    Guid EventId { get; }

    /// <summary>เวลาที่เหตุการณ์เกิดขึ้น</summary>
    DateTimeOffset OccurredAt { get; }
}
