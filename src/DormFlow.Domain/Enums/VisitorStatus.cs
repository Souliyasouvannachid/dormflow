namespace DormFlow.Domain.Enums;

/// <summary>ສະຖານະນັກທ່ອງທ່ຽວ</summary>
public enum VisitorStatus
{
    /// <summary>ຂ້ອຍເຂົ້າໄປໃນຫໍພັກແລະບໍ່ໄດ້ອອກໄປ</summary>
    CheckedIn = 1,

    /// <summary>ອອກຈາກຫໍພັກ</summary>
    CheckedOut = 2,
}
