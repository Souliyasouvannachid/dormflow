namespace DormFlow.Application.Common.Authorization;

/// <summary>
/// รหัสสิทธิ์ทั้งหมดของระบบ
/// </summary>
/// <remarks>
/// ไฟล์นี้คือแหล่งความจริงเดียวของรายการสิทธิ์
/// ตาราง permissions ในฐานข้อมูลถูก Seed จากรายการนี้ ไม่ได้ให้ผู้ใช้เพิ่มเอง
/// เพราะทุกรหัสต้องมีจุดที่ตรวจสอบอยู่ในโค้ดจริง มิฉะนั้นจะเป็นสิทธิ์ที่ไม่มีผลใด ๆ
/// </remarks>
public static class PermissionCodes
{
    /// <summary>สิทธิ์เกี่ยวกับองค์กร</summary>
    public static class Organizations
    {
        /// <summary>ดูข้อมูลองค์กร</summary>
        public const string View = "organizations.view";

        /// <summary>สร้างองค์กรใหม่</summary>
        public const string Create = "organizations.create";

        /// <summary>แก้ไขข้อมูลองค์กร</summary>
        public const string Update = "organizations.update";

        /// <summary>ลบองค์กร</summary>
        public const string Delete = "organizations.delete";
    }

    /// <summary>สิทธิ์เกี่ยวกับหอพัก</summary>
    public static class Dormitories
    {
        /// <summary>ดูข้อมูลหอพัก</summary>
        public const string View = "dormitories.view";

        /// <summary>สร้างหอพักใหม่</summary>
        public const string Create = "dormitories.create";

        /// <summary>แก้ไขข้อมูลหอพัก</summary>
        public const string Update = "dormitories.update";

        /// <summary>ลบหอพัก</summary>
        public const string Delete = "dormitories.delete";
    }

    /// <summary>สิทธิ์เกี่ยวกับอาคาร</summary>
    public static class Buildings
    {
        /// <summary>ดูข้อมูลอาคาร</summary>
        public const string View = "buildings.view";

        /// <summary>สร้างอาคารใหม่</summary>
        public const string Create = "buildings.create";

        /// <summary>แก้ไขข้อมูลอาคาร</summary>
        public const string Update = "buildings.update";

        /// <summary>ลบอาคาร</summary>
        public const string Delete = "buildings.delete";
    }

    /// <summary>สิทธิ์เกี่ยวกับชั้นของอาคาร</summary>
    public static class Floors
    {
        /// <summary>ดูข้อมูลชั้น</summary>
        public const string View = "floors.view";

        /// <summary>สร้างชั้นใหม่</summary>
        public const string Create = "floors.create";

        /// <summary>แก้ไขข้อมูลชั้น</summary>
        public const string Update = "floors.update";

        /// <summary>ลบชั้น</summary>
        public const string Delete = "floors.delete";
    }

    /// <summary>สิทธิ์เกี่ยวกับประเภทห้อง</summary>
    public static class RoomTypes
    {
        /// <summary>ดูข้อมูลประเภทห้อง</summary>
        public const string View = "room-types.view";

        /// <summary>สร้างประเภทห้องใหม่</summary>
        public const string Create = "room-types.create";

        /// <summary>แก้ไขข้อมูลประเภทห้อง</summary>
        public const string Update = "room-types.update";

        /// <summary>ลบประเภทห้อง</summary>
        public const string Delete = "room-types.delete";
    }

    /// <summary>สิทธิ์เกี่ยวกับห้องพัก</summary>
    public static class Rooms
    {
        /// <summary>ดูข้อมูลห้องพัก</summary>
        public const string View = "rooms.view";

        /// <summary>สร้างห้องพักใหม่</summary>
        public const string Create = "rooms.create";

        /// <summary>แก้ไขข้อมูลห้องพัก</summary>
        public const string Update = "rooms.update";

        /// <summary>ลบห้องพัก</summary>
        public const string Delete = "rooms.delete";

        /// <summary>เปลี่ยนสถานะห้องพัก</summary>
        public const string ChangeStatus = "rooms.change-status";
    }

    /// <summary>สิทธิ์เกี่ยวกับผู้เช่า</summary>
    public static class Tenants
    {
        /// <summary>ดูข้อมูลผู้เช่า</summary>
        public const string View = "tenants.view";

        /// <summary>สร้างข้อมูลผู้เช่าใหม่</summary>
        public const string Create = "tenants.create";

        /// <summary>แก้ไขข้อมูลผู้เช่า</summary>
        public const string Update = "tenants.update";
    }

    /// <summary>สิทธิ์เกี่ยวกับสัญญาเช่า</summary>
    public static class Contracts
    {
        /// <summary>ดูสัญญาเช่า</summary>
        public const string View = "contracts.view";

        /// <summary>สร้างสัญญาเช่า</summary>
        public const string Create = "contracts.create";

        /// <summary>เปิดใช้งานสัญญาเช่า</summary>
        public const string Activate = "contracts.activate";

        /// <summary>ยกเลิกหรือสิ้นสุดสัญญาเช่า</summary>
        public const string Terminate = "contracts.terminate";
    }

    /// <summary>สิทธิ์เกี่ยวกับการจดมิเตอร์</summary>
    public static class MeterReadings
    {
        /// <summary>ดูค่ามิเตอร์</summary>
        public const string View = "meter-readings.view";

        /// <summary>บันทึกค่ามิเตอร์</summary>
        public const string Create = "meter-readings.create";
    }

    /// <summary>สิทธิ์เกี่ยวกับใบแจ้งหนี้</summary>
    public static class Invoices
    {
        /// <summary>ดูใบแจ้งหนี้</summary>
        public const string View = "invoices.view";

        /// <summary>สร้างใบแจ้งหนี้</summary>
        public const string Create = "invoices.create";

        /// <summary>ออกใบแจ้งหนี้ให้ผู้เช่า</summary>
        public const string Issue = "invoices.issue";

        /// <summary>ยกเลิกใบแจ้งหนี้</summary>
        public const string Cancel = "invoices.cancel";
    }

    /// <summary>สิทธิ์เกี่ยวกับการชำระเงิน</summary>
    public static class Payments
    {
        /// <summary>ดูรายการชำระเงิน</summary>
        public const string View = "payments.view";

        /// <summary>บันทึกการชำระเงิน</summary>
        public const string Create = "payments.create";

        /// <summary>อนุมัติการชำระเงิน</summary>
        public const string Approve = "payments.approve";

        /// <summary>คืนเงิน</summary>
        public const string Refund = "payments.refund";
    }

    /// <summary>สิทธิ์เกี่ยวกับงานซ่อมบำรุง</summary>
    public static class Maintenance
    {
        /// <summary>ดูงานซ่อมบำรุง</summary>
        public const string View = "maintenance.view";

        /// <summary>มอบหมายงานซ่อมบำรุง</summary>
        public const string Assign = "maintenance.assign";

        /// <summary>อัปเดตสถานะงานซ่อมบำรุง</summary>
        public const string UpdateStatus = "maintenance.update-status";
    }

    /// <summary>สิทธิ์เกี่ยวกับรายงาน</summary>
    public static class Reports
    {
        /// <summary>ดูรายงาน</summary>
        public const string View = "reports.view";

        /// <summary>ส่งออกรายงาน</summary>
        public const string Export = "reports.export";
    }

    /// <summary>สิทธิ์เกี่ยวกับการดูแลระบบ</summary>
    public static class Administration
    {
        /// <summary>จัดการผู้ใช้งาน</summary>
        public const string ManageUsers = "users.manage";

        /// <summary>จัดการบทบาทและสิทธิ์</summary>
        public const string ManageRoles = "roles.manage";

        /// <summary>จัดการค่าตั้งของระบบ</summary>
        public const string ManageSettings = "settings.manage";

        /// <summary>ดู Audit Log</summary>
        public const string ViewAuditLogs = "audit-logs.view";
    }

    /// <summary>นิยามของสิทธิ์ทั้งหมด เรียงตามโมดูล</summary>
    public static IReadOnlyList<PermissionDefinition> All { get; } =
    [
        new(Organizations.View, "ดูข้อมูลองค์กร", PermissionModules.Organizations),
        new(Organizations.Create, "สร้างองค์กร", PermissionModules.Organizations),
        new(Organizations.Update, "แก้ไขข้อมูลองค์กร", PermissionModules.Organizations),
        new(Organizations.Delete, "ลบองค์กร", PermissionModules.Organizations),

        new(Dormitories.View, "ดูข้อมูลหอพัก", PermissionModules.Dormitories),
        new(Dormitories.Create, "สร้างหอพัก", PermissionModules.Dormitories),
        new(Dormitories.Update, "แก้ไขข้อมูลหอพัก", PermissionModules.Dormitories),
        new(Dormitories.Delete, "ลบหอพัก", PermissionModules.Dormitories),

        new(Buildings.View, "ดูข้อมูลอาคาร", PermissionModules.Buildings),
        new(Buildings.Create, "สร้างอาคาร", PermissionModules.Buildings),
        new(Buildings.Update, "แก้ไขข้อมูลอาคาร", PermissionModules.Buildings),
        new(Buildings.Delete, "ลบอาคาร", PermissionModules.Buildings),

        new(Floors.View, "ดูข้อมูลชั้น", PermissionModules.Floors),
        new(Floors.Create, "สร้างชั้น", PermissionModules.Floors),
        new(Floors.Update, "แก้ไขข้อมูลชั้น", PermissionModules.Floors),
        new(Floors.Delete, "ลบชั้น", PermissionModules.Floors),

        new(RoomTypes.View, "ดูข้อมูลประเภทห้อง", PermissionModules.RoomTypes),
        new(RoomTypes.Create, "สร้างประเภทห้อง", PermissionModules.RoomTypes),
        new(RoomTypes.Update, "แก้ไขข้อมูลประเภทห้อง", PermissionModules.RoomTypes),
        new(RoomTypes.Delete, "ลบประเภทห้อง", PermissionModules.RoomTypes),

        new(Rooms.View, "ดูข้อมูลห้องพัก", PermissionModules.Rooms),
        new(Rooms.Create, "สร้างห้องพัก", PermissionModules.Rooms),
        new(Rooms.Update, "แก้ไขข้อมูลห้องพัก", PermissionModules.Rooms),
        new(Rooms.Delete, "ลบห้องพัก", PermissionModules.Rooms),
        new(Rooms.ChangeStatus, "เปลี่ยนสถานะห้องพัก", PermissionModules.Rooms),

        new(Tenants.View, "ดูข้อมูลผู้เช่า", PermissionModules.Tenants),
        new(Tenants.Create, "สร้างข้อมูลผู้เช่า", PermissionModules.Tenants),
        new(Tenants.Update, "แก้ไขข้อมูลผู้เช่า", PermissionModules.Tenants),

        new(Contracts.View, "ดูสัญญาเช่า", PermissionModules.Contracts),
        new(Contracts.Create, "สร้างสัญญาเช่า", PermissionModules.Contracts),
        new(Contracts.Activate, "เปิดใช้งานสัญญาเช่า", PermissionModules.Contracts),
        new(Contracts.Terminate, "สิ้นสุดสัญญาเช่า", PermissionModules.Contracts),

        new(MeterReadings.View, "ดูค่ามิเตอร์", PermissionModules.MeterReadings),
        new(MeterReadings.Create, "บันทึกค่ามิเตอร์", PermissionModules.MeterReadings),

        new(Invoices.View, "ดูใบแจ้งหนี้", PermissionModules.Invoices),
        new(Invoices.Create, "สร้างใบแจ้งหนี้", PermissionModules.Invoices),
        new(Invoices.Issue, "ออกใบแจ้งหนี้", PermissionModules.Invoices),
        new(Invoices.Cancel, "ยกเลิกใบแจ้งหนี้", PermissionModules.Invoices),

        new(Payments.View, "ดูรายการชำระเงิน", PermissionModules.Payments),
        new(Payments.Create, "บันทึกการชำระเงิน", PermissionModules.Payments),
        new(Payments.Approve, "อนุมัติการชำระเงิน", PermissionModules.Payments),
        new(Payments.Refund, "คืนเงิน", PermissionModules.Payments),

        new(Maintenance.View, "ดูงานซ่อมบำรุง", PermissionModules.Maintenance),
        new(Maintenance.Assign, "มอบหมายงานซ่อมบำรุง", PermissionModules.Maintenance),
        new(Maintenance.UpdateStatus, "อัปเดตสถานะงานซ่อมบำรุง", PermissionModules.Maintenance),

        new(Reports.View, "ดูรายงาน", PermissionModules.Reports),
        new(Reports.Export, "ส่งออกรายงาน", PermissionModules.Reports),

        new(Administration.ManageUsers, "จัดการผู้ใช้งาน", PermissionModules.Administration),
        new(Administration.ManageRoles, "จัดการบทบาทและสิทธิ์", PermissionModules.Administration),
        new(Administration.ManageSettings, "จัดการค่าตั้งของระบบ", PermissionModules.Administration),
        new(Administration.ViewAuditLogs, "ดู Audit Log", PermissionModules.Administration),
    ];

    /// <summary>รหัสสิทธิ์ทั้งหมดในรูปแบบชุดที่ค้นหาได้เร็ว</summary>
    public static IReadOnlySet<string> AllCodes { get; } =
        All.Select(permission => permission.Code).ToHashSet(StringComparer.Ordinal);

    /// <summary>ตรวจว่ารหัสสิทธิ์ที่ระบุมีอยู่จริงในระบบหรือไม่</summary>
    /// <param name="code">รหัสสิทธิ์ที่ต้องการตรวจ</param>
    public static bool Exists(string code) => AllCodes.Contains(code);
}
