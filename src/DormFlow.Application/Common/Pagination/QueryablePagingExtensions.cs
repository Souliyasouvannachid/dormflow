using DormFlow.Contracts.Common;

namespace DormFlow.Application.Common.Pagination;

/// <summary>
/// ส่วนขยายของ <see cref="IQueryable{T}"/> สำหรับการแบ่งหน้า
/// </summary>
public static class QueryablePagingExtensions
{
    /// <summary>ใส่เงื่อนไข Skip และ Take ตามพารามิเตอร์การแบ่งหน้า</summary>
    /// <typeparam name="TSource">ชนิดของข้อมูลใน Query</typeparam>
    /// <param name="source">Query ต้นทาง</param>
    /// <param name="query">พารามิเตอร์การแบ่งหน้าที่ปรับค่าแล้ว</param>
    public static IQueryable<TSource> ApplyPaging<TSource>(this IQueryable<TSource> source, PagedQuery query)
    {
        ArgumentNullException.ThrowIfNull(source);

        return source.Skip(query.Skip).Take(query.Take);
    }

    /// <summary>เรียงลำดับตามทิศทางที่ระบุ</summary>
    /// <typeparam name="TSource">ชนิดของข้อมูลใน Query</typeparam>
    /// <typeparam name="TKey">ชนิดของคีย์ที่ใช้เรียง</typeparam>
    /// <param name="source">Query ต้นทาง</param>
    /// <param name="keySelector">นิพจน์เลือกคีย์ที่ใช้เรียง</param>
    /// <param name="direction">ทิศทางการเรียงลำดับ</param>
    public static IOrderedQueryable<TSource> OrderByDirection<TSource, TKey>(
        this IQueryable<TSource> source,
        System.Linq.Expressions.Expression<Func<TSource, TKey>> keySelector,
        SortDirection direction)
    {
        ArgumentNullException.ThrowIfNull(source);

        return direction == SortDirection.Desc
            ? source.OrderByDescending(keySelector)
            : source.OrderBy(keySelector);
    }

    /// <summary>เรียงลำดับต่อจากคีย์ก่อนหน้าตามทิศทางที่ระบุ</summary>
    /// <typeparam name="TSource">ชนิดของข้อมูลใน Query</typeparam>
    /// <typeparam name="TKey">ชนิดของคีย์ที่ใช้เรียง</typeparam>
    /// <param name="source">Query ที่เรียงลำดับแล้ว</param>
    /// <param name="keySelector">นิพจน์เลือกคีย์ที่ใช้เรียง</param>
    /// <param name="direction">ทิศทางการเรียงลำดับ</param>
    public static IOrderedQueryable<TSource> ThenByDirection<TSource, TKey>(
        this IOrderedQueryable<TSource> source,
        System.Linq.Expressions.Expression<Func<TSource, TKey>> keySelector,
        SortDirection direction)
    {
        ArgumentNullException.ThrowIfNull(source);

        return direction == SortDirection.Desc
            ? source.ThenByDescending(keySelector)
            : source.ThenBy(keySelector);
    }
}
