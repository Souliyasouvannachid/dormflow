using DormFlow.Application.Common.Pagination;
using DormFlow.Contracts.Common;
using DormFlow.Domain.Entities;
using DormFlow.Domain.Enums;

namespace DormFlow.Application.Features.Reservations;

/// <summary>ການກັ່ນຕອງສໍາລັບການດຶງການຈອງ paginated</summary>
public sealed record ReservationListFilter(
    Guid? OrganizationId,
    Guid? DormitoryId,
    Guid? RoomId,
    Guid? TenantId,
    ReservationStatus? Status,
    IReadOnlyCollection<Guid>? RestrictToDormitoryIds);

/// <summary>ເຂົ້າເຖິງຂໍ້ມູນການຈອງຢູ່ໃນຖານຂໍ້ມູນ</summary>
public interface IReservationRepository
{
    /// <summary>ອ່ານການຈອງຈາກລະຫັດອ້າງອີງ</summary>
    Task<Reservation?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// ກວດເບິ່ງວ່າຫ້ອງທີ່ລະບຸໄວ້ມີການຈອງທີ່ຫ້າວຫັນ (Pending) ລໍຖ້າຢູ່ຫຼືບໍ່?
    /// ໃຊ້ເພື່ອປ້ອງກັນການຈອງສອງຄັ້ງຂອງຫ້ອງດຽວກັນເຖິງແມ່ນວ່າໃນກໍລະນີຂອງການຮ້ອງຂໍເຂົ້າມາໃນເວລາດຽວກັນ
    /// </summary>
    Task<bool> HasPendingReservationAsync(Guid roomId, CancellationToken cancellationToken = default);

    /// <summary>ດຶງເອົາລາຍຊື່ການຈອງທີ່ວາງໄວ້ຕາມຕົວກອງທີ່ລະບຸໄວ້</summary>
    Task<PagedResult<Reservation>> GetPagedAsync(
        ReservationListFilter filter,
        PagedQuery query,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// ດຶງເອົາການຈອງທີ່ຍັງຄົງຢູ່ Pending ແຕ່ການຈອງຫ້ອງພັກໝົດອາຍຸແລ້ວ (ExpiresAt ໄດ້ຜ່ານໄປໃນເວລາທີ່ກໍານົດໄວ້)
    /// ກາງຄືນ Entity ປ່ຽນແບບຟອມການຕິດຕາມ (ບໍ່ AsNoTracking) ເນື່ອງຈາກວ່າມັນຖືກເອີ້ນວ່າສືບຕໍ່ແກ້ໄຂສະຖານະການທັນທີ
    /// ໃຊ້ໂດຍ Background Job ເທົ່ານັ້ນ, ບໍ່ໄດ້ຖືກກັ່ນຕອງໂດຍອົງກອນ/ຫໍພັກເພາະວ່າພວກເຂົາເຮັດວຽກທົ່ວລະບົບທັງຝົດ
    /// </summary>
    Task<IReadOnlyList<Reservation>> GetExpiredPendingAsync(
        DateTimeOffset asOf,
        int batchSize,
        CancellationToken cancellationToken = default);

    /// <summary>ນັບການຈອງ Pending ທີ່ຈະໝົດອາຍຸຮອດເວລາກຳນົດດ້ວຍ query ດຽວ</summary>
    Task<int> GetPendingExpiringCountAsync(
        ReservationListFilter filter,
        DateTimeOffset expiresBeforeOrAt,
        CancellationToken cancellationToken = default);

    /// <summary>ເພີ່ມການຈອງໃໝ່</summary>
    Task AddAsync(Reservation reservation, CancellationToken cancellationToken = default);

    /// <summary>ບັນທຶກການປ່ຽນແປງທັງໝົດໃສ່ຖານຂໍ້ມູນ. ລວມທັງການປ່ຽນແປງໃນ Entity ທຸລະກໍາທີ່ຕິດຕາມອື່ນແມ່ນຢູ່ໃນທຸລະກໍາດຽວກັນ</summary>
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
