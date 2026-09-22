using DormFlow.Contracts.Buildings;
using DormFlow.Contracts.Common;

namespace DormFlow.Application.Features.Buildings;

/// <summary>Use Case ໂມດູນການກໍ່ສ້າງທັງຝົດ</summary>
public interface IBuildingService
{
    /// <summary>ດຶງຂໍ້ມູນລາຍຊື່ຕຶກອາຄານ</summary>
    Task<PagedResult<BuildingResponse>> GetPagedAsync(
        BuildingListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>ອ່ານຂໍ້ມູນການກໍ່ສ້າງຈາກລະຫັດອ້າງອີງ</summary>
    Task<BuildingResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>ສ້າງອາຄານໃຝ່</summary>
    Task<BuildingResponse> CreateAsync(CreateBuildingRequest request, CancellationToken cancellationToken = default);

    /// <summary>ແກ້ໄຂຂໍ້ມູນອາຄານ</summary>
    Task<BuildingResponse> UpdateAsync(
        Guid id,
        UpdateBuildingRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>ປ່ຽນສະຖານະການກໍ່ສ້າງ</summary>
    Task<BuildingResponse> ChangeStatusAsync(
        Guid id,
        string status,
        CancellationToken cancellationToken = default);

    /// <summary>ລຶບອາຄານ</summary>
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
