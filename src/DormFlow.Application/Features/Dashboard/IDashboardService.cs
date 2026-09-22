using DormFlow.Contracts.Dashboard;

namespace DormFlow.Application.Features.Dashboard;

/// <summary>Use Case ຂອງ dashboard ໄດ້</summary>
public interface IDashboardService
{
    /// <summary>ສະຜອງພາບລວມຂອງ dashboard ສໍາລັບຜູ້ໃຊ້ເຂົ້າສູ່ລະບົບ</summary>
    Task<DashboardSummaryResponse> GetSummaryAsync(CancellationToken cancellationToken = default);
}
