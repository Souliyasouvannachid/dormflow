using DormFlow.Contracts.Common;

namespace DormFlow.Application.Common.Pagination;

/// <summary>
/// พารามิเตอร์การแบ่งหน้าที่ผ่านการตรวจสอบและปรับค่าให้อยู่ในขอบเขตที่ยอมรับแล้ว
/// ชั้น Application จะรับเฉพาะชนิดนี้ ไม่รับค่าดิบจาก Query String โดยตรง
/// </summary>
/// <param name="Page">หมายเลขหน้า เริ่มที่ 1</param>
/// <param name="PageSize">จำนวนรายการต่อหน้า อยู่ระหว่าง MinPageSize ถึง MaxPageSize</param>
/// <param name="Search">คำค้นหาที่ตัดช่องว่างหัวท้ายแล้ว เป็น null เมื่อไม่ได้ระบุ</param>
/// <param name="SortBy">ชื่อฟิลด์ที่ใช้เรียงลำดับ เป็น null เมื่อไม่ได้ระบุ</param>
/// <param name="SortDirection">ทิศทางการเรียงลำดับ</param>
public readonly record struct PagedQuery(
    int Page,
    int PageSize,
    string? Search,
    string? SortBy,
    SortDirection SortDirection)
{
    /// <summary>จำนวนรายการที่ต้องข้ามก่อนถึงหน้าที่ต้องการ</summary>
    public int Skip => (Page - PaginationDefaults.FirstPage) * PageSize;

    /// <summary>จำนวนรายการที่ต้องดึง</summary>
    public int Take => PageSize;

    /// <summary>มีคำค้นหาที่ใช้งานได้หรือไม่</summary>
    public bool HasSearch => !string.IsNullOrWhiteSpace(Search);
}
