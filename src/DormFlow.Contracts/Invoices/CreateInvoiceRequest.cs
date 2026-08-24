namespace DormFlow.Contracts.Invoices;

/// <summary>คำขอออกใบแจ้งหนี้ใหม่ในสถานะร่าง</summary>
public class CreateInvoiceRequest
{
    /// <summary>สัญญาเช่าที่ต้องการออกบิล ต้องอยู่ในสถานะเปิดใช้งาน (Active) เท่านั้น</summary>
    public Guid ContractId { get; set; }

    /// <summary>
    /// เดือนที่ต้องการออกบิล ระบบจะปรับให้เป็นวันที่ 1 ของเดือนโดยอัตโนมัติ
    /// และดึงค่ามิเตอร์น้ำ/ไฟของห้องในเดือนนี้มาคำนวณค่าน้ำ-ไฟให้ (เป็น 0 ถ้าไม่มีค่ามิเตอร์ของเดือนนี้)
    /// </summary>
    public DateOnly PeriodMonth { get; set; }

    /// <summary>วันที่ออกใบแจ้งหนี้ ถ้าไม่ระบุใช้วันที่ปัจจุบัน</summary>
    public DateOnly? IssueDate { get; set; }

    /// <summary>หมายเหตุเพิ่มเติม</summary>
    public string? Notes { get; set; }
}
