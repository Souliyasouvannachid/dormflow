using DormFlow.Contracts.Common;
using DormFlow.Contracts.Organizations;

namespace DormFlow.Application.Features.Organizations;

/// <summary>Use Case ທັງຝົດຂອງໂມດູນອົງກອນ</summary>
public interface IOrganizationService
{
    /// <summary>ດຶງເອົາບັນຊີລາຍຊື່ຂອງອົງກອນ paginated</summary>
    Task<PagedResult<OrganizationResponse>> GetPagedAsync(
        OrganizationListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>ອ່ານຂໍ້ມູນອົງກອນຈາກລະຫັດການສົ່ງຕໍ່</summary>
    Task<OrganizationResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>ສ້າງອົງກອນໃຝ່</summary>
    Task<OrganizationResponse> CreateAsync(
        CreateOrganizationRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>ແກ້ໄຂຂໍ້ມູນອົງກອນ</summary>
    Task<OrganizationResponse> UpdateAsync(
        Guid id,
        UpdateOrganizationRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>ປ່ຽນສະຖານະອົງກອນ</summary>
    Task<OrganizationResponse> ChangeStatusAsync(
        Guid id,
        string status,
        CancellationToken cancellationToken = default);

    /// <summary>ລຶບອົງກອນ</summary>
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
