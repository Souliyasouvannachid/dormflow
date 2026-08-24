namespace DormFlow.Contracts.Organizations;

/// <summary>คำขอสร้างองค์กรใหม่</summary>
public class CreateOrganizationRequest
{
    /// <summary>รหัสองค์กร ไม่ซ้ำกันทั้งระบบ</summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>ชื่อองค์กร</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>เลขประจำตัวผู้เสียภาษี</summary>
    public string? TaxId { get; set; }

    /// <summary>เบอร์โทรศัพท์ติดต่อ</summary>
    public string? Phone { get; set; }

    /// <summary>อีเมลติดต่อ</summary>
    public string? Email { get; set; }

    /// <summary>ที่อยู่</summary>
    public string? Address { get; set; }
}
