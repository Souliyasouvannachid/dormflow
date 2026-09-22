using DormFlow.Contracts.Common;

namespace DormFlow.Application.Common.Pagination;

/// <summary>
/// ພາລາມິເຕີຜ້າທີ່ກວດສອບແລະດັດແປງພາຍໃນຂອບເຂດຈໍາກັດທີ່ຍອມຮັບໄດ້
/// ຊັ້ນ Application ຈະຍອມຮັບພຽງແຕ່ປະເພດນີ້. ບໍ່ໄດ້ຮັບຄ່າດິບຈາກ Query String ໂດຍກົງ
/// </summary>
/// <param name="Page">ເລກໜ້າເລີ່ມຕົ້ນທີ່ 1</param>
/// <param name="PageSize">ຈຳນວນລາຍການຕໍ່ໜ້າແມ່ນຢູ່ລະຫວ່າງ MinPageSize ກັບ MaxPageSize</param>
/// <param name="Search">ຄຳສັບຊອກຫາທີ່ມີຊ່ອງຫວ່າງອອກແມ່ນ: null ເມື່ອບໍ່ໄດ້ລະບຸ</param>
/// <param name="SortBy">ຊື່ຂອງພາກສະຜາມທີ່ໃຊ້ໃນການຈັດລຽງແມ່ນ null ເມື່ອບໍ່ໄດ້ລະບຸ</param>
/// <param name="SortDirection">ທິດທາງການຈັດຮຽງ</param>
public readonly record struct PagedQuery(
    int Page,
    int PageSize,
    string? Search,
    string? SortBy,
    SortDirection SortDirection)
{
    /// <summary>ຈໍານວນລາຍການທີ່ຕ້ອງຂ້າມກ່ອນທີ່ຈະໄປເຖິງຜ້າທີ່ຕ້ອງການ</summary>
    public int Skip => (Page - PaginationDefaults.FirstPage) * PageSize;

    /// <summary>ຈໍານວນລາຍການທີ່ຈະເອົາມາ</summary>
    public int Take => PageSize;

    /// <summary>ມີຄໍາຄົ້ນຫາທີ່ໃຊ້ໄດ້ບໍ?</summary>
    public bool HasSearch => !string.IsNullOrWhiteSpace(Search);
}
