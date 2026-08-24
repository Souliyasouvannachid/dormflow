namespace DormFlow.Domain.Enums;

/// <summary>สถานะของใบแจ้งหนี้</summary>
public enum InvoiceStatus
{
    /// <summary>ร่าง ยังไม่ออกให้ผู้เช่า แก้ไข/ยกเลิกได้อิสระ</summary>
    Draft = 1,

    /// <summary>ออกให้ผู้เช่าแล้ว รอชำระ</summary>
    Issued = 2,

    /// <summary>ชำระแล้วบางส่วน</summary>
    PartiallyPaid = 3,

    /// <summary>ชำระครบแล้ว</summary>
    Paid = 4,

    /// <summary>เกินกำหนดชำระ</summary>
    Overdue = 5,

    /// <summary>ยกเลิก</summary>
    Cancelled = 6,
}
