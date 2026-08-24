using DormFlow.Application.Common.Abstractions;

namespace DormFlow.Application.Common.Authorization;

/// <summary>
/// สิทธิ์เริ่มต้นของแต่ละบทบาท ใช้ตอน Seed ข้อมูล
/// </summary>
/// <remarks>
/// เป็นเพียงค่าตั้งต้นเท่านั้น ผู้ดูแลระบบปรับสิทธิ์ของแต่ละบทบาทได้ภายหลัง
/// ระบบจะไม่เขียนทับสิทธิ์ที่ถูกแก้ไขไปแล้วในการ Seed รอบถัดไป
/// </remarks>
public static class RolePermissionDefaults
{
    /// <summary>
    /// สิทธิ์เริ่มต้นของทุกบทบาท
    /// </summary>
    /// <remarks>
    /// SuperAdmin ได้รับทุกสิทธิ์เพื่อความครบถ้วนของข้อมูล
    /// แต่ในทางปฏิบัติการตรวจสิทธิ์จะผ่านทันทีจากบทบาทอยู่แล้ว
    /// </remarks>
    public static IReadOnlyDictionary<string, IReadOnlyList<string>> ByRole { get; } =
        new Dictionary<string, IReadOnlyList<string>>(StringComparer.Ordinal)
        {
            [DormFlowRoles.SuperAdmin] = [.. PermissionCodes.AllCodes],

            // เจ้าของกิจการดูแลได้ทุกอย่างในองค์กรของตน
            // แต่ไม่มีสิทธิ์สร้างหรือลบองค์กร ซึ่งเป็นงานระดับผู้ดูแลระบบ
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

                PermissionCodes.Maintenance.View,
                PermissionCodes.Maintenance.Assign,
                PermissionCodes.Maintenance.UpdateStatus,

                PermissionCodes.Reports.View,
                PermissionCodes.Reports.Export,

                PermissionCodes.Administration.ManageUsers,
                PermissionCodes.Administration.ManageRoles,
                PermissionCodes.Administration.ManageSettings,
                PermissionCodes.Administration.ViewAuditLogs,
            ],

            // ผู้จัดการหอพักดูแลงานประจำวันได้ทั้งหมด
            // แต่ไม่มีสิทธิ์แก้โครงสร้างองค์กร จัดการผู้ใช้ หรือคืนเงิน
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

                PermissionCodes.Maintenance.View,
                PermissionCodes.Maintenance.Assign,
                PermissionCodes.Maintenance.UpdateStatus,

                PermissionCodes.Reports.View,
                PermissionCodes.Reports.Export,
            ],

            // ฝ่ายบัญชีดูแลเรื่องเงินทั้งหมด และดูข้อมูลอื่นได้เท่าที่จำเป็นต่อการออกเอกสาร
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
            ],

            // เจ้าหน้าที่ทั่วไปทำงานหน้าเคาน์เตอร์ ดูข้อมูลได้ และบันทึกงานประจำวันได้บางส่วน
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

                PermissionCodes.Maintenance.View,
                PermissionCodes.Maintenance.UpdateStatus,
            ],

            // ช่างซ่อมบำรุงเห็นเฉพาะข้อมูลที่จำเป็นต่อการเข้าไปทำงานในห้อง
            [DormFlowRoles.Technician] =
            [
                PermissionCodes.Dormitories.View,
                PermissionCodes.Buildings.View,
                PermissionCodes.Floors.View,
                PermissionCodes.Rooms.View,

                PermissionCodes.Maintenance.View,
                PermissionCodes.Maintenance.UpdateStatus,
            ],

            // ผู้เช่าไม่มีสิทธิ์ฝั่งผู้ดูแลเลย
            // ข้อมูลของตนเองจะเข้าถึงผ่าน Endpoint เฉพาะที่ตรวจจากรหัสผู้ใช้โดยตรง
            [DormFlowRoles.Tenant] = [],
        };

    /// <summary>อ่านสิทธิ์เริ่มต้นของบทบาทที่ระบุ คืนรายการว่างเมื่อไม่พบบทบาทนั้น</summary>
    /// <param name="roleName">ชื่อบทบาท</param>
    public static IReadOnlyList<string> ForRole(string roleName)
        => ByRole.TryGetValue(roleName, out IReadOnlyList<string>? permissions) ? permissions : [];
}
