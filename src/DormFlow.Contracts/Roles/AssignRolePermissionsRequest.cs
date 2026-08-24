namespace DormFlow.Contracts.Roles;

/// <summary>คำขอกำหนดสิทธิ์ให้บทบาท</summary>
/// <remarks>รายการสิทธิ์ที่ส่งมาจะแทนที่สิทธิ์เดิมของบทบาททั้งหมด ไม่ใช่การเพิ่มเติม</remarks>
public class AssignRolePermissionsRequest
{
    /// <summary>รหัสสิทธิ์ที่ต้องการกำหนดให้บทบาทนี้</summary>
    public IReadOnlyList<string> Permissions { get; set; } = [];
}
