namespace DormFlow.Domain.Enums;

/// <summary>หมวดหมู่ของค่าใช้จ่ายในการดำเนินงาน</summary>
public enum ExpenseCategory
{
    /// <summary>ค่าน้ำ ค่าไฟ ค่าสาธารณูปโภคส่วนกลาง</summary>
    Utilities = 1,

    /// <summary>ค่าซ่อมบำรุงและวัสดุอุปกรณ์</summary>
    Maintenance = 2,

    /// <summary>เงินเดือนและค่าตอบแทนพนักงาน</summary>
    Salaries = 3,

    /// <summary>วัสดุสิ้นเปลืองและของใช้สำนักงาน</summary>
    Supplies = 4,

    /// <summary>ค่าการตลาดและโฆษณา</summary>
    Marketing = 5,

    /// <summary>อื่น ๆ</summary>
    Other = 6,
}
