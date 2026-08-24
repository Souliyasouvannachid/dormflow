using DormFlow.Domain.Common;
using DormFlow.Domain.Enums;

namespace DormFlow.Domain.Entities;

/// <summary>
/// พัสดุ/ไปรษณีย์หนึ่งชิ้นที่มาถึงหอพักสำหรับผู้เช่า
/// </summary>
/// <remarks>
/// ผูกกับห้องเสมอ (เพื่อขอบเขตองค์กร/หอพักและเพื่อให้เจ้าหน้าที่รู้ว่าต้องแจ้งห้องไหน)
/// ส่วนผู้เช่าเป็นทางเลือก เพราะบางครั้งเจ้าหน้าที่บันทึกพัสดุที่มาถึงก่อนยืนยันได้ว่าเป็นของใครในห้อง
/// </remarks>
public class Parcel : TenantEntity
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

    /// <summary>เวลาที่รับพัสดุไว้ที่หอพัก</summary>
    public DateTimeOffset ReceivedAt { get; set; }

    /// <summary>สถานะของพัสดุ</summary>
    public ParcelStatus Status { get; set; } = ParcelStatus.Received;

    /// <summary>เวลาที่ผู้เช่ามารับพัสดุ เป็น null เมื่อยังไม่มารับ</summary>
    public DateTimeOffset? PickedUpAt { get; set; }

    /// <summary>ชื่อผู้มารับพัสดุจริง (อาจไม่ใช่ผู้เช่าโดยตรง เช่น เพื่อนร่วมห้อง)</summary>
    public string? PickedUpByName { get; set; }

    /// <summary>หมายเหตุเพิ่มเติม</summary>
    public string? Notes { get; set; }
}
