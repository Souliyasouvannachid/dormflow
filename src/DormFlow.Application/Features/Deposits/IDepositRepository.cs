using DormFlow.Application.Common.Pagination;
using DormFlow.Contracts.Common;
using DormFlow.Domain.Entities;
using DormFlow.Domain.Enums;

namespace DormFlow.Application.Features.Deposits;

/// <summary>ตัวกรองสำหรับดึงรายการเงินมัดจำแบบแบ่งหน้า</summary>
public sealed record DepositListFilter(
    Guid? OrganizationId,
    Guid? DormitoryId,
    Guid? ContractId,
    DepositStatus? Status,
    IReadOnlyCollection<Guid>? RestrictToDormitoryIds);

/// <summary>เข้าถึงข้อมูลเงินมัดจำในฐานข้อมูล</summary>
public interface IDepositRepository
{
    /// <summary>อ่านเงินมัดจำจากรหัสอ้างอิง</summary>
    Task<Deposit?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>ตรวจว่าสัญญาเช่านี้มีการรับเงินมัดจำไว้แล้วหรือยัง</summary>
    Task<bool> ExistsByContractIdAsync(Guid contractId, CancellationToken cancellationToken = default);

    /// <summary>ดึงรายการเงินมัดจำแบบแบ่งหน้าตามตัวกรองที่ระบุ</summary>
    Task<PagedResult<Deposit>> GetPagedAsync(
        DepositListFilter filter,
        PagedQuery query,
        CancellationToken cancellationToken = default);

    /// <summary>เพิ่มเงินมัดจำใหม่</summary>
    Task AddAsync(Deposit deposit, CancellationToken cancellationToken = default);

    /// <summary>บันทึกการเปลี่ยนแปลงทั้งหมดลงฐานข้อมูล</summary>
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
