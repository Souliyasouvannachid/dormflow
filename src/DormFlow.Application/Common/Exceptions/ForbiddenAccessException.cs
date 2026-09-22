namespace DormFlow.Application.Common.Exceptions;

/// <summary>
/// ຜູ້ໃຊ້ໄດ້ຢັ້ງຢືນຕົວຕົນຂອງເຂົາເຈົ້າແຕ່ບໍ່ໄດ້ຮັບອະນຸຍາດໃຫ້ເຮັດທຸລະກໍານີ້ API ຈະປ່ຽນເປັນ HTTP 403
/// </summary>
public class ForbiddenAccessException : Exception
{
    /// <summary>ສ້າງ Exception ດ້ວຍຂໍ້ຄວາມເບື້ອງຕົ້ນ</summary>
    public ForbiddenAccessException()
        : base("ທ່ານບໍ່ມີສິດເຮັດລາຍການນີ້")
    {
    }

    /// <summary>ສ້າງ Exception ກັບຂໍ້ຄວາມທີ່ກໍາຜົດເອງ</summary>
    /// <param name="message">ຂໍ້ຄວາມອະທິບາຍສາເຫດ</param>
    public ForbiddenAccessException(string message)
        : base(message)
    {
    }

    /// <summary>ສ້າງ Exception ກັບຂໍ້ຄວາມແລະ Exception ຕົ້ນກໍາເນີດ</summary>
    /// <param name="message">ຂໍ້ຄວາມອະທິບາຍສາເຫດ</param>
    /// <param name="innerException">Exception ຕົ້ນກໍາເນີດ</param>
    public ForbiddenAccessException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
