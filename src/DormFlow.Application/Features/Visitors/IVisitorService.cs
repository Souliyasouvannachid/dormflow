using DormFlow.Contracts.Common;
using DormFlow.Contracts.Visitors;
using DormFlow.Domain.Enums;

namespace DormFlow.Application.Features.Visitors;

/// <summary>Use Case ໂມດູນແຂກທັງໝົດ</summary>
public interface IVisitorService
{
    /// <summary>ດຶງເອົາລາຍຊື່ຜູ້ເຂົ້າຊົມທີ່ມີ paginated</summary>
    Task<PagedResult<VisitorResponse>> GetPagedAsync(
        VisitorListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>ນັບຜູ້ເຂົ້າຊົມຕາມສະຖານະ</summary>
    Task<int> GetCountAsync(VisitorStatus status, CancellationToken cancellationToken = default);

    /// <summary>ອ່ານຂໍ້ມູນຜູ້ເຂົ້າຊົມຈາກລະຫັດການສົ່ງຕໍ່</summary>
    Task<VisitorResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>ບັນທຶກການເຂົ້າມາຂອງນັກທ່ອງທ່ຽວໃຝ່</summary>
    Task<VisitorResponse> CheckInAsync(
        CheckInVisitorRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>ການອອກຈາກບັນທຶກສາມາດເຮັດໄດ້ໂດຍຜູ້ເຂົ້າຊົມທີ່ຍັງບໍ່ທັນໄດ້ອອກ</summary>
    Task<VisitorResponse> CheckOutAsync(Guid id, CancellationToken cancellationToken = default);
}
