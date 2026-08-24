namespace DormFlow.Domain.Common;

/// <summary>
/// สัญญาพื้นฐานของ Entity ทุกตัวในระบบ ใช้ UUID (<see cref="Guid"/>) เป็น Primary Key เสมอ
/// </summary>
public interface IEntity
{
    /// <summary>รหัสอ้างอิงหลักของ Entity</summary>
    Guid Id { get; }
}
