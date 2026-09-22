namespace DormFlow.Domain.Enums;

/// <summary>ສະຖານະບັນຊີຜູ້ໃຊ້</summary>
public enum UserStatus
{
    /// <summary>ສາມາດໃຊ້ໄດ້ຕາມປົກກະຕິ</summary>
    Active = 1,

    /// <summary>ຄົນພິການ. ບໍ່ສາມາດເຂົ້າສູ່ລະບົບໄດ້</summary>
    Inactive = 2,

    /// <summary>ໂຈະ. ບໍ່ສາມາດເຂົ້າສູ່ລະບົບໄດ້ຈົນກ່ວາຜູ້ເບິ່ງແຍງລະບົບຈະເອົາການລະງັບອອກ</summary>
    Suspended = 3,
}
