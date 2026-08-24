namespace DormFlow.Contracts.Common;

/// <summary>
/// ค่าเริ่มต้นและขอบเขตของการแบ่งหน้าที่ใช้ร่วมกันทั้งระบบ
/// </summary>
public static class PaginationDefaults
{
    /// <summary>หมายเลขหน้าแรก</summary>
    public const int FirstPage = 1;

    /// <summary>จำนวนรายการต่อหน้าเมื่อผู้เรียกไม่ระบุ</summary>
    public const int DefaultPageSize = 20;

    /// <summary>จำนวนรายการต่อหน้าที่น้อยที่สุดที่ยอมรับ</summary>
    public const int MinPageSize = 1;

    /// <summary>
    /// จำนวนรายการต่อหน้าที่มากที่สุดที่ยอมรับ
    /// ป้องกันการดึงข้อมูลทั้งตารางในคำขอเดียวจนกระทบฐานข้อมูล
    /// </summary>
    public const int MaxPageSize = 200;
}
