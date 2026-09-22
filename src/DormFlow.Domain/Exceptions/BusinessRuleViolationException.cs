namespace DormFlow.Domain.Exceptions;

/// <summary>
/// ຄວາມຜິດພາດເມື່ອກົດລະບຽບທຸລະກິດທີ່ຖືກລະຫັດຖືກລະເມີດ
/// ຕົວຢ່າງ, ຢ່າມີຕົວເລກຫ້ອງຊໍ້າກັນພາຍໃນອາຄານດຽວກັນ
/// </summary>
public class BusinessRuleViolationException : DomainException
{
    /// <summary>ສ້າງ Exception ດ້ວຍຂໍ້ຄວາມເບື້ອງຕົ້ນ</summary>
    public BusinessRuleViolationException()
        : base("ຂໍ້ມູນຂັດກັບກົດລະບຽບທຸລະກິດ")
    {
        RuleCode = "business_rule.violated";
    }

    /// <summary>ສ້າງ Exception ກັບຂໍ້ຄວາມທີ່ກໍາຜົດເອງ</summary>
    /// <param name="message">ຂໍ້ຄວາມອະທິບາຍສາເຫດ</param>
    public BusinessRuleViolationException(string message)
        : base(message)
    {
        RuleCode = "business_rule.violated";
    }

    /// <summary>ສ້າງ Exception ກັບຂໍ້ຄວາມແລະ Exception ຕົ້ນກໍາເນີດ</summary>
    /// <param name="message">ຂໍ້ຄວາມອະທິບາຍສາເຫດ</param>
    /// <param name="innerException">Exception ຕົ້ນກໍາເນີດ</param>
    public BusinessRuleViolationException(string message, Exception innerException)
        : base(message, innerException)
    {
        RuleCode = "business_rule.violated";
    }

    /// <summary>ສ້າງ Exception ດ້ວຍລະຫັດກົດລະບຽບແລະຂໍ້ຄວາມ</summary>
    /// <param name="ruleCode">ID ກົດລະບຽບທຸລະກິດ, ເຊັ່ນ: rooms.duplicate_room_number</param>
    /// <param name="message">ຂໍ້ຄວາມອະທິບາຍເຫດຜົນສໍາລັບຜູ້ໃຊ້</param>
    public BusinessRuleViolationException(string ruleCode, string message)
        : base(message)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(ruleCode);
        RuleCode = ruleCode;
    }

    /// <summary>ID ກົດລະບຽບທຸລະກິດທີ່ຖືກລະເມີດແມ່ນຖືກນໍາໃຊ້ເພື່ອ Client ສາມາດຈໍາແນກກໍລະນີຂອງຄວາມຜິດພາດ</summary>
    public string RuleCode { get; }
}
