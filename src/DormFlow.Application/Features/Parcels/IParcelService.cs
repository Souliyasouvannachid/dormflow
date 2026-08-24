using DormFlow.Contracts.Common;
using DormFlow.Contracts.Parcels;

namespace DormFlow.Application.Features.Parcels;

/// <summary>Use Case ทั้งหมดของโมดูลพัสดุ</summary>
public interface IParcelService
{
    /// <summary>ดึงรายการพัสดุแบบแบ่งหน้า</summary>
    Task<PagedResult<ParcelResponse>> GetPagedAsync(
        ParcelListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>อ่านข้อมูลพัสดุจากรหัสอ้างอิง</summary>
    Task<ParcelResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>บันทึกพัสดุที่มาถึงใหม่</summary>
    Task<ParcelResponse> LogAsync(
        LogParcelRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>บันทึกการรับพัสดุ ทำได้เฉพาะพัสดุที่ยังไม่ถูกรับเท่านั้น</summary>
    Task<ParcelResponse> PickupAsync(
        Guid id,
        PickupParcelRequest request,
        CancellationToken cancellationToken = default);
}
