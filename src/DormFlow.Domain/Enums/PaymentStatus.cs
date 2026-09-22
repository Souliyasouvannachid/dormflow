namespace DormFlow.Domain.Enums;

/// <summary>ສະຖານະຂອງລາຍການການຈ່າຍເງິນ</summary>
public enum PaymentStatus
{
    /// <summary>ບັນທຶກແລ້ວ, ລໍຖ້າການຢືນຢັນ, ຍັງບໍ່ໄດ້ຫັກອອກຈາກຈໍານວນໃບແຈ້ງຜີ້</summary>
    Pending = 1,

    /// <summary>ຢືນຢັນແລ້ວ, ຈຳນວນໃບແຈ້ງໜີ້ໄດ້ຖືກຫັກອອກແລ້ວ</summary>
    Confirmed = 2,

    /// <summary>ເງິນໄດ້ຖືກສົ່ງຄືນແລະຈໍານວນເງິນທີ່ຖືກຫັກອອກຈາກໃບແຈ້ງຜີ້ຄືນເງິນ</summary>
    Refunded = 3,
}
