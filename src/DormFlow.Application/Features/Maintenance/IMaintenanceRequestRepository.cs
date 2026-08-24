using DormFlow.Application.Common.Pagination;
using DormFlow.Contracts.Common;
using DormFlow.Domain.Entities;
using DormFlow.Domain.Enums;

namespace DormFlow.Application.Features.Maintenance;

/// <summary>ตัวกรองสำหรับดึงรายการงานแจ้งซ่อมบำรุงแบบแบ่งหน้า</summary>
public sealed record MaintenanceRequestListFilter(
    Guid? DormitoryId,
    Guid? RoomId,
    MaintenanceStatus? Status,
    MaintenancePriority? Priority,
    Guid? AssignedTechnicianId,
    IReadOnlyCollection<Guid>? RestrictToDormitoryIds);

/// <summary>เข้าถึงข้อมูลงานแจ้งซ่อมบำรุงในฐานข้อมูล</summary>
public interface IMaintenanceRequestRepository
{
    /// <summary>อ่านงานแจ้งซ่อมบำรุงจากรหัสอ้างอิง</summary>
    Task<MaintenanceRequest?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// ตรวจว่าห้องนี้ยังมีงานแจ้งซ่อมบำรุงอื่นที่ยังไม่ปิด (Reported/Assigned/InProgress) ค้างอยู่หรือไม่
    /// ใช้ตัดสินว่าจะคืนสถานะห้องเป็นว่างได้หรือยัง ไม่นับรวมงานที่ระบุใน excludeRequestId
    /// </summary>
    Task<bool> HasOpenRequestForRoomAsync(
        Guid roomId,
        Guid excludeRequestId,
        CancellationToken cancellationToken = default);

    /// <summary>ดึงรายการงานแจ้งซ่อมบำรุงแบบแบ่งหน้าตามตัวกรองที่ระบุ</summary>
    Task<PagedResult<MaintenanceRequest>> GetPagedAsync(
        MaintenanceRequestListFilter filter,
        PagedQuery query,
        CancellationToken cancellationToken = default);

    /// <summary>เพิ่มงานแจ้งซ่อมบำรุงใหม่</summary>
    Task AddAsync(MaintenanceRequest maintenanceRequest, CancellationToken cancellationToken = default);

    /// <summary>บันทึกการเปลี่ยนแปลงทั้งหมดลงฐานข้อมูล รวมถึงการเปลี่ยนแปลงของ Entity อื่นที่ถูกติดตามอยู่ในธุรกรรมเดียวกัน</summary>
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
