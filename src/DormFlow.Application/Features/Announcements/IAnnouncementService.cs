using DormFlow.Contracts.Announcements;
using DormFlow.Contracts.Common;

namespace DormFlow.Application.Features.Announcements;

/// <summary>Use Case ทั้งหมดของโมดูลประกาศ</summary>
public interface IAnnouncementService
{
    /// <summary>ดึงรายการประกาศแบบแบ่งหน้า</summary>
    Task<PagedResult<AnnouncementResponse>> GetPagedAsync(
        AnnouncementListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>อ่านข้อมูลประกาศจากรหัสอ้างอิง</summary>
    Task<AnnouncementResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>สร้างประกาศใหม่ในสถานะร่าง</summary>
    Task<AnnouncementResponse> CreateAsync(
        CreateAnnouncementRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>เผยแพร่ประกาศ ทำได้เฉพาะประกาศที่ยังเป็นร่างเท่านั้น</summary>
    Task<AnnouncementResponse> PublishAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>เก็บประกาศเข้าคลัง ทำได้เฉพาะประกาศที่เผยแพร่แล้วเท่านั้น</summary>
    Task<AnnouncementResponse> ArchiveAsync(Guid id, CancellationToken cancellationToken = default);
}
