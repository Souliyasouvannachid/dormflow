namespace DormFlow.Contracts.Dormitories;

/// <summary>คำขอสร้างหอพักใหม่</summary>
public class CreateDormitoryRequest
{
    /// <summary>
    /// องค์กรเจ้าของหอพัก มีผลเฉพาะเมื่อผู้เรียกเป็น SuperAdmin เท่านั้น
    /// ผู้ใช้อื่นถูกบังคับให้สร้างในองค์กรของตนเองเสมอไม่ว่าจะระบุค่านี้หรือไม่
    /// </summary>
    public Guid? OrganizationId { get; set; }

    /// <summary>รหัสหอพัก ไม่ซ้ำกันภายในองค์กรเดียวกัน</summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>ชื่อหอพัก</summary>
    public string Name { get; set; } = string.Empty;

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
    public string LateFeeType { get; set; } = "Fixed";

    /// <summary>จำนวนค่าปรับชำระล่าช้า</summary>
    public decimal LateFeeAmount { get; set; }
}
