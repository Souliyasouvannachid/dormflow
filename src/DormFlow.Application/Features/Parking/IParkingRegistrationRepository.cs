using DormFlow.Application.Common.Pagination;
using DormFlow.Contracts.Common;
using DormFlow.Domain.Entities;
using DormFlow.Domain.Enums;

namespace DormFlow.Application.Features.Parking;

/// <summary>ການກັ່ນຕອງສໍາລັບການດຶງເອົາການລົງທະບຽນບ່ອນຈອດລົດ paginated</summary>
public sealed record ParkingRegistrationListFilter(
    Guid? OrganizationId,
    Guid? DormitoryId,
    Guid? TenantId,
    ParkingRegistrationStatus? Status,
    IReadOnlyCollection<Guid>? RestrictToDormitoryIds);

/// <summary>ເຂົ້າເຖິງຂໍ້ມູນການລົງທະບຽນບ່ອນຈອດລົດໃນຖານຂໍ້ມູນ</summary>
public interface IParkingRegistrationRepository
{
    /// <summary>ອ່ານການລົງທະບຽນບ່ອນຈອດລົດຈາກລະຫັດອ້າງອີງ</summary>
    Task<ParkingRegistration?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>ກວດສອບວ່າລົດຄັນນີ້ມີໃບທະບຽນທີ່ຍັງບໍ່ທັນຄົບຖ້ວນ (Active) ເຈົ້າຢູ່ໃນອົງກອນດຽວກັນບໍ?</summary>
    Task<bool> HasActiveByLicensePlateAsync(
        Guid organizationId,
        string licensePlate,
        CancellationToken cancellationToken = default);

    /// <summary>ດຶງເອົາບັນຊີລາຍຊື່ການລົງທະບຽນບ່ອນຈອດລົດ paginated ໂດຍອີງໃສ່ການກັ່ນຕອງທີ່ລະບຸໄວ້</summary>
    Task<PagedResult<ParkingRegistration>> GetPagedAsync(
        ParkingRegistrationListFilter filter,
        PagedQuery query,
        CancellationToken cancellationToken = default);

    /// <summary>ເພີ່ມທະບຽນບ່ອນຈອດລົດໃໝ່</summary>
    Task AddAsync(ParkingRegistration registration, CancellationToken cancellationToken = default);

    /// <summary>ບັນທຶກການປ່ຽນແປງທັງໝົດໃສ່ຖານຂໍ້ມູນ</summary>
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
