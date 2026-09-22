namespace DormFlow.Contracts.Common;

/// <summary>
/// ຜົນໄດ້ຮັບໃນຜ້ານໍາໃຊ້ກັບທັງຝົດ List Endpoint
/// </summary>
/// <typeparam name="TItem">ປະເພດຂອງລາຍການໃນຜ້າ</typeparam>
public class PagedResult<TItem>
{
    /// <summary>ບັນຊີລາຍຊື່ຂອງຂໍ້ມູນສໍາລັບຜ້າປະຈຸບັນ</summary>
    public IReadOnlyList<TItem> Items { get; init; } = [];

    /// <summary>ໝາຍເລກໜ້າປັດຈຸບັນເລີ່ມຕົ້ນທີ່ 1</summary>
    public int Page { get; init; } = PaginationDefaults.FirstPage;

    /// <summary>ຈໍານວນລາຍການຕໍ່ຜ້າ</summary>
    public int PageSize { get; init; } = PaginationDefaults.DefaultPageSize;

    /// <summary>ຈໍານວນລາຍການທັງໝົດທີ່ກົງກັບເງື່ອນໄຂ (ກ່ອນທີ່ຈະແຕກຜ້າ)</summary>
    public long TotalItems { get; init; }

    /// <summary>ຈຳນວນໜ້າທັງໝົດ</summary>
    public int TotalPages => PageSize <= 0 ? 0 : (int)Math.Ceiling(TotalItems / (double)PageSize);

    /// <summary>ມີໜ້າກ່ອນໜ້ານີ້ບໍ?</summary>
    public bool HasPreviousPage => Page > PaginationDefaults.FirstPage;

    /// <summary>ມີໜ້າຕໍ່ໄປບໍ?</summary>
    public bool HasNextPage => Page < TotalPages;
}

/// <summary>
/// ຕົວຊ່ວຍສ້າງ <see cref="PagedResult{TItem}"/>
/// </summary>
/// <remarks>
/// ບໍ່ມີການແບ່ງອອກເປັນຫ້ອງຮຽນ generic ເພື່ອໃຫ້ສາມາດດໍາເນີນການໄດ້ໂດຍບໍ່ຕ້ອງລະບຸປະເພດອີກເທື່ອຜຶ່ງ
/// ແລະໃນຄໍາສັ່ງທີ່ຈະບໍ່ມີ static member ແມ່ນຢູ່ generic type
/// </remarks>
public static class PagedResult
{
    /// <summary>ສ້າງຜົນໄດ້ຮັບ paginated</summary>
    /// <typeparam name="TItem">ປະເພດຂອງລາຍການໃນຜ້າ</typeparam>
    /// <param name="items">ບັນຊີລາຍຊື່ຂອງຂໍ້ມູນສໍາລັບຜ້າປະຈຸບັນ</param>
    /// <param name="page">ໝາຍເລກໜ້າປັດຈຸບັນ</param>
    /// <param name="pageSize">ຈໍານວນລາຍການຕໍ່ຜ້າ</param>
    /// <param name="totalItems">ຈໍານວນລາຍການທັງໝົດທີ່ກົງກັບເງື່ອນໄຂ</param>
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

    /// <summary>ສ້າງຜົນໄດ້ຮັບແບບ paginated ໂດຍບໍ່ມີຂໍ້ມູນ</summary>
    /// <typeparam name="TItem">ປະເພດຂອງລາຍການໃນຜ້າ</typeparam>
    /// <param name="page">ໝາຍເລກໜ້າປັດຈຸບັນ</param>
    /// <param name="pageSize">ຈໍານວນລາຍການຕໍ່ຜ້າ</param>
    public static PagedResult<TItem> Empty<TItem>(
        int page = PaginationDefaults.FirstPage,
        int pageSize = PaginationDefaults.DefaultPageSize)
        => new() { Items = [], Page = page, PageSize = pageSize, TotalItems = 0 };
}
