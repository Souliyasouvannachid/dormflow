namespace DormFlow.Contracts.Common;

/// <summary>
/// ຕົວກໍານົດການມາດຕະຖານຂອງທຸກໆ List Endpoint
/// ມູນຄ່າທີ່ໄດ້ຮັບແມ່ນມູນຄ່າດິບຈາກ Query String ແລະຕ້ອງຖືກປັບໃຫ້ຢູ່ໃນຂອບເຂດທີ່ຍອມຮັບໄດ້ຕະຫຼອດເວລາກ່ອນທີ່ຈະໃຊ້
/// </summary>
public class PagedQueryRequest
{
    /// <summary>ໝາຍເລກໜ້າທີ່ຕ້ອງການເລີ່ມຕົ້ນທີ່ 1</summary>
    public int Page { get; set; } = PaginationDefaults.FirstPage;

    /// <summary>ຈໍານວນລາຍການຕໍ່ຜ້າ, ສູງສຸດ <see cref="PaginationDefaults.MaxPageSize"/></summary>
    public int PageSize { get; set; } = PaginationDefaults.DefaultPageSize;

    /// <summary>ເງື່ອນໄຂການຊອກຫາຂໍ້ຄວາມຟຣີ</summary>
    public string? Search { get; set; }

    /// <summary>ຊື່ຂອງຊ່ອງຂໍ້ມູນທີ່ໃຊ້ໃນການຈັດຮຽງ, e.g roomNumber</summary>
    public string? SortBy { get; set; }

    /// <summary>ທິດທາງການຈັດຮຽງ</summary>
    public SortDirection SortDirection { get; set; } = SortDirection.Asc;
}
