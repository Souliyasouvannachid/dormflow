namespace DormFlow.Application.Common.Exceptions;

/// <summary>
/// ບໍ່ພົບຂໍ້ມູນທີ່ຮ້ອງຂໍ. ຫຼືຜູ້ໃຊ້ບໍ່ມີສິດທີ່ຈະເຫັນຂໍ້ມູນນັ້ນ
/// ຊັ້ນ API ຈະປ່ຽນເປັນ HTTP 404
/// </summary>
/// <remarks>
/// ເມື່ອຂໍ້ມູນຢູ່ນອກຂອບເຂດອົງກອນຂອງຜູ້ໃຊ້, ຕອບສະໜອງ 404 ແທນ 403
/// ເພື່ອປ້ອງກັນບໍ່ໃຫ້ຜູ້ໂທຄາດເດົາວ່າຂໍ້ມູນນັ້ນມີຢູ່ໃນອົງກອນອື່ນ
/// </remarks>
public class NotFoundException : Exception
{
    /// <summary>ສ້າງ Exception ດ້ວຍຂໍ້ຄວາມເບື້ອງຕົ້ນ</summary>
    public NotFoundException()
        : base("ບໍ່ພົບຂໍ້ມູນທີ່ຕ້ອງການ")
    {
    }

    /// <summary>ສ້າງ Exception ກັບຂໍ້ຄວາມທີ່ກໍາຜົດເອງ</summary>
    /// <param name="message">ຂໍ້ຄວາມອະທິບາຍສາເຫດ</param>
    public NotFoundException(string message)
        : base(message)
    {
    }

    /// <summary>ສ້າງ Exception ກັບຂໍ້ຄວາມແລະ Exception ຕົ້ນກໍາເນີດ</summary>
    /// <param name="message">ຂໍ້ຄວາມອະທິບາຍສາເຫດ</param>
    /// <param name="innerException">Exception ຕົ້ນກໍາເນີດ</param>
    public NotFoundException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    /// <summary>ສ້າງ Exception ຈາກຊື່ Entity ແລະກະແຈທີ່ຄົ້ນຫາ</summary>
    /// <param name="entityName">ຊື່ Entity ເຊັ່ນ Room</param>
    /// <param name="key">ກະແຈຄົ້ນຫາ</param>
    public static NotFoundException For(string entityName, object key)
        => new($"ບໍ່ພົບຂໍ້ມູນ {entityName} ທີ່ອ້າງອີງ {key}");
}
