using DormFlow.Contracts.Common;
using DormFlow.Contracts.MeterReadings;

namespace DormFlow.Application.Features.MeterReadings;

/// <summary>Use Case ทั้งหมดของโมดูลค่ามิเตอร์</summary>
public interface IMeterReadingService
{
    /// <summary>ดึงรายการค่ามิเตอร์แบบแบ่งหน้า</summary>
    Task<PagedResult<MeterReadingResponse>> GetPagedAsync(
        MeterReadingListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>อ่านค่ามิเตอร์จากรหัสอ้างอิง</summary>
    Task<MeterReadingResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// บันทึกค่ามิเตอร์ใหม่ ระบบจะดึงค่าครั้งก่อนหน้าของห้อง+ประเภทนี้มาเป็น PreviousReading ให้อัตโนมัติ
    /// </summary>
    Task<MeterReadingResponse> CreateAsync(
        CreateMeterReadingRequest request,
        CancellationToken cancellationToken = default);
}
