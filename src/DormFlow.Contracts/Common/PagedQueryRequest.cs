namespace DormFlow.Contracts.Common;

/// <summary>
/// พารามิเตอร์มาตรฐานของทุก List Endpoint
/// ค่าที่รับเข้ามาเป็นค่าดิบจาก Query String และต้องถูกปรับให้อยู่ในขอบเขตที่ยอมรับก่อนใช้งานเสมอ
/// </summary>
public class PagedQueryRequest
{
    /// <summary>หมายเลขหน้าที่ต้องการ เริ่มที่ 1</summary>
    public int Page { get; set; } = PaginationDefaults.FirstPage;

    /// <summary>จำนวนรายการต่อหน้า สูงสุด <see cref="PaginationDefaults.MaxPageSize"/></summary>
    public int PageSize { get; set; } = PaginationDefaults.DefaultPageSize;

    /// <summary>คำค้นหาแบบข้อความอิสระ</summary>
    public string? Search { get; set; }

    /// <summary>ชื่อฟิลด์ที่ใช้เรียงลำดับ เช่น roomNumber</summary>
    public string? SortBy { get; set; }

    /// <summary>ทิศทางการเรียงลำดับ</summary>
    public SortDirection SortDirection { get; set; } = SortDirection.Asc;
}
