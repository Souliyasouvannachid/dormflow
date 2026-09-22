namespace DormFlow.Domain.Enums;

/// <summary>ປະເພດຂອງຄ່າໃຊ້ຈ່າຍໃນການດໍາເນີນງານ</summary>
public enum ExpenseCategory
{
    /// <summary>ໃບບິນຄ່ານ້ຳ, ຄ່າໄຟຟ້າ, ໃບເກັບເງິນຄ່າສາທາລະນູປະໂພກ</summary>
    Utilities = 1,

    /// <summary>ຄ່າໃຊ້ຈ່າຍໃນການບໍາລຸງຮັກສາແລະອຸປະກອນ</summary>
    Maintenance = 2,

    /// <summary>ເງິນເດືອນ ແລະ ຄ່າຊົດເຊີຍພະນັກງານ</summary>
    Salaries = 3,

    /// <summary>ເຄື່ອງໃຊ້ຫ້ອງການ ແລະເຄື່ອງໃຊ້</summary>
    Supplies = 4,

    /// <summary>ຄ່າໃຊ້ຈ່າຍໃນການຕະຫຼາດແລະການໂຄສະນາ</summary>
    Marketing = 5,

    /// <summary>ອື່ນໆ</summary>
    Other = 6,
}
