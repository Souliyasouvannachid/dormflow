using DormFlow.Contracts.Common;
using DormFlow.Contracts.Settings;

namespace DormFlow.Application.Features.Settings;

/// <summary>Use Case ทั้งหมดของโมดูลค่าตั้ง</summary>
public interface ISettingService
{
    /// <summary>ดึงรายการค่าตั้งแบบแบ่งหน้า</summary>
    Task<PagedResult<SettingResponse>> GetPagedAsync(
        SettingListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>อ่านค่าตั้งจากคีย์</summary>
    Task<SettingResponse> GetByKeyAsync(
        string key,
        Guid? organizationId,
        CancellationToken cancellationToken = default);

    /// <summary>สร้างหรือแก้ไขค่าตั้งตามคีย์ที่ระบุ</summary>
    Task<SettingResponse> UpsertAsync(
        string key,
        UpsertSettingRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>ลบค่าตั้งตามคีย์ที่ระบุ</summary>
    Task DeleteAsync(string key, Guid? organizationId, CancellationToken cancellationToken = default);
}
