namespace DormFlow.Domain.Enums;

/// <summary>ສະຖານະປະກາດ</summary>
public enum AnnouncementStatus
{
    /// <summary>ຮ່າງດັ່ງກ່າວຍັງບໍ່ທັນໄດ້ຮັບການພິມເຜີຍແຜ່</summary>
    Draft = 1,

    /// <summary>ຈັດພີມມາ</summary>
    Published = 2,

    /// <summary>ເກັບໄວ້ແລ້ວ, ບໍ່ໄດ້ສະແດງອີກຕໍ່ໄປ</summary>
    Archived = 3,
}
