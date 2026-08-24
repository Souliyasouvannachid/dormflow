namespace DormFlow.Contracts.Parcels;

/// <summary>คำขอบันทึกพัสดุที่มาถึง</summary>
public class LogParcelRequest
{
    /// <summary>ห้องที่พัสดุนี้ส่งถึง</summary>
    public Guid RoomId { get; set; }

    /// <summary>ผู้เช่าที่พัสดุนี้ส่งถึง เป็น null เมื่อยังไม่ยืนยันว่าเป็นของใครในห้อง</summary>
    public Guid? TenantId { get; set; }

    /// <summary>บริษัทขนส่ง</summary>
    public string? CourierName { get; set; }

    /// <summary>เลขพัสดุ</summary>
    public string? TrackingNumber { get; set; }

    /// <summary>คำอธิบายพัสดุเพิ่มเติม</summary>
    public string? Description { get; set; }

    /// <summary>หมายเหตุเพิ่มเติม</summary>
    public string? Notes { get; set; }
}
