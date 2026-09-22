namespace DormFlow.Domain.Common;

/// <summary>
/// ສັນຍາພື້ນຖານຂອງ Entity ທຸກຢ່າງໃນລະບົບໃຊ້ UUID (<see cref="Guid"/>) ແມ່ນ Primary Key ສະເໝີ
/// </summary>
public interface IEntity
{
    /// <summary>ລະຫັດການອ້າງອີງຫຼັກຂອງ Entity</summary>
    Guid Id { get; }
}
