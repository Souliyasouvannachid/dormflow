using DormFlow.Application.Common.Abstractions;

namespace DormFlow.Application.Common.Authorization;

/// <summary>
/// ການອະນຸຍາດເລີ່ມຕົ້ນສໍາລັບແຕ່ລະບົດບາດ, ການນໍາໃຊ້ພາກສ່ວນ Seed ຂໍ້ມູນ
/// </summary>
/// <remarks>
/// ມັນເປັນພຽງແຕ່ຄ່າເລີ່ມຕົ້ນ. ຜູ້ຈັດການສາມາດປັບການອະນຸຍາດສໍາລັບແຕ່ລະບົດບາດຕໍ່ມາ
/// ການອະນຸຍາດທີ່ຖືກແກ້ໄຂຈະບໍ່ຖືກຂຽນທັບໃນ Seed ຮອບຕໍ່ໄປ
/// </remarks>
public static class RolePermissionDefaults
{
    /// <summary>
    /// ການອະນຸຍາດເລີ່ມຕົ້ນສໍາລັບທຸກບົດບາດ
    /// </summary>
    /// <remarks>
    /// SuperAdmin ໄດ້ຮັບສິດທັງຝົດສໍາລັບຄວາມສົມບູນຂອງຂໍ້ມູນ
    /// ແຕ່ໃນທາງປະຕິບັດ, ການກວດສອບຄວາມຖືກຕ້ອງຈະຖືກຜ່ານທັນທີຈາກບົດບາດທີ່ມີຢູ່ແລ້ວ
    /// </remarks>
    public static IReadOnlyDictionary<string, IReadOnlyList<string>> ByRole { get; } =
        new Dictionary<string, IReadOnlyList<string>>(StringComparer.Ordinal)
        {
            [DormFlowRoles.SuperAdmin] = [.. PermissionCodes.AllCodes],

            // ເຈົ້າຂອງທຸລະກິດສາມາດເບິ່ງແຍງທຸກຢ່າງໃນອົງກອນຂອງພວກເຂົາ
            // ແຕ່ທ່ານບໍ່ໄດ້ຮັບອະນຸຍາດໃຫ້ສ້າງຫຼືລຶບອົງກອນ. ເຊິ່ງເປັນວຽກລະດັບຜູ້ບໍລິຫານ
            [DormFlowRoles.Owner] =
            [
                PermissionCodes.Organizations.View,
                PermissionCodes.Organizations.Update,

                PermissionCodes.Dormitories.View,
                PermissionCodes.Dormitories.Create,
                PermissionCodes.Dormitories.Update,
                PermissionCodes.Dormitories.Delete,

                PermissionCodes.Buildings.View,
                PermissionCodes.Buildings.Create,
                PermissionCodes.Buildings.Update,
                PermissionCodes.Buildings.Delete,

                PermissionCodes.Floors.View,
                PermissionCodes.Floors.Create,
                PermissionCodes.Floors.Update,
                PermissionCodes.Floors.Delete,

                PermissionCodes.RoomTypes.View,
                PermissionCodes.RoomTypes.Create,
                PermissionCodes.RoomTypes.Update,
                PermissionCodes.RoomTypes.Delete,

                PermissionCodes.Rooms.View,
                PermissionCodes.Rooms.Create,
                PermissionCodes.Rooms.Update,
                PermissionCodes.Rooms.Delete,
                PermissionCodes.Rooms.ChangeStatus,

                PermissionCodes.Tenants.View,
                PermissionCodes.Tenants.Create,
                PermissionCodes.Tenants.Update,

                PermissionCodes.Reservations.View,
                PermissionCodes.Reservations.Create,
                PermissionCodes.Reservations.Cancel,

                PermissionCodes.Contracts.View,
                PermissionCodes.Contracts.Create,
                PermissionCodes.Contracts.Activate,
                PermissionCodes.Contracts.Terminate,

                PermissionCodes.MeterReadings.View,
                PermissionCodes.MeterReadings.Create,

                PermissionCodes.Invoices.View,
                PermissionCodes.Invoices.Create,
                PermissionCodes.Invoices.Issue,
                PermissionCodes.Invoices.Cancel,

                PermissionCodes.Payments.View,
                PermissionCodes.Payments.Create,
                PermissionCodes.Payments.Approve,
                PermissionCodes.Payments.Refund,

                PermissionCodes.Deposits.View,
                PermissionCodes.Deposits.Collect,
                PermissionCodes.Deposits.Refund,

                PermissionCodes.Receipts.View,
                PermissionCodes.Receipts.Issue,

                PermissionCodes.Expenses.View,
                PermissionCodes.Expenses.Create,

                PermissionCodes.Parcels.View,
                PermissionCodes.Parcels.Log,
                PermissionCodes.Parcels.Pickup,

                PermissionCodes.Visitors.View,
                PermissionCodes.Visitors.CheckIn,
                PermissionCodes.Visitors.CheckOut,

                PermissionCodes.Parking.View,
                PermissionCodes.Parking.Register,
                PermissionCodes.Parking.Release,

                PermissionCodes.Maintenance.View,
                PermissionCodes.Maintenance.Assign,
                PermissionCodes.Maintenance.UpdateStatus,

                PermissionCodes.Announcements.View,
                PermissionCodes.Announcements.Create,
                PermissionCodes.Announcements.Publish,

                PermissionCodes.Notifications.Send,

                PermissionCodes.Reports.View,
                PermissionCodes.Reports.Export,

                PermissionCodes.Administration.ManageUsers,
                PermissionCodes.Administration.ManageRoles,
                PermissionCodes.Administration.ManageSettings,
                PermissionCodes.Administration.ViewAuditLogs,

                PermissionCodes.Dashboard.View,
            ],

            // ຜູ້ຈັດການຫໍພັກສາມາດດູແລວຽກງານປະຈໍາວັນທັງຝົດ
            // ແຕ່ທ່ານບໍ່ມີການອະນຸຍາດໃຫ້ແກ້ໄຂໂຄງສ້າງອົງກອນ, ການຄຸ້ມຄອງຜູ້ໃຊ້, ຫຼືການສົ່ງເງິນຄືນ
            [DormFlowRoles.DormManager] =
            [
                PermissionCodes.Dormitories.View,
                PermissionCodes.Dormitories.Update,

                PermissionCodes.Buildings.View,
                PermissionCodes.Buildings.Create,
                PermissionCodes.Buildings.Update,

                PermissionCodes.Floors.View,
                PermissionCodes.Floors.Create,
                PermissionCodes.Floors.Update,

                PermissionCodes.RoomTypes.View,
                PermissionCodes.RoomTypes.Create,
                PermissionCodes.RoomTypes.Update,

                PermissionCodes.Rooms.View,
                PermissionCodes.Rooms.Create,
                PermissionCodes.Rooms.Update,
                PermissionCodes.Rooms.ChangeStatus,

                PermissionCodes.Tenants.View,
                PermissionCodes.Tenants.Create,
                PermissionCodes.Tenants.Update,

                PermissionCodes.Reservations.View,
                PermissionCodes.Reservations.Create,
                PermissionCodes.Reservations.Cancel,

                PermissionCodes.Contracts.View,
                PermissionCodes.Contracts.Create,
                PermissionCodes.Contracts.Activate,
                PermissionCodes.Contracts.Terminate,

                PermissionCodes.MeterReadings.View,
                PermissionCodes.MeterReadings.Create,

                PermissionCodes.Invoices.View,
                PermissionCodes.Invoices.Create,
                PermissionCodes.Invoices.Issue,

                PermissionCodes.Payments.View,
                PermissionCodes.Payments.Create,

                PermissionCodes.Deposits.View,
                PermissionCodes.Deposits.Collect,

                PermissionCodes.Receipts.View,
                PermissionCodes.Receipts.Issue,

                PermissionCodes.Expenses.View,
                PermissionCodes.Expenses.Create,

                PermissionCodes.Parcels.View,
                PermissionCodes.Parcels.Log,
                PermissionCodes.Parcels.Pickup,

                PermissionCodes.Visitors.View,
                PermissionCodes.Visitors.CheckIn,
                PermissionCodes.Visitors.CheckOut,

                PermissionCodes.Parking.View,
                PermissionCodes.Parking.Register,
                PermissionCodes.Parking.Release,

                PermissionCodes.Maintenance.View,
                PermissionCodes.Maintenance.Assign,
                PermissionCodes.Maintenance.UpdateStatus,

                PermissionCodes.Announcements.View,
                PermissionCodes.Announcements.Create,
                PermissionCodes.Announcements.Publish,

                PermissionCodes.Notifications.Send,

                PermissionCodes.Reports.View,
                PermissionCodes.Reports.Export,

                PermissionCodes.Dashboard.View,
            ],

            // ພະແນກບັນຊີເບິ່ງແຍງທຸກເລື່ອງການເງິນ. ແລະເບິ່ງຂໍ້ມູນອື່ນໆຕາມຄວາມຈໍາເປັນສໍາລັບການອອກເອກະສານ
            [DormFlowRoles.Accountant] =
            [
                PermissionCodes.Dormitories.View,
                PermissionCodes.Buildings.View,
                PermissionCodes.Rooms.View,
                PermissionCodes.Tenants.View,
                PermissionCodes.Reservations.View,
                PermissionCodes.Contracts.View,
                PermissionCodes.MeterReadings.View,

                PermissionCodes.Invoices.View,
                PermissionCodes.Invoices.Create,
                PermissionCodes.Invoices.Issue,
                PermissionCodes.Invoices.Cancel,

                PermissionCodes.Payments.View,
                PermissionCodes.Payments.Create,
                PermissionCodes.Payments.Approve,
                PermissionCodes.Payments.Refund,

                PermissionCodes.Deposits.View,
                PermissionCodes.Deposits.Collect,
                PermissionCodes.Deposits.Refund,

                PermissionCodes.Receipts.View,
                PermissionCodes.Receipts.Issue,

                PermissionCodes.Expenses.View,
                PermissionCodes.Expenses.Create,

                PermissionCodes.Reports.View,
                PermissionCodes.Reports.Export,

                PermissionCodes.Dashboard.View,
            ],

            // ພະນັກງານທົ່ວໄປເຮັດວຽກຢູ່ຕໍ່ໜ້າເຄົາເຕີ, ສາມາດເບິ່ງຂໍ້ມູນ ແລະ ສາມາດບັນທຶກບາງວຽກປະຈໍາວັນໄດ້
            [DormFlowRoles.Staff] =
            [
                PermissionCodes.Dormitories.View,
                PermissionCodes.Buildings.View,
                PermissionCodes.Floors.View,
                PermissionCodes.RoomTypes.View,

                PermissionCodes.Rooms.View,
                PermissionCodes.Rooms.ChangeStatus,

                PermissionCodes.Tenants.View,
                PermissionCodes.Tenants.Create,
                PermissionCodes.Tenants.Update,

                PermissionCodes.Reservations.View,
                PermissionCodes.Reservations.Create,
                PermissionCodes.Reservations.Cancel,

                PermissionCodes.Contracts.View,

                PermissionCodes.MeterReadings.View,
                PermissionCodes.MeterReadings.Create,

                PermissionCodes.Invoices.View,
                PermissionCodes.Payments.View,
                PermissionCodes.Deposits.View,
                PermissionCodes.Receipts.View,

                PermissionCodes.Parcels.View,
                PermissionCodes.Parcels.Log,
                PermissionCodes.Parcels.Pickup,

                PermissionCodes.Visitors.View,
                PermissionCodes.Visitors.CheckIn,
                PermissionCodes.Visitors.CheckOut,

                PermissionCodes.Parking.View,
                PermissionCodes.Parking.Register,
                PermissionCodes.Parking.Release,

                PermissionCodes.Maintenance.View,
                PermissionCodes.Maintenance.UpdateStatus,

                PermissionCodes.Announcements.View,

                PermissionCodes.Dashboard.View,
            ],

            // ນັກວິຊາການບໍາລຸງຮັກສາເຫັນພຽງແຕ່ຂໍ້ມູນທີ່ພວກເຂົາຕ້ອງການເພື່ອເຮັດວຽກຢູ່ໃນຫ້ອງ
            [DormFlowRoles.Technician] =
            [
                PermissionCodes.Dormitories.View,
                PermissionCodes.Buildings.View,
                PermissionCodes.Floors.View,
                PermissionCodes.Rooms.View,

                PermissionCodes.Maintenance.View,
                PermissionCodes.Maintenance.UpdateStatus,

                PermissionCodes.Dashboard.View,
            ],

            // ຜູ້ເຊົ່າບໍ່ມີສິດຄຸ້ມຄອງທັງໝົດ
            // ຂໍ້ມູນຂອງທ່ານເອງຈະຖືກເຂົ້າເຖິງໂດຍຜ່ານ Endpoint ພຽງແຕ່ຜູ້ທີ່ຖືກກວດສອບໂດຍກົງຈາກ ID ຜູ້ໃຊ້
            [DormFlowRoles.Tenant] = [],
        };

    /// <summary>ອ່ານການອະນຸຍາດເລີ່ມຕົ້ນຂອງບົດບາດທີ່ກໍານົດໄວ້. ສົ່ງຄືນລາຍຊື່ຫວ່າງເປົ່າເມື່ອບໍ່ພົບບົດບາດ</summary>
    /// <param name="roleName">ຊື່ບົດບາດ</param>
    public static IReadOnlyList<string> ForRole(string roleName)
        => ByRole.TryGetValue(roleName, out IReadOnlyList<string>? permissions) ? permissions : [];
}
