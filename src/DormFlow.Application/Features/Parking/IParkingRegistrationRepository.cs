using DormFlow.Application.Common.Pagination;
using DormFlow.Contracts.Common;
using DormFlow.Domain.Entities;
using DormFlow.Domain.Enums;

namespace DormFlow.Application.Features.Parking;

/// <summary>ตัวกรองสำหรับดึงรายการจดทะเบียนที่จอดรถแบบแบ่งหน้า</summary>
public sealed record ParkingRegistrationListFilter(
    Guid? OrganizationId,
    Guid? DormitoryId,
    Guid? TenantId,
    ParkingRegistrationStatus? Status,
    IReadOnlyCollection<Guid>? RestrictToDormitoryIds);

/// <summary>เข้าถึงข้อมูลการจดทะเบียนที่จอดรถในฐานข้อมูล</summary>
public interface IParkingRegistrationRepository
{
    /// <summary>อ่านการจดทะเบียนที่จอดรถจากรหัสอ้างอิง</summary>
    Task<ParkingRegistration?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>ตรวจว่าทะเบียนรถนี้มีการจดทะเบียนที่ยังไม่สิ้นสุด (Active) อยู่ในองค์กรเดียวกันหรือไม่</summary>
    Task<bool> HasActiveByLicensePlateAsync(
        Guid organizationId,
        string licensePlate,
        CancellationToken cancellationToken = default);

    /// <summary>ดึงรายการจดทะเบียนที่จอดรถแบบแบ่งหน้าตามตัวกรองที่ระบุ</summary>
    Task<PagedResult<ParkingRegistration>> GetPagedAsync(
        ParkingRegistrationListFilter filter,
        PagedQuery query,
        CancellationToken cancellationToken = default);

    /// <summary>เพิ่มการจดทะเบียนที่จอดรถใหม่</summary>
    Task AddAsync(ParkingRegistration registration, CancellationToken cancellationToken = default);

    /// <summary>บันทึกการเปลี่ยนแปลงทั้งหมดลงฐานข้อมูล</summary>
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
