namespace DormFlow.Domain.Enums;

/// <summary>ສະຖານະຂອງເງິນຝາກ</summary>
public enum DepositStatus
{
    /// <summary>ເງິນຝາກທີ່ໄດ້ຮັບຍັງບໍ່ມີການຄືນເງິນ</summary>
    Held = 1,

    /// <summary>ບາງສ່ວນຂອງເງິນຝາກໄດ້ຖືກສົ່ງຄືນ</summary>
    PartiallyRefunded = 2,

    /// <summary>ເງິນຝາກໄດ້ຖືກສົ່ງຄືນຢ່າງເຕັມສ່ວນ</summary>
    Refunded = 3,

    /// <summary>ເງິນຝາກທັງໝົດຖືກສູນເສຍ, ບໍ່ມີການຄືນເງິນ</summary>
    Forfeited = 4,
}
