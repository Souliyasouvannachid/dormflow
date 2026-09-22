namespace DormFlow.Domain.Exceptions;

/// <summary>
/// ຄວາມຜິດພາດທີ່ເກີດຈາກກົດລະບຽບທຸລະກິດໃນ Domain
/// ຊັ້ນ API ຈະປ່ຽນ Exception ຄອບຄົວນີ້ແມ່ນ HTTP 409 Conflict
/// </summary>
public class DomainException : Exception
{
    /// <summary>ສ້າງ Exception ດ້ວຍຂໍ້ຄວາມເບື້ອງຕົ້ນ</summary>
    public DomainException()
        : base("ເກີດຂໍ້ຜິດພາດຈາກກົດລະບຽບທຸລະກິດ")
    {
    }

    /// <summary>ສ້າງ Exception ກັບຂໍ້ຄວາມທີ່ກໍາຜົດເອງ</summary>
    /// <param name="message">ຂໍ້ຄວາມອະທິບາຍສາເຫດ</param>
    public DomainException(string message)
        : base(message)
    {
    }

    /// <summary>ສ້າງ Exception ກັບຂໍ້ຄວາມແລະ Exception ຕົ້ນກໍາເນີດ</summary>
    /// <param name="message">ຂໍ້ຄວາມອະທິບາຍສາເຫດ</param>
    /// <param name="innerException">Exception ຕົ້ນກໍາເນີດ</param>
    public DomainException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
