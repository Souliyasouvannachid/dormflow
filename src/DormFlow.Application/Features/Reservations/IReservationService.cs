using DormFlow.Contracts.Common;
using DormFlow.Contracts.Reservations;

namespace DormFlow.Application.Features.Reservations;

/// <summary>Use Case ทั้งหมดของโมดูลการจอง</summary>
public interface IReservationService
{
    /// <summary>ดึงรายการจองแบบแบ่งหน้า</summary>
    Task<PagedResult<ReservationResponse>> GetPagedAsync(
        ReservationListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>อ่านข้อมูลการจองจากรหัสอ้างอิง</summary>
    Task<ReservationResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>สร้างการจองใหม่ ห้องจะถูกเปลี่ยนสถานะเป็นถูกจองทันที</summary>
    Task<ReservationResponse> CreateAsync(
        CreateReservationRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>ยกเลิกการจอง ห้องจะถูกปล่อยว่างคืนถ้ายังอยู่ในสถานะถูกจองจากรายการนี้</summary>
    Task<ReservationResponse> CancelAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// หมดอายุการจองที่ยังเป็น Pending แต่เลยเวลากันห้อง (ExpiresAt) แล้วทั้งหมด ห้องจะถูกปล่อยว่างคืนถ้ายังอยู่
    /// ในสถานะถูกจองจากรายการนั้น เรียกโดย Background Job เป็นระยะ ทำงานข้ามทุกองค์กร ไม่มีการตรวจสิทธิ์ผู้ใช้
    /// </summary>
    /// <returns>จำนวนรายการที่ถูกหมดอายุในการเรียกครั้งนี้</returns>
    Task<int> ExpirePastDueReservationsAsync(CancellationToken cancellationToken = default);
}
