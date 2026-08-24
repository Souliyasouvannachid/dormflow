namespace DormFlow.Contracts.Common;

/// <summary>
/// ผลลัพธ์แบบแบ่งหน้าที่ใช้กับทุก List Endpoint
/// </summary>
/// <typeparam name="TItem">ชนิดของรายการในหน้า</typeparam>
public class PagedResult<TItem>
{
    /// <summary>รายการข้อมูลของหน้าปัจจุบัน</summary>
    public IReadOnlyList<TItem> Items { get; init; } = [];

    /// <summary>หมายเลขหน้าปัจจุบัน เริ่มที่ 1</summary>
    public int Page { get; init; } = PaginationDefaults.FirstPage;

    /// <summary>จำนวนรายการต่อหน้า</summary>
    public int PageSize { get; init; } = PaginationDefaults.DefaultPageSize;

    /// <summary>จำนวนรายการทั้งหมดที่ตรงเงื่อนไข (ก่อนแบ่งหน้า)</summary>
    public long TotalItems { get; init; }

    /// <summary>จำนวนหน้าทั้งหมด</summary>
    public int TotalPages => PageSize <= 0 ? 0 : (int)Math.Ceiling(TotalItems / (double)PageSize);

    /// <summary>มีหน้าก่อนหน้าหรือไม่</summary>
    public bool HasPreviousPage => Page > PaginationDefaults.FirstPage;

    /// <summary>มีหน้าถัดไปหรือไม่</summary>
    public bool HasNextPage => Page < TotalPages;
}

/// <summary>
/// ตัวช่วยสร้าง <see cref="PagedResult{TItem}"/>
/// </summary>
/// <remarks>
/// แยกเป็นคลาสไม่มี generic เพื่อให้เรียกใช้ได้โดยไม่ต้องระบุชนิดซ้ำ
/// และเพื่อไม่ให้มี static member อยู่บน generic type
/// </remarks>
public static class PagedResult
{
    /// <summary>สร้างผลลัพธ์แบบแบ่งหน้า</summary>
    /// <typeparam name="TItem">ชนิดของรายการในหน้า</typeparam>
    /// <param name="items">รายการข้อมูลของหน้าปัจจุบัน</param>
    /// <param name="page">หมายเลขหน้าปัจจุบัน</param>
    /// <param name="pageSize">จำนวนรายการต่อหน้า</param>
    /// <param name="totalItems">จำนวนรายการทั้งหมดที่ตรงเงื่อนไข</param>
    public static PagedResult<TItem> Create<TItem>(
        IReadOnlyList<TItem> items,
        int page,
        int pageSize,
        long totalItems)
    {
        ArgumentNullException.ThrowIfNull(items);

        return new PagedResult<TItem>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalItems = totalItems,
        };
    }

    /// <summary>สร้างผลลัพธ์แบบแบ่งหน้าที่ไม่มีข้อมูล</summary>
    /// <typeparam name="TItem">ชนิดของรายการในหน้า</typeparam>
    /// <param name="page">หมายเลขหน้าปัจจุบัน</param>
    /// <param name="pageSize">จำนวนรายการต่อหน้า</param>
    public static PagedResult<TItem> Empty<TItem>(
        int page = PaginationDefaults.FirstPage,
        int pageSize = PaginationDefaults.DefaultPageSize)
        => new() { Items = [], Page = page, PageSize = pageSize, TotalItems = 0 };
}
