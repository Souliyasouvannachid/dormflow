namespace DormFlow.Application.Common.Exceptions;

/// <summary>
/// ການຮ້ອງຂໍຂັດກັບສະຖານະປັດຈຸບັນຂອງຂໍ້ມູນ, ເຊັ່ນລະຫັດຊໍ້າກັນ API ຈະປ່ຽນເປັນ HTTP 409
/// </summary>
public class ConflictException : Exception
{
    /// <summary>ສ້າງ Exception ດ້ວຍຂໍ້ຄວາມເບື້ອງຕົ້ນ</summary>
    public ConflictException()
        : base("ຂໍ້ມູນຂັດແຍ້ງກັບຂໍ້ມູນທີ່ມີຢູ່ໃນລະບົບ")
    {
    }

    /// <summary>ສ້າງ Exception ກັບຂໍ້ຄວາມທີ່ກໍາຜົດເອງ</summary>
    /// <param name="message">ຂໍ້ຄວາມອະທິບາຍສາເຫດ</param>
    public ConflictException(string message)
        : base(message)
    {
    }

    /// <summary>ສ້າງ Exception ກັບຂໍ້ຄວາມແລະ Exception ຕົ້ນກໍາເນີດ</summary>
    /// <param name="message">ຂໍ້ຄວາມອະທິບາຍສາເຫດ</param>
    /// <param name="innerException">Exception ຕົ້ນກໍາເນີດ</param>
    public ConflictException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
