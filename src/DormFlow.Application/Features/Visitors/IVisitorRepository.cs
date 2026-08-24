using DormFlow.Application.Common.Pagination;
using DormFlow.Contracts.Common;
using DormFlow.Domain.Entities;
using DormFlow.Domain.Enums;

namespace DormFlow.Application.Features.Visitors;

/// <summary>ตัวกรองสำหรับดึงรายการผู้มาเยือนแบบแบ่งหน้า</summary>
public sealed record VisitorListFilter(
    Guid? OrganizationId,
    Guid? DormitoryId,
    Guid? RoomId,
    VisitorStatus? Status,
    IReadOnlyCollection<Guid>? RestrictToDormitoryIds);

/// <summary>เข้าถึงข้อมูลผู้มาเยือนในฐานข้อมูล</summary>
public interface IVisitorRepository
{
    /// <summary>อ่านรายการผู้มาเยือนจากรหัสอ้างอิง</summary>
    Task<Visitor?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>ดึงรายการผู้มาเยือนแบบแบ่งหน้าตามตัวกรองที่ระบุ</summary>
    Task<PagedResult<Visitor>> GetPagedAsync(
        VisitorListFilter filter,
        PagedQuery query,
        CancellationToken cancellationToken = default);

    /// <summary>เพิ่มรายการผู้มาเยือนใหม่</summary>
    Task AddAsync(Visitor visitor, CancellationToken cancellationToken = default);

    /// <summary>บันทึกการเปลี่ยนแปลงทั้งหมดลงฐานข้อมูล</summary>
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
