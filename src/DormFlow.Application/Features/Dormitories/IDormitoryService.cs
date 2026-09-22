using DormFlow.Contracts.Common;
using DormFlow.Contracts.Dormitories;

namespace DormFlow.Application.Features.Dormitories;

/// <summary>Use Case ໂມດູນຫໍພັກທັງໝົດ</summary>
public interface IDormitoryService
{
    /// <summary>ດຶງເອົາລາຍຊື່ຫໍພັກທີ່ຕັ້ງໄວ້ແລ້ວ</summary>
    /// <param name="request">ຕົວກໍານົດການຊອກຫາ</param>
    /// <param name="cancellationToken">Token ສໍາລັບການຍົກເລີກຄໍາສັ່ງ</param>
    Task<PagedResult<DormitoryResponse>> GetPagedAsync(
        DormitoryListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>ອ່ານຂໍ້ມູນຫໍພັກຈາກລະຫັດອ້າງອີງ</summary>
    /// <param name="id">ລະຫັດຫໍພັກ</param>
    /// <param name="cancellationToken">Token ສໍາລັບການຍົກເລີກຄໍາສັ່ງ</param>
    Task<DormitoryResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>ສ້າງຫໍພັກໃໝ່</summary>
    /// <param name="request">ຂໍ້ມູນກ່ຽວກັບຫໍພັກທີ່ເຈົ້າຕ້ອງການສ້າງ</param>
    /// <param name="cancellationToken">Token ສໍາລັບການຍົກເລີກຄໍາສັ່ງ</param>
    Task<DormitoryResponse> CreateAsync(
        CreateDormitoryRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>ແກ້ໄຂຂໍ້ມູນຫໍພັກ</summary>
    /// <param name="id">ລະຫັດຫໍພັກທີ່ທ່ານຕ້ອງການແກ້ໄຂ</param>
    /// <param name="request">ຂໍ້ມູນໃຝ່</param>
    /// <param name="cancellationToken">Token ສໍາລັບການຍົກເລີກຄໍາສັ່ງ</param>
    Task<DormitoryResponse> UpdateAsync(
        Guid id,
        UpdateDormitoryRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>ປ່ຽນສະຖານະຫໍພັກ</summary>
    /// <param name="id">ລະຫັດຫໍພັກທີ່ທ່ານຕ້ອງການປ່ຽນສະຖານະ</param>
    /// <param name="status">ສະຖານະໃໝ່ຖືກສົ່ງເປັນຊື່ enum ເຊັ່ນ Active</param>
    /// <param name="cancellationToken">Token ສໍາລັບການຍົກເລີກຄໍາສັ່ງ</param>
    Task<DormitoryResponse> ChangeStatusAsync(
        Guid id,
        string status,
        CancellationToken cancellationToken = default);

    /// <summary>ລຶບຫໍພັກ</summary>
    /// <param name="id">ລະຫັດຫໍພັກທີ່ທ່ານຕ້ອງການລຶບ</param>
    /// <param name="cancellationToken">Token ສໍາລັບການຍົກເລີກຄໍາສັ່ງ</param>
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
