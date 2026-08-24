using System.Reflection;
using DormFlow.Domain.Entities;
using DormFlow.Infrastructure.Identity;
using DormFlow.Infrastructure.Persistence.Extensions;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DormFlow.Infrastructure.Persistence;

/// <summary>
/// DbContext หลักของ DormFlow
/// </summary>
/// <remarks>
/// สืบทอดจาก <see cref="IdentityDbContext{TUser, TRole, TKey}"/> เพื่อให้ตารางของ ASP.NET Core Identity
/// อยู่ในฐานข้อมูลเดียวกับข้อมูลธุรกิจ จึงใช้ธุรกรรมเดียวครอบการสร้างผู้ใช้พร้อมข้อมูลที่เกี่ยวข้องได้
/// การตั้งค่าแต่ละ Entity อยู่ในคลาส IEntityTypeConfiguration แยกไฟล์ใต้โฟลเดอร์ Configurations
/// และถูกโหลดอัตโนมัติด้วย ApplyConfigurationsFromAssembly
/// </remarks>
public class DormFlowDbContext(DbContextOptions<DormFlowDbContext> options)
    : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>(options)
{
    /// <summary>Assembly ที่เก็บ Entity Configuration และ Migration ทั้งหมด</summary>
    public static Assembly InfrastructureAssembly => typeof(DormFlowDbContext).Assembly;

    /// <summary>
    /// ชื่อตารางที่ใช้บันทึกประวัติ Migration
    /// </summary>
    /// <remarks>
    /// ต้องเป็นค่าเดียวกันทั้งฝั่งลงทะเบียน DI ตอนแอปทำงานจริง (<see cref="DependencyInjection"/>)
    /// และฝั่ง <see cref="DormFlowDbContextFactory"/> ที่ dotnet ef ใช้ตอน design time
    /// ถ้าค่าไม่ตรงกัน dotnet ef จะสร้างและบันทึกลงตารางคนละชื่อกับที่แอปมองหาตอนรันจริง
    /// ทำให้แอปเข้าใจผิดว่ายังไม่มี Migration ใดถูกใช้เลยและพยายามสร้างตารางซ้ำ
    /// </remarks>
    public const string MigrationsHistoryTableName = "__ef_migrations_history";

    /// <summary>องค์กรที่ใช้งานระบบ</summary>
    public DbSet<Organization> Organizations => Set<Organization>();

    /// <summary>สิทธิ์การใช้งานทั้งหมดของระบบ</summary>
    public DbSet<Permission> Permissions => Set<Permission>();

    /// <summary>ความสัมพันธ์ระหว่างบทบาทกับสิทธิ์</summary>
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();

    /// <summary>Refresh Token ที่ออกให้ผู้ใช้</summary>
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    /// <summary>บันทึกเหตุการณ์สำคัญของระบบ</summary>
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    /// <summary>หอพักทั้งหมดในระบบ</summary>
    public DbSet<Dormitory> Dormitories => Set<Dormitory>();

    /// <summary>อาคารทั้งหมดในระบบ</summary>
    public DbSet<Building> Buildings => Set<Building>();

    /// <summary>ชั้นของอาคารทั้งหมดในระบบ</summary>
    public DbSet<Floor> Floors => Set<Floor>();

    /// <summary>ประเภทห้องทั้งหมดในระบบ</summary>
    public DbSet<RoomType> RoomTypes => Set<RoomType>();

    /// <summary>ห้องพักทั้งหมดในระบบ</summary>
    public DbSet<Room> Rooms => Set<Room>();

    /// <summary>การมอบหมายผู้ใช้ให้ดูแลหอพัก</summary>
    public DbSet<UserDormitory> UserDormitories => Set<UserDormitory>();

    /// <summary>ผู้เช่าทั้งหมดในระบบ</summary>
    public DbSet<Tenant> Tenants => Set<Tenant>();

    /// <summary>การจองห้องพักทั้งหมดในระบบ</summary>
    public DbSet<Reservation> Reservations => Set<Reservation>();

    /// <summary>สัญญาเช่าทั้งหมดในระบบ</summary>
    public DbSet<Contract> Contracts => Set<Contract>();

    /// <summary>ค่ามิเตอร์ทั้งหมดในระบบ</summary>
    public DbSet<MeterReading> MeterReadings => Set<MeterReading>();

    /// <summary>ใบแจ้งหนี้ทั้งหมดในระบบ</summary>
    public DbSet<Invoice> Invoices => Set<Invoice>();

    /// <summary>รายการชำระเงินทั้งหมดในระบบ</summary>
    public DbSet<Payment> Payments => Set<Payment>();

    /// <summary>งานแจ้งซ่อมบำรุงทั้งหมดในระบบ</summary>
    public DbSet<MaintenanceRequest> MaintenanceRequests => Set<MaintenanceRequest>();

    /// <summary>เงินมัดจำทั้งหมดในระบบ</summary>
    public DbSet<Deposit> Deposits => Set<Deposit>();

    /// <summary>ใบเสร็จรับเงินทั้งหมดในระบบ</summary>
    public DbSet<Receipt> Receipts => Set<Receipt>();

    /// <summary>ค่าใช้จ่ายทั้งหมดในระบบ</summary>
    public DbSet<Expense> Expenses => Set<Expense>();

    /// <summary>พัสดุทั้งหมดในระบบ</summary>
    public DbSet<Parcel> Parcels => Set<Parcel>();

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        base.OnModelCreating(builder);

        builder.ApplyConfigurationsFromAssembly(InfrastructureAssembly);

        // ต้องเรียกหลัง ApplyConfigurationsFromAssembly เพื่อให้ข้อตกลงร่วมทับค่าที่ยังไม่ได้กำหนดเอง
        builder.ApplyDormFlowConventions();
    }

    /// <inheritdoc />
    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        ArgumentNullException.ThrowIfNull(configurationBuilder);

        base.ConfigureConventions(configurationBuilder);

        // จำกัดความยาวเริ่มต้นของ string เพื่อไม่ให้เผลอสร้างคอลัมน์ text ที่ไม่มีขอบเขต
        configurationBuilder.Properties<string>().HaveMaxLength(256);
    }
}
