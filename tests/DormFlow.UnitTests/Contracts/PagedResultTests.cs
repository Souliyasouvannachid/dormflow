using DormFlow.Contracts.Common;
using FluentAssertions;
using Xunit;

namespace DormFlow.UnitTests.Contracts;

/// <summary>ตรวจการคำนวณจำนวนหน้าและธงบอกหน้าถัดไปของผลลัพธ์แบบแบ่งหน้า</summary>
public class PagedResultTests
{
    [Theory]
    [InlineData(100, 20, 5)]
    [InlineData(101, 20, 6)]
    [InlineData(19, 20, 1)]
    [InlineData(0, 20, 0)]
    [InlineData(1, 1, 1)]
    public void TotalPages_ควรปัดเศษขึ้นเสมอ(long totalItems, int pageSize, int expectedTotalPages)
    {
        PagedResult<int> result = PagedResult.Create<int>([], 1, pageSize, totalItems);

        result.TotalPages.Should().Be(expectedTotalPages);
    }

    [Fact]
    public void หน้าแรก_ต้องไม่มีหน้าก่อนหน้าแต่มีหน้าถัดไป()
    {
        PagedResult<int> result = PagedResult.Create([1, 2, 3], page: 1, pageSize: 20, totalItems: 100);

        result.HasPreviousPage.Should().BeFalse();
        result.HasNextPage.Should().BeTrue();
    }

    [Fact]
    public void หน้าสุดท้าย_ต้องมีหน้าก่อนหน้าแต่ไม่มีหน้าถัดไป()
    {
        PagedResult<int> result = PagedResult.Create([1, 2, 3], page: 5, pageSize: 20, totalItems: 100);

        result.HasPreviousPage.Should().BeTrue();
        result.HasNextPage.Should().BeFalse();
    }

    [Fact]
    public void Empty_ต้องไม่มีรายการและไม่มีหน้าถัดไป()
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
    public void Create_ต้องไม่รับรายการที่เป็น_null()
    {
        Action act = () => PagedResult.Create<int>(null!, 1, 20, 0);

        act.Should().Throw<ArgumentNullException>();
    }
}
