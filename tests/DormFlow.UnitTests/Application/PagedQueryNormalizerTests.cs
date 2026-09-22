using DormFlow.Application.Common.Pagination;
using DormFlow.Contracts.Common;
using FluentAssertions;
using Xunit;

namespace DormFlow.UnitTests.Application;

/// <summary>
/// ໃຫ້ແນ່ໃຈວ່າຕົວກໍານົດການຈັດສັນທີ່ຜູ້ໂທສົ່ງຖືກປັບສະເຝີພາຍໃນຂອບເຂດຈໍາກັດທີ່ປອດໄພ
/// ເນື່ອງຈາກວ່າມູນຄ່າ pageSize=100000 ມັນດຶງຕາຕະລາງທັງຝົດໃນຄໍາຮ້ອງຂໍດຽວ
/// </summary>
public class PagedQueryNormalizerTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-999)]
    public void ໜ້ອຍກວ່າໜຶ່ງໜ້າ_ຕ້ອງຕັ້ງເປັນຜ້າທໍາອິດ(int requestedPage)
    {
        PagedQuery query = PagedQueryNormalizer.Normalize(new PagedQueryRequest { Page = requestedPage });

        query.Page.Should().Be(PaginationDefaults.FirstPage);
    }

    [Fact]
    public void ຂະໜາດໃບໜ້າເກີນເພດານ_ຕ້ອງໄດ້ຮັບການຕ່ໍາສຸດເພດານ()
    {
        PagedQuery query = PagedQueryNormalizer.Normalize(new PagedQueryRequest { PageSize = 100_000 });

        query.PageSize.Should().Be(PaginationDefaults.MaxPageSize);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public void ຂະໜາດໜ້າໜ້ອຍກວ່າໜຶ່ງ_ຕ້ອງກັບຄືນຫາຄ່າເລີ່ມຕົ້ນ(int requestedPageSize)
    {
        PagedQuery query = PagedQueryNormalizer.Normalize(new PagedQueryRequest { PageSize = requestedPageSize });

        query.PageSize.Should().Be(PaginationDefaults.DefaultPageSize);
    }

    [Fact]
    public void ຊອກຫາຄໍາ_ຊ່ອງຫວ່າງຫົວແລະຫາງຕ້ອງໄດ້ຮັບການຕັດ()
    {
        PagedQuery query = PagedQueryNormalizer.Normalize(new PagedQueryRequest { Search = "  A101  " });

        query.Search.Should().Be("A101");
        query.HasSearch.Should().BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void ຄໍາຄົ້ນຫາຫວ່າງເປົ່າ_ຕ້ອງກາຍເປັນ_null(string? search)
    {
        PagedQuery query = PagedQueryNormalizer.Normalize(new PagedQueryRequest { Search = search });

        query.Search.Should().BeNull();
        query.HasSearch.Should().BeFalse();
    }

    [Fact]
    public void ການຈັດຮຽງຊ່ອງຂໍ້ມູນບໍ່ໄດ້ຢູ່ໃນລາຍການທີ່ອະນຸຍາດ_ຕ້ອງກັບຄືນຫາຄ່າເລີ່ມຕົ້ນ()
    {
        PagedQuery query = PagedQueryNormalizer.Normalize(
            new PagedQueryRequest { SortBy = "'; DROP TABLE rooms; --" },
            allowedSortFields: ["roomNumber", "monthlyRent"],
            defaultSortBy: "roomNumber");

        query.SortBy.Should().Be("roomNumber");
    }

    [Fact]
    public void ຈັດຮຽງຊ່ອງຂໍ້ມູນລວມຢູ່ໃນລາຍການອະນຸຍາດ_ຕ້ອງສົ່ງຄືນຄ່າຕາມທີ່ປະກາດ()
    {
        PagedQuery query = PagedQueryNormalizer.Normalize(
            new PagedQueryRequest { SortBy = "MONTHLYRENT" },
            allowedSortFields: ["roomNumber", "monthlyRent"],
            defaultSortBy: "roomNumber");

        query.SortBy.Should().Be("monthlyRent");
    }

    [Fact]
    public void Skip_ແລະ_Take_ຕ້ອງໄດ້ຮັບການຄິດໄລ່ຈາກຜ້າທີ່ປັບ()
    {
        PagedQuery query = PagedQueryNormalizer.Normalize(new PagedQueryRequest { Page = 3, PageSize = 20 });

        query.Skip.Should().Be(40);
        query.Take.Should().Be(20);
    }

    [Fact]
    public void ທິດທາງການຈັດຮຽງ_ຄ່າທີ່ລະບຸໄວ້ໂດຍຜູ້ໂທຕ້ອງຖືກຮັກສາໄວ້()
    {
        PagedQuery query = PagedQueryNormalizer.Normalize(
            new PagedQueryRequest { SortDirection = SortDirection.Desc });

        query.SortDirection.Should().Be(SortDirection.Desc);
    }

    [Fact]
    public void Normalize_ບໍ່ຄວນຍອມຮັບ_request_ນັ້ນແມ່ນ_null()
    {
        Action act = () => PagedQueryNormalizer.Normalize(null!);

        act.Should().Throw<ArgumentNullException>();
    }
}
