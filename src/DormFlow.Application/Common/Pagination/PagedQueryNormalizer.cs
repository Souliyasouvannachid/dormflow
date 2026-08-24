using DormFlow.Contracts.Common;

namespace DormFlow.Application.Common.Pagination;

/// <summary>
/// แปลงพารามิเตอร์ดิบจาก Query String ให้เป็น <see cref="PagedQuery"/> ที่ปลอดภัยต่อการนำไปใช้
/// </summary>
/// <remarks>
/// ปรับค่าแทนการโยนข้อผิดพลาด เพราะพารามิเตอร์การแบ่งหน้าที่เกินขอบเขต
/// ไม่ใช่ความผิดพลาดของข้อมูลทางธุรกิจ แต่เป็นการใช้งานที่ระบบควรรองรับได้อย่างปลอดภัย
/// </remarks>
public static class PagedQueryNormalizer
{
    /// <summary>ปรับค่าพารามิเตอร์การแบ่งหน้าให้อยู่ในขอบเขตที่ยอมรับ</summary>
    /// <param name="request">พารามิเตอร์ดิบจากผู้เรียก</param>
    /// <param name="allowedSortFields">
    /// รายชื่อฟิลด์ที่อนุญาตให้เรียงลำดับได้ ป้องกันการส่งชื่อฟิลด์ที่ไม่มีอยู่จริง
    /// ส่ง null เมื่อยังไม่ต้องการจำกัด
    /// </param>
    /// <param name="defaultSortBy">ฟิลด์เรียงลำดับเริ่มต้นเมื่อผู้เรียกไม่ระบุหรือระบุค่าที่ไม่อนุญาต</param>
    public static PagedQuery Normalize(
        PagedQueryRequest request,
        IReadOnlyCollection<string>? allowedSortFields = null,
        string? defaultSortBy = null)
    {
        ArgumentNullException.ThrowIfNull(request);

        int page = request.Page < PaginationDefaults.FirstPage
            ? PaginationDefaults.FirstPage
            : request.Page;

        int pageSize = request.PageSize < PaginationDefaults.MinPageSize
            ? PaginationDefaults.DefaultPageSize
            : Math.Min(request.PageSize, PaginationDefaults.MaxPageSize);

        string? search = string.IsNullOrWhiteSpace(request.Search)
            ? null
            : request.Search.Trim();

        string? sortBy = ResolveSortBy(request.SortBy, allowedSortFields, defaultSortBy);

        return new PagedQuery(page, pageSize, search, sortBy, request.SortDirection);
    }

    private static string? ResolveSortBy(
        string? requestedSortBy,
        IReadOnlyCollection<string>? allowedSortFields,
        string? defaultSortBy)
    {
        if (string.IsNullOrWhiteSpace(requestedSortBy))
        {
            return defaultSortBy;
        }

        string trimmed = requestedSortBy.Trim();

        if (allowedSortFields is null)
        {
            return trimmed;
        }

        // เทียบแบบไม่สนตัวพิมพ์ แต่คืนค่าตามที่ประกาศไว้ในรายการที่อนุญาต
        foreach (string allowed in allowedSortFields)
        {
            if (string.Equals(allowed, trimmed, StringComparison.OrdinalIgnoreCase))
            {
                return allowed;
            }
        }

        return defaultSortBy;
    }
}
