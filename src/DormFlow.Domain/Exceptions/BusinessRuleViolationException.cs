namespace DormFlow.Domain.Exceptions;

/// <summary>
/// ข้อผิดพลาดเมื่อมีการละเมิดกฎทางธุรกิจที่ระบุรหัสกำกับได้
/// เช่น ห้ามมีเลขห้องซ้ำภายในอาคารเดียวกัน
/// </summary>
public class BusinessRuleViolationException : DomainException
{
    /// <summary>สร้าง Exception พร้อมข้อความเริ่มต้น</summary>
    public BusinessRuleViolationException()
        : base("ข้อมูลขัดกับกฎทางธุรกิจ")
    {
        RuleCode = "business_rule.violated";
    }

    /// <summary>สร้าง Exception พร้อมข้อความที่กำหนดเอง</summary>
    /// <param name="message">ข้อความอธิบายสาเหตุ</param>
    public BusinessRuleViolationException(string message)
        : base(message)
    {
        RuleCode = "business_rule.violated";
    }

    /// <summary>สร้าง Exception พร้อมข้อความและ Exception ต้นทาง</summary>
    /// <param name="message">ข้อความอธิบายสาเหตุ</param>
    /// <param name="innerException">Exception ต้นทาง</param>
    public BusinessRuleViolationException(string message, Exception innerException)
        : base(message, innerException)
    {
        RuleCode = "business_rule.violated";
    }

    /// <summary>สร้าง Exception พร้อมรหัสกฎและข้อความ</summary>
    /// <param name="ruleCode">รหัสกฎทางธุรกิจ เช่น rooms.duplicate_room_number</param>
    /// <param name="message">ข้อความอธิบายสาเหตุสำหรับผู้ใช้</param>
    public BusinessRuleViolationException(string ruleCode, string message)
        : base(message)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(ruleCode);
        RuleCode = ruleCode;
    }

    /// <summary>รหัสกฎทางธุรกิจที่ถูกละเมิด ใช้ให้ Client แยกแยะกรณีผิดพลาดได้</summary>
    public string RuleCode { get; }
}
