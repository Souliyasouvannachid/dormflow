namespace DormFlow.Application.Common.Authorization;

/// <summary>ຊື່ຂອງໂມດູນທີ່ໃຊ້ເພື່ອຈັດກຸ່ມການອະນຸຍາດໃນຜ້າຈໍການອະນຸຍາດ</summary>
public static class PermissionModules
{
    /// <summary>ອົງກອນ</summary>
    public const string Organizations = "ອົງກອນ";

    /// <summary>ຫໍພັກ</summary>
    public const string Dormitories = "ຫໍພັກ";

    /// <summary>ອາຄານ</summary>
    public const string Buildings = "ອາຄານ";

    /// <summary>ຊັ້ນ</summary>
    public const string Floors = "ຊັ້ນ";

    /// <summary>ປະເພດຫ້ອງ</summary>
    public const string RoomTypes = "ປະເພດຫ້ອງ";

    /// <summary>ຫ້ອງ</summary>
    public const string Rooms = "ຫ້ອງ";

    /// <summary>ຜູ້ເຊົ່າ</summary>
    public const string Tenants = "ຜູ້ເຊົ່າ";

    /// <summary>ການຈອງ</summary>
    public const string Reservations = "ການຈອງ";

    /// <summary>ສັນຍາເຊົ່າ</summary>
    public const string Contracts = "ສັນຍາເຊົ່າ";

    /// <summary>ການບັນທຶກແມັດ</summary>
    public const string MeterReadings = "ຄ່າມິເຕີ";

    /// <summary>ໃບເກັບເງິນ</summary>
    public const string Invoices = "ໃບແຈ້ງໜີ້";

    /// <summary>ການຈ່າຍເງິນ</summary>
    public const string Payments = "ການຊຳລະ";

    /// <summary>ຝາກ</summary>
    public const string Deposits = "ເງິນມັດຈຳ";

    /// <summary>ໃບຮັບເງິນ</summary>
    public const string Receipts = "ໃບຮັບເງິນ";

    /// <summary>ຄ່າໃຊ້ຈ່າຍໃນການດໍາເນີນງານ</summary>
    public const string Expenses = "ຄ່າໃຊ້ຈ່າຍ";

    /// <summary>ພັດສະດຸ/ຫ້ອງການໄປສະນີຂອງຜູ້ເຊົ່າ</summary>
    public const string Parcels = "ພັດສະດຸ";

    /// <summary>ນັກທ່ອງທ່ຽວ</summary>
    public const string Visitors = "ຜູ້ມາຢ້ຽມຢາມ";

    /// <summary>ການລົງທະບຽນບ່ອນຈອດລົດ</summary>
    public const string Parking = "ບ່ອນຈອດລົດ";

    /// <summary>ວຽກງານບໍາລຸງຮັກສາ</summary>
    public const string Maintenance = "ງານສ້ອມແປງ";

    /// <summary>ປະກາດ</summary>
    public const string Announcements = "ປະກາດ";

    /// <summary>ການແຈ້ງເຕືອນ</summary>
    public const string Notifications = "ການແຈ້ງເຕືອນ";

    /// <summary>ລາຍງານ</summary>
    public const string Reports = "ລາຍງານ";

    /// <summary>ການບໍລິຫານລະບົບ</summary>
    public const string Administration = "ການບໍລິຫານ";

    /// <summary>ແຜງໜ້າປັດ</summary>
    public const string Dashboard = "ແຜງໜ້າປັດ";
}
