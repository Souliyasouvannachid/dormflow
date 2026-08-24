using DormFlow.Domain.Common;
using DormFlow.Domain.Enums;

namespace DormFlow.Domain.Entities;

/// <summary>
/// ใบแจ้งหนี้ของสัญญาเช่าหนึ่งฉบับในรอบบิลหนึ่งเดือน
/// </summary>
/// <remarks>
/// รายการค่าใช้จ่ายเก็บเป็นฟิลด์ตรง ๆ (ค่าเช่า/ค่าน้ำ/ค่าไฟ/ค่าปรับ) แทนการแยกเป็นตาราง Line Item
/// เพราะจำนวนประเภทค่าใช้จ่ายคงที่และจำกัดในขอบเขตนี้ ไม่มีความจำเป็นต้องรองรับรายการที่ไม่รู้จำนวนล่วงหน้า
/// </remarks>
public class Invoice : TenantEntity
{
    /// <summary>สัญญาเช่าที่ใบแจ้งหนี้นี้อ้างอิงถึง</summary>
    public Guid ContractId { get; set; }

    /// <summary>เลขที่ใบแจ้งหนี้ ไม่ซ้ำกันภายในองค์กรเดียวกัน</summary>
    public required string InvoiceNumber { get; set; }

    /// <summary>เดือนที่ออกบิล ปรับให้เป็นวันที่ 1 ของเดือนเสมอ</summary>
    public DateOnly PeriodMonth { get; set; }

    /// <summary>วันที่ออกใบแจ้งหนี้</summary>
    public DateOnly IssueDate { get; set; }

    /// <summary>วันที่ครบกำหนดชำระ</summary>
    public DateOnly DueDate { get; set; }

    /// <summary>ค่าเช่ารายเดือน คัดลอกมาจากสัญญาเช่าตอนออกบิล</summary>
    public decimal RentAmount { get; set; }

    /// <summary>ค่าน้ำ คำนวณจากหน่วยที่ใช้ในค่ามิเตอร์ของรอบบิลนี้ x อัตราค่าน้ำของหอพัก</summary>
    public decimal WaterAmount { get; set; }

    /// <summary>ค่าไฟ คำนวณจากหน่วยที่ใช้ในค่ามิเตอร์ของรอบบิลนี้ x อัตราค่าไฟของหอพัก</summary>
    public decimal ElectricityAmount { get; set; }

    /// <summary>ค่าปรับชำระล่าช้า เป็น 0 จนกว่าจะถูกคำนวณตอนเกินกำหนดชำระ</summary>
    public decimal LateFeeAmount { get; set; }

    /// <summary>ยอดรวมทั้งบิล (RentAmount + WaterAmount + ElectricityAmount + LateFeeAmount)</summary>
    public decimal TotalAmount { get; set; }

    /// <summary>ยอดที่ชำระแล้วสะสมจากรายการชำระเงินที่ยืนยันแล้วทั้งหมดของบิลนี้</summary>
    public decimal PaidAmount { get; set; }

    /// <summary>สถานะของใบแจ้งหนี้</summary>
    public InvoiceStatus Status { get; set; } = InvoiceStatus.Draft;

    /// <summary>หมายเหตุเพิ่มเติม</summary>
    public string? Notes { get; set; }
}
