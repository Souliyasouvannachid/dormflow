using DormFlow.Application.Common.Pagination;
using DormFlow.Contracts.Common;
using Microsoft.EntityFrameworkCore;

namespace DormFlow.Infrastructure.Persistence.Extensions;

/// <summary>
/// ส่วนขยายที่แปลง <see cref="IQueryable{T}"/> ของ EF Core ให้เป็นผลลัพธ์แบบแบ่งหน้า
/// </summary>
/// <remarks>
/// อยู่ในชั้น Infrastructure เพราะต้องพึ่ง EF Core ในการสั่ง query แบบ asynchronous
/// ส่วนตรรกะการคำนวณหน้าอยู่ในชั้น Application ที่ทดสอบได้โดยไม่ต้องมีฐานข้อมูล
/// </remarks>
public static class QueryablePagingAsyncExtensions
{
    /// <summary>
    /// นับจำนวนรายการทั้งหมดแล้วดึงเฉพาะหน้าที่ต้องการ
    /// </summary>
    /// <typeparam name="TSource">ชนิดของข้อมูลใน Query</typeparam>
    /// <param name="source">Query ที่ใส่เงื่อนไขกรองและเรียงลำดับเรียบร้อยแล้ว</param>
    /// <param name="query">พารามิเตอร์การแบ่งหน้าที่ปรับค่าแล้ว</param>
    /// <param name="cancellationToken">โทเคนสำหรับยกเลิกคำสั่ง</param>
    public static async Task<PagedResult<TSource>> ToPagedResultAsync<TSource>(
        this IQueryable<TSource> source,
        PagedQuery query,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(source);

        long totalItems = await source.LongCountAsync(cancellationToken);

        if (totalItems == 0)
        {
            return PagedResult.Empty<TSource>(query.Page, query.PageSize);
        }

        List<TSource> items = await source
            .ApplyPaging(query)
            .ToListAsync(cancellationToken);

        return PagedResult.Create(items, query.Page, query.PageSize, totalItems);
    }

    /// <summary>
    /// แปลงผลลัพธ์เป็นรูปแบบแบ่งหน้าพร้อมแม็ปเป็น DTO ในคำสั่งเดียว
    /// </summary>
    /// <typeparam name="TSource">ชนิดของข้อมูลใน Query</typeparam>
    /// <typeparam name="TResult">ชนิดของ DTO ที่ต้องการ</typeparam>
    /// <param name="source">Query ที่ใส่เงื่อนไขกรองและเรียงลำดับเรียบร้อยแล้ว</param>
    /// <param name="selector">นิพจน์แม็ปข้อมูลเป็น DTO ทำงานที่ฝั่งฐานข้อมูล</param>
    /// <param name="query">พารามิเตอร์การแบ่งหน้าที่ปรับค่าแล้ว</param>
    /// <param name="cancellationToken">โทเคนสำหรับยกเลิกคำสั่ง</param>
    public static Task<PagedResult<TResult>> ToPagedResultAsync<TSource, TResult>(
        this IQueryable<TSource> source,
        System.Linq.Expressions.Expression<Func<TSource, TResult>> selector,
        PagedQuery query,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(source);

        return source.Select(selector).ToPagedResultAsync(query, cancellationToken);
    }
}
