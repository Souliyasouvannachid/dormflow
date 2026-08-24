using DormFlow.Contracts.Common;
using DormFlow.Contracts.Maintenance;

namespace DormFlow.Application.Features.Maintenance;

/// <summary>Use Case ทั้งหมดของโมดูลงานแจ้งซ่อมบำรุง</summary>
public interface IMaintenanceRequestService
{
    /// <summary>ดึงรายการงานแจ้งซ่อมบำรุงแบบแบ่งหน้า</summary>
    Task<PagedResult<MaintenanceRequestResponse>> GetPagedAsync(
        MaintenanceRequestListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>อ่านข้อมูลงานแจ้งซ่อมบำรุงจากรหัสอ้างอิง</summary>
    Task<MaintenanceRequestResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>แจ้งซ่อมบำรุงใหม่</summary>
    Task<MaintenanceRequestResponse> CreateAsync(
        CreateMaintenanceRequestRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// มอบหมาย (หรือมอบหมายซ้ำ) ช่างซ่อมบำรุงให้กับงานนี้
    /// ห้องจะเปลี่ยนเป็นสถานะซ่อมบำรุงถ้ายังว่างอยู่
    /// </summary>
    Task<MaintenanceRequestResponse> AssignAsync(
        Guid id,
        AssignMaintenanceRequestRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// เปลี่ยนสถานะงานซ่อมบำรุงตามลำดับที่กำหนด (ไม่รวมการมอบหมายช่าง ต้องใช้ <see cref="AssignAsync"/> แทน)
    /// </summary>
    Task<MaintenanceRequestResponse> ChangeStatusAsync(
        Guid id,
        string status,
        CancellationToken cancellationToken = default);
}
