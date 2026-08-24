using DormFlow.Application.Common.Pagination;
using DormFlow.Contracts.Common;
using DormFlow.Domain.Entities;
using DormFlow.Domain.Enums;

namespace DormFlow.Application.Features.Contracts;

/// <summary>ตัวกรองสำหรับดึงรายการสัญญาเช่าแบบแบ่งหน้า</summary>
public sealed record ContractListFilter(
    Guid? OrganizationId,
    Guid? DormitoryId,
    Guid? RoomId,
    Guid? TenantId,
    ContractStatus? Status,
    IReadOnlyCollection<Guid>? RestrictToDormitoryIds);

/// <summary>เข้าถึงข้อมูลสัญญาเช่าในฐานข้อมูล</summary>
public interface IContractRepository
{
    /// <summary>อ่านสัญญาเช่าจากรหัสอ้างอิง</summary>
    Task<Contract?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>ตรวจว่ามีเลขที่สัญญานี้อยู่แล้วในองค์กรเดียวกันหรือไม่ ใช้ตอนสุ่มเลขที่สัญญาเพื่อกันการชนกัน</summary>
    Task<bool> ExistsByContractNumberAsync(
        Guid organizationId,
        string contractNumber,
        CancellationToken cancellationToken = default);

    /// <summary>ตรวจว่าห้องที่ระบุมีสัญญาที่ยังไม่สิ้นสุด (Draft หรือ Active) ค้างอยู่หรือไม่</summary>
    Task<bool> HasOpenContractAsync(Guid roomId, CancellationToken cancellationToken = default);

    /// <summary>ดึงรายการสัญญาเช่าแบบแบ่งหน้าตามตัวกรองที่ระบุ</summary>
    Task<PagedResult<Contract>> GetPagedAsync(
        ContractListFilter filter,
        PagedQuery query,
        CancellationToken cancellationToken = default);

    /// <summary>เพิ่มสัญญาเช่าใหม่</summary>
    Task AddAsync(Contract contract, CancellationToken cancellationToken = default);

    /// <summary>บันทึกการเปลี่ยนแปลงทั้งหมดลงฐานข้อมูล รวมถึงการเปลี่ยนแปลงของ Entity อื่นที่ถูกติดตามอยู่ในธุรกรรมเดียวกัน</summary>
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
