using DormFlow.Contracts.Common;
using DormFlow.Contracts.Parking;

namespace DormFlow.Application.Features.Parking;

/// <summary>Use Case ທັງຝົດຂອງໂມດູນການລົງທະບຽນບ່ອນຈອດລົດ</summary>
public interface IParkingRegistrationService
{
    /// <summary>ດຶງລາຍຊື່ບ່ອນຈອດລົດທີ່ລົງທະບຽນ</summary>
    Task<PagedResult<ParkingRegistrationResponse>> GetPagedAsync(
        ParkingRegistrationListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>ອ່ານຂໍ້ມູນການລົງທະບຽນບ່ອນຈອດລົດຈາກລະຫັດອ້າງອີງ</summary>
    Task<ParkingRegistrationResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>ລົງທະບຽນບ່ອນຈອດລົດໃໝ່ ທະບຽນລົດດຽວກັນບໍ່ສາມາດທັບຊ້ອນກັນໄດ້ກັບທະບຽນລົດທີ່ຍັງບໍ່ທັນສຳເລັດ</summary>
    Task<ParkingRegistrationResponse> RegisterAsync(
        RegisterParkingRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>ການສິ້ນສຸດການລົງທະບຽນບ່ອນຈອດລົດສາມາດເຮັດໄດ້ພຽງແຕ່ສໍາລັບລາຍການທີ່ຍັງລົງທະບຽນ</summary>
    Task<ParkingRegistrationResponse> ReleaseAsync(Guid id, CancellationToken cancellationToken = default);
}
