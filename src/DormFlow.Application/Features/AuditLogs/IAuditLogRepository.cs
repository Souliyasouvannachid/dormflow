using DormFlow.Application.Common.Pagination;
using DormFlow.Contracts.Common;
using DormFlow.Domain.Entities;
using DormFlow.Domain.Enums;

namespace DormFlow.Application.Features.AuditLogs;

/// <summary>ການກັ່ນຕອງສໍາລັບການດຶງລາຍການ Audit Log ປະເພດການແບ່ງໜ້າ</summary>
public sealed record AuditLogListFilter(
    Guid? OrganizationId,
    Guid? DormitoryId,
    Guid? UserId,
    AuditAction? Action,
    string? EntityName,
    DateTimeOffset? From,
    DateTimeOffset? To,
    IReadOnlyCollection<Guid>? RestrictToDormitoryIds);

/// <summary>ເຂົ້າເຖິງຂໍ້ມູນ Audit Log ໃນຖານຂໍ້ມູນ</summary>
public interface IAuditLogRepository
{
    /// <summary>ດຶງເອົາລາຍການ Audit Log Pagination ອີງຕາມຕົວກອງທີ່ລະບຸ ຄັດຈາກເຫດການຫຼ້າສຸດໄປຫາເຫດການເກົ່າແກ່ທີ່ສຸດ</summary>
    Task<PagedResult<AuditLog>> GetPagedAsync(
        AuditLogListFilter filter,
        PagedQuery query,
        CancellationToken cancellationToken = default);
}
