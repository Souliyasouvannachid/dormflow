using DormFlow.Application.Common.Pagination;
using DormFlow.Contracts.Common;
using DormFlow.Domain.Entities;
using DormFlow.Domain.Enums;

namespace DormFlow.Application.Features.Reservations;

/// <summary>ตัวกรองสำหรับดึงรายการจองแบบแบ่งหน้า</summary>
public sealed record ReservationListFilter(
    Guid? DormitoryId,
    Guid? RoomId,
    Guid? TenantId,
    ReservationStatus? Status,
    IReadOnlyCollection<Guid>? RestrictToDormitoryIds);

/// <summary>เข้าถึงข้อมูลการจองในฐานข้อมูล</summary>
public interface IReservationRepository
{
    /// <summary>อ่านการจองจากรหัสอ้างอิง</summary>
    Task<Reservation?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// ตรวจว่าห้องที่ระบุมีการจองที่ยังไม่สิ้นสุด (Pending) ค้างอยู่หรือไม่
    /// ใช้ป้องกันการจองห้องเดียวกันซ้ำซ้อนแม้ในกรณีที่มีคำขอเข้ามาพร้อมกัน
    /// </summary>
    Task<bool> HasPendingReservationAsync(Guid roomId, CancellationToken cancellationToken = default);

    /// <summary>ดึงรายการจองแบบแบ่งหน้าตามตัวกรองที่ระบุ</summary>
    Task<PagedResult<Reservation>> GetPagedAsync(
        ReservationListFilter filter,
        PagedQuery query,
        CancellationToken cancellationToken = default);

    /// <summary>เพิ่มการจองใหม่</summary>
    Task AddAsync(Reservation reservation, CancellationToken cancellationToken = default);

    /// <summary>บันทึกการเปลี่ยนแปลงทั้งหมดลงฐานข้อมูล รวมถึงการเปลี่ยนแปลงของ Entity อื่นที่ถูกติดตามอยู่ในธุรกรรมเดียวกัน</summary>
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
