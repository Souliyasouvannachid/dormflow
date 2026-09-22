using DormFlow.Contracts.Common;

namespace DormFlow.Application.Common.Pagination;

/// <summary>
/// ແປງຕົວກໍານົດການດິບຈາກ Query String ໃຫ້ມັນເປັນ <see cref="PagedQuery"/> ທີ່ປອດໄພໃນການນໍາໃຊ້
/// </summary>
/// <remarks>
/// ປັບຄ່າແທນທີ່ຈະຖິ້ມຄວາມຜິດພາດ. ເນື່ອງຈາກວ່າຕົວກໍານົດການແບ່ງຜ້າເກີນຂອບເຂດ
/// ມັນບໍ່ແມ່ນຄວາມຜິດພາດໃນຂໍ້ມູນທຸລະກິດ. ແຕ່ມັນເປັນການນໍາໃຊ້ທີ່ລະບົບຄວນຈະສາມາດສະຜັບສະຜູນໄດ້ຢ່າງປອດໄພ
/// </remarks>
public static class PagedQueryNormalizer
{
    /// <summary>ປັບຕົວກໍານົດການ pagination ພາຍໃນຂອບເຂດຈໍາກັດທີ່ຍອມຮັບໄດ້</summary>
    /// <param name="request">ຕົວກໍານົດການດິບຈາກຜູ້ໂທ</param>
    /// <param name="allowedSortFields">
    /// ລາຍຊື່ຊ່ອງຂໍ້ມູນທີ່ອະນຸຍາດໃຫ້ຈັດຮຽງ ປ້ອງກັນບໍ່ໃຫ້ສົ່ງຊື່ຊ່ອງຂໍ້ມູນທີ່ບໍ່ມີຢູ່ແລ້ວ
    /// ສົ່ງ null ໃນເວລາທີ່ທ່ານບໍ່ຕ້ອງການທີ່ຈະຈໍາກັດມັນເທື່ອ
    /// </param>
    /// <param name="defaultSortBy">ຊ່ອງຂໍ້ມູນການຈັດຮຽງເປັນຄ່າເລີ່ມຕົ້ນເມື່ອຜູ້ໂທບໍ່ໄດ້ລະບຸ ຫຼືລະບຸຄ່າທີ່ບໍ່ອະນຸຍາດ</param>
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

        // ປຽບທຽບໂດຍບໍ່ມີການເອົາໃຈໃສ່ກັບກໍລະນີ. ແຕ່ມັນຟື້ນຟູມູນຄ່າຕາມທີ່ປະກາດໄວ້ໃນບັນຊີຂາວ
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
