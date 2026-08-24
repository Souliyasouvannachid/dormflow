namespace DormFlow.Contracts.Visitors;

/// <summary>คำขอบันทึกการเข้าของผู้มาเยือน</summary>
public class CheckInVisitorRequest
{
    /// <summary>ห้องที่ผู้มาเยือนมาเยี่ยม</summary>
    public Guid RoomId { get; set; }

    /// <summary>ผู้เช่าที่ผู้มาเยือนมาเยี่ยม เป็น null เมื่อไม่ระบุ</summary>
    public Guid? TenantId { get; set; }

    /// <summary>ชื่อผู้มาเยือน</summary>
    public string VisitorName { get; set; } = string.Empty;

    /// <summary>เบอร์โทรศัพท์ผู้มาเยือน</summary>
    public string? VisitorPhone { get; set; }

    /// <summary>เลขบัตรประจำตัวประชาชนหรือหนังสือเดินทางของผู้มาเยือน</summary>
    public string? IdNumber { get; set; }

    /// <summary>วัตถุประสงค์การมาเยือน</summary>
    public string? Purpose { get; set; }

    /// <summary>หมายเหตุเพิ่มเติม</summary>
    public string? Notes { get; set; }
}
