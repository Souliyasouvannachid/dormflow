using DormFlow.Application.Common.Pagination;
using DormFlow.Contracts.Common;
using DormFlow.Domain.Entities;
using DormFlow.Domain.Enums;

namespace DormFlow.Application.Features.Parcels;

/// <summary>ตัวกรองสำหรับดึงรายการพัสดุแบบแบ่งหน้า</summary>
public sealed record ParcelListFilter(
    Guid? OrganizationId,
    Guid? DormitoryId,
    Guid? RoomId,
    ParcelStatus? Status,
    IReadOnlyCollection<Guid>? RestrictToDormitoryIds);

/// <summary>เข้าถึงข้อมูลพัสดุในฐานข้อมูล</summary>
public interface IParcelRepository
{
    /// <summary>อ่านพัสดุจากรหัสอ้างอิง</summary>
    Task<Parcel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>ดึงรายการพัสดุแบบแบ่งหน้าตามตัวกรองที่ระบุ</summary>
    Task<PagedResult<Parcel>> GetPagedAsync(
        ParcelListFilter filter,
        PagedQuery query,
        CancellationToken cancellationToken = default);

    /// <summary>เพิ่มพัสดุใหม่</summary>
    Task AddAsync(Parcel parcel, CancellationToken cancellationToken = default);

    /// <summary>บันทึกการเปลี่ยนแปลงทั้งหมดลงฐานข้อมูล</summary>
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
