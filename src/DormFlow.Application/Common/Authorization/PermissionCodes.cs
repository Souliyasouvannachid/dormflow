namespace DormFlow.Application.Common.Authorization;

/// <summary>
/// ສິດທັງຝົດຂອງລະບົບ
/// </summary>
/// <remarks>
/// ໄຟລ໌ນີ້ແມ່ນແຫຼ່ງຄວາມຈິງອັນດຽວສຳລັບລາຍຊື່ການອະນຸຍາດ
/// ໂຕະ permissions ໃນຖານຂໍ້ມູນ Seed ຈາກບັນຊີລາຍຊື່ນີ້, ຜູ້ໃຊ້ບໍ່ໄດ້ຮັບອະນຸຍາດໃຫ້ເພີ່ມມັນດ້ວຍຕົນເອງ
/// ເພາະທຸກລະຫັດຕ້ອງມີຈຸດກວດກາຢູ່ໃນລະຫັດຕົວຈິງ. ຖ້າບໍ່ດັ່ງນັ້ນ, ມັນເປັນສິດທີ່ບໍ່ມີຜົນກະທົບ
/// </remarks>
public static class PermissionCodes
{
    /// <summary>ສິດການຈັດຕັ້ງ</summary>
    public static class Organizations
    {
        /// <summary>ເບິ່ງຂໍ້ມູນອົງກອນ</summary>
        public const string View = "organizations.view";

        /// <summary>ສ້າງອົງກອນໃຝ່</summary>
        public const string Create = "organizations.create";

        /// <summary>ແກ້ໄຂຂໍ້ມູນອົງກອນ</summary>
        public const string Update = "organizations.update";

        /// <summary>ລຶບອົງກອນ</summary>
        public const string Delete = "organizations.delete";
    }

    /// <summary>ສິດໃນຫໍພັກ</summary>
    public static class Dormitories
    {
        /// <summary>ເບິ່ງຂໍ້ມູນຫໍພັກ</summary>
        public const string View = "dormitories.view";

        /// <summary>ສ້າງຫໍພັກໃໝ່</summary>
        public const string Create = "dormitories.create";

        /// <summary>ແກ້ໄຂຂໍ້ມູນຫໍພັກ</summary>
        public const string Update = "dormitories.update";

        /// <summary>ລຶບຫໍພັກ</summary>
        public const string Delete = "dormitories.delete";
    }

    /// <summary>ສິດການກໍ່ສ້າງ</summary>
    public static class Buildings
    {
        /// <summary>ເບິ່ງຂໍ້ມູນອາຄານ</summary>
        public const string View = "buildings.view";

        /// <summary>ສ້າງອາຄານໃຝ່</summary>
        public const string Create = "buildings.create";

        /// <summary>ແກ້ໄຂຂໍ້ມູນອາຄານ</summary>
        public const string Update = "buildings.update";

        /// <summary>ລຶບອາຄານ</summary>
        public const string Delete = "buildings.delete";
    }

    /// <summary>ສິດກ່ຽວກັບຊັ້ນຂອງອາຄານ</summary>
    public static class Floors
    {
        /// <summary>ເບິ່ງຂໍ້ມູນຊັ້ນ</summary>
        public const string View = "floors.view";

        /// <summary>ສ້າງຊັ້ນຂໍ້ມູນໃໝ່</summary>
        public const string Create = "floors.create";

        /// <summary>ແກ້ໄຂຂໍ້ມູນຊັ້ນ</summary>
        public const string Update = "floors.update";

        /// <summary>ລຶບຊັ້ນ</summary>
        public const string Delete = "floors.delete";
    }

    /// <summary>ສິດກ່ຽວກັບປະເພດຫ້ອງ</summary>
    public static class RoomTypes
    {
        /// <summary>ເບິ່ງຂໍ້ມູນປະເພດຫ້ອງ</summary>
        public const string View = "room-types.view";

        /// <summary>ສ້າງປະເພດຫ້ອງໃຝ່</summary>
        public const string Create = "room-types.create";

        /// <summary>ແກ້ໄຂຂໍ້ມູນປະເພດຫ້ອງ</summary>
        public const string Update = "room-types.update";

        /// <summary>ລຶບປະເພດຫ້ອງ</summary>
        public const string Delete = "room-types.delete";
    }

    /// <summary>ສິດຫ້ອງ</summary>
    public static class Rooms
    {
        /// <summary>ເບິ່ງຂໍ້ມູນຫ້ອງ</summary>
        public const string View = "rooms.view";

        /// <summary>ສ້າງຫ້ອງໃຝ່</summary>
        public const string Create = "rooms.create";

        /// <summary>ແກ້ໄຂຂໍ້ມູນຫ້ອງ</summary>
        public const string Update = "rooms.update";

        /// <summary>ລຶບຫ້ອງ</summary>
        public const string Delete = "rooms.delete";

        /// <summary>ປ່ຽນສະຖານະຫ້ອງ</summary>
        public const string ChangeStatus = "rooms.change-status";
    }

    /// <summary>ສິດກ່ຽວກັບຜູ້ເຊົ່າ</summary>
    public static class Tenants
    {
        /// <summary>ເບິ່ງຂໍ້ມູນຜູ້ເຊົ່າ</summary>
        public const string View = "tenants.view";

        /// <summary>ສ້າງຂໍ້ມູນຜູ້ເຊົ່າໃຝ່</summary>
        public const string Create = "tenants.create";

        /// <summary>ແກ້ໄຂຂໍ້ມູນຜູ້ເຊົ່າ</summary>
        public const string Update = "tenants.update";
    }

    /// <summary>
    /// ສິດກ່ຽວກັບການຈອງຫ້ອງ
    /// </summary>
    /// <remarks>
    /// ມັນບໍ່ໄດ້ຢູ່ໃນບັນຊີລາຍຊື່ການອະນຸຍາດຕົ້ນສະບັບ Phase 1 ເພີ່ມໃນຂັ້ນຕອນການສ້າງໂມດູນ Reservations ຂອງ Phase 2
    /// ມີຊື່ໃນຮູບແບບດຽວກັນກັບການອະນຸຍາດອື່ນໆທັງຝົດເພື່ອຄວາມສອດຄ່ອງ
    /// </remarks>
    public static class Reservations
    {
        /// <summary>ເບິ່ງລາຍຊື່ການຈອງ</summary>
        public const string View = "reservations.view";

        /// <summary>ສ້າງການຈອງໃຝ່</summary>
        public const string Create = "reservations.create";

        /// <summary>ຍົກເລີກການຈອງ</summary>
        public const string Cancel = "reservations.cancel";
    }

    /// <summary>ສິດກ່ຽວກັບສັນຍາເຊົ່າ</summary>
    public static class Contracts
    {
        /// <summary>ເບິ່ງຂໍ້ຕົກລົງການເຊົ່າ</summary>
        public const string View = "contracts.view";

        /// <summary>ສ້າງສັນຍາເຊົ່າ</summary>
        public const string Create = "contracts.create";

        /// <summary>ເປີດໃຊ້ຂໍ້ຕົກລົງການເຊົ່າ</summary>
        public const string Activate = "contracts.activate";

        /// <summary>ຍົກເລີກ ຫຼືສິ້ນສຸດສັນຍາເຊົ່າ</summary>
        public const string Terminate = "contracts.terminate";
    }

    /// <summary>ສິດກ່ຽວກັບການລົງທະບຽນແມັດ</summary>
    public static class MeterReadings
    {
        /// <summary>ເບິ່ງຄ່າແມັດ</summary>
        public const string View = "meter-readings.view";

        /// <summary>ບັນທຶກຄ່າແມັດ</summary>
        public const string Create = "meter-readings.create";
    }

    /// <summary>ສິດກ່ຽວກັບໃບແຈ້ງໜີ້</summary>
    public static class Invoices
    {
        /// <summary>ເບິ່ງໃບແຈ້ງໜີ້</summary>
        public const string View = "invoices.view";

        /// <summary>ສ້າງໃບແຈ້ງໜີ້</summary>
        public const string Create = "invoices.create";

        /// <summary>ອອກໃບແຈ້ງໜີ້ໃຫ້ຜູ້ເຊົ່າ</summary>
        public const string Issue = "invoices.issue";

        /// <summary>ຍົກເລີກໃບແຈ້ງໜີ້</summary>
        public const string Cancel = "invoices.cancel";
    }

    /// <summary>ສິດໃນການຊໍາລະເງິນ</summary>
    public static class Payments
    {
        /// <summary>ເບິ່ງລາຍຊື່ການຈ່າຍເງິນ</summary>
        public const string View = "payments.view";

        /// <summary>ບັນທຶກການຈ່າຍເງິນ</summary>
        public const string Create = "payments.create";

        /// <summary>ອະນຸມັດການຈ່າຍເງິນ</summary>
        public const string Approve = "payments.approve";

        /// <summary>ເງິນຄືນ</summary>
        public const string Refund = "payments.refund";
    }

    /// <summary>
    /// ສິດກ່ຽວກັບເງິນຝາກ
    /// </summary>
    /// <remarks>
    /// ມັນບໍ່ໄດ້ຢູ່ໃນບັນຊີລາຍຊື່ການອະນຸຍາດຕົ້ນສະບັບ Phase 1 ເພີ່ມໃນຂັ້ນຕອນການສ້າງໂມດູນ Deposits ຂອງ Phase 2
    /// ມີຊື່ໃນຮູບແບບດຽວກັນກັບການອະນຸຍາດອື່ນໆທັງຝົດເພື່ອຄວາມສອດຄ່ອງ
    /// </remarks>
    public static class Deposits
    {
        /// <summary>ເບິ່ງລາຍການເງິນຝາກ</summary>
        public const string View = "deposits.view";

        /// <summary>ບັນທຶກການຮັບເງິນຝາກ</summary>
        public const string Collect = "deposits.collect";

        /// <summary>ຝາກເງິນຄືນ</summary>
        public const string Refund = "deposits.refund";
    }

    /// <summary>
    /// ສິດກ່ຽວກັບໃບຮັບເງິນ
    /// </summary>
    /// <remarks>
    /// ມັນບໍ່ໄດ້ຢູ່ໃນບັນຊີລາຍຊື່ການອະນຸຍາດຕົ້ນສະບັບ Phase 1 ເພີ່ມໃນຂັ້ນຕອນການສ້າງໂມດູນ Receipts ຂອງ Phase 2
    /// ມີຊື່ໃນຮູບແບບດຽວກັນກັບການອະນຸຍາດອື່ນໆທັງຝົດເພື່ອຄວາມສອດຄ່ອງ
    /// </remarks>
    public static class Receipts
    {
        /// <summary>ເບິ່ງໃບຮັບເງິນ</summary>
        public const string View = "receipts.view";

        /// <summary>ອອກໃບຮັບເງິນ</summary>
        public const string Issue = "receipts.issue";
    }

    /// <summary>
    /// ສິດກ່ຽວກັບຄ່າໃຊ້ຈ່າຍໃນການດໍາເນີນງານ
    /// </summary>
    /// <remarks>
    /// ມັນບໍ່ໄດ້ຢູ່ໃນບັນຊີລາຍຊື່ການອະນຸຍາດຕົ້ນສະບັບ Phase 1 ເພີ່ມໃນຂັ້ນຕອນການສ້າງໂມດູນ Expenses ຂອງ Phase 2
    /// ມີຊື່ໃນຮູບແບບດຽວກັນກັບການອະນຸຍາດອື່ນໆທັງຝົດເພື່ອຄວາມສອດຄ່ອງ
    /// </remarks>
    public static class Expenses
    {
        /// <summary>ເບິ່ງລາຍການຄ່າໃຊ້ຈ່າຍ</summary>
        public const string View = "expenses.view";

        /// <summary>ບັນທຶກຄ່າໃຊ້ຈ່າຍ</summary>
        public const string Create = "expenses.create";
    }

    /// <summary>
    /// ສິດກ່ຽວກັບການ parcels/ຫ້ອງການໄປສະນີຂອງຜູ້ເຊົ່າ
    /// </summary>
    /// <remarks>
    /// ມັນບໍ່ໄດ້ຢູ່ໃນບັນຊີລາຍຊື່ການອະນຸຍາດຕົ້ນສະບັບ Phase 1 ເພີ່ມໃນຂັ້ນຕອນການສ້າງໂມດູນ Parcels ຂອງ Phase 2
    /// ມີຊື່ໃນຮູບແບບດຽວກັນກັບການອະນຸຍາດອື່ນໆທັງຝົດເພື່ອຄວາມສອດຄ່ອງ
    /// </remarks>
    public static class Parcels
    {
        /// <summary>ເບິ່ງລາຍການ parcel</summary>
        public const string View = "parcels.view";

        /// <summary>ບັນທຶກຊຸດທີ່ມາຮອດແລ້ວ</summary>
        public const string Log = "parcels.log";

        /// <summary>ບັນທຶກຂອງການຮັບການພັດທະນາ</summary>
        public const string Pickup = "parcels.pickup";
    }

    /// <summary>
    /// ສິດກ່ຽວກັບນັກທ່ອງທ່ຽວ
    /// </summary>
    /// <remarks>
    /// ມັນບໍ່ໄດ້ຢູ່ໃນບັນຊີລາຍຊື່ການອະນຸຍາດຕົ້ນສະບັບ Phase 1 ເພີ່ມໃນຂັ້ນຕອນການສ້າງໂມດູນ Visitors ຂອງ Phase 2
    /// ມີຊື່ໃນຮູບແບບດຽວກັນກັບການອະນຸຍາດອື່ນໆທັງຝົດເພື່ອຄວາມສອດຄ່ອງ
    /// </remarks>
    public static class Visitors
    {
        /// <summary>ເບິ່ງລາຍຊື່ຜູ້ເຂົ້າຊົມ</summary>
        public const string View = "visitors.view";

        /// <summary>ບັນທຶກການເຂົ້າຂອງນັກທ່ອງທ່ຽວ</summary>
        public const string CheckIn = "visitors.checkin";

        /// <summary>ບັນທຶກຜູ້ເຂົ້າຊົມ</summary>
        public const string CheckOut = "visitors.checkout";
    }

    /// <summary>
    /// ສິດກ່ຽວກັບການລົງທະບຽນບ່ອນຈອດລົດ
    /// </summary>
    /// <remarks>
    /// ມັນບໍ່ໄດ້ຢູ່ໃນບັນຊີລາຍຊື່ການອະນຸຍາດຕົ້ນສະບັບ Phase 1 ເພີ່ມໃນຂັ້ນຕອນການສ້າງໂມດູນ Parking ຂອງ Phase 2
    /// ມີຊື່ໃນຮູບແບບດຽວກັນກັບການອະນຸຍາດອື່ນໆທັງຝົດເພື່ອຄວາມສອດຄ່ອງ
    /// </remarks>
    public static class Parking
    {
        /// <summary>ເບິ່ງບັນຊີລາຍຊື່ການລົງທະບຽນບ່ອນຈອດລົດ</summary>
        public const string View = "parking.view";

        /// <summary>ລົງທະບຽນບ່ອນຈອດລົດໃຝ່</summary>
        public const string Register = "parking.register";

        /// <summary>ສິ້ນສຸດການລົງທະບຽນບ່ອນຈອດລົດ</summary>
        public const string Release = "parking.release";
    }

    /// <summary>
    /// ສິດໃນການປະກາດ
    /// </summary>
    /// <remarks>
    /// ມັນບໍ່ໄດ້ຢູ່ໃນບັນຊີລາຍຊື່ການອະນຸຍາດຕົ້ນສະບັບ Phase 1 ເພີ່ມໃນຂັ້ນຕອນການສ້າງໂມດູນ Announcements ຂອງ Phase 2
    /// ມີຊື່ໃນຮູບແບບດຽວກັນກັບການອະນຸຍາດອື່ນໆທັງຝົດເພື່ອຄວາມສອດຄ່ອງ
    /// ສິດ Publish ກວມເອົາທັງການພິມເຜີຍແຜ່ແລະການເກັບຂໍ້ມູນ (ບໍ່ມີການແບ່ງແຍກສິດ Archive ແຍກຕ່າງຫາກ)
    /// ເນື່ອງຈາກວ່າທັງສອງແມ່ນການປ່ຽນແປງສະຖານະທີ່ສະເຝີເຮັດໂດຍອໍານາດການພິມດຽວກັນ
    /// </remarks>
    public static class Announcements
    {
        /// <summary>ເບິ່ງລາຍການປະກາດ</summary>
        public const string View = "announcements.view";

        /// <summary>ສ້າງການປະກາດໃຝ່</summary>
        public const string Create = "announcements.create";

        /// <summary>ເຜີຍແຜ່ ຫຼືຈັດເກັບການປະກາດ</summary>
        public const string Publish = "announcements.publish";
    }

    /// <summary>
    /// ສິດການແຈ້ງການ
    /// </summary>
    /// <remarks>
    /// ມັນບໍ່ໄດ້ຢູ່ໃນບັນຊີລາຍຊື່ການອະນຸຍາດຕົ້ນສະບັບ Phase 1 ເພີ່ມໃນຂັ້ນຕອນການສ້າງໂມດູນ Notifications ຂອງ Phase 2
    /// ມີພຽງແຕ່ສິດທີ່ຈະສົ່ງ. ເນື່ອງຈາກວ່າການເບິ່ງແລະຝາຍການອ່ານແຈ້ງການແມ່ນເປີດດ້ວຍຕົນເອງສໍາລັບການເຂົ້າສູ່ລະບົບຜູ້ໃຊ້
    /// ທຸກຄົນສາມາດເຮັດໄດ້ສະເໝີ (ມັກ `GET /auth/me`) ບໍ່ໄດ້ຜູກມັດກັບສິດໃດໆເນື່ອງຈາກວ່າມັນເປັນຂໍ້ມູນຂອງເຂົາເຈົ້າເອງໂດຍກົງ
    /// </remarks>
    public static class Notifications
    {
        /// <summary>ສົ່ງການແຈ້ງເຕືອນໃຫ້ຜູ້ໃຊ້</summary>
        public const string Send = "notifications.send";
    }

    /// <summary>ສິດກ່ຽວກັບວຽກງານບໍາລຸງຮັກສາ</summary>
    public static class Maintenance
    {
        /// <summary>ເບິ່ງວຽກບໍາລຸງຮັກສາ</summary>
        public const string View = "maintenance.view";

        /// <summary>ມອບວຽກບໍາລຸງຮັກສາ</summary>
        public const string Assign = "maintenance.assign";

        /// <summary>ອັບເດດສະຖານະການບຳລຸງຮັກສາ</summary>
        public const string UpdateStatus = "maintenance.update-status";
    }

    /// <summary>ສິດໃນການລາຍງານ</summary>
    public static class Reports
    {
        /// <summary>ເບິ່ງບົດລາຍງານ</summary>
        public const string View = "reports.view";

        /// <summary>ລາຍງານການສົ່ງອອກ</summary>
        public const string Export = "reports.export";
    }

    /// <summary>ສິດການຄຸ້ມຄອງລະບົບ</summary>
    public static class Administration
    {
        /// <summary>ຈັດການຜູ້ໃຊ້</summary>
        public const string ManageUsers = "users.manage";

        /// <summary>ຄຸ້ມຄອງບົດບາດແລະການອະນຸຍາດ</summary>
        public const string ManageRoles = "roles.manage";

        /// <summary>ຈັດການການຕັ້ງຄ່າລະບົບ</summary>
        public const string ManageSettings = "settings.manage";

        /// <summary>ສັງເກດເບິ່ງ Audit Log</summary>
        public const string ViewAuditLogs = "audit-logs.view";
    }

    /// <summary>
    /// ການອະນຸຍາດ Dashboard
    /// </summary>
    /// <remarks>
    /// ມັນບໍ່ໄດ້ຢູ່ໃນບັນຊີລາຍຊື່ການອະນຸຍາດຕົ້ນສະບັບ Phase 1/2 ເພີ່ມໃນລະຫວ່າງການສ້າງ Endpoint ພາບລວມຂອງ Dashboard
    /// ນີ້ແມ່ນສິດດຽວທີ່ມີຈຸດປະສົງເພື່ອໃຫ້ໄດ້ທຸກໜ້າທີ່ຂອງຕົວແທນ (ຍົກເວັ້ນ Tenant) ເນື່ອງຈາກວ່າ dashboard ໄດ້
    /// ສົມທົບຂໍ້ມູນຈາກຫຼາຍໂມດູນທີ່ມີບົດບາດທີ່ແນ່ນອນ (ເຊັ່ນ Technician) ບໍ່ມີສິດໃນການເບິ່ງໂດຍກົງ (ເຊັ່ນ reports.view)
    /// ແຕ່ທ່ານຍັງຈໍາເປັນຕ້ອງສາມາດເຫັນພາບລວມກ່ຽວກັບຕົວທ່ານເອງ
    /// </remarks>
    public static class Dashboard
    {
        /// <summary>ເບິ່ງພາບລວມຂອງ dashboard</summary>
        public const string View = "dashboard.view";
    }

    /// <summary>ຄໍານິຍາມຂອງສິດທັງຝົດຈັດລຽງຕາມໂມດູນ</summary>
    public static IReadOnlyList<PermissionDefinition> All { get; } =
    [
        new(Organizations.View, "ເບິ່ງຂໍ້ມູນອົງກອນ", PermissionModules.Organizations),
        new(Organizations.Create, "ສ້າງອົງກອນ", PermissionModules.Organizations),
        new(Organizations.Update, "ແກ້ໄຂຂໍ້ມູນອົງກອນ", PermissionModules.Organizations),
        new(Organizations.Delete, "ລຶບອົງກອນ", PermissionModules.Organizations),

        new(Dormitories.View, "ເບິ່ງຂໍ້ມູນຫໍພັກ", PermissionModules.Dormitories),
        new(Dormitories.Create, "ສ້າງຫໍພັກ", PermissionModules.Dormitories),
        new(Dormitories.Update, "ແກ້ໄຂຂໍ້ມູນຫໍພັກ", PermissionModules.Dormitories),
        new(Dormitories.Delete, "ລຶບຫໍພັກ", PermissionModules.Dormitories),

        new(Buildings.View, "ເບິ່ງຂໍ້ມູນອາຄານ", PermissionModules.Buildings),
        new(Buildings.Create, "ສ້າງອາຄານ", PermissionModules.Buildings),
        new(Buildings.Update, "ແກ້ໄຂຂໍ້ມູນອາຄານ", PermissionModules.Buildings),
        new(Buildings.Delete, "ລຶບອາຄານ", PermissionModules.Buildings),

        new(Floors.View, "ເບິ່ງຂໍ້ມູນຊັ້ນ", PermissionModules.Floors),
        new(Floors.Create, "ສ້າງຊັ້ນ", PermissionModules.Floors),
        new(Floors.Update, "ແກ້ໄຂຂໍ້ມູນຊັ້ນ", PermissionModules.Floors),
        new(Floors.Delete, "ລຶບຊັ້ນ", PermissionModules.Floors),

        new(RoomTypes.View, "ເບິ່ງຂໍ້ມູນປະເພດຫ້ອງ", PermissionModules.RoomTypes),
        new(RoomTypes.Create, "ສ້າງປະເພດຫ້ອງ", PermissionModules.RoomTypes),
        new(RoomTypes.Update, "ແກ້ໄຂຂໍ້ມູນປະເພດຫ້ອງ", PermissionModules.RoomTypes),
        new(RoomTypes.Delete, "ລຶບປະເພດຫ້ອງ", PermissionModules.RoomTypes),

        new(Rooms.View, "ເບິ່ງຂໍ້ມູນຫ້ອງພັກ", PermissionModules.Rooms),
        new(Rooms.Create, "ສ້າງຫ້ອງພັກ", PermissionModules.Rooms),
        new(Rooms.Update, "ແກ້ໄຂຂໍ້ມູນຫ້ອງພັກ", PermissionModules.Rooms),
        new(Rooms.Delete, "ລຶບຫ້ອງພັກ", PermissionModules.Rooms),
        new(Rooms.ChangeStatus, "ປ່ຽນສະຖານະຫ້ອງພັກ", PermissionModules.Rooms),

        new(Tenants.View, "ເບິ່ງຂໍ້ມູນຜູ້ເຊົ່າ", PermissionModules.Tenants),
        new(Tenants.Create, "ສ້າງຂໍ້ມູນຜູ້ເຊົ່າ", PermissionModules.Tenants),
        new(Tenants.Update, "ແກ້ໄຂຂໍ້ມູນຜູ້ເຊົ່າ", PermissionModules.Tenants),

        new(Reservations.View, "ເບິ່ງລາຍການຈອງ", PermissionModules.Reservations),
        new(Reservations.Create, "ສ້າງການຈອງໃໝ່", PermissionModules.Reservations),
        new(Reservations.Cancel, "ຍົກເລີກການຈອງ", PermissionModules.Reservations),

        new(Contracts.View, "ເບິ່ງສັນຍາເຊົ່າ", PermissionModules.Contracts),
        new(Contracts.Create, "ສ້າງສັນຍາເຊົ່າ", PermissionModules.Contracts),
        new(Contracts.Activate, "ເປີດໃຊ້ງານສັນຍາເຊົ່າ", PermissionModules.Contracts),
        new(Contracts.Terminate, "ສິ້ນສຸດສັນຍາເຊົ່າ", PermissionModules.Contracts),

        new(MeterReadings.View, "ເບິ່ງຄ່າມິເຕີ", PermissionModules.MeterReadings),
        new(MeterReadings.Create, "ບັນທຶກຄ່າມິເຕີ", PermissionModules.MeterReadings),

        new(Invoices.View, "ເບິ່ງໃບແຈ້ງໜີ້", PermissionModules.Invoices),
        new(Invoices.Create, "ສ້າງໃບແຈ້ງໜີ້", PermissionModules.Invoices),
        new(Invoices.Issue, "ອອກໃບແຈ້ງໜີ້", PermissionModules.Invoices),
        new(Invoices.Cancel, "ຍົກເລີກໃບແຈ້ງໜີ້", PermissionModules.Invoices),

        new(Payments.View, "ເບິ່ງລາຍການຊຳລະເງິນ", PermissionModules.Payments),
        new(Payments.Create, "ບັນທຶກການຊຳລະເງິນ", PermissionModules.Payments),
        new(Payments.Approve, "ອະນຸມັດການຊຳລະເງິນ", PermissionModules.Payments),
        new(Payments.Refund, "ຄືນເງິນ", PermissionModules.Payments),

        new(Deposits.View, "ເບິ່ງລາຍການເງິນມັດຈຳ", PermissionModules.Deposits),
        new(Deposits.Collect, "ບັນທຶກການຮັບເງິນມັດຈຳ", PermissionModules.Deposits),
        new(Deposits.Refund, "ຄືນເງິນມັດຈຳ", PermissionModules.Deposits),

        new(Receipts.View, "ເບິ່ງໃບຮັບເງິນ", PermissionModules.Receipts),
        new(Receipts.Issue, "ອອກໃບຮັບເງິນ", PermissionModules.Receipts),

        new(Expenses.View, "ເບິ່ງລາຍການຄ່າໃຊ້ຈ່າຍ", PermissionModules.Expenses),
        new(Expenses.Create, "ບັນທຶກຄ່າໃຊ້ຈ່າຍ", PermissionModules.Expenses),

        new(Parcels.View, "ເບິ່ງລາຍການພັດສະດຸ", PermissionModules.Parcels),
        new(Parcels.Log, "ບັນທຶກພັດສະດຸທີ່ມາຮອດ", PermissionModules.Parcels),
        new(Parcels.Pickup, "ບັນທຶກການຮັບພັດສະດຸ", PermissionModules.Parcels),

        new(Visitors.View, "ເບິ່ງລາຍການຜູ້ມາຢ້ຽມຢາມ", PermissionModules.Visitors),
        new(Visitors.CheckIn, "ບັນທຶກການເຂົ້າຂອງຜູ້ມາຢ້ຽມຢາມ", PermissionModules.Visitors),
        new(Visitors.CheckOut, "ບັນທຶກການອອກຂອງຜູ້ມາຢ້ຽມຢາມ", PermissionModules.Visitors),

        new(Parking.View, "ເບິ່ງລາຍການຈົດທະບຽນບ່ອນຈອດລົດ", PermissionModules.Parking),
        new(Parking.Register, "ຈົດທະບຽນບ່ອນຈອດລົດໃໝ່", PermissionModules.Parking),
        new(Parking.Release, "ສິ້ນສຸດການຈົດທະບຽນບ່ອນຈອດລົດ", PermissionModules.Parking),

        new(Announcements.View, "ເບິ່ງລາຍການປະກາດ", PermissionModules.Announcements),
        new(Announcements.Create, "ສ້າງປະກາດໃໝ່", PermissionModules.Announcements),
        new(Announcements.Publish, "ເຜີຍແຜ່ ຫຼື ເກັບປະກາດເຂົ້າສາງ", PermissionModules.Announcements),

        new(Notifications.Send, "ສົ່ງການແຈ້ງເຕືອນຫາຜູ້ໃຊ້", PermissionModules.Notifications),

        new(Maintenance.View, "ເບິ່ງງານສ້ອມແປງ", PermissionModules.Maintenance),
        new(Maintenance.Assign, "ມອບໝາຍງານສ້ອມແປງ", PermissionModules.Maintenance),
        new(Maintenance.UpdateStatus, "ອັບເດດສະຖານະງານສ້ອມແປງ", PermissionModules.Maintenance),

        new(Reports.View, "ເບິ່ງລາຍງານ", PermissionModules.Reports),
        new(Reports.Export, "ສົ່ງອອກລາຍງານ", PermissionModules.Reports),

        new(Administration.ManageUsers, "ຈັດການຜູ້ໃຊ້ງານ", PermissionModules.Administration),
        new(Administration.ManageRoles, "ຈັດການບົດບາດ ແລະ ສິດ", PermissionModules.Administration),
        new(Administration.ManageSettings, "ຈັດການການຕັ້ງຄ່າຂອງລະບົບ", PermissionModules.Administration),
        new(Administration.ViewAuditLogs, "ເບິ່ງ Audit Log", PermissionModules.Administration),

        new(Dashboard.View, "ເບິ່ງພາບລວມແດຊບອດ", PermissionModules.Dashboard),
    ];

    /// <summary>ລະຫັດໃບອະນຸຍາດທັງໝົດໃນຮູບແບບທີ່ສາມາດຊອກຫາໄດ້ໄວ</summary>
    public static IReadOnlySet<string> AllCodes { get; } =
        All.Select(permission => permission.Code).ToHashSet(StringComparer.Ordinal);

    /// <summary>ກວດເບິ່ງວ່າລະຫັດການອະນຸຍາດທີ່ລະບຸນັ້ນມີຢູ່ໃນລະບົບຫຼືບໍ່</summary>
    /// <param name="code">ລະຫັດການອະນຸຍາດທີ່ທ່ານຕ້ອງການກວດສອບ</param>
    public static bool Exists(string code) => AllCodes.Contains(code);
}
