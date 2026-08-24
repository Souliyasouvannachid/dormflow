namespace DormFlow.Contracts.Settings;

/// <summary>คำขอสร้างหรือแก้ไขค่าตั้งหนึ่งรายการ (Key มาจาก Route)</summary>
public class UpsertSettingRequest
{
    /// <summary>องค์กรเป้าหมาย จำเป็นเฉพาะเมื่อผู้เรียกเป็น SuperAdmin ผู้ใช้อื่นถูกบังคับใช้องค์กรของตนเสมอ</summary>
    public Guid? OrganizationId { get; set; }

    /// <summary>ค่าของค่าตั้ง</summary>
    public string Value { get; set; } = string.Empty;
}
