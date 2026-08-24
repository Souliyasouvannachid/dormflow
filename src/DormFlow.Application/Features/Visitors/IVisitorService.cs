using DormFlow.Contracts.Common;
using DormFlow.Contracts.Visitors;

namespace DormFlow.Application.Features.Visitors;

/// <summary>Use Case ทั้งหมดของโมดูลผู้มาเยือน</summary>
public interface IVisitorService
{
    /// <summary>ดึงรายการผู้มาเยือนแบบแบ่งหน้า</summary>
    Task<PagedResult<VisitorResponse>> GetPagedAsync(
        VisitorListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>อ่านข้อมูลผู้มาเยือนจากรหัสอ้างอิง</summary>
    Task<VisitorResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>บันทึกการเข้าของผู้มาเยือนใหม่</summary>
    Task<VisitorResponse> CheckInAsync(
        CheckInVisitorRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>บันทึกการออก ทำได้เฉพาะผู้มาเยือนที่ยังไม่ออกเท่านั้น</summary>
    Task<VisitorResponse> CheckOutAsync(Guid id, CancellationToken cancellationToken = default);
}
