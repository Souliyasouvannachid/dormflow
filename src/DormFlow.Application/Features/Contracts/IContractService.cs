using DormFlow.Contracts.Common;
using DormFlow.Contracts.Leases;

namespace DormFlow.Application.Features.Contracts;

/// <summary>Use Case ทั้งหมดของโมดูลสัญญาเช่า</summary>
public interface IContractService
{
    /// <summary>ดึงรายการสัญญาเช่าแบบแบ่งหน้า</summary>
    Task<PagedResult<ContractResponse>> GetPagedAsync(
        ContractListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>อ่านข้อมูลสัญญาเช่าจากรหัสอ้างอิง</summary>
    Task<ContractResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>สร้างสัญญาเช่าใหม่ในสถานะร่าง</summary>
    Task<ContractResponse> CreateAsync(CreateContractRequest request, CancellationToken cancellationToken = default);

    /// <summary>เปิดใช้งานสัญญาเช่า (เช็คอิน) ห้องจะถูกเปลี่ยนสถานะเป็นมีผู้เช่าอยู่</summary>
    Task<ContractResponse> ActivateAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>สิ้นสุดสัญญาเช่า (เช็คเอาท์) ห้องจะถูกเปลี่ยนสถานะเป็นรอทำความสะอาด</summary>
    Task<ContractResponse> TerminateAsync(
        Guid id,
        TerminateContractRequest request,
        CancellationToken cancellationToken = default);
}
