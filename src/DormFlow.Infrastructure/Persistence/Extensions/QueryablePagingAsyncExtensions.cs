using DormFlow.Application.Common.Pagination;
using DormFlow.Contracts.Common;
using Microsoft.EntityFrameworkCore;

namespace DormFlow.Infrastructure.Persistence.Extensions;

/// <summary>
/// ສ່ວນຂະຫຍາຍທີ່ແປງແລ້ວ <see cref="IQueryable{T}"/> ຂອງ EF Core ເຂົ້າໄປໃນຜົນໄດ້ຮັບ paginated
/// </summary>
/// <remarks>
/// ໃນຫ້ອງຮຽນ Infrastructure ເນື່ອງຈາກວ່າຂ້າພະເຈົ້າຕ້ອງຂຶ້ນກັບ EF Core ເພື່ອສັ່ງ query ຕົວແບບ asynchronous
/// ເຫດຜົນການຄິດໄລ່ຜ້າແມ່ນຢູ່ໃນຊັ້ນ Application ທີ່ສາມາດທົດສອບໄດ້ໂດຍບໍ່ມີຖານຂໍ້ມູນ
/// </remarks>
public static class QueryablePagingAsyncExtensions
{
    /// <summary>
    /// ນັບຈໍານວນລາຍການທັງຝົດແລະສະກັດພຽງແຕ່ຜ້າທີ່ຕ້ອງການ
    /// </summary>
    /// <typeparam name="TSource">ປະເພດຂອງຂໍ້ມູນໃນ Query</typeparam>
    /// <param name="source">Query ບ່ອນທີ່ເງື່ອນໄຂໄດ້ຖືກກັ່ນຕອງແລະຈັດຮຽງຢ່າງສໍາເລັດຜົນ</param>
    /// <param name="query">ປັບຕົວກໍານົດການ pagination</param>
    /// <param name="cancellationToken">Token ສໍາລັບການຍົກເລີກຄໍາສັ່ງ</param>
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
    /// ປ່ຽນຜົນໄດ້ຮັບເປັນຮູບແບບ pagination ດ້ວຍການສ້າງແຜນທີ່ເປັນ DTO ໃນຄໍາສັ່ງຜຶ່ງ
    /// </summary>
    /// <typeparam name="TSource">ປະເພດຂອງຂໍ້ມູນໃນ Query</typeparam>
    /// <typeparam name="TResult">ປະເພດຂອງ DTO ຕ້ອງການ</typeparam>
    /// <param name="source">Query ບ່ອນທີ່ເງື່ອນໄຂໄດ້ຖືກກັ່ນຕອງແລະຈັດຮຽງຢ່າງສໍາເລັດຜົນ</param>
    /// <param name="selector">ການສະແດງອອກແຜນທີ່ຂໍ້ມູນແມ່ນ DTO ເຮັດວຽກຢູ່ໃນດ້ານຖານຂໍ້ມູນ</param>
    /// <param name="query">ປັບຕົວກໍານົດການ pagination</param>
    /// <param name="cancellationToken">Token ສໍາລັບການຍົກເລີກຄໍາສັ່ງ</param>
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
