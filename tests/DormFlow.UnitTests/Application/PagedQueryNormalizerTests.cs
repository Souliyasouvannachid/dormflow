using DormFlow.Application.Common.Pagination;
using DormFlow.Contracts.Common;
using FluentAssertions;
using Xunit;

namespace DormFlow.UnitTests.Application;

/// <summary>
/// ตรวจว่าพารามิเตอร์การแบ่งหน้าที่ผู้เรียกส่งมาถูกปรับให้อยู่ในขอบเขตที่ปลอดภัยเสมอ
/// เพราะค่าอย่าง pageSize=100000 จะดึงข้อมูลทั้งตารางออกมาในคำขอเดียว
/// </summary>
public class PagedQueryNormalizerTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-999)]
    public void หน้าน้อยกว่าหนึ่ง_ต้องถูกปรับเป็นหน้าแรก(int requestedPage)
    {
        PagedQuery query = PagedQueryNormalizer.Normalize(new PagedQueryRequest { Page = requestedPage });

        query.Page.Should().Be(PaginationDefaults.FirstPage);
    }

    [Fact]
    public void ขนาดหน้าเกินเพดาน_ต้องถูกลดลงมาที่เพดาน()
    {
        PagedQuery query = PagedQueryNormalizer.Normalize(new PagedQueryRequest { PageSize = 100_000 });

        query.PageSize.Should().Be(PaginationDefaults.MaxPageSize);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public void ขนาดหน้าน้อยกว่าหนึ่ง_ต้องกลับไปใช้ค่าเริ่มต้น(int requestedPageSize)
    {
        PagedQuery query = PagedQueryNormalizer.Normalize(new PagedQueryRequest { PageSize = requestedPageSize });

        query.PageSize.Should().Be(PaginationDefaults.DefaultPageSize);
    }

    [Fact]
    public void คำค้นหา_ต้องถูกตัดช่องว่างหัวท้าย()
    {
        PagedQuery query = PagedQueryNormalizer.Normalize(new PagedQueryRequest { Search = "  A101  " });

        query.Search.Should().Be("A101");
        query.HasSearch.Should().BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void คำค้นหาที่ว่าง_ต้องกลายเป็น_null(string? search)
    {
        PagedQuery query = PagedQueryNormalizer.Normalize(new PagedQueryRequest { Search = search });

        query.Search.Should().BeNull();
        query.HasSearch.Should().BeFalse();
    }

    [Fact]
    public void ฟิลด์เรียงลำดับที่ไม่อยู่ในรายการอนุญาต_ต้องกลับไปใช้ค่าเริ่มต้น()
    {
        PagedQuery query = PagedQueryNormalizer.Normalize(
            new PagedQueryRequest { SortBy = "'; DROP TABLE rooms; --" },
            allowedSortFields: ["roomNumber", "monthlyRent"],
            defaultSortBy: "roomNumber");

        query.SortBy.Should().Be("roomNumber");
    }

    [Fact]
    public void ฟิลด์เรียงลำดับที่อยู่ในรายการอนุญาต_ต้องคืนค่าตามที่ประกาศไว้()
    {
        PagedQuery query = PagedQueryNormalizer.Normalize(
            new PagedQueryRequest { SortBy = "MONTHLYRENT" },
            allowedSortFields: ["roomNumber", "monthlyRent"],
            defaultSortBy: "roomNumber");

        query.SortBy.Should().Be("monthlyRent");
    }

    [Fact]
    public void Skip_และ_Take_ต้องคำนวณจากหน้าที่ปรับค่าแล้ว()
    {
        PagedQuery query = PagedQueryNormalizer.Normalize(new PagedQueryRequest { Page = 3, PageSize = 20 });

        query.Skip.Should().Be(40);
        query.Take.Should().Be(20);
    }

    [Fact]
    public void ทิศทางการเรียงลำดับ_ต้องคงค่าที่ผู้เรียกระบุ()
    {
        PagedQuery query = PagedQueryNormalizer.Normalize(
            new PagedQueryRequest { SortDirection = SortDirection.Desc });

        query.SortDirection.Should().Be(SortDirection.Desc);
    }

    [Fact]
    public void Normalize_ต้องไม่รับ_request_ที่เป็น_null()
    {
        Action act = () => PagedQueryNormalizer.Normalize(null!);

        act.Should().Throw<ArgumentNullException>();
    }
}
