using DormFlow.Contracts.Common;

namespace DormFlow.Application.Common.Pagination;

/// <summary>
/// ການຂະຫຍາຍຂອງ <see cref="IQueryable{T}"/> ສໍາລັບ pagination
/// </summary>
public static class QueryablePagingExtensions
{
    /// <summary>ໃສ່ເງື່ອນໄຂ Skip ແລະ Take ອີງຕາມຕົວກໍານົດການ pagination</summary>
    /// <typeparam name="TSource">ປະເພດຂອງຂໍ້ມູນໃນ Query</typeparam>
    /// <param name="source">Query ຕົ້ນກໍາເນີດ</param>
    /// <param name="query">ປັບຕົວກໍານົດການ pagination</param>
    public static IQueryable<TSource> ApplyPaging<TSource>(this IQueryable<TSource> source, PagedQuery query)
    {
        ArgumentNullException.ThrowIfNull(source);

        return source.Skip(query.Skip).Take(query.Take);
    }

    /// <summary>ຮຽງລໍາດັບໃນທິດທາງທີ່ກໍານົດໄວ້</summary>
    /// <typeparam name="TSource">ປະເພດຂອງຂໍ້ມູນໃນ Query</typeparam>
    /// <typeparam name="TKey">ປະເພດຂອງກະແຈຈັດຮຽງທີ່ໃຊ້</typeparam>
    /// <param name="source">Query ຕົ້ນກໍາເນີດ</param>
    /// <param name="keySelector">ການຄັດເລືອກການສະແດງອອກທີ່ສໍາຄັນ</param>
    /// <param name="direction">ທິດທາງການຈັດຮຽງ</param>
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

    /// <summary>ການຈັດຮຽງສືບຕໍ່ຈາກຄີທີ່ຜ່ານມາໃນທິດທາງທີ່ກໍານົດໄວ້</summary>
    /// <typeparam name="TSource">ປະເພດຂອງຂໍ້ມູນໃນ Query</typeparam>
    /// <typeparam name="TKey">ປະເພດຂອງກະແຈຈັດຮຽງທີ່ໃຊ້</typeparam>
    /// <param name="source">Query ຈັດຮຽງ</param>
    /// <param name="keySelector">ການຄັດເລືອກການສະແດງອອກທີ່ສໍາຄັນ</param>
    /// <param name="direction">ທິດທາງການຈັດຮຽງ</param>
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
