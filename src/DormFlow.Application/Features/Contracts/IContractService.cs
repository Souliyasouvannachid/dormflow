using DormFlow.Contracts.Common;
using DormFlow.Contracts.Leases;

namespace DormFlow.Application.Features.Contracts;

/// <summary>Use Case ໂມດູນການເຊົ່າທັງໝົດ</summary>
public interface IContractService
{
    /// <summary>ດຶງເອົາບັນຊີລາຍຊື່ paginated ຂອງສັນຍາເຊົ່າ</summary>
    Task<PagedResult<ContractResponse>> GetPagedAsync(
        ContractListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>ອ່ານຂໍ້ມູນການເຊົ່າຈາກລະຫັດອ້າງອີງ</summary>
    Task<ContractResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>ສ້າງສັນຍາເຊົ່າໃໝ່ຢູ່ໃນລັດສະບັບຮ່າງ</summary>
    Task<ContractResponse> CreateAsync(CreateContractRequest request, CancellationToken cancellationToken = default);

    /// <summary>ເປີດໃຊ້ຂໍ້ຕົກລົງການເຊົ່າ (ເຊັກອິນ) ສະຖານະຫ້ອງຈະຖືກປ່ຽນເປັນ occupied</summary>
    Task<ContractResponse> ActivateAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>ສິ້ນສຸດສັນຍາເຊົ່າ (ກວດເບິ່ງ) ຫ້ອງຈະຖືກປ່ຽນເປັນສະຖານະການອະນາໄມທີ່ລໍຖ້າຢູ່</summary>
    Task<ContractResponse> TerminateAsync(
        Guid id,
        TerminateContractRequest request,
        CancellationToken cancellationToken = default);
}
