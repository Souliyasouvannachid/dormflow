using DormFlow.Contracts.Common;
using FluentAssertions;
using Xunit;

namespace DormFlow.UnitTests.Contracts;

/// <summary>ກວດເບິ່ງການຄິດໄລ່ການນັບໜ້າ ແລະທຸງໜ້າຕໍ່ໄປສຳລັບຜົນໄດ້ຮັບທີ່ເຮັດເປັນ paginated</summary>
public class PagedResultTests
{
    [Theory]
    [InlineData(100, 20, 5)]
    [InlineData(101, 20, 6)]
    [InlineData(19, 20, 1)]
    [InlineData(0, 20, 0)]
    [InlineData(1, 1, 1)]
    public void TotalPages_ສະເໝີ(long totalItems, int pageSize, int expectedTotalPages)
    {
        PagedResult<int> result = PagedResult.Create<int>([], 1, pageSize, totalItems);

        result.TotalPages.Should().Be(expectedTotalPages);
    }

    [Fact]
    public void ຜ້າທໍາອິດ_ຈະຕ້ອງບໍ່ມີໜ້າກ່ອນໜ້າແຕ່ຕ້ອງມີໜ້າຕໍ່ໄປ()
    {
        PagedResult<int> result = PagedResult.Create([1, 2, 3], page: 1, pageSize: 20, totalItems: 100);

        result.HasPreviousPage.Should().BeFalse();
        result.HasNextPage.Should().BeTrue();
    }

    [Fact]
    public void ໜ້າສຸດທ້າຍ_ຕ້ອງມີໜ້າກ່ອນໜ້າແຕ່ບໍ່ແມ່ນໜ້າຕໍ່ໄປ()
    {
        PagedResult<int> result = PagedResult.Create([1, 2, 3], page: 5, pageSize: 20, totalItems: 100);

        result.HasPreviousPage.Should().BeTrue();
        result.HasNextPage.Should().BeFalse();
    }

    [Fact]
    public void Empty_ຈະຕ້ອງບໍ່ມີລາຍການແລະບໍ່ມີໜ້າຕໍ່ໄປ()
    {
        PagedResult<string> result = PagedResult.Empty<string>(page: 3, pageSize: 50);

        result.Items.Should().BeEmpty();
        result.TotalItems.Should().Be(0);
        result.TotalPages.Should().Be(0);
        result.HasNextPage.Should().BeFalse();
        result.Page.Should().Be(3);
        result.PageSize.Should().Be(50);
    }

    [Fact]
    public void Create_ຕ້ອງບໍ່ຍອມຮັບລາຍການທີ່ມີ_null()
    {
        Action act = () => PagedResult.Create<int>(null!, 1, 20, 0);

        act.Should().Throw<ArgumentNullException>();
    }
}
