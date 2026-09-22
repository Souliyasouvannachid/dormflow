using DormFlow.Contracts.Announcements;
using DormFlow.Contracts.Common;

namespace DormFlow.Application.Features.Announcements;

/// <summary>Use Case ໂມດູນການປະກາດທັງໝົດ</summary>
public interface IAnnouncementService
{
    /// <summary>ດຶງເອົາລາຍຊື່ການປະກາດແບບ paginated</summary>
    Task<PagedResult<AnnouncementResponse>> GetPagedAsync(
        AnnouncementListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>ອ່ານຂໍ້ມູນການປະກາດຈາກລະຫັດການສົ່ງຕໍ່</summary>
    Task<AnnouncementResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>ສ້າງການປະກາດໃໝ່ໃນສະຖານະສະບັບຮ່າງ</summary>
    Task<AnnouncementResponse> CreateAsync(
        CreateAnnouncementRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>ປະກາດການປະກາດພຽງແຕ່ການປະກາດທີ່ຍັງຮ່າງແມ່ນສາມາດເຮັດໄດ້</summary>
    Task<AnnouncementResponse> PublishAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>ການປະກາດຂອງຮ້ານໃນສາງສາມາດເຮັດການປະກາດທີ່ໄດ້ຖືກເຜີຍແຜ່ແລ້ວເທົ່ານັ້ນ</summary>
    Task<AnnouncementResponse> ArchiveAsync(Guid id, CancellationToken cancellationToken = default);
}
