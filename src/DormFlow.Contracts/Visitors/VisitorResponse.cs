namespace DormFlow.Contracts.Visitors;

/// <summary>ข้อมูลการเข้า-ออกของผู้มาเยือนหนึ่งรายการ</summary>
public class VisitorResponse
{
    /// <summary>รหัสอ้างอิงของรายการ</summary>
    public Guid Id { get; init; }

    /// <summary>องค์กรที่เกี่ยวข้อง</summary>
    public Guid OrganizationId { get; init; }

    /// <summary>หอพักที่เกี่ยวข้อง</summary>
    public Guid DormitoryId { get; init; }

    /// <summary>ห้องที่ผู้มาเยือนมาเยี่ยม</summary>
    public Guid RoomId { get; init; }

    /// <summary>ผู้เช่าที่ผู้มาเยือนมาเยี่ยม</summary>
    public Guid? TenantId { get; init; }

    /// <summary>ชื่อผู้มาเยือน</summary>
    public string VisitorName { get; init; } = string.Empty;

    /// <summary>เบอร์โทรศัพท์ผู้มาเยือน</summary>
    public string? VisitorPhone { get; init; }

    /// <summary>เลขบัตรประจำตัวประชาชนหรือหนังสือเดินทางของผู้มาเยือน</summary>
    public string? IdNumber { get; init; }

    /// <summary>วัตถุประสงค์การมาเยือน</summary>
    public string? Purpose { get; init; }

    /// <summary>เวลาที่เข้าหอพัก</summary>
    public DateTimeOffset CheckInAt { get; init; }

    /// <summary>สถานะของผู้มาเยือน</summary>
    public string Status { get; init; } = string.Empty;

    /// <summary>เวลาที่ออกจากหอพัก</summary>
    public DateTimeOffset? CheckOutAt { get; init; }

    /// <summary>หมายเหตุเพิ่มเติม</summary>
    public string? Notes { get; init; }

    /// <summary>เวลาที่สร้างข้อมูล</summary>
    public DateTimeOffset CreatedAt { get; init; }
}
