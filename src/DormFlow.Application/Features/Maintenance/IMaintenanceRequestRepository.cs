using DormFlow.Application.Common.Pagination;
using DormFlow.Contracts.Common;
using DormFlow.Domain.Entities;
using DormFlow.Domain.Enums;

namespace DormFlow.Application.Features.Maintenance;

/// <summary>ການກັ່ນຕອງສໍາລັບການດຶງບັນຊີລາຍຊື່ paginated ຂອງການແຈ້ງເຕືອນວຽກງານບໍາລຸງຮັກສາ</summary>
public sealed record MaintenanceRequestListFilter(
    Guid? OrganizationId,
    Guid? DormitoryId,
    Guid? RoomId,
    MaintenanceStatus? Status,
    MaintenancePriority? Priority,
    Guid? AssignedTechnicianId,
    IReadOnlyCollection<Guid>? RestrictToDormitoryIds);

/// <summary>ເຂົ້າເຖິງຂໍ້ມູນການບໍາລຸງຮັກສາໃນຖານຂໍ້ມູນ</summary>
public interface IMaintenanceRequestRepository
{
    /// <summary>ອ່ານການແຈ້ງເຕືອນການບຳລຸງຮັກສາຈາກລະຫັດອ້າງອີງ</summary>
    Task<MaintenanceRequest?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// ກວດເບິ່ງວ່າຫ້ອງນີ້ຍັງມີວຽກບໍາລຸງຮັກສາອື່ນທີ່ຍັງບໍ່ໄດ້ປິດ (Reported/Assigned/InProgress) ລໍຖ້າຢູ່ຫຼືບໍ່?
    /// ໃຊ້ເພື່ອຕັດສິນໃຈວ່າສະຖານະຂອງຫ້ອງສາມາດກັບຄືນໄປເປັນຟຣີຫຼືບໍ່. ບໍ່ລວມເອົາວຽກທີ່ລະບຸໄວ້ໃນ excludeRequestId
    /// </summary>
    Task<bool> HasOpenRequestForRoomAsync(
        Guid roomId,
        Guid excludeRequestId,
        CancellationToken cancellationToken = default);

    /// <summary>ດຶງເອົາບັນຊີລາຍຊື່ paginated ຂອງວຽກງານບໍາລຸງຮັກສາໂດຍອີງໃສ່ການກັ່ນຕອງທີ່ກໍານົດໄວ້</summary>
    Task<PagedResult<MaintenanceRequest>> GetPagedAsync(
        MaintenanceRequestListFilter filter,
        PagedQuery query,
        CancellationToken cancellationToken = default);

    /// <summary>ນັບຈຳນວນວຽກແຍກຕາມສະຖານະດ້ວຍ query ດຽວ</summary>
    Task<IReadOnlyDictionary<MaintenanceStatus, int>> GetStatusCountsAsync(
        MaintenanceRequestListFilter filter,
        CancellationToken cancellationToken = default);

    /// <summary>ເພີ່ມວຽກບໍາລຸງຮັກສາໃຝ່</summary>
    Task AddAsync(MaintenanceRequest maintenanceRequest, CancellationToken cancellationToken = default);

    /// <summary>ບັນທຶກການປ່ຽນແປງທັງໝົດໃສ່ຖານຂໍ້ມູນ. ລວມທັງການປ່ຽນແປງໃນ Entity ທຸລະກໍາທີ່ຕິດຕາມອື່ນແມ່ນຢູ່ໃນທຸລະກໍາດຽວກັນ</summary>
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
