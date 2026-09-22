using DormFlow.Contracts.Common;
using DormFlow.Contracts.Rooms;

namespace DormFlow.Application.Features.Rooms;

/// <summary>Use Case ໂມດູນຫ້ອງທັງໝົດ</summary>
public interface IRoomService
{
    /// <summary>ດຶງລາຍຊື່ຫ້ອງທີ່ມີ paginated</summary>
    Task<PagedResult<RoomResponse>> GetPagedAsync(
        RoomListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>ດຶງເອົາລາຍການແບ່ງໜ້າຂອງຫ້ອງທີ່ມີໃຫ້ເຊົ່າ</summary>
    Task<PagedResult<RoomResponse>> GetAvailableAsync(
        RoomListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>ສະຫຼຸບຂອງຈໍານວນຫ້ອງທີ່ແຍກຕາມສະຖານະ</summary>
    Task<RoomSummaryResponse> GetSummaryAsync(
        RoomSummaryRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>ອ່ານຂໍ້ມູນຫ້ອງຈາກລະຫັດອ້າງອີງ</summary>
    Task<RoomResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>ສ້າງຫ້ອງໃຝ່</summary>
    Task<RoomResponse> CreateAsync(CreateRoomRequest request, CancellationToken cancellationToken = default);

    /// <summary>ແກ້ໄຂຂໍ້ມູນຫ້ອງ</summary>
    Task<RoomResponse> UpdateAsync(
        Guid id,
        UpdateRoomRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>ປ່ຽນສະຖານະຫ້ອງ</summary>
    Task<RoomResponse> ChangeStatusAsync(Guid id, string status, CancellationToken cancellationToken = default);

    /// <summary>ລຶບຫ້ອງ</summary>
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
