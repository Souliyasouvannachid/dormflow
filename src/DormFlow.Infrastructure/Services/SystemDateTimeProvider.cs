using DormFlow.Application.Common.Abstractions;

namespace DormFlow.Infrastructure.Services;

/// <summary>
/// ແຫຼ່ງເວລາທີ່ແທ້ຈິງຂອງລະບົບຖືກສົ່ງຄືນ UTC ສະເຝີເຮັດໃຫ້ມັນລົງ timestamptz ສາມາດຈັບຄູ່ທຸກອຸປະກອນ
/// </summary>
public sealed class SystemDateTimeProvider : IDateTimeProvider
{
    /// <inheritdoc />
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}
