using DormFlow.Contracts.Common;
using DormFlow.Contracts.Settings;

namespace DormFlow.Application.Features.Settings;

/// <summary>Use Case ການຕັ້ງຄ່າໂມດູນທັງຝົດ</summary>
public interface ISettingService
{
    /// <summary>ດຶງເອົາບັນຊີລາຍຊື່ paginated ຂອງການຕັ້ງຄ່າ</summary>
    Task<PagedResult<SettingResponse>> GetPagedAsync(
        SettingListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>ອ່ານການຕັ້ງຄ່າຈາກຄີ</summary>
    Task<SettingResponse> GetByKeyAsync(
        string key,
        Guid? organizationId,
        CancellationToken cancellationToken = default);

    /// <summary>ສ້າງ ຫຼືແກ້ໄຂການຕັ້ງຄ່າໂດຍອີງໃສ່ລະຫັດທີ່ລະບຸ</summary>
    Task<SettingResponse> UpsertAsync(
        string key,
        UpsertSettingRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>ລຶບການຕັ້ງຄ່າໂດຍອີງໃສ່ລະຫັດທີ່ລະບຸ</summary>
    Task DeleteAsync(string key, Guid? organizationId, CancellationToken cancellationToken = default);
}
