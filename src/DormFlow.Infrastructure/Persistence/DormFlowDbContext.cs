using System.Reflection;
using DormFlow.Domain.Entities;
using DormFlow.Infrastructure.Identity;
using DormFlow.Infrastructure.Persistence.Extensions;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DormFlow.Infrastructure.Persistence;

/// <summary>
/// DbContext ຕົ້ນຕໍຂອງ DormFlow
/// </summary>
/// <remarks>
/// ສືບທອດມາຈາກ <see cref="IdentityDbContext{TUser, TRole, TKey}"/> ດັ່ງນັ້ນຕາຕະລາງຂອງ ASP.NET Core Identity
/// ມັນຢູ່ໃນຖານຂໍ້ມູນດຽວກັນກັບຂໍ້ມູນທຸລະກິດ. ດັ່ງນັ້ນ, ການເຮັດທຸລະກໍາດຽວສາມາດຖືກນໍາໃຊ້ເພື່ອກວມເອົາການສ້າງຜູ້ໃຊ້ທີ່ມີຂໍ້ມູນທີ່ກ່ຽວຂ້ອງ
/// ແຕ່ລະການຕັ້ງຄ່າ Entity ໃນຫ້ອງຮຽນ IEntityTypeConfiguration ແຍກໄຟລ໌ພາຍໃຕ້ໂຟນເດີ Configurations
/// ແລະຖືກໂຫຼດໂດຍອັດຕະໂນມັດ ApplyConfigurationsFromAssembly
/// </remarks>
public class DormFlowDbContext(DbContextOptions<DormFlowDbContext> options)
    : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>(options)
{
    /// <summary>Assembly ການເກັບຮັກສາ Entity Configuration ແລະ Migration ທັງໝົດ</summary>
    public static Assembly InfrastructureAssembly => typeof(DormFlowDbContext).Assembly;

    /// <summary>
    /// ຊື່ຂອງຕາຕະລາງທີ່ໃຊ້ໃນການບັນທຶກປະຫວັດສາດ Migration
    /// </summary>
    /// <remarks>
    /// ຕ້ອງເປັນຄ່າດຽວກັນທັງສອງດ້ານທີ່ລົງທະບຽນ DI ເມື່ອ app ເຮັດວຽກຕົວຈິງ (<see cref="DependencyInjection"/>)
    /// ແລະຝັ່ງ <see cref="DormFlowDbContextFactory"/> ທີ່ dotnet ef ໃຊ້ຕອນ design time
    /// ຖ້າຄ່າບໍ່ກົງກັນ dotnet ef ມັນຈະຖືກສ້າງແລະບັນທຶກໄວ້ໃນຕາຕະລາງທີ່ມີຊື່ທີ່ແຕກຕ່າງກັນກ່ວາສິ່ງທີ່ app ຊອກຫາໃນເວລາທີ່ມັນດໍາເນີນການຕົວຈິງ
    /// ເຮັດໃຫ້ແອັບຯເຂົ້າໃຈຜິດວ່າມັນບໍ່ມີເທື່ອ Migration ຖືກນໍາໃຊ້ທັງຝົດແລະຄວາມພະຍາຍາມເພື່ອຊ້ໍາຕາຕະລາງແມ່ນເຮັດ
    /// </remarks>
    public const string MigrationsHistoryTableName = "__ef_migrations_history";

    /// <summary>ອົງກອນທີ່ໃຊ້ລະບົບ</summary>
    public DbSet<Organization> Organizations => Set<Organization>();

    /// <summary>ສິດທັງຝົດໃນການນໍາໃຊ້ລະບົບ</summary>
    public DbSet<Permission> Permissions => Set<Permission>();

    /// <summary>ຄວາມສຳພັນລະຫວ່າງບົດບາດ ແລະ ສິດ</summary>
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();

    /// <summary>Refresh Token ອອກໃຫ້ຜູ້ໃຊ້</summary>
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    /// <summary>ບັນທຶກເຫດການລະບົບທີ່ສໍາຄັນ</summary>
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    /// <summary>ຫໍພັກທັງໝົດໃນລະບົບ</summary>
    public DbSet<Dormitory> Dormitories => Set<Dormitory>();

    /// <summary>ອາຄານທັງຝົດໃນລະບົບ</summary>
    public DbSet<Building> Buildings => Set<Building>();

    /// <summary>ຊັ້ນອາຄານທັງຝົດໃນລະບົບ</summary>
    public DbSet<Floor> Floors => Set<Floor>();

    /// <summary>ທຸກປະເພດຫ້ອງໃນລະບົບ</summary>
    public DbSet<RoomType> RoomTypes => Set<RoomType>();

    /// <summary>ຫ້ອງທັງຝົດໃນລະບົບ</summary>
    public DbSet<Room> Rooms => Set<Room>();

    /// <summary>ການມອບຝາຍໃຫ້ຜູ້ໃຊ້ດູແລຫໍພັກ</summary>
    public DbSet<UserDormitory> UserDormitories => Set<UserDormitory>();

    /// <summary>ຜູ້ເຊົ່າທັງໝົດໃນລະບົບ</summary>
    public DbSet<Tenant> Tenants => Set<Tenant>();

    /// <summary>ການຈອງຫ້ອງທັງຝົດໃນລະບົບ</summary>
    public DbSet<Reservation> Reservations => Set<Reservation>();

    /// <summary>ສັນຍາເຊົ່າທັງໝົດໃນລະບົບ</summary>
    public DbSet<Contract> Contracts => Set<Contract>();

    /// <summary>ຄ່າແມັດທັງໝົດໃນລະບົບ</summary>
    public DbSet<MeterReading> MeterReadings => Set<MeterReading>();

    /// <summary>ໃບແຈ້ງໜີ້ທັງໝົດໃນລະບົບ</summary>
    public DbSet<Invoice> Invoices => Set<Invoice>();

    /// <summary>ລາຍການການຈ່າຍເງິນທັງໝົດໃນລະບົບ</summary>
    public DbSet<Payment> Payments => Set<Payment>();

    /// <summary>ວຽກງານບໍາລຸງຮັກສາທັງຝົດໃນລະບົບ</summary>
    public DbSet<MaintenanceRequest> MaintenanceRequests => Set<MaintenanceRequest>();

    /// <summary>ເງິນຝາກທັງຝົດໃນລະບົບ</summary>
    public DbSet<Deposit> Deposits => Set<Deposit>();

    /// <summary>ໃບຮັບເງິນທັງໝົດໃນລະບົບ</summary>
    public DbSet<Receipt> Receipts => Set<Receipt>();

    /// <summary>ຄ່າໃຊ້ຈ່າຍທັງຝົດໃນລະບົບ</summary>
    public DbSet<Expense> Expenses => Set<Expense>();

    /// <summary>parcels ທັງຝົດໃນລະບົບ</summary>
    public DbSet<Parcel> Parcels => Set<Parcel>();

    /// <summary>ຜູ້ເຂົ້າຊົມທັງຝົດໃນລະບົບ</summary>
    public DbSet<Visitor> Visitors => Set<Visitor>();

    /// <summary>ການລົງທະບຽນບ່ອນຈອດລົດທັງຝົດໃນລະບົບ</summary>
    public DbSet<ParkingRegistration> ParkingRegistrations => Set<ParkingRegistration>();

    /// <summary>ປະກາດທັງໝົດໃນລະບົບ</summary>
    public DbSet<Announcement> Announcements => Set<Announcement>();

    /// <summary>ການແຈ້ງເຕືອນທັງໝົດໃນລະບົບ</summary>
    public DbSet<Notification> Notifications => Set<Notification>();

    /// <summary>ການຕັ້ງຄ່າທັງຝົດໃນລະບົບ</summary>
    public DbSet<Setting> Settings => Set<Setting>();

    public DbSet<BillingPeriod> BillingPeriods => Set<BillingPeriod>();
    public DbSet<InvoiceCharge> InvoiceCharges => Set<InvoiceCharge>();
    public DbSet<ContractRenewal> ContractRenewals => Set<ContractRenewal>();
    public DbSet<MaintenanceEvent> MaintenanceEvents => Set<MaintenanceEvent>();

    /// <inheritdoc />
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await WorkflowEffects.ApplyAsync(this, cancellationToken);
        return await base.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        base.OnModelCreating(builder);

        builder.ApplyConfigurationsFromAssembly(InfrastructureAssembly);
        builder.Entity<BillingPeriod>().ToTable("billing_periods");
        builder.Entity<BillingPeriod>().HasIndex(x => new { x.DormitoryId, x.Month }).IsUnique();
        builder.Entity<InvoiceCharge>().ToTable("invoice_charges");
        builder.Entity<InvoiceCharge>().HasOne<Invoice>().WithMany().HasForeignKey(x => x.InvoiceId).OnDelete(DeleteBehavior.Restrict);
        builder.Entity<ContractRenewal>().ToTable("contract_renewals");
        builder.Entity<ContractRenewal>().HasOne<Contract>().WithMany().HasForeignKey(x => x.ContractId).OnDelete(DeleteBehavior.Restrict);
        builder.Entity<MaintenanceEvent>().ToTable("maintenance_events");
        builder.Entity<MaintenanceEvent>().Property(x => x.Note).HasMaxLength(2000);
        builder.Entity<MaintenanceEvent>().HasOne<MaintenanceRequest>().WithMany().HasForeignKey(x => x.MaintenanceRequestId).OnDelete(DeleteBehavior.Restrict);
        builder.Entity<Expense>().HasOne<MaintenanceRequest>().WithMany().HasForeignKey(x => x.MaintenanceRequestId).OnDelete(DeleteBehavior.Restrict);
        builder.Entity<MaintenanceRequest>().HasOne<Tenant>().WithMany().HasForeignKey(x => x.TenantId).OnDelete(DeleteBehavior.Restrict);

        // ຕ້ອງໂທຫາພາຍຫຼັງ ApplyConfigurationsFromAssembly ເພື່ອອະນຸຍາດໃຫ້ສັນຍາລວມເພື່ອ override ມູນຄ່າທີ່ຍັງບໍ່ທັນໄດ້ປັບແຕ່ງ
        builder.ApplyDormFlowConventions();
    }

    /// <inheritdoc />
    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        ArgumentNullException.ThrowIfNull(configurationBuilder);

        base.ConfigureConventions(configurationBuilder);

        // ຈໍາກັດຄວາມຍາວເບື້ອງຕົ້ນຂອງ string ເພື່ອບໍ່ໃຫ້ສ້າງຖັນໂດຍບັງເອີນ text ບໍ່ມີຂອບເຂດ
        configurationBuilder.Properties<string>().HaveMaxLength(256);
    }
}
