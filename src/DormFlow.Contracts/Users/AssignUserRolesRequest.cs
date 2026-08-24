namespace DormFlow.Contracts.Users;

/// <summary>คำขอกำหนดบทบาทให้ผู้ใช้</summary>
/// <remarks>รายการบทบาทที่ส่งมาจะแทนที่บทบาทเดิมของผู้ใช้ทั้งหมด ไม่ใช่การเพิ่มเติม</remarks>
public class AssignUserRolesRequest
{
    /// <summary>รายชื่อบทบาทที่ต้องการกำหนดให้ผู้ใช้</summary>
    public IReadOnlyList<string> Roles { get; set; } = [];
}
