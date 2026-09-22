using DormFlow.Contracts.Common;
using DormFlow.Contracts.Reservations;

namespace DormFlow.Application.Features.Reservations;

/// <summary>Use Case ໂມດູນການຈອງທັງໝົດ</summary>
public interface IReservationService
{
    /// <summary>ດຶງເອົາລາຍຊື່ການຈອງແບບ paginated</summary>
    Task<PagedResult<ReservationResponse>> GetPagedAsync(
        ReservationListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>ນັບການຈອງ Pending ທີ່ກຳລັງຈະໝົດອາຍຸສຳລັບ Dashboard</summary>
    Task<int> GetPendingExpiringCountAsync(
        DateTimeOffset expiresBeforeOrAt,
        CancellationToken cancellationToken = default);

    /// <summary>ອ່ານຂໍ້ມູນການຈອງຈາກລະຫັດອ້າງອີງ</summary>
    Task<ReservationResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>ສ້າງການຈອງໃໝ່ ສະຖານະຫ້ອງຈະຖືກປ່ຽນເປັນຈອງທັນທີ</summary>
    Task<ReservationResponse> CreateAsync(
        CreateReservationRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>ຍົກເລີກການຈອງ. ຫ້ອງຈະຖືກປ່ອຍໄວ້ໃນຕອນກາງຄືນຖ້າຫາກວ່າມັນຍັງຢູ່ໃນສະຖານະສະຫງວນຈາກບັນຊີລາຍຊື່ນີ້</summary>
    Task<ReservationResponse> CancelAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// ການຈອງໝົດອາຍຸແລ້ວ Pending ແຕ່ມັນເຖິງເວລາທີ່ຈະອອກຈາກຫ້ອງ (ExpiresAt) ຫຼັງຈາກນັ້ນ, ຫ້ອງທັງຝົດຈະໄດ້ຮັບການປົດປ່ອຍສໍາລັບການໃນຕອນກາງຄືນຖ້າຫາກວ່າເຂົາເຈົ້າຍັງມີ
    /// ໃນສະຖານະທີ່ຖືກສະຫງວນຈາກບັນຊີລາຍຊື່ນັ້ນ, ເອີ້ນວ່າໂດຍ Background Job ແຕ່ລະໄລຍະ, ເຮັດວຽກທົ່ວທຸກອົງກອນ. ບໍ່ມີການກວດສອບຜູ້ໃຊ້
    /// </summary>
    /// <returns>ຈຳນວນລາຍການໝົດອາຍຸໃນການໂທນີ້</returns>
    Task<int> ExpirePastDueReservationsAsync(CancellationToken cancellationToken = default);
}
