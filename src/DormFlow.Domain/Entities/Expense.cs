using DormFlow.Domain.Common;
using DormFlow.Domain.Enums;

namespace DormFlow.Domain.Entities;

/// <summary>
/// ค่าใช้จ่ายในการดำเนินงานหนึ่งรายการ (ค่าน้ำ-ไฟส่วนกลาง, ค่าซ่อมบำรุง, เงินเดือน ฯลฯ)
/// </summary>
/// <remarks>
/// เป็นบันทึกฝั่งรายจ่ายของหอพัก แยกจาก Invoice/Payment ที่เป็นฝั่งรายรับจากผู้เช่าโดยสิ้นเชิง
/// ไม่มีสถานะหรือ Workflow อนุมัติ เพราะเป็นเพียงสมุดบันทึกรายจ่าย ไม่ใช่รายรับที่ต้องกระทบยอดใบแจ้งหนี้ใด
/// </remarks>
public class Expense : TenantEntity
{
    /// <summary>หมวดหมู่ของค่าใช้จ่าย</summary>
    public ExpenseCategory Category { get; set; }

    /// <summary>จำนวนเงิน</summary>
    public decimal Amount { get; set; }

    /// <summary>วันที่จ่ายค่าใช้จ่าย</summary>
    public DateOnly ExpenseDate { get; set; }

    /// <summary>คำอธิบายรายการ</summary>
    public required string Description { get; set; }

    /// <summary>ผู้รับเงิน/ร้านค้า/ผู้ให้บริการ</summary>
    public string? PaidTo { get; set; }

    /// <summary>หมายเหตุเพิ่มเติม</summary>
    public string? Notes { get; set; }
}
