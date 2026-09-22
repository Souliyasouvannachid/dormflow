namespace DormFlow.Application.Common.Abstractions;

/// <summary>
/// ແຫຼ່ງເວລາຂອງລະບົບຖືກແຍກອອກເປັນ Abstraction ຂຽນ Unit Test ທີ່ສາມາດຄວບຄຸມເວລາ
/// </summary>
public interface IDateTimeProvider
{
    /// <summary>ເວລາປະຈຸບັນໃນຮູບແບບ UTC ເອົາລົງ PostgreSQL ແມ່ນ timestamptz</summary>
    DateTimeOffset UtcNow { get; }
}
