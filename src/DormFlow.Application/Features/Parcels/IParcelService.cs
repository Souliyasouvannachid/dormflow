using DormFlow.Contracts.Common;
using DormFlow.Contracts.Parcels;
using DormFlow.Domain.Enums;

namespace DormFlow.Application.Features.Parcels;

/// <summary>Use Case ທັງຝົດຂອງໂມດູນ parcel</summary>
public interface IParcelService
{
    /// <summary>ດຶງເອົາບັນຊີລາຍຊື່ paginated ຂອງ parcels</summary>
    Task<PagedResult<ParcelResponse>> GetPagedAsync(
        ParcelListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>ນັບ parcel ຕາມສະຖານະ</summary>
    Task<int> GetCountAsync(ParcelStatus status, CancellationToken cancellationToken = default);

    /// <summary>ອ່ານຂໍ້ມູນ parcel ຈາກລະຫັດອ້າງອີງ</summary>
    Task<ParcelResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>ບັນທຶກການມາເຖິງ parcel ໃຝ່</summary>
    Task<ParcelResponse> LogAsync(
        LogParcelRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>ບັນທຶກການຮັບ parcel ນີ້ສາມາດເຮັດໄດ້ພຽງແຕ່ສໍາລັບ parcel ທີ່ຍັງບໍ່ທັນໄດ້ຮັບ</summary>
    Task<ParcelResponse> PickupAsync(
        Guid id,
        PickupParcelRequest request,
        CancellationToken cancellationToken = default);
}
