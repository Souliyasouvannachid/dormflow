namespace DormFlow.Domain.Enums;

/// <summary>ສະຖານະໃບແຈ້ງໜີ້</summary>
public enum InvoiceStatus
{
    /// <summary>ຮ່າງດັ່ງກ່າວຍັງບໍ່ທັນໄດ້ອອກໃຫ້ຜູ້ເຊົ່າ. ແກ້ໄຂ/ຍົກເລີກໄດ້ຢ່າງເສລີ</summary>
    Draft = 1,

    /// <summary>ອອກໃຫ້ຜູ້ເຊົ່າ, ລໍຖ້າການຈ່າຍເງິນ</summary>
    Issued = 2,

    /// <summary>ຈ່າຍເປັນບາງສ່ວນ</summary>
    PartiallyPaid = 3,

    /// <summary>ຈ່າຍເຕັມ</summary>
    Paid = 4,

    /// <summary>ເກີນກຳນົດ</summary>
    Overdue = 5,

    /// <summary>ຍົກເລີກ</summary>
    Cancelled = 6,
}
