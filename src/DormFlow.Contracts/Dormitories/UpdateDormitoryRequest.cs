namespace DormFlow.Contracts.Dormitories;

/// <summary>คำขอแก้ไขข้อมูลหอพัก</summary>
/// <remarks>ไม่มีฟิลด์ OrganizationId เพราะการย้ายหอพักข้ามองค์กรไม่ใช่การแก้ไขข้อมูลปกติ</remarks>
public class UpdateDormitoryRequest
{
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

    /// <summary>อัตราค่าน้ำต่อหน่วย ใช้คำนวณค่าน้ำในใบแจ้งหนี้จากค่ามิเตอร์</summary>
    public decimal WaterRatePerUnit { get; set; }

    /// <summary>อัตราค่าไฟต่อหน่วย ใช้คำนวณค่าไฟในใบแจ้งหนี้จากค่ามิเตอร์</summary>
    public decimal ElectricityRatePerUnit { get; set; }
}
