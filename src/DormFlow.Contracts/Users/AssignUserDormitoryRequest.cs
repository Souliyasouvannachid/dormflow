namespace DormFlow.Contracts.Users;

/// <summary>คำขอมอบหมายให้ผู้ใช้ดูแลหอพักหนึ่งแห่ง</summary>
public class AssignUserDormitoryRequest
{
    /// <summary>หอพักที่ต้องการมอบหมาย</summary>
    public Guid DormitoryId { get; set; }

    /// <summary>ตั้งให้เป็นหอพักหลักของผู้ใช้รายนี้หรือไม่</summary>
    public bool IsPrimary { get; set; }
}
