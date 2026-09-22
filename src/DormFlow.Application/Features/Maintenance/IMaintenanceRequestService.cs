using DormFlow.Contracts.Common;
using DormFlow.Contracts.Maintenance;
using DormFlow.Domain.Enums;

namespace DormFlow.Application.Features.Maintenance;

/// <summary>Use Case ໂມດູນການແຈ້ງເຕືອນການບຳລຸງຮັກສາທັງໝົດ</summary>
public interface IMaintenanceRequestService
{
    /// <summary>ດຶງເອົາບັນຊີລາຍຊື່ paginated ຂອງການແຈ້ງເຕືອນວຽກງານບໍາລຸງຮັກສາ</summary>
    Task<PagedResult<MaintenanceRequestResponse>> GetPagedAsync(
        MaintenanceRequestListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>ນັບວຽກບຳລຸງຮັກສາຕາມສະຖານະສຳລັບ Dashboard</summary>
    Task<IReadOnlyDictionary<MaintenanceStatus, int>> GetStatusCountsAsync(
        CancellationToken cancellationToken = default);

    /// <summary>ອ່ານຂໍ້ມູນການແຈ້ງເຕືອນການບຳລຸງຮັກສາຈາກລະຫັດອ້າງອີງ</summary>
    Task<MaintenanceRequestResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>ແຈ້ງການບໍາລຸງຮັກສາໃຝ່</summary>
    Task<MaintenanceRequestResponse> CreateAsync(
        CreateMaintenanceRequestRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// ມອບໝາຍ (ຫຼືມອບໝາຍຄືນໃໝ່) ຊ່າງສ້ອມແປງສຳລັບວຽກນີ້
    /// ຫ້ອງຈະຖືກປ່ຽນເປັນສະຖານະບໍາລຸງຮັກສາຖ້າມັນຍັງຫວ່າງຢູ່
    /// </summary>
    Task<MaintenanceRequestResponse> AssignAsync(
        Guid id,
        AssignMaintenanceRequestRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// ປ່ຽນສະຖານະວຽກບຳລຸງຮັກສາຕາມລຳດັບທີ່ກຳນົດໄວ້ (ບໍ່ລວມເອົາການມອບໝາຍນັກວິຊາການ. ຕ້ອງການ <see cref="AssignAsync"/> ແທນ)
    /// </summary>
    Task<MaintenanceRequestResponse> ChangeStatusAsync(
        Guid id,
        string status,
        CancellationToken cancellationToken = default);
}
