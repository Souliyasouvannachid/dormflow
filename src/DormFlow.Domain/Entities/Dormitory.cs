using DormFlow.Domain.Common;
using DormFlow.Domain.Enums;

namespace DormFlow.Domain.Entities;

/// <summary>
/// หอพักหนึ่งแห่งภายในองค์กร
/// </summary>
/// <remarks>
/// สืบทอดจาก <see cref="BaseEntity"/> ไม่ใช่ <see cref="TenantEntity"/>
/// เพราะหอพักคือตัวขอบเขตของ DormitoryId เอง จึงไม่มีหอพักแม่ให้อ้างอิง
/// เช่นเดียวกับที่ <see cref="Organization"/> ไม่สืบทอด TenantEntity
/// </remarks>
public class Dormitory : BaseEntity
{
    /// <summary>องค์กรเจ้าของหอพัก</summary>
    public Guid OrganizationId { get; set; }

    /// <summary>รหัสหอพัก ไม่ซ้ำกันภายในองค์กรเดียวกัน</summary>
    public required string Code { get; set; }

    /// <summary>ชื่อหอพัก</summary>
    public required string Name { get; set; }

    /// <summary>คำอธิบายเพิ่มเติม</summary>
    public string? Description { get; set; }

    /// <summary>เบอร์โทรศัพท์ติดต่อ</summary>
    public string? Phone { get; set; }

    /// <summary>อีเมลติดต่อ</summary>
    public string? Email { get; set; }

    /// <summary>ที่อยู่</summary>
    public string? Address { get; set; }

    /// <summary>จังหวัด</summary>
    public string? Province { get; set; }

    /// <summary>อำเภอ/เขต</summary>
    public string? District { get; set; }

    /// <summary>ตำบล/แขวง</summary>
    public string? Subdistrict { get; set; }

    /// <summary>รหัสไปรษณีย์</summary>
    public string? PostalCode { get; set; }

    /// <summary>ละติจูด</summary>
    public decimal? Latitude { get; set; }

    /// <summary>ลองจิจูด</summary>
    public decimal? Longitude { get; set; }

    /// <summary>วันที่ในแต่ละเดือนที่ออกใบแจ้งหนี้ (1-28)</summary>
    public int BillingDay { get; set; } = 1;

    /// <summary>วันที่ครบกำหนดชำระในแต่ละเดือน (1-31)</summary>
    public int PaymentDueDay { get; set; } = 5;

    /// <summary>รูปแบบการคิดค่าปรับชำระล่าช้า</summary>
    public LateFeeType LateFeeType { get; set; } = LateFeeType.Fixed;

    /// <summary>จำนวนค่าปรับชำระล่าช้า ตามรูปแบบที่กำหนดใน <see cref="LateFeeType"/></summary>
    public decimal LateFeeAmount { get; set; }

    /// <summary>สถานะของหอพัก</summary>
    public DormitoryStatus Status { get; set; } = DormitoryStatus.Active;
}
