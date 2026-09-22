using DormFlow.Contracts.Common;
using DormFlow.Contracts.RoomTypes;

namespace DormFlow.Application.Features.RoomTypes;

/// <summary>Use Case ໂມດູນປະເພດຫ້ອງທັງຝົດ</summary>
public interface IRoomTypeService
{
    /// <summary>ດຶງເອົາບັນຊີລາຍຊື່ paginated ຂອງປະເພດຫ້ອງ</summary>
    Task<PagedResult<RoomTypeResponse>> GetPagedAsync(
        RoomTypeListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>ອ່ານຂໍ້ມູນປະເພດຫ້ອງຈາກລະຫັດອ້າງອີງ</summary>
    Task<RoomTypeResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>ສ້າງປະເພດຫ້ອງໃຝ່</summary>
    Task<RoomTypeResponse> CreateAsync(
        CreateRoomTypeRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>ແກ້ໄຂຂໍ້ມູນປະເພດຫ້ອງ</summary>
    Task<RoomTypeResponse> UpdateAsync(
        Guid id,
        UpdateRoomTypeRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>ປ່ຽນສະຖານະປະເພດຫ້ອງ</summary>
    Task<RoomTypeResponse> ChangeStatusAsync(
        Guid id,
        string status,
        CancellationToken cancellationToken = default);

    /// <summary>ລຶບປະເພດຫ້ອງ</summary>
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
