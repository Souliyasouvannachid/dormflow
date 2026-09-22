using DormFlow.Contracts.Common;
using DormFlow.Contracts.Tenants;

namespace DormFlow.Application.Features.Tenants;

/// <summary>Use Case ໂມດູນຜູ້ເຊົ່າທັງໝົດ</summary>
public interface ITenantService
{
    /// <summary>ດຶງເອົາລາຍຊື່ຜູ້ເຊົ່າທີ່ຕັ້ງໄວ້ແລ້ວ</summary>
    Task<PagedResult<TenantResponse>> GetPagedAsync(
        TenantListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>ອ່ານຂໍ້ມູນຜູ້ເຊົ່າຈາກລະຫັດອ້າງອີງ</summary>
    Task<TenantResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>ສ້າງຂໍ້ມູນຜູ້ເຊົ່າໃຝ່</summary>
    Task<TenantResponse> CreateAsync(CreateTenantRequest request, CancellationToken cancellationToken = default);

    /// <summary>ແກ້ໄຂຂໍ້ມູນຜູ້ເຊົ່າ</summary>
    Task<TenantResponse> UpdateAsync(
        Guid id,
        UpdateTenantRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>ປ່ຽນສະຖານະຜູ້ເຊົ່າ</summary>
    Task<TenantResponse> ChangeStatusAsync(Guid id, string status, CancellationToken cancellationToken = default);
}
