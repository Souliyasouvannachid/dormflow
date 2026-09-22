using DormFlow.Contracts.Common;
using DormFlow.Contracts.MeterReadings;

namespace DormFlow.Application.Features.MeterReadings;

/// <summary>Use Case ໂມດູນຄ່າແມັດທັງໝົດ</summary>
public interface IMeterReadingService
{
    /// <summary>ດຶງເອົາບັນຊີລາຍຊື່ paginated ຂອງຄ່າແມັດ</summary>
    Task<PagedResult<MeterReadingResponse>> GetPagedAsync(
        MeterReadingListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>ອ່ານຄ່າແມັດຈາກລະຫັດອ້າງອີງ</summary>
    Task<MeterReadingResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// ບັນທຶກຄ່າແມັດໃໝ່ ລະບົບຈະດຶງເອົາຄ່າກ່ອນໜ້າຂອງຫ້ອງ+ປະເພດນີ້ມາເປັນ PreviousReading ອັດຕະໂນມັດ
    /// </summary>
    Task<MeterReadingResponse> CreateAsync(
        CreateMeterReadingRequest request,
        CancellationToken cancellationToken = default);
}
