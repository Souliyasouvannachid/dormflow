using DormFlow.Contracts.Common;
using DormFlow.Contracts.Receipts;

namespace DormFlow.Application.Features.Receipts;

/// <summary>Use Case ໂມດູນໃບຮັບເງິນທັງໝົດ</summary>
public interface IReceiptService
{
    /// <summary>ດຶງເອົາລາຍຊື່ໃບຮັບເງິນທີ່ໄດ້ວາງໄວ້ແລ້ວ</summary>
    Task<PagedResult<ReceiptResponse>> GetPagedAsync(
        ReceiptListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>ອ່ານຂໍ້ມູນໃບຮັບເງິນຈາກລະຫັດອ້າງອີງ</summary>
    Task<ReceiptResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// ອອກໃບຮັບເງິນສໍາລັບລາຍການການຈ່າຍເງິນທີ່ຢືນຢັນ. ທຸລະກຳການຈ່າຍເງິນໜຶ່ງສາມາດອອກໄດ້ເທື່ອດຽວເທົ່ານັ້ນ
    /// </summary>
    Task<ReceiptResponse> IssueAsync(
        IssueReceiptRequest request,
        CancellationToken cancellationToken = default);
}
