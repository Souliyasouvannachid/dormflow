using DormFlow.Application.Common.Pagination;
using DormFlow.Contracts.Common;
using DormFlow.Domain.Entities;
using DormFlow.Domain.Enums;

namespace DormFlow.Application.Features.MeterReadings;

/// <summary>ການກັ່ນຕອງສໍາລັບການດຶງບັນຊີລາຍຊື່ paginated ຂອງຄ່າແມັດ</summary>
public sealed record MeterReadingListFilter(
    Guid? OrganizationId,
    Guid? DormitoryId,
    Guid? RoomId,
    MeterType? MeterType,
    DateOnly? PeriodMonth,
    IReadOnlyCollection<Guid>? RestrictToDormitoryIds);

/// <summary>ເຂົ້າເຖິງຂໍ້ມູນແມັດໃນຖານຂໍ້ມູນ</summary>
public interface IMeterReadingRepository
{
    /// <summary>ອ່ານຄ່າແມັດຈາກລະຫັດອ້າງອີງ</summary>
    Task<MeterReading?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>ກວດເບິ່ງວ່າຫ້ອງ+ປະເພດເຄື່ອງວັດແທກນີ້ໄດ້ບັນທຶກຄ່າສໍາລັບເດືອນທີ່ລະບຸໄວ້ບໍ?</summary>
    Task<bool> ExistsForPeriodAsync(
        Guid roomId,
        MeterType meterType,
        DateOnly periodMonth,
        CancellationToken cancellationToken = default);

    /// <summary>ອ່ານການອ່ານແມັດຫຼ້າສຸດສໍາລັບຫ້ອງ+ປະເພດນີ້ແມ່ນກ່ອນເດືອນທີ່ກໍານົດໄວ້. ໃຊ້ເປັນຄ່າເລີ່ມຕົ້ນ PreviousReading</summary>
    Task<MeterReading?> GetLatestBeforePeriodAsync(
        Guid roomId,
        MeterType meterType,
        DateOnly periodMonth,
        CancellationToken cancellationToken = default);

    /// <summary>ອ່ານແມັດຫ້ອງ+ປະເພດເຄື່ອງວັດແທກນີ້ໃນເດືອນທີ່ລະບຸໄວ້ໃຊ້ໃນເວລາອອກໃບແຈ້ງໜີ້</summary>
    Task<MeterReading?> GetForPeriodAsync(
        Guid roomId,
        MeterType meterType,
        DateOnly periodMonth,
        CancellationToken cancellationToken = default);

    /// <summary>ດຶງເອົາບັນຊີລາຍຊື່ paginated ຂອງຄ່າແມັດໂດຍອີງໃສ່ຕົວກອງທີ່ລະບຸ</summary>
    Task<PagedResult<MeterReading>> GetPagedAsync(
        MeterReadingListFilter filter,
        PagedQuery query,
        CancellationToken cancellationToken = default);

    /// <summary>ເພີ່ມຄ່າແມັດໃໝ່</summary>
    Task AddAsync(MeterReading meterReading, CancellationToken cancellationToken = default);

    /// <summary>ບັນທຶກການປ່ຽນແປງທັງໝົດໃສ່ຖານຂໍ້ມູນ</summary>
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
