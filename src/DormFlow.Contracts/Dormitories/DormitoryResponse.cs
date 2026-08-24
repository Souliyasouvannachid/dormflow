namespace DormFlow.Contracts.Dormitories;

/// <summary>ข้อมูลหอพักหนึ่งแห่ง</summary>
public class DormitoryResponse
{
    /// <summary>รหัสอ้างอิงของหอพัก</summary>
    public Guid Id { get; init; }

    /// <summary>องค์กรเจ้าของหอพัก</summary>
    public Guid OrganizationId { get; init; }

    /// <summary>รหัสหอพัก</summary>
    public string Code { get; init; } = string.Empty;

    /// <summary>ชื่อหอพัก</summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>คำอธิบายเพิ่มเติม</summary>
    public string? Description { get; init; }

    /// <summary>เบอร์โทรศัพท์ติดต่อ</summary>
    public string? Phone { get; init; }

    /// <summary>อีเมลติดต่อ</summary>
    public string? Email { get; init; }

    /// <summary>ที่อยู่</summary>
    public string? Address { get; init; }

    /// <summary>จังหวัด</summary>
    public string? Province { get; init; }

    /// <summary>อำเภอ/เขต</summary>
    public string? District { get; init; }

    /// <summary>ตำบล/แขวง</summary>
    public string? Subdistrict { get; init; }

    /// <summary>รหัสไปรษณีย์</summary>
    public string? PostalCode { get; init; }

    /// <summary>ละติจูด</summary>
    public decimal? Latitude { get; init; }

    /// <summary>ลองจิจูด</summary>
    public decimal? Longitude { get; init; }

    /// <summary>วันที่ในแต่ละเดือนที่ออกใบแจ้งหนี้</summary>
    public int BillingDay { get; init; }

    /// <summary>วันที่ครบกำหนดชำระในแต่ละเดือน</summary>
    public int PaymentDueDay { get; init; }

    /// <summary>รูปแบบการคิดค่าปรับชำระล่าช้า</summary>
    public string LateFeeType { get; init; } = string.Empty;

    /// <summary>จำนวนค่าปรับชำระล่าช้า</summary>
    public decimal LateFeeAmount { get; init; }

    /// <summary>อัตราค่าน้ำต่อหน่วย</summary>
    public decimal WaterRatePerUnit { get; init; }

    /// <summary>อัตราค่าไฟต่อหน่วย</summary>
    public decimal ElectricityRatePerUnit { get; init; }

    /// <summary>สถานะของหอพัก</summary>
    public string Status { get; init; } = string.Empty;

    /// <summary>เวลาที่สร้างข้อมูล</summary>
    public DateTimeOffset CreatedAt { get; init; }

    /// <summary>เวลาที่แก้ไขข้อมูลล่าสุด</summary>
    public DateTimeOffset? UpdatedAt { get; init; }
}
