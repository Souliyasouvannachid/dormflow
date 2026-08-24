using DormFlow.Application.Common.Pagination;
using DormFlow.Contracts.Common;
using DormFlow.Domain.Entities;
using DormFlow.Domain.Enums;

namespace DormFlow.Application.Features.Tenants;

/// <summary>ตัวกรองสำหรับดึงรายการผู้เช่าแบบแบ่งหน้า</summary>
public sealed record TenantListFilter(Guid? OrganizationId, TenantStatus? Status);

/// <summary>เข้าถึงข้อมูลผู้เช่าในฐานข้อมูล</summary>
public interface ITenantRepository
{
    /// <summary>อ่านผู้เช่าจากรหัสอ้างอิง</summary>
    Task<Tenant?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>ตรวจว่ามีผู้เช่าที่ใช้เลขบัตรประชาชนนี้อยู่แล้วในองค์กรเดียวกันหรือไม่</summary>
    Task<bool> ExistsByNationalIdAsync(
        Guid organizationId,
        string nationalId,
        Guid? excludeTenantId,
        CancellationToken cancellationToken = default);

    /// <summary>ดึงรายการผู้เช่าแบบแบ่งหน้าตามตัวกรองที่ระบุ</summary>
    Task<PagedResult<Tenant>> GetPagedAsync(
        TenantListFilter filter,
        PagedQuery query,
        CancellationToken cancellationToken = default);

    /// <summary>เพิ่มผู้เช่าใหม่</summary>
    Task AddAsync(Tenant tenant, CancellationToken cancellationToken = default);

    /// <summary>บันทึกการเปลี่ยนแปลงทั้งหมดลงฐานข้อมูล</summary>
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
