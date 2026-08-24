namespace DormFlow.Contracts.Organizations;

/// <summary>ข้อมูลองค์กรหนึ่งแห่ง</summary>
public class OrganizationResponse
{
    /// <summary>รหัสอ้างอิงขององค์กร</summary>
    public Guid Id { get; init; }

    /// <summary>รหัสองค์กร</summary>
    public string Code { get; init; } = string.Empty;

    /// <summary>ชื่อองค์กร</summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>เลขประจำตัวผู้เสียภาษี</summary>
    public string? TaxId { get; init; }

    /// <summary>เบอร์โทรศัพท์ติดต่อ</summary>
    public string? Phone { get; init; }

    /// <summary>อีเมลติดต่อ</summary>
    public string? Email { get; init; }

    /// <summary>ที่อยู่</summary>
    public string? Address { get; init; }

    /// <summary>สถานะขององค์กร</summary>
    public string Status { get; init; } = string.Empty;

    /// <summary>เวลาที่สร้างข้อมูล</summary>
    public DateTimeOffset CreatedAt { get; init; }

    /// <summary>เวลาที่แก้ไขข้อมูลล่าสุด</summary>
    public DateTimeOffset? UpdatedAt { get; init; }
}
