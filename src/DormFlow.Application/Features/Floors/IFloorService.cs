using DormFlow.Contracts.Common;
using DormFlow.Contracts.Floors;

namespace DormFlow.Application.Features.Floors;

/// <summary>Use Case ໂມດູນຊັ້ນກໍ່ສ້າງທັງໝົດ</summary>
public interface IFloorService
{
    /// <summary>ດຶງຂໍ້ມູນລາຍຊື່ຊັ້ນໜ້າຂອງຊັ້ນຂໍ້ມູນ</summary>
    Task<PagedResult<FloorResponse>> GetPagedAsync(
        FloorListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>ອ່ານຂໍ້ມູນຊັ້ນຈາກລະຫັດອ້າງອີງ</summary>
    Task<FloorResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>ສ້າງຊັ້ນຂໍ້ມູນໃໝ່</summary>
    Task<FloorResponse> CreateAsync(CreateFloorRequest request, CancellationToken cancellationToken = default);

    /// <summary>ແກ້ໄຂຂໍ້ມູນຊັ້ນ</summary>
    Task<FloorResponse> UpdateAsync(
        Guid id,
        UpdateFloorRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>ປ່ຽນສະຖານະຊັ້ນຮຽນ</summary>
    Task<FloorResponse> ChangeStatusAsync(Guid id, string status, CancellationToken cancellationToken = default);

    /// <summary>ລຶບຊັ້ນ</summary>
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
